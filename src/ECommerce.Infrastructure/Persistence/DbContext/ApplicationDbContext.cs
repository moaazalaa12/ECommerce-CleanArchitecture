using ECommerce.Domain.Common;
using ECommerce.Domain.Entities.CatalogEntities;
using ECommerce.Domain.Entities.OrderEntities;
using ECommerce.Domain.Entities.UserEntities;
using ECommerce.Domain.ValueObjects;
using ECommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.DbContext;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    // Catalog DbSets
    // ===========================
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<ProductImage> Reviews { get; set; }

    // ===========================
    // Order DbSets
    // ===========================
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<CouponUsage> CouponUsages { get; set; }
    public DbSet<StockReservation> StockReservations { get; set; }

    // ===========================
    // User DbSets
    // ===========================
    public DbSet<Address> Addresses { get; set; }

    public DbSet<UserAddress> UserAddresses { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<WishlistItem> WishlistItems { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    // The constructor passes configurations to the base DbContext class
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // This single line scans the Infrastructure project and automatically 
        // applies all classes that implement IEntityTypeConfiguration<T>.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Property(x => x.CreatedAt).IsModified = false;
                    entry.Property(x => x.CreatedBy).IsModified = false;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    entry.Entity.IsDeleted = true;
                    entry.Property(x => x.CreatedAt).IsModified = false;
                    entry.Property(x => x.CreatedBy).IsModified = false;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
