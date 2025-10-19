using Clean.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clean.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.Property(u => u.UserName).IsRequired().HasMaxLength(75);
        builder.Property(u => u.Name).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(75);
        builder.Property(u => u.RegistrationDate).HasDefaultValueSql("NOW()");
        builder.Property(u => u.PhoneNumber).HasMaxLength(25);

        builder.HasMany(u => u.Orders)
            .WithOne(o => o.User);
    }
}