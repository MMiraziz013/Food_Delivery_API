using System.Net;
using Clean.Application.Abstractions;
using Clean.Application.Dtos.Filters;
using Clean.Application.Dtos.Menu;
using Clean.Application.Dtos.Order;
using Clean.Application.Dtos.OrderDetails;
using Clean.Application.Responses;
using Clean.Domain.Entities;
using Clean.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Clean.Application.Services.Order;

public class OrderService : IOrderService
{
    private readonly IDataContext _context;
    private readonly UserManager<Domain.Entities.User> _userManager;

    public OrderService(IDataContext context, UserManager<Domain.Entities.User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    public async Task<PaginatedResponse<GetOrderDto>> GetOrdersAsync(OrderPaginationFilter pagination, string userId)
    {
        var query = _context.Orders
            .Include(o => o.Restaurant)
            .Include(o=> o.User)
            .Include(o=> o.Courier)
            .ThenInclude(c=> c.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.FromRestaurant))
        {
            query = query.Where(r => r.Restaurant != null &&
                                     r.Restaurant.Name.ToLower() == pagination.FromRestaurant.ToLower());
        }


        if (pagination.PaymentMethod.HasValue)
            query = query.Where(o => o.PaymentMethod == pagination.PaymentMethod.Value);

        if (pagination.OrderStatus.HasValue)
            query = query.Where(o => o.OrderStatus == pagination.OrderStatus.Value);

        query = query.Where(o => o.UserId.ToString() == userId);
        
        var total = await query.CountAsync();
        

        query = query.OrderBy(b => b.Id)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize);

        var orderItems = await query.Select(o => new GetOrderDto()
        {
            Id = o.Id,
            OrderStatus = o.OrderStatus,
            CreatedAt = o.CreatedAt,
            DeliveredAt = o.DeliveredAt,
            TotalAmount = o.OrderDetails.Sum(or=> or.Price),
            DeliveryAddress = o.DeliveryAddress,
            PaymentMethod = o.PaymentMethod,
            PaymentStatus = o.PaymentStatus,
            RestaurantName = o.Restaurant.Name,
            CourierName = o.Courier.User.Name,
            OrderDetails = o.OrderDetails.Select(or=> new GetOrderDetailDto
            {
                OrderId = or.OrderId,
                MenuItem = or.MenuItem.Name,
                Quantity = or.Quantity,
                Price = or.Price,
                SpecialInstructions = or.SpecialInstructions,
            }).ToList(),
        }).ToListAsync();

        return new PaginatedResponse<GetOrderDto>(orderItems, pagination.PageNumber, pagination.PageSize, total);
    }

    public async Task<Response<GetOrderDto>> AddOrderAsync(AddOrderDto dto, string userId)
    {
        var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Name.ToLower() == dto.FromRestaurant.ToLower());

        if (restaurant is null)
            return new Response<GetOrderDto>(HttpStatusCode.BadRequest, "No such restaurant found.");

        var courier = await _context.Couriers
            .Include(c=> c.User)
            .FirstOrDefaultAsync(c => c.Status == CourierStatus.Active);

        if (courier is null)
            return new Response<GetOrderDto>(HttpStatusCode.BadRequest, "No available couriers. Please try later.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return new Response<GetOrderDto>(HttpStatusCode.BadRequest, "Invalid user.");

        var orderDetails = new List<OrderDetail>();
        decimal totalAmount = 0;

        foreach (var itemDto in dto.OrderItems)
        {
            var menuItem = await _context.Menus
                .FirstOrDefaultAsync(m => m.Name.ToLower() == itemDto.MenuItem.ToLower());

            if (menuItem is null)
                return new Response<GetOrderDto>(HttpStatusCode.BadRequest, $"Menu item '{itemDto.MenuItem}' not found.");

            var orderDetail = new OrderDetail
            {
                MenuItem = menuItem,
                MenuItemId = menuItem.Id,
                Quantity = itemDto.Quantity,
                Price = menuItem.Price * itemDto.Quantity,
                SpecialInstructions = itemDto.SpecialInstructions
            };

            totalAmount += orderDetail.Price;
            orderDetails.Add(orderDetail);
        }

        var order = new Domain.Entities.Order
        {
            OrderStatus = OrderStatus.Created,
            DeliveryAddress = dto.DeliveryAddress,
            PaymentMethod = dto.PaymentMethod,
            PaymentStatus = PaymentStatus.Completed,
            User = user,
            UserId = user.Id,
            Restaurant = restaurant,
            RestaurantId = restaurant.Id,
            Courier = courier,
            CourierId = courier.Id,
            OrderDetails = orderDetails,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        var result = new GetOrderDto
        {
            Id = order.Id,
            OrderStatus = order.OrderStatus,
            DeliveryAddress = order.DeliveryAddress,
            PaymentMethod = order.PaymentMethod,
            PaymentStatus = order.PaymentStatus,
            RestaurantName = order.Restaurant.Name,
            CourierName = order.Courier.User.Name,
            TotalAmount = totalAmount,
            CreatedAt = order.CreatedAt,
            OrderDetails = orderDetails.Select(od => new GetOrderDetailDto
            {
                OrderId = order.Id,
                MenuItem = od.MenuItem.Name,
                Quantity = od.Quantity,
                Price = od.Price,
                SpecialInstructions = od.SpecialInstructions
            }).ToList()
        };

        return new Response<GetOrderDto>(HttpStatusCode.Created, "Order created successfully.", result);
    }


    public async Task<Response<string>> UpdateOrderAsync(UpdateOrderDto dto)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == dto.Id);

        if (order is null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Such order is not found.");

        }

        if (order.OrderStatus != OrderStatus.Created || order.OrderStatus != OrderStatus.Confirmed)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Order cannot be updated once in progress.");

        }


        if (string.IsNullOrWhiteSpace(dto.DeliveryAddress) == false)
        {
            order.DeliveryAddress = dto.DeliveryAddress;
        }

        if (dto.PaymentMethod.HasValue)
        {
            order.PaymentMethod = dto.PaymentMethod.Value;
        }
        
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        return new Response<string>(HttpStatusCode.OK, "Order updated successfully.");
    }


    public async Task<Response<string>> CancelOrderByIdAsync(int id)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Such order is not found.");
        }

        if (order.OrderStatus == OrderStatus.InProgress)
        {
            return new Response<string>(HttpStatusCode.BadRequest,
                "You cannot cancel this order, it's in progress already");
        }

        if (order.OrderStatus is OrderStatus.Delivered)
        {
            return new Response<string>(HttpStatusCode.BadRequest,
                "You cannot cancel this order already, it's delivered!");
        }

        order.OrderStatus = OrderStatus.Cancelled;
        order.PaymentStatus = PaymentStatus.Refunded;
        var isCancelled = await _context.SaveChangesAsync();

        return new Response<string>(HttpStatusCode.OK, "Order cancelled");
    }
}

//TODO: Check the OrderService implementation