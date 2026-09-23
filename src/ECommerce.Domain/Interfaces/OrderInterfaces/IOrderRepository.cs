using ECommerce.Domain.Entities.OrderEntities;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Interfaces.OrderInterfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order?> GetOrderWithDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<Order> Orders, int TotalCount)> GetPagedUserOrdersAsync(
            Guid userId,
            int pageNumber,
            int pageSize,
            OrderSortColumn? sortBy,
            bool isDescending,
            CancellationToken cancellationToken = default);
    }
}
