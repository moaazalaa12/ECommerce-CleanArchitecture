using ECommerce.Application;
using ECommerce.Application.Interfaces.Notifications;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.BackgroundJobs;
using ECommerce.WebAPI.Handlers;
using ECommerce.WebAPI.Hubs;
using ECommerce.WebAPI.Services;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.Sources.Clear();


builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();



// 1. Add API Controllers
builder.Services.AddControllers();

// 2. Wire up Clean Architecture Layers
// This calls the DependencyInjection.cs files you created in the other layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var tempKey = "TEMP_SUPER_SECRET_KEY_1234567890_ABCDEFGHIJKLMNOP";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tempKey))
        };

        // For SignalR, we need to allow the token to be passed in the query string for WebSocket connections
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/hubs/notifications"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// 3. Global Exception Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(); // Required for the exception handler to format responses

// 4. OpenAPI & Swagger UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Note: builder.Services.AddOpenApi() generates the spec, but AddSwaggerGen() is needed for the actual UI.

builder.Services.AddDataProtection();


builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
builder.Services.AddScoped<IRealtimeNotifier, SignalRRealtimeNotifier>();
//builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
var app = builder.Build();

app.UseHangfireDashboard("/hangfire");

RecurringJob.AddOrUpdate<StockReservationCleanupJob>(
    "stock-reservation-cleanup",
    job => job.ExecuteAsync(),
    "*/5 * * * *");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 5. Middleware Pipeline (Order is critical here)
app.UseExceptionHandler(); // Catches crashes before they hit the user
app.UseHttpsRedirection();

app.UseCors("SignalRPolicy");
app.UseAuthentication(); // Must be BEFORE Authorization
app.UseAuthorization();

// 6. Map Controller Endpoints
app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

// 7. Optional: Run Seeders on Startup
/*
using (var scope = app.Services.CreateScope())
{
    var roleSeeder = scope.ServiceProvider.GetRequiredService<RoleSeeder>();
    await roleSeeder.SeedAsync();
}
*/

app.Run();