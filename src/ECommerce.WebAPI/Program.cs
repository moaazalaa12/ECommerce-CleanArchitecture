using ECommerce.Application; 
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Identity;
using ECommerce.Infrastructure.Persistence.DbContext;
using Microsoft.AspNetCore.Identity;
// using Presentation.Middleware; // Uncomment when you add your GlobalExceptionHandler

var builder = WebApplication.CreateBuilder(args);

// 1. Add API Controllers
builder.Services.AddControllers();

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                    .AddRoles<IdentityRole<Guid>>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();
// 2. Wire up Clean Architecture Layers
// This calls the DependencyInjection.cs files you created in the other layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// 3. Global Exception Handling
// builder.Services.AddExceptionHandler<GlobalExceptionHandler>(); // Uncomment when created
builder.Services.AddProblemDetails(); // Required for the exception handler to format responses

// 4. OpenAPI & Swagger UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Note: builder.Services.AddOpenApi() generates the spec, but AddSwaggerGen() is needed for the actual UI.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 5. Middleware Pipeline (Order is critical here)
app.UseExceptionHandler(); // Catches crashes before they hit the user
app.UseHttpsRedirection();

builder.Services.AddDataProtection();

app.UseAuthentication(); // Must be BEFORE Authorization
app.UseAuthorization();

// 6. Map Controller Endpoints
app.MapControllers();

// 7. Optional: Run Seeders on Startup
/*
using (var scope = app.Services.CreateScope())
{
    var roleSeeder = scope.ServiceProvider.GetRequiredService<RoleSeeder>();
    await roleSeeder.SeedAsync();
}
*/

app.Run();