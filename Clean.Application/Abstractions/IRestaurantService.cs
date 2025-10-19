using Clean.Application.Dtos.Filters;
using Clean.Application.Dtos.Restaurant;
using Clean.Application.Responses;

namespace Clean.Application.Abstractions;

public interface IRestaurantService
{
    public Task<PaginatedResponse<GetRestaurantDto>> GetRestaurantsAsync(RestaurantPaginationFilter pagination);
    public Task<Response<GetRestaurantDto>> GetRestaurantByIdAsync(int id);
    public Task<Response<GetRestaurantDto>> AddRestaurantAsync(AddRestaurantDto dto);
    public Task<Response<string>> UpdateRestaurantAsync(UpdateRestaurantDto dto);
    public Task<Response<string>> DeactivateRestaurantByIdAsync(int id);
}