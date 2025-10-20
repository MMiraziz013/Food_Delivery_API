using Clean.Application.Dtos.Order;

namespace Clean.Application.Dtos.Analytics;

public class CourierDeliverOrdersDto
{
    public int CourierId { get; set; }
    public string Username { get; set; }
    public List<GetOrderDto> DeliveredOrders { get; set; }
}