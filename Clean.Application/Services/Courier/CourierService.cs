using System.Net;
using Clean.Application.Abstractions;
using Clean.Application.Dtos.Courier;
using Clean.Application.Dtos.Order;
using Clean.Application.Dtos.OrderDetails;
using Clean.Application.Responses;
using Clean.Application.Security.Permission;
using Clean.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Clean.Application.Services.Courier;

public class CourierService : ICourierService
{
    private readonly IDataContext _context;
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public CourierService(
        IDataContext context, 
        UserManager<Domain.Entities.User> userManager, 
        RoleManager<IdentityRole<int>> roleManager
        )
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    public async Task<Response<string>> PromoteToCourierAsync(string userId, CreateCourierDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        await _userManager.AddToRoleAsync(user, RoleConstants.Courier);
        user.Role = UserRole.Courier;
        await _userManager.UpdateAsync(user);

        var courier = new Domain.Entities.Courier
        {
            UserId = user.Id,
            User = user,
            Status = CourierStatus.Active,
            TransportType = dto.TransportType,
            CurrentLocation = dto.CurrentLocation,
            Rating = 0,
        };
        _context.Couriers.Add(courier);
        await _context.SaveChangesAsync();

        return new Response<string>(HttpStatusCode.OK, "User became a courier!");
    }

    public async Task<Response<string>> CompleteOrderAsync(int orderId)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order is null)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "No such order to complete");
        }

        order.OrderStatus = OrderStatus.Delivered;
        order.DeliveredAt = DateTime.UtcNow;

        var isUpdated = await _context.SaveChangesAsync();
        if (isUpdated > 0)
        {
            return new Response<string>(HttpStatusCode.OK, message: "order delivered!");

        }
        
        return new Response<string>(HttpStatusCode.BadRequest, "Error while completing the order, try again");


    }

    public async Task<Response<GetCourierDto>> UpdateCourierProfileAsync(UpdateCourierDto dto)
    {
        var courier = await _context.Couriers
            .Include(c=> c.User)
            .FirstOrDefaultAsync(c => c.Id == dto.Id);
        if (courier is null)
        {
            return new Response<GetCourierDto>(HttpStatusCode.BadRequest, "No such courier in the database");
        }

        courier.Status = dto.Status;
        courier.CurrentLocation = dto.CurrentLocation;
        courier.Rating = dto.Rating;
        courier.TransportType = dto.TransportType;

        var isUpdated = await _context.SaveChangesAsync();
        if (isUpdated > 0)
        {
            var updatedCourier = new GetCourierDto
            {
                Id = courier.Id,
                CurrentLocation = courier.CurrentLocation,
                Rating = courier.Rating,
                Status = courier.Status,
                TransportType = courier.TransportType,
                UserId = courier.UserId,
                UserName = courier.User.UserName!
            };

            return new Response<GetCourierDto>(HttpStatusCode.OK, "Courier Profile Update Successfully", updatedCourier);
        }
        
        
        return new Response<GetCourierDto>(HttpStatusCode.BadRequest, "Error while updating courier profile");
    }

    public async Task<Response<string>> DeleteCourierAccountAsync(int courierId)
    {
        var courier = await _context.Couriers.FirstOrDefaultAsync(c => c.Id == courierId);
        if (courier is null)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "No such courier in the database");

        }

        _context.Couriers.Remove(courier);
        var isRemoved = await _context.SaveChangesAsync();
        if (isRemoved > 0)
        {
            return new Response<string>(HttpStatusCode.OK, "Courier Profile Deleted Successfully");
        }
        
        return new Response<string>(HttpStatusCode.BadRequest, "Error while deleting courier profile");
    }

    public async Task<Response<List<GetCourierDto>>> GetCouriersAsync()
    {
        var couriers = await _context.Couriers
            .Include(c => c.User)
            .Include(c => c.Orders)
            .ThenInclude(o => o.Restaurant)
            .Include(c => c.Orders)
            .ThenInclude(o => o.OrderDetails)
            .ThenInclude(od => od.MenuItem)
            .Where(c => c.Status != CourierStatus.Inactive)
            .ToListAsync();


        var courierList = couriers.Select(c => new GetCourierDto
        {
            Id = c.Id,
            CurrentLocation = c.CurrentLocation,
            Rating = c.Rating,
            Status = c.Status,
            TransportType = c.TransportType,
            UserId = c.UserId,
            UserName = c.User.UserName!,
            Orders = c.Orders.Select(o => new GetOrderDto
            {
                Id = o.Id,
                OrderStatus = o.OrderStatus,
                DeliveryAddress = o.DeliveryAddress,
                PaymentMethod = o.PaymentMethod,
                PaymentStatus = o.PaymentStatus,
                RestaurantName = o.Restaurant.Name,
                CourierName = o.Courier.User.Name,
                TotalAmount = o.OrderDetails.Sum(item => item.Price),
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
        }).ToList();

        return new Response<List<GetCourierDto>>(HttpStatusCode.OK, "Couriers retrieved!", courierList);
    }
}