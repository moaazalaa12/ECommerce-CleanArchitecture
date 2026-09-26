using ECommerce.Domain.Entities.OrderEntities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces.OrderInterfaces;
using ECommerce.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            
        }

        public async Task<Order?> GetOrderWithDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
           
            return await _dbSet
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Coupon)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<(IReadOnlyList<Order> Orders, int TotalCount)> GetPagedUserOrdersAsync(
            Guid userId,
            int pageNumber,
            int pageSize,
            OrderSortColumn? sortBy,
            bool isDescending,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet
                .Where(o => o.UserId == userId)
                .AsNoTracking();

            var totalCount = await query.CountAsync(cancellationToken);

            if (sortBy.HasValue)
            {
                query = sortBy.Value switch
                {
                    OrderSortColumn.TotalAmount => isDescending ? query.OrderByDescending(o => o.TotalAmount) : query.OrderBy(o => o.TotalAmount),
                    OrderSortColumn.Status => isDescending ? query.OrderByDescending(o => o.OrderStatus) : query.OrderBy(o => o.OrderStatus),
                    _ => isDescending ? query.OrderByDescending(o => o.OrderDate) : query.OrderBy(o => o.OrderDate)
                };
            }
            else
            {
                query = query.OrderByDescending(o => o.OrderDate);
            }

            var orders = await query
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (orders, totalCount);
        }

        public async Task<Order?> GetOrderByPaymentIntentIdAsync(string paymentIntentId)
        {
            return await _dbContext.Orders
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o =>
                    o.Payments.Any(p => p.TransactionId == paymentIntentId));
        }
    }
}