using Clean.Application.Abstractions;
using Clean.Application.Dtos.Filters;
using Clean.Application.Dtos.Restaurant;
using Clean.Application.Security.Permission;
using Microsoft.AspNetCore.Mvc;

namespace Food_Delivery_API.Controllers;

[ApiController]
[Route("api/restaurant/[controller]")]
public class RestaurantController : Controller
{
    private readonly IRestaurantService _restaurantService;

    public RestaurantController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    [HttpPost("add-restaurant")]
    [PermissionAuthorize(PermissionConstants.Restaurants.Manage)]
    public async Task<IActionResult> AddRestaurantAsync(AddRestaurantDto dto)
    {
        var response = await _restaurantService.AddRestaurantAsync(dto);
        return Ok(response);
    }

    [HttpGet("get-restaurants")]
    [PermissionAuthorize(PermissionConstants.Restaurants.View)]
    public async Task<IActionResult> GetRestaurantPaginatedAsync([FromQuery]RestaurantPaginationFilter filter)
    {
        var response = await _restaurantService.GetRestaurantsAsync(filter);
        return Ok(response);
    }

    [HttpGet("get-restaurant")]
    [PermissionAuthorize(PermissionConstants.Restaurants.View)]
    public async Task<IActionResult> GetRestaurantByIdAsync(int id)
    {
        var response = await _restaurantService.GetRestaurantByIdAsync(id);
        return Ok(response);
    }

    [HttpPut("update-restaurant")]
    [PermissionAuthorize(PermissionConstants.Restaurants.Manage)]
    public async Task<IActionResult> UpdateRestaurantAsync(UpdateRestaurantDto dto)
    {
        var response = await _restaurantService.UpdateRestaurantAsync(dto);
        return Ok(response);
    }

    [HttpPut("deactivate-restaurant")]
    [PermissionAuthorize(PermissionConstants.Restaurants.Manage)]
    public async Task<IActionResult> DeactivateRestaurantAsync([FromQuery] int id)
    {
        var response = await _restaurantService.DeactivateRestaurantByIdAsync(id);
        return Ok(response);
    }
}