using Clean.Application.Dtos.Order;
using Clean.Domain.Enums;

namespace Clean.Application.Dtos.Courier;

public class GetCourierDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public CourierStatus Status { get; set; }
    public string CurrentLocation { get; set; }
    public decimal Rating { get; set; }
    public TransportType TransportType { get; set; }
    public string UserName { get; set; }

    public List<GetOrderDto> Orders { get; set; }
}