using Clean.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clean.Infrastructure.Data.Configurations;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("menus");
        builder.Property(m => m.Name).IsRequired().HasMaxLength(50);
        builder.Property(m => m.Description).HasMaxLength(200);
        builder.Property(m => m.Category).IsRequired().HasMaxLength(80);
        builder.Property(m => m.PhotoUrl).HasMaxLength(250);

        builder.HasOne(m => m.Restaurant)
            .WithMany(r => r.Menus)
            .HasForeignKey(m => m.RestaurantId);

        builder.HasMany(m => m.OrderDetails)
            .WithOne(o => o.MenuItem);
    }
}