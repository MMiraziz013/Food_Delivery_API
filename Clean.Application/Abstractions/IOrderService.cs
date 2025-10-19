using Clean.Application.Dtos.Filters;
using Clean.Application.Dtos.Menu;
using Clean.Application.Dtos.Order;
using Clean.Application.Responses;

namespace Clean.Application.Abstractions;

public interface IOrderService
{
    public Task<PaginatedResponse<GetOrderDto>> GetOrdersAsync(OrderPaginationFilter pagination);
    public Task<Response<GetOrderDto>> AddOrderAsync(AddOrderDto dto, string userId);
    public Task<Response<string>> UpdateOrderAsync(UpdateOrderDto dto);
    public Task<Response<string>> CancelOrderByIdAsync(int id);
}