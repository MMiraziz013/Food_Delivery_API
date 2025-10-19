using Clean.Application.Abstractions;
using Clean.Domain.Entities;
using Clean.Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Clean.Infrastructure.Data;

public class DataContext : IdentityDbContext<User, IdentityRole<int>, int>, IDataContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }

    public DbSet<Courier> Couriers { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return base.SaveChangesAsync(ct);
    }

    public async Task MigrateAsync()    
    {
        await Database.MigrateAsync();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(RestaurantConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(MenuConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(OrderConfigurations).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(OrderDetailConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(CourierConfiguration).Assembly);

    } 
}