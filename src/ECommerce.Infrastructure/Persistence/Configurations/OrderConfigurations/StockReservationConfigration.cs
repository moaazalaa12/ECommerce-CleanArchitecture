using ECommerce.Domain.Entities.OrderEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.Persistence.Configurations.OrderConfigurations
{
    public class StockReservationConfigration : IEntityTypeConfiguration<StockReservation>
    {
        public void Configure(EntityTypeBuilder<StockReservation> builder)
        {
            builder.ToTable("StockReservations");
            builder.HasKey(sr => sr.Id);

            builder.Property(sr => sr.ProductId).IsRequired();
            builder.Property(sr => sr.Quantity).IsRequired();
            builder.Property(sr => sr.ReservationDate).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            builder.Property(sr => sr.ExpiryDate).IsRequired();
            builder.Property(sr => sr.IsReleased).IsRequired().HasDefaultValue(false);

            builder.HasOne(sr => sr.Product)
                .WithMany()
                .HasForeignKey(sr => sr.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sr => sr.Order)
                .WithMany()
                .HasForeignKey(sr => sr.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(sr => sr.ProductId);
            builder.HasIndex(sr => sr.ExpiryDate);
        }
    }
}
