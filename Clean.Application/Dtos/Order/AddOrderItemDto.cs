namespace Clean.Application.Dtos.Order;

public class AddOrderItemDto
{
    public string MenuItem { get; set; }
    public int Quantity { get; set; }
    public string? SpecialInstructions { get; set; }
}