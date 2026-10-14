using ECommerce.Domain.Entities.UserEntities;
using ECommerce.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.UserConfigurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");
            builder.HasKey(n => n.Id);

            builder.Property(n => n.UserId).IsRequired();
            builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
            builder.Property(n => n.Message).IsRequired().HasMaxLength(1000);
            builder.Property(n => n.Type).IsRequired().HasConversion<string>().HasMaxLength(50);
            builder.Property(n => n.IsRead).IsRequired().HasDefaultValue(false);
            builder.Property(n => n.RelatedEntityId);
            builder.Property(n => n.ReadAt);

            builder.HasIndex(n => new { n.UserId, n.IsRead, n.CreatedAt });

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
