using Clean.Application.Abstractions;
using Clean.Domain.Entities;
using Clean.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Clean.Infrastructure.Data.Seed;

public class DomainSeeder
{
    private readonly IDataContext _context;

    public DomainSeeder(IDataContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // await SeedCouriersAsync();
        // await SeedSampleOrdersAsync();
    }

    // private async Task SeedCouriersAsync()
    // {
    //     if (_context.Couriers.Any()) return;
    //
    //     var couriers = new List<Courier>
    //     {
    //         new()
    //         {
    //             Status = CourierStatus.Active,
    //             TransportType = TransportType.Bicycle,
    //             CurrentLocation = "5th Avenue, Manhattan"
    //         },
    //         new()
    //         {
    //             Status = CourierStatus.Active,
    //             TransportType =  TransportType.Motorcycle,
    //             CurrentLocation = "at Home"
    //         }
    //     };
    //
    //     _context.Couriers.AddRange(couriers);
    //     await _context.SaveChangesAsync();
    // }

    // private async Task SeedSampleOrdersAsync()
    // {
    //     if (_context.Orders.Any()) return;
    //
    //     var order = new Order
    //     {
    //         order = "Sample Order",
    //         OrderStatus = OrderStatus.Confirmed,
    //         CreatedAt = DateTime.UtcNow,
    //         TotalAmount = 100,
    //
    //     };
    //
    //     _context.Orders.Add(order);
    //     await _context.SaveChangesAsync();
    // }
}

public static class Roles
{
    public const string Admin = "Admin";
    public const string Client = "Client";
    public const string Courier = "Courier";
}