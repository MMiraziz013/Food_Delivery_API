using Clean.Application.Dtos.Order;
using Clean.Application.Dtos.User;
using Clean.Application.Responses;

namespace Clean.Application.Abstractions;

public interface IUserService
{
    public Task<Response<string>> RegisterUserAsync(RegisterUserDto register);

    public Task<Response<object>> LoginUserAsync(LoginDto login);
    
    public Task<Response<UserProfileDto>> GetUserProfileAsync(string userId);
    
    public Task<Response<string>> UpdatePasswordAsync(UpdatePasswordDto dto, string userId);

    public Task<Response<string>> UpdateMyProfileAsync(UpdateUserProfileDto update, string userId);

    public Task<Response<List<GetOrderDto>>> GetMyOrdersAsync();
}