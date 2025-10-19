using Clean.Application.Abstractions;
using Clean.Application.Dtos.Filters;
using Clean.Application.Dtos.Menu;
using Clean.Application.Security.Permission;
using Microsoft.AspNetCore.Mvc;

namespace Food_Delivery_API.Controllers;

[ApiController]
[Route("api/menu/[controller]")]
public class MenuController : Controller
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet("get-menus")]
    [PermissionAuthorize(PermissionConstants.Menus.View)]
    public async Task<IActionResult> GetMenusPaginatedAsync([FromQuery]MenuPaginationFilter filter)
    {
        var response = await _menuService.GetMenusAsync(filter);
        return Ok(response);
    }

    [HttpPost("add-menu-item")] 
    [PermissionAuthorize(PermissionConstants.Menus.Manage)]
    public async Task<IActionResult> AddMenuItemAsync(AddMenuItemDto dto)
    {
        var response = await _menuService.AddMenuAsync(dto);
        return Ok(response);
    }

    [HttpPut("update-menu")]
    [PermissionAuthorize(PermissionConstants.Menus.Manage)]
    public async Task<IActionResult> UpdateMenuItemAsync(UpdateMenuItemDto dto)
    {
        var response = await _menuService.UpdateMenuAsync(dto);
        return Ok(response);
    }

    [HttpPut("deactivate-menu-item")]
    [PermissionAuthorize(PermissionConstants.Menus.Manage)]
    public async Task<IActionResult> DeactivateMenuItemAsync(int id)
    {
        var response = await _menuService.DeactivateMenuByIdAsync(id);
        return Ok(response);
    }
}