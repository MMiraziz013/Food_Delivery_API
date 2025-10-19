using Clean.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clean.Infrastructure.Data.Configurations;

public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.ToTable("order_details");

        builder.Property(or => or.SpecialInstructions).HasMaxLength(200);

        builder.HasOne(or => or.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(or => or.OrderId);

        builder.HasOne(or => or.MenuItem)
            .WithMany(m => m.OrderDetails)
            .HasForeignKey(or => or.MenuItemId);
    }
}