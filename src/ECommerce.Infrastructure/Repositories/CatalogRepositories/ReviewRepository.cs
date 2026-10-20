using ECommerce.Domain.Entities.CatalogEntities;
using ECommerce.Domain.Interfaces.CatalogInterfaces;
using ECommerce.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories.CatalogRepositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<(Review Review, string UserName)>> GetProductReviewsWithUserNameAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var reviewswithUserNames = await (from r in _dbContext.Reviews.AsNoTracking() 
                                       join u in _dbContext.Users.AsNoTracking() on r.UserId equals u.Id
                                       where r.ProductId == productId
                                       orderby r.CreatedAt descending
                                        select new { Review = r, UserName = u.UserName ?? "Unknown User" }
                                       ).ToListAsync(cancellationToken);

            return reviewswithUserNames.Select(x => (x.Review, x.UserName)).ToList();
        }

        public async Task<Review?> GetUserReviewForProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId, cancellationToken);
        }
    }
}
