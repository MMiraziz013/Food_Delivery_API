using System.Net;
using Clean.Application.Abstractions;
using Clean.Application.Dtos.Courier;
using Clean.Application.Responses;
using Clean.Application.Security.Permission;
using Clean.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Clean.Application.Services.Courier;

public class CourierService
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

}