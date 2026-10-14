using ECommerce.Domain.Entities.UserEntities;
using ECommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Interfaces.UserInterfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<int> CountUnreadAsync(Guid userId, CancellationToken cancellationToken = default);

        Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<Notification> Items, int TotalCount)> GetPagedUserNotificationsAsync(
            Guid userId,
            bool unreadOnly,
            int pageNumber,
            int pageSize,
            NotificationSortColumn? sortBy,
            bool isDescending,
            CancellationToken cancellationToken = default);
    }
}
