using ECommerce.Domain.Entities.CatalogEntities;
using ECommerce.Domain.Interfaces.CatalogInterfaces;
using ECommerce.Infrastructure.Persistence.DbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
