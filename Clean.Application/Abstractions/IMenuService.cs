using Clean.Application.Dtos.Filters;
using Clean.Application.Dtos.Menu;
using Clean.Application.Dtos.Restaurant;
using Clean.Application.Responses;

namespace Clean.Application.Abstractions;

public interface IMenuService
{
    public Task<PaginatedResponse<GetMenuDto>> GetMenusAsync(MenuPaginationFilter pagination);
    public Task<Response<GetMenuDto>> AddMenuAsync(AddMenuItemDto dto);
    public Task<Response<string>> UpdateMenuAsync(UpdateMenuItemDto dto);
    public Task<Response<string>> DeactivateMenuByIdAsync(int id);
}