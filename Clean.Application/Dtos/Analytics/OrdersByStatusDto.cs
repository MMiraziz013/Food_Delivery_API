using Clean.Application.Dtos.Order;
using Clean.Domain.Enums;

namespace Clean.Application.Dtos.Analytics;

public class OrdersByStatusDto
{
    public OrderStatus Status { get; set; }
    public int Count { get; set; }
}
