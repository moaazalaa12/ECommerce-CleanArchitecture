using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();


       
        // 1. Register AutoMapper
        services.AddAutoMapper(cfg => cfg.AddMaps(assembly));

        // 2. Register FluentValidation (Scans the Validators folders)
        services.AddValidatorsFromAssembly(assembly);

        // 3. Register MediatR & Pipeline Behaviors (CQRS)
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
        });

        // 4. Register Standard Services (The Pragmatic Approach)
         //services.AddScoped<ICourseService, CourseService>();

        return services;
    }
}
