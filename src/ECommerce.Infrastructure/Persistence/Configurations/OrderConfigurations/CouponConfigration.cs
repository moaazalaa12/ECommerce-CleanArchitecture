using ECommerce.Domain.Entities.OrderEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.Persistence.Configurations.OrderConfigurations
{
    public class CouponConfigration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.ToTable("Coupons");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Code).IsRequired().HasMaxLength(50);
            builder.Property(c => c.DiscountType).IsRequired().HasConversion<string>().HasMaxLength(20);
            builder.Property(c => c.DiscountValue).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(c => c.MinOrderAmount).HasColumnType("decimal(18,2)");
            builder.Property(c => c.ExpiryDate).IsRequired();
            builder.Property(c => c.MaxUsageCount).IsRequired();
            builder.Property(c => c.CurrentUsageCount).IsRequired().HasDefaultValue(0);
            builder.Property(c => c.IsActive).IsRequired().HasDefaultValue(true);

            builder.HasIndex(c => c.Code).IsUnique();
        }
    }
}
