using ECommerce.Domain.Entities.OrderEntities;
using ECommerce.Domain.Interfaces.OrderInterfaces;
using ECommerce.Infrastructure.Persistence.DbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.Repositories
{
    public class CouponRepository : GenericRepository<Coupon>, ICouponRepository
    {
        public CouponRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
