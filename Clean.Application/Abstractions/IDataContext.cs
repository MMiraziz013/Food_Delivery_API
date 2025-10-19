using Clean.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Clean.Application.Abstractions;

public interface IDataContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Courier> Couriers { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task MigrateAsync();
    
    DatabaseFacade Database { get; } // add this line
}