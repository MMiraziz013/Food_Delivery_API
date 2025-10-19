namespace Clean.Domain.Entities;

public class Restaurant
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public decimal Rating { get; set; }
    public string WorkingHours { get; set; }
    public string Description { get; set; }
    public string ContactPhone { get; set; }
    public bool IsActive { get; set; }
    public decimal MinOrderAmount { get; set; }
    public decimal DeliveryPrice { get; set; }

    public List<Menu> Menus { get; set; } = new List<Menu>();
    public List<Order> Orders { get; set; } = new List<Order>();
}