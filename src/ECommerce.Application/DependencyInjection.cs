using ECommerce.Application.Behaviors;
using ECommerce.Application.Interfaces.Notifications;
using ECommerce.Application.Interfaces.Reviews;
using ECommerce.Application.Interfaces.Wishlist;
using ECommerce.Application.Services;
using ECommerce.Application.Services.Reviews;
using ECommerce.Application.Services.Wishlist;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
namespace ECommerce.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(cfg => cfg.AddMaps(assembly));

        services.AddValidatorsFromAssembly(assembly);

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);

            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IWishlistService, WishlistService>();
        services.AddScoped<IReviewService, ReviewService>();

        return services;
    }
}