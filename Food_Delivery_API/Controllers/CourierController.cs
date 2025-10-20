using System.Security.Claims;
using Clean.Application.Abstractions;
using Clean.Application.Dtos.Courier;
using Clean.Application.Security.Permission;
using Microsoft.AspNetCore.Mvc;

namespace Food_Delivery_API.Controllers;

[ApiController]
[Route("api/courier/[controller]")]
public class CourierController : Controller
{
    private readonly ICourierService _courierService;

    public CourierController(ICourierService courierService)
    {
        _courierService = courierService;
    }

    [HttpPost("become-courier")]
    [PermissionAuthorize(PermissionConstants.User.Manage)]
    public async Task<IActionResult> BecomeCourierAsync(CreateCourierDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var response = await _courierService.PromoteToCourierAsync(userId!, dto);
        return Ok(response);
    }

    [HttpOptions("register-courier/{id}")]
    [PermissionAuthorize(PermissionConstants.Couriers.Manage)]
    public async Task<IActionResult> RegisterCourierAsync(string id, CreateCourierDto dto)
    {
        var response = await _courierService.PromoteToCourierAsync(id, dto);
        return Ok(response);
    }

    [HttpGet("get-couriers")]
    [PermissionAuthorize(PermissionConstants.Couriers.View)]
    public async Task<IActionResult> GetCouriersAsync()
    {
        var response = await _courierService.GetCouriersAsync();
        return Ok(response);
    }

    [HttpPut("edit-courier")]
    [PermissionAuthorize(PermissionConstants.Couriers.Manage)]
    public async Task<IActionResult> UpdateCourierAsync(UpdateCourierDto dto)
    {
        var response = await _courierService.UpdateCourierProfileAsync(dto);
        return Ok(response);
    }

    [HttpPut("complete-order")]
    [PermissionAuthorize(PermissionConstants.Orders.Manage)]
    public async Task<IActionResult> CompleteOrderAsync(int orderId)
    {
        var response = await _courierService.CompleteOrderAsync(orderId);
        return Ok(response);
    }

    [HttpDelete("delete-courier")]
    [PermissionAuthorize(PermissionConstants.Couriers.Manage)]
    public async Task<IActionResult> DeleteCourierAsync(int courierId)
    {
        var response = await _courierService.DeleteCourierAccountAsync(courierId);
        return Ok(response);
    }
}