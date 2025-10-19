using Clean.Application.Dtos.OrderDetails;
using Clean.Domain.Entities;
using Clean.Domain.Enums;

namespace Clean.Application.Dtos.Order;

public class GetOrderDto
{
    public int Id { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public decimal TotalAmount { get; set; }
    public string DeliveryAddress { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string RestaurantName { get; set; }
    public string CourierName { get; set; }
    public List<GetOrderDetailDto> OrderDetails { get; set; }
}