using System.Security.Claims;
using Clean.Application.Abstractions;
using Clean.Application.Dtos.User;
using Clean.Application.Security.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Food_Delivery_API.Controllers;

[ApiController]
[Route("api/user/[controller]")]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUserAsync(RegisterUserDto dto)
    {
        var response = await _userService.RegisterUserAsync(dto);
        return Ok(response);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> RegisterUserAsync(LoginDto dto)
    {
        var response = await _userService.LoginUserAsync(dto);
        return Ok(response);
    }

    [HttpPut("update-password")]
    [Authorize]
    public async Task<IActionResult> UpdateMyPasswordAsync(UpdatePasswordDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var response = await _userService.UpdatePasswordAsync(dto, userId!);
        return Ok(response);
    }

    [HttpPut("update-profile")]
    [PermissionAuthorize(PermissionConstants.User.Manage)]
    public async Task<IActionResult> UpdateMyProfileAsync(UpdateUserProfileDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var response = await _userService.UpdateMyProfileAsync(dto, userId!);
        return Ok(response);
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetMyProfileInfoAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var response = await _userService.GetUserProfileAsync(userId!);
        return Ok(response);
    }
}
