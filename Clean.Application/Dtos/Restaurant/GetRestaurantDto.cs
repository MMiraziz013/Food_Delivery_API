using Clean.Application.Dtos.Menu;
using Clean.Application.Dtos.Order;

namespace Clean.Application.Dtos.Restaurant;

public class GetRestaurantDto
{
    public string Name { get; set; }
    public string Address { get; set; }
    public decimal Rating { get; set; }
    public string WorkingHours { get; set; }
    public string? Description { get; set; }
    public string ContactPhone { get; set; }
    public bool IsActive { get; set; }
    public decimal MinOrderAmount { get; set; }
    public decimal DeliveryPrice { get; set; }

    public List<GetMenuDto> Menus { get; set; }
}