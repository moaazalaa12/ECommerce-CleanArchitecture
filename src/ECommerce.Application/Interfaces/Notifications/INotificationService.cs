using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.Notifications;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.Interfaces.Notifications
{
    public interface INotificationService
    {
        Task SendNotificationAsync(Guid userId, string title, string message,
            NotificationType type, Guid? relatedEntityId = null,
            CancellationToken cancellationToken = default);

        Task<PagedResponse<NotificationDto>> GetUserNotificationsAsync(
            Guid userId,
            PagedRequest<NotificationSortColumn> request,
            bool unreadOnly = false,
            CancellationToken cancellationToken = default);

        Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId,
            CancellationToken cancellationToken = default);

        Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}