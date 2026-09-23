using ECommerce.Domain.Common;
using ECommerce.Domain.Entities.CatalogEntities;
using ECommerce.Domain.Entities.OrderEntities;
using ECommerce.Domain.Entities.UserEntities;
using ECommerce.Domain.ValueObjects;
using ECommerce.Infrastructure.Identity;
using ECommerce.Infrastructure.Persistence.ValueGenerators;
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
    public DbSet<Review> Reviews { get; set; }

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

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var idProperty = entityType.FindProperty(nameof(BaseEntity.Id));
                if (idProperty != null && idProperty.ClrType == typeof(Guid))
                {
                    idProperty.SetValueGeneratorFactory((_, _) => new GuidV7ValueGenerator());
                    idProperty.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
                }

                modelBuilder.Entity(entityType.ClrType, builder =>
                {
                    builder.Property<DateTime>(nameof(BaseEntity.CreatedAt))
                        .IsRequired()
                        .HasDefaultValueSql("GETUTCDATE()");

                    builder.Property<DateTime?>(nameof(BaseEntity.UpdatedAt));

                    builder.Property<bool>(nameof(BaseEntity.IsDeleted))
                        .IsRequired()
                        .HasDefaultValue(false);

                    builder.Property<DateTime?>(nameof(BaseEntity.DeletedAt));

                    builder.Property<Guid?>(nameof(BaseEntity.CreatedBy));
                    builder.Property<Guid?>(nameof(BaseEntity.UpdatedBy));

                    var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                    var property = System.Linq.Expressions.Expression.PropertyOrField(parameter, nameof(BaseEntity.IsDeleted));
                    var entityNotDeleted = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
                    var lambda = System.Linq.Expressions.Expression.Lambda(entityNotDeleted, parameter);

                    builder.HasQueryFilter(lambda);
                });
            }
        }

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
