using System.Net;
using Clean.Application.Abstractions;
using Clean.Application.Dtos.Analytics;
using Clean.Application.Dtos.Courier;
using Clean.Application.Dtos.Menu;
using Clean.Application.Dtos.Order;
using Clean.Application.Dtos.OrderDetails;
using Clean.Application.Dtos.Restaurant;
using Clean.Application.Responses;
using Clean.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Clean.Application.Services.Analytics;

public class AnalyticsService : IAnalyticsService
{
    private readonly IDataContext _context;
    private readonly UserManager<Domain.Entities.User> _userManager;

    public AnalyticsService(IDataContext context, UserManager<Domain.Entities.User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    public async Task<Response<List<GetRestaurantDto>>> GetActiveRestaurantsByRatingAsync()
    {
        var restaurants = await _context.Restaurants
            .Where(r => r.IsActive == true)
            .OrderByDescending(r=> r.Rating)
            .ToListAsync();

        var restaurantList = restaurants.Select(r => new GetRestaurantDto
        {
            Name = r.Name,
            Address = r.Address,
            ContactPhone = r.ContactPhone,
            Description = r.Description,
            DeliveryPrice = r.DeliveryPrice,
            IsActive = r.IsActive,
            MinOrderAmount = r.MinOrderAmount,
            Rating = r.Rating,
            WorkingHours = r.WorkingHours,
            Menus = r.Menus.Select(m => new GetMenuDto
            {
                Name = m.Name,
                Category = m.Category,
                Description = m.Description,
                IsAvailable = m.IsAvailable,
                PhotoUrl = m.PhotoUrl,
                PreparationTime = m.PreparationTime,
                Weight = m.Weight,
                Price = m.Price
            }).ToList(),
        }).ToList();


        return new Response<List<GetRestaurantDto>>(HttpStatusCode.OK, "restaurant retrieved!", restaurantList);
    }

    public async Task<Response<List<GetMenuDto>>> GetDishesByPriceAsync(decimal maxPrice)
    {
        var items = await _context.Menus
            .Include(m=> m.Restaurant)
            .Where(m => m.Price <= maxPrice)
            .Where(m=> m.Restaurant.IsActive)
            .Where(m=>m.IsAvailable)
            .ToListAsync();

        var menuItems = items.Select(i => new GetMenuDto
        {
            Name = i.Name,
            Description = i.Description,
            Category = i.Category,
            IsAvailable = i.IsAvailable,
            PreparationTime = i.PreparationTime,
            Weight = i.Weight,
            PhotoUrl = i.PhotoUrl,
            Price = i.Price,
            RestaurantName = i.Restaurant.Name
        }).ToList();

        return new Response<List<GetMenuDto>>(HttpStatusCode.OK, "Dished retrieved!", menuItems);
    }

    public async Task<Response<List<OrdersByStatusDto>>> GetOrdersByStatusCountAsync()
    {
        var grouped = await _context.Orders
            .GroupBy(o => o.OrderStatus)
            .Select(g => new OrdersByStatusDto
            {
                Status = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        return new Response<List<OrdersByStatusDto>>(HttpStatusCode.OK, "Orders by status retrieved successfully.", grouped);
    }

    public async Task<Response<List<AvgPriceByCategoryDto>>> GetAvgPriceByCategoryAsync()
    {
        var averages = await _context.Menus
            .GroupBy(m => m.Category)
            .Select(g => new AvgPriceByCategoryDto
            {
                CategoryName = g.Key,
                AveragePrice = g.Average(m => m.Price)
            })
            .OrderByDescending(a => a.AveragePrice)
            .ToListAsync();

        return new Response<List<AvgPriceByCategoryDto>>(HttpStatusCode.OK, "Average price by category calculated successfully.", averages);
    }

    public async Task<Response<List<UserTotalOrdersDto>>> GetUsersWithTotalOrdersAsync()
    {
        var users = await _context.Orders
            .Include(o => o.User)
            .GroupBy(o => new { o.UserId, o.User.Name })
            .Select(g => new UserTotalOrdersDto
            {
                Username = g.Key.Name,
                TotalOrders = g.Count()
            })
            .OrderByDescending(u => u.TotalOrders)
            .ToListAsync();

        return new Response<List<UserTotalOrdersDto>>(HttpStatusCode.OK, "Users with total orders retrieved successfully.", users);
    }

    public async Task<Response<CourierDeliverOrdersDto>> GetOrdersDeliverByCourier(int courierId)
    {
        var courier = await _context.Couriers
            .Include(c => c.User)
            .Include(c => c.Orders)
            .ThenInclude(o => o.Restaurant)
            .Include(c => c.Orders)
            .ThenInclude(o => o.OrderDetails)
            .ThenInclude(od => od.MenuItem)
            .Where(c => c.Status != CourierStatus.Inactive)
            .Where(c=> c.Id == courierId)
            .FirstOrDefaultAsync();
        
        if (courier is null)
            return new Response<CourierDeliverOrdersDto>(HttpStatusCode.NotFound, "Courier not found.");

        var deliveredOrders = courier.Orders.Where(o => o.OrderStatus == OrderStatus.Delivered).ToList();

        var result = new CourierDeliverOrdersDto
        {
            CourierId = courier.Id,
            Username = courier.User.Name,
            DeliveredOrders = deliveredOrders.Select(o=> new GetOrderDto
            {
                Id = o.Id,
                OrderStatus = o.OrderStatus,
                DeliveryAddress = o.DeliveryAddress,
                PaymentMethod = o.PaymentMethod,
                PaymentStatus = o.PaymentStatus,
                RestaurantName = o.Restaurant.Name,
                CourierName = o.Courier.User.Name,
                TotalAmount = o.OrderDetails.Sum(or=> or.Price),
                CreatedAt = o.CreatedAt,
                OrderDetails = o.OrderDetails.Select(od => new GetOrderDetailDto
                {
                    OrderId = o.Id,
                    MenuItem = od.MenuItem.Name,
                    Quantity = od.Quantity,
                    Price = od.Price,
                    SpecialInstructions = od.SpecialInstructions
                }).ToList()
            }).ToList()
        };

        return new Response<CourierDeliverOrdersDto>(HttpStatusCode.OK, "Courier's delivered orders count retrieved successfully.", result);
    }

    public async Task<Response<List<GetOrderDto>>> GetTotalOrdersForTodayAsync()
    {
        var today = DateTime.UtcNow.Date;

        var orders = await _context.Orders
            .Include(o => o.Restaurant)
            .Include(o => o.Courier).ThenInclude(c => c.User)
            .Include(o => o.OrderDetails).ThenInclude(od => od.MenuItem)
            .Where(o => o.CreatedAt.Date == today)
            .ToListAsync();

        var result = orders.Select(o => new GetOrderDto
        {
            Id = o.Id,
            OrderStatus = o.OrderStatus,
            CreatedAt = o.CreatedAt,
            DeliveredAt = o.DeliveredAt,
            DeliveryAddress = o.DeliveryAddress,
            PaymentMethod = o.PaymentMethod,
            PaymentStatus = o.PaymentStatus,
            RestaurantName = o.Restaurant.Name,
            CourierName = o.Courier.User.Name,
            TotalAmount = o.OrderDetails.Sum(od => od.Price),
            OrderDetails = o.OrderDetails.Select(od => new GetOrderDetailDto
            {
                MenuItem = od.MenuItem.Name,
                Quantity = od.Quantity,
                Price = od.Price,
                SpecialInstructions = od.SpecialInstructions
            }).ToList()
        }).ToList();

        return new Response<List<GetOrderDto>>(HttpStatusCode.OK, "Today's orders retrieved successfully.", result);
    }

    public async Task<Response<List<GetCourierDto>>> GetTopFiveCouriersAsync()
    {
        var couriers = await _context.Couriers
            .Include(c => c.User)
            .Include(c => c.Orders)
            .ThenInclude(o => o.Restaurant)
            .Include(c => c.Orders)
            .ThenInclude(o => o.OrderDetails)
            .ThenInclude(od => od.MenuItem)
            .Where(c => c.Status != CourierStatus.Inactive)
            .OrderByDescending(c => c.Orders.Count(o => o.OrderStatus == OrderStatus.Delivered))
            .Take(5)
            .ToListAsync();

        var result = couriers.Select(c => new GetCourierDto
        {
            Id = c.Id,
            UserName = c.User.Name,
            Rating = c.Rating,
            Status = c.Status,
            TransportType = c.TransportType,
            Orders = c.Orders.Select(o=> new GetOrderDto
            {
                Id = o.Id,
                OrderStatus = o.OrderStatus,
                CreatedAt = o.CreatedAt,
                DeliveredAt = o.DeliveredAt,
                DeliveryAddress = o.DeliveryAddress,
                PaymentMethod = o.PaymentMethod,
                PaymentStatus = o.PaymentStatus,
                RestaurantName = o.Restaurant.Name,
                CourierName = o.Courier.User.Name,
                TotalAmount = o.OrderDetails.Sum(od => od.Price),
                OrderDetails = o.OrderDetails.Select(od => new GetOrderDetailDto
                {
                    MenuItem = od.MenuItem.Name,
                    Quantity = od.Quantity,
                    Price = od.Price,
                    SpecialInstructions = od.SpecialInstructions
                }).ToList()
            }).ToList()
        }).ToList();

        return new Response<List<GetCourierDto>>(HttpStatusCode.OK, "Top five couriers retrieved successfully.", result);
    }

    public async Task<Response<List<GetOrderDto>>> OrdersAboveAverageAsync()
    {
        var avgPrice = await _context.Orders
            .Where(o => o.OrderDetails.Any())
            .AverageAsync(o => o.OrderDetails.Sum(od => od.Price));

        var orders = await _context.Orders
            .Include(o => o.Restaurant)
            .Include(o => o.Courier).ThenInclude(c => c.User)
            .Include(o => o.OrderDetails).ThenInclude(od => od.MenuItem)
            .Where(o => o.OrderDetails.Sum(od => od.Price) > avgPrice)
            .ToListAsync();

        var result = orders.Select(o => new GetOrderDto
        {
            Id = o.Id,
            OrderStatus = o.OrderStatus,
            CreatedAt = o.CreatedAt,
            DeliveryAddress = o.DeliveryAddress,
            TotalAmount = o.OrderDetails.Sum(od => od.Price),
            RestaurantName = o.Restaurant.Name,
            CourierName = o.Courier.User.Name
        }).ToList();

        return new Response<List<GetOrderDto>>(HttpStatusCode.OK, "Orders above average price retrieved successfully.", result);
    }

    public async Task<Response<List<GetPopularRestaurantDto>>> GetMostPopularRestaurantsLastMonthAsync()
    {
        var lastMonth = DateTime.UtcNow.AddMonths(-1);

        var popularRestaurants = await _context.Orders
            .Include(o => o.Restaurant)
            .Where(o => o.CreatedAt >= lastMonth)
            .GroupBy(o => new { o.RestaurantId, o.Restaurant.Name, o.Restaurant.Rating })
            .Select(g => new
            {
                g.Key.RestaurantId,
                g.Key.Name,
                g.Key.Rating,
                OrderCount = g.Count()
            })
            .OrderByDescending(g => g.OrderCount)
            .Take(5)
            .ToListAsync();

        var result = popularRestaurants.Select(r => new GetPopularRestaurantDto
        {
            RestaurantId = r.RestaurantId,
            Name = r.Name,
            Rating = r.Rating,
             OrderCount = r.OrderCount
        }).ToList();

        return new Response<List<GetPopularRestaurantDto>>(HttpStatusCode.OK, "Most popular restaurants from last month retrieved successfully.", result);
    }

}