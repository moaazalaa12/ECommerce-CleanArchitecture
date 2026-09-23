using ECommerce.Domain.Entities.OrderEntities;
using ECommerce.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.Persistence.Configurations.OrderConfigurations
{
    public class OrderConfigration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.UserId).IsRequired();
            builder.Property(o => o.OrderDate).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            builder.Property(o => o.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(o => o.OrderStatus).IsRequired().HasConversion<string>().HasMaxLength(20);
            builder.Property(o => o.PaymentStatus).IsRequired().HasConversion<string>().HasMaxLength(20);
            builder.Property(o => o.ShippingFee).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(o => o.DeliveryMethod).IsRequired().HasMaxLength(50);
            builder.Property(o => o.DiscountAmount).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);

            // Value Object: ShippingAddress
            builder.OwnsOne(o => o.ShippingAddress, sa =>
            {
                sa.Property(a => a.Street).HasColumnName("ShippingStreet").HasMaxLength(200).IsRequired();
                sa.Property(a => a.City).HasColumnName("ShippingCity").HasMaxLength(100).IsRequired();
                sa.Property(a => a.State).HasColumnName("ShippingState").HasMaxLength(100).IsRequired();
                sa.Property(a => a.Country).HasColumnName("ShippingCountry").HasMaxLength(100).IsRequired();
                sa.Property(a => a.ZipCode).HasColumnName("ShippingZipCode").HasMaxLength(20).IsRequired();
            });

            builder.HasOne<ApplicationUser>()
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Coupon)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CouponId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.Payments)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(o => o.UserId);
            builder.HasIndex(o => o.OrderDate);
        }
    }
}
