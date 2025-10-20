using Clean.Application.Abstractions;
using Clean.Application.Security.Permission;
using Microsoft.AspNetCore.Mvc;

namespace Food_Delivery_API.Controllers;

[ApiController]
[Route("api/analytics/[controller]")]
public class AnalyticsController : Controller
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("get-best-restaurants")]
    [PermissionAuthorize(PermissionConstants.Restaurants.View)]
    public async Task<IActionResult> GetRestaurantsByRatingAsync()
    {
        var response = await _analyticsService.GetActiveRestaurantsByRatingAsync();
        return Ok(response);
    }

    [HttpGet("available-dishes")]
    [PermissionAuthorize(PermissionConstants.Menus.View)]
    public async Task<IActionResult> GetDishesByPrice([FromQuery] decimal maxPrice)
    {
        var response = await _analyticsService.GetDishesByPriceAsync(maxPrice);
        return Ok(response);
    }
    [HttpGet("menus/average-price-by-category")]
    [PermissionAuthorize(PermissionConstants.Menus.View)]
    public async Task<IActionResult> GetAvgPriceByCategoryAsync()
    {
        var response = await _analyticsService.GetAvgPriceByCategoryAsync();
        return Ok(response);
    }

    [HttpGet("users/total-orders")]
    [PermissionAuthorize(PermissionConstants.Orders.View)]
    public async Task<IActionResult> GetUsersWithTotalOrdersAsync()
    {
        var response = await _analyticsService.GetUsersWithTotalOrdersAsync();
        return Ok(response);
    }

    [HttpGet("couriers/{courierId}/delivered-orders")]
    [PermissionAuthorize(PermissionConstants.Couriers.View)]
    public async Task<IActionResult> GetOrdersDeliverByCourier(int courierId)
    {
        var response = await _analyticsService.GetOrdersDeliverByCourier(courierId);
        return Ok(response);
    }

    [HttpGet("orders/today")]
    [PermissionAuthorize(PermissionConstants.Orders.View)]
    public async Task<IActionResult> GetTotalOrdersForTodayAsync()
    {
        var response = await _analyticsService.GetTotalOrdersForTodayAsync();
        return Ok(response);
    }

    [HttpGet("couriers/top-five")]
    [PermissionAuthorize(PermissionConstants.Couriers.View)]
    public async Task<IActionResult> GetTopFiveCouriersAsync()
    {
        var response = await _analyticsService.GetTopFiveCouriersAsync();
        return Ok(response);
    }

    [HttpGet("orders/above-average")]
    [PermissionAuthorize(PermissionConstants.Orders.View)]
    public async Task<IActionResult> OrdersAboveAverageAsync()
    {
        var response = await _analyticsService.OrdersAboveAverageAsync();
        return Ok(response);
    }

    [HttpGet("restaurants/most-popular-last-month")]
    [PermissionAuthorize(PermissionConstants.Restaurants.View)]
    public async Task<IActionResult> GetMostPopularRestaurantsLastMonthAsync()
    {
        var response = await _analyticsService.GetMostPopularRestaurantsLastMonthAsync();
        return Ok(response);
    }
    
}