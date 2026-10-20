using ECommerce.Domain.Entities.UserEntities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces.UserInterfaces;
using ECommerce.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories.UserRepositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<int> CountUnreadAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);
        }

        public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            await _dbSet
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(n => n.IsRead, true)
                    .SetProperty(n => n.ReadAt, DateTime.UtcNow),
                    cancellationToken);
        }

        public async Task<(IReadOnlyList<Notification> Items, int TotalCount)> GetPagedUserNotificationsAsync(
            Guid userId,
            bool unreadOnly,
            int pageNumber,
            int pageSize,
            NotificationSortColumn? sortBy,
            bool isDescending,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(n => n.UserId == userId);

            if (unreadOnly)
                query = query.Where(n => !n.IsRead);

            var totalCount = await query.CountAsync(cancellationToken);

            query = (sortBy, isDescending) switch
            {
                (NotificationSortColumn.Title, false) =>
                    query.OrderBy(n => n.Title).ThenByDescending(n => n.CreatedAt),
                (NotificationSortColumn.Title, true) =>
                    query.OrderByDescending(n => n.Title).ThenByDescending(n => n.CreatedAt),

                (NotificationSortColumn.Type, false) =>
                    query.OrderBy(n => n.Type).ThenByDescending(n => n.CreatedAt),
                (NotificationSortColumn.Type, true) =>
                    query.OrderByDescending(n => n.Type).ThenByDescending(n => n.CreatedAt),

                (NotificationSortColumn.IsRead, false) =>
                    query.OrderBy(n => n.IsRead).ThenByDescending(n => n.CreatedAt),
                (NotificationSortColumn.IsRead, true) =>
                    query.OrderByDescending(n => n.IsRead).ThenByDescending(n => n.CreatedAt),

                (NotificationSortColumn.CreatedAt, false) =>
                    query.OrderBy(n => n.CreatedAt),
                (NotificationSortColumn.CreatedAt, true) =>
                    query.OrderByDescending(n => n.CreatedAt),

                _ => query.OrderByDescending(n => n.CreatedAt) // Default
            };

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
