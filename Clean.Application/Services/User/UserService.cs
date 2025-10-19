using System.Net;
using Clean.Application.Abstractions;
using Clean.Application.Dtos.Order;
using Clean.Application.Dtos.User;
using Clean.Application.Responses;
using Clean.Application.Security.Permission;
using Clean.Application.Services.Jwt;
using Clean.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Clean.Application.Services.User;

public class UserService : IUserService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IJwtTokenService _tokenService;

    public UserService(
        UserManager<Domain.Entities.User> userManager, 
        RoleManager<IdentityRole<int>> roleManager, 
        IConfiguration configuration,
        IJwtTokenService tokenService)
    {
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
    }
    
    public async Task<Response<string>> RegisterUserAsync(RegisterUserDto register)
    {
        var mappedUser = new Domain.Entities.User
        {
            UserName = register.Username,
            Name = register.Name,
            Address = register.Address,
            PhoneNumber = register.Phone,
            Email = register.Email,
            Role = UserRole.Client
        };

        var response = await _userManager.CreateAsync(mappedUser, register.Password);
        if (response.Succeeded)
        {
            await _userManager.AddToRoleAsync(mappedUser, "Client");
            return new Response<string>(HttpStatusCode.OK, message: "Registration successful");
        }

        return new Response<string>(HttpStatusCode.BadRequest, response.Errors.Select(e => e.Description).ToList());
    }

    public async Task<Response<object>> LoginUserAsync(LoginDto login)
    {
        var user = await _userManager.FindByNameAsync(login.Username);
        if (user is null)
        {
            return new Response<object>(HttpStatusCode.BadRequest, "No such user in the system");
        }

        var checkPassword = await _userManager.CheckPasswordAsync(user, login.Password);
        if (checkPassword)
        {
            var token = await _tokenService.GenerateJwtToken(user);
            return new Response<object>(HttpStatusCode.OK, "Login Successful", new
            {
                Token = token,
                ExpiresAt = DateTime.Now.AddMinutes(double.Parse(_configuration["JWT:AccessTokenMinutes"]!))
                    .ToString("g")
            });
        }
        
        return new Response<object>(HttpStatusCode.BadRequest, "Incorrect Password");
    }

    public async Task<Response<UserProfileDto>> GetUserProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            throw new ArgumentException("User is unverified, token is expired, please re-login!");
        }

        var profileDto = new UserProfileDto
        {
            Username = user.UserName!,
            Name = user.Name,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber!,
            Address = user.Address!,
            RegistrationDate = user.RegistrationDate.ToString("g"),
            Role = user.Role
        };

        return new Response<UserProfileDto>(HttpStatusCode.OK, "Profile Info Retrieved!",profileDto);
    }

    public async Task<Response<string>> UpdatePasswordAsync(UpdatePasswordDto dto, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user!, dto.CurrentPassword);
        if (isPasswordCorrect)
        {
            var result = await _userManager.ChangePasswordAsync(user!, dto.CurrentPassword, dto.NewPassword);
            if (result.Succeeded)
            {
                return new Response<string>(HttpStatusCode.OK, message: "Password Changed Successfully");
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new Response<string>(HttpStatusCode.BadRequest, message: "Password change failed", errors);
        }

        return new Response<string>(HttpStatusCode.BadRequest, message: "Incorrect Current Password!");
    }

    public async Task<Response<string>> UpdateMyProfileAsync(UpdateUserProfileDto update, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        user!.Address = update.Address;
        user.PhoneNumber = update.Phone;
        user.Email = update.Email;
        user.UserName = update.Username;
        user.Name = update.Name;
        var isUpdated = await _userManager.UpdateAsync(user);
        
        if (isUpdated.Succeeded)
        {
            return new Response<string>(HttpStatusCode.OK, message: "Profile Updated");
        }

        var errors = string.Join(", ", isUpdated.Errors.Select(e => e.Description));
        return new Response<string>(HttpStatusCode.BadGateway, message: "Error while updating address", errors);
    }

    public async Task<Response<List<GetOrderDto>>> GetMyOrdersAsync()
    {
        throw new NotImplementedException();
    }
    
    
}