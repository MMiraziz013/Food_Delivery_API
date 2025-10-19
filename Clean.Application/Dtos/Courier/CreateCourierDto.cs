using Clean.Domain.Enums;

namespace Clean.Application.Dtos.Courier;

public class CreateCourierDto
{
    public string CurrentLocation { get; set; }
    public TransportType TransportType { get; set; }
}