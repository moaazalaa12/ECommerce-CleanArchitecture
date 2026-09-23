using ECommerce.Domain.Entities.OrderEntities;
using ECommerce.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.Persistence.Configurations.OrderConfigurations
{
    public class CouponUsageConfigration : IEntityTypeConfiguration<CouponUsage>
    {
        public void Configure(EntityTypeBuilder<CouponUsage> builder)
        {
            builder.ToTable("CouponUsages");
            builder.HasKey(cu => cu.Id);

            builder.Property(cu => cu.UserId).IsRequired();
            builder.Property(cu => cu.CouponId).IsRequired();
            builder.Property(cu => cu.OrderId).IsRequired();
            builder.Property(cu => cu.UsedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(cu => cu.Coupon)
                .WithMany(c => c.Usages)
                .HasForeignKey(cu => cu.CouponId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cu => cu.Order)
                .WithMany()
                .HasForeignKey(cu => cu.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(cu => cu.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(cu => new { cu.UserId, cu.CouponId });
        }
    
    }
}
