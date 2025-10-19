using Clean.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clean.Infrastructure.Data.Configurations;

public class CourierConfiguration : IEntityTypeConfiguration<Courier>
{
    public void Configure(EntityTypeBuilder<Courier> builder)
    {
        builder.ToTable("couriers");

        builder.Property(c => c.CurrentLocation).HasMaxLength(200);
        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId);
    }
}