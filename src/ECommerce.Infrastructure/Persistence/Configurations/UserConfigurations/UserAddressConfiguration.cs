using ECommerce.Domain.Entities.UserEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.Persistence.Configurations.UserConfigurations
{
    public class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
    {
        public void Configure(EntityTypeBuilder<UserAddress> builder)
        {
            builder.OwnsOne(ua => ua.AddressDetails, ad =>
            {
                ad.Property(a => a.Street).HasColumnName("Street").HasMaxLength(200).IsRequired();
                ad.Property(a => a.City).HasColumnName("City").HasMaxLength(100).IsRequired();
                ad.Property(a => a.State).HasColumnName("State").HasMaxLength(100).IsRequired();
                ad.Property(a => a.Country).HasColumnName("Country").HasMaxLength(100).IsRequired();
                ad.Property(a => a.ZipCode).HasColumnName("ZipCode").HasMaxLength(20).IsRequired();
            });
        }
    }
}
