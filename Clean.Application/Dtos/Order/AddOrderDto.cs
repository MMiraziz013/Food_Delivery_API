using Clean.Application.Dtos.OrderDetails;
using Clean.Domain.Entities;
using Clean.Domain.Enums;

namespace Clean.Application.Dtos.Order;

public class AddOrderDto
{
    public string DeliveryAddress { get; set; }
    public PaymentMethod PaymentMethod { get; set; }

    public string OrderedBy { get; set; }
    public string FromRestaurant { get; set; }
    public string AppointedCourier { get; set; }
    public string FirstMenuItem { get; set; }

    public List<AddOrderItemDto> OrderItems { get; set; }
}