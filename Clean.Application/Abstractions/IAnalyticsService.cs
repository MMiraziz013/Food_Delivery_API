using Clean.Application.Dtos.Analytics;
using Clean.Application.Dtos.Courier;
using Clean.Application.Dtos.Menu;
using Clean.Application.Dtos.Order;
using Clean.Application.Dtos.Restaurant;
using Clean.Application.Responses;

namespace Clean.Application.Abstractions;

public interface IAnalyticsService
{
    public Task<Response<List<GetRestaurantDto>>> GetActiveRestaurantsByRatingAsync();
    public Task<Response<List<GetMenuDto>>> GetDishesByPriceAsync(decimal price);
    public Task<Response<List<OrdersByStatusDto>>> GetOrdersByStatusCountAsync();
    public Task<Response<List<AvgPriceByCategoryDto>>> GetAvgPriceByCategoryAsync();
    public Task<Response<List<UserTotalOrdersDto>>> GetUsersWithTotalOrdersAsync();
    public Task<Response<CourierDeliverOrdersDto>> GetOrdersDeliverByCourier(int courierId);
    public Task<Response<List<GetOrderDto>>> GetTotalOrdersForTodayAsync();
    public Task<Response<List<GetCourierDto>>> GetTopFiveCouriersAsync();
    public Task<Response<List<GetOrderDto>>> OrdersAboveAverageAsync();
    public Task<Response<List<GetPopularRestaurantDto>>> GetMostPopularRestaurantsLastMonthAsync();
}