using Clean.Domain.Enums;

namespace Clean.Application.Dtos.Order;

public class UpdateOrderDto
{
    public int Id { get; set; }
    public string? DeliveryAddress { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
}