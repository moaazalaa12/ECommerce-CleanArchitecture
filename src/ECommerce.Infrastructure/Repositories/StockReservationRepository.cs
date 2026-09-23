using ECommerce.Domain.Entities.OrderEntities;
using ECommerce.Domain.Interfaces.OrderInterfaces;
using ECommerce.Infrastructure.Persistence.DbContext;

namespace ECommerce.Infrastructure.Repositories
{
    public class StockReservationRepository : GenericRepository<StockReservation>, IStockReservationRepository
    {
        public StockReservationRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
