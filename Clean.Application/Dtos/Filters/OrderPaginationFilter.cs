using Clean.Domain.Enums;

namespace Clean.Application.Dtos.Filters;

public class OrderPaginationFilter : PaginationFilter
{
    public OrderStatus? OrderStatus { get; set; }
    public string? FromRestaurant { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
}