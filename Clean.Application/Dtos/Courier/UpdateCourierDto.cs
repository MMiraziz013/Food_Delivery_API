using Clean.Domain.Enums;

namespace Clean.Application.Dtos.Courier;

public class UpdateCourierDto
{
    public int Id { get; set; }
    public CourierStatus Status { get; set; }
    public string CurrentLocation { get; set; }
    public decimal Rating { get; set; }
    public TransportType TransportType { get; set; }
}