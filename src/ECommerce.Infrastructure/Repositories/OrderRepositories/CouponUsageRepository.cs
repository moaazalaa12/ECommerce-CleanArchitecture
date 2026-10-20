using ECommerce.Domain.Entities.OrderEntities;
using ECommerce.Domain.Interfaces.OrderInterfaces;
using ECommerce.Infrastructure.Persistence.DbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.Repositories.OrderRepositories
{
    public class CouponUsageRepository : GenericRepository<CouponUsage>, ICouponUsageRepository
    {
        public CouponUsageRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
