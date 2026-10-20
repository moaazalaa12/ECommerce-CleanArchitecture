using ECommerce.Domain.Entities.CatalogEntities;
using ECommerce.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ECommerce.Infrastructure.Persistence.Configurations.CatalogConfigurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews", t =>
                t.HasCheckConstraint("CK_Reviews_Rating", "[Rating] >= 1 AND [Rating] <= 5"));

            builder.HasKey(r => r.Id);
            
            builder.HasIndex(r => new { r.UserId, r.ProductId })
                   .IsUnique();

            builder.Property(r => r.Rating)
                   .IsRequired();

            builder.Property(r => r.Comment)
                   .HasMaxLength(1000);

            builder.HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
