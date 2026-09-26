using ECommerce.Application.Interfaces.Identity;
using ECommerce.Application.Interfaces.Payment;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Interfaces.CatalogInterfaces;
using ECommerce.Domain.Interfaces.OrderInterfaces;
using ECommerce.Infrastructure.ExternalServices.Payment;
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

        services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));

        // 2. Register Repositories & Unit of Work
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IStockReservationRepository, StockReservationRepository>();
        services.AddScoped<ICouponUsageRepository, CouponUsageRepository>();
        services.AddScoped<ICouponRepository, CouponRepository>();

        // 3. Register External Services
        // services.AddTransient<IEmailService, SmtpEmailService>();
        services.AddScoped<IPaymentService, PaymentService>();

        // 4. Register Seeders
        // services.AddScoped<RoleSeeder>();
        // services.AddScoped<CategorySeeder>();

        // 5. Register Background Jobs (Hosted Services)
        // services.AddHostedService<StockReservationCleanupJob>();

        // 6. Register Identity
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            // 1. (Password Settings)
            options.Password.RequireDigit = true;    
            options.Password.RequireLowercase = true;   
            options.Password.RequireUppercase = true;        
            options.Password.RequireNonAlphanumeric = true;     
            options.Password.RequiredLength = 8;             
            options.Password.RequiredUniqueChars = 4;           

            // 2.  (Lockout Settings)
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); 
            options.Lockout.MaxFailedAccessAttempts = 5;                      
            options.Lockout.AllowedForNewUsers = true;                      

            // 3. (User Settings)
            options.User.RequireUniqueEmail = true;           
            options.User.AllowedUserNameCharacters =            
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

            // (Sign-in Settings)
            options.SignIn.RequireConfirmedEmail = true;      
            options.SignIn.RequireConfirmedPhoneNumber = false; 
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}
