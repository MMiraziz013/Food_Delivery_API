using Clean.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clean.Infrastructure.Data.Configurations;

public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant >
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.ToTable("restaurants");

        builder.Property(r => r.Name).IsRequired().HasMaxLength(50);
        builder.Property(r => r.Address).IsRequired().HasMaxLength(100);
        builder.Property(r => r.WorkingHours).IsRequired().HasMaxLength(50);
        builder.Property(r => r.ContactPhone).IsRequired().HasMaxLength(25);
        
        builder.HasMany(r => r.Menus)
            .WithOne(m => m.Restaurant);
        
        builder.HasMany(r => r.Orders)
            .WithOne(o => o.Restaurant);
    }
}