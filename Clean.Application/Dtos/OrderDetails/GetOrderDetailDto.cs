namespace Clean.Application.Dtos.OrderDetails;

public class GetOrderDetailDto
{
    public int OrderId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string SpecialInstructions { get; set; }

    public string MenuItem { get; set; }
}