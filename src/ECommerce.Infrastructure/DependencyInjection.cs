using ECommerce.Application.Interfaces.Identity;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Infrastructure.Identity;
using ECommerce.Infrastructure.Persistence.DbContext;
using ECommerce.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Register the Database Context
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // 2. Register Repositories & Unit of Work
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        //services.AddScoped<IOrderRepository, OrderRepository>(); // Specific repo example
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // 3. Register External Services
        // services.AddTransient<IEmailService, SmtpEmailService>();
        // services.AddTransient<IPaymentGateway, StripePaymentGateway>();

        // 4. Register Seeders
        // services.AddScoped<RoleSeeder>();
        // services.AddScoped<CategorySeeder>();

        // 5. Register Background Jobs (Hosted Services)
        // services.AddHostedService<StockReservationCleanupJob>();

        // 6. Register Identity
        services.AddIdentityCore<ApplicationUser>(options => {
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}
