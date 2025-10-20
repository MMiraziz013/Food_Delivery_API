using System.Security.Claims;
using Clean.Application.Abstractions;
using Clean.Application.Dtos.Filters;
using Clean.Application.Dtos.Order;
using Clean.Application.Security.Permission;
using Microsoft.AspNetCore.Mvc;

namespace Food_Delivery_API.Controllers;

[ApiController]
[Route("api/order/[controller]")]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("get-orders")]
    [PermissionAuthorize(PermissionConstants.Orders.View)]
    public async Task<IActionResult> GetMyOrdersAsync([FromQuery] OrderPaginationFilter filter)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var response = await _orderService.GetOrdersAsync(filter, userId!);
        return Ok(response);
    }
    
    [HttpPost("create-order")]
    [PermissionAuthorize(PermissionConstants.Orders.Create)]
    public async Task<IActionResult> CreateOrderAsync(AddOrderDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var response = await _orderService.AddOrderAsync(dto, userId!);
        return Ok(response);
    }

    [HttpPut("update-order")]
    [PermissionAuthorize(PermissionConstants.Orders.Manage)]
    public async Task<IActionResult> UpdateOrderAsync(UpdateOrderDto dto)
    {
        var response = await _orderService.UpdateOrderAsync(dto);
        return Ok(response);
    }

    [HttpPut("cancel-order")]
    [PermissionAuthorize(PermissionConstants.Orders.Manage)]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var response = await _orderService.CancelOrderByIdAsync(id);
        return Ok(response);
    }
}