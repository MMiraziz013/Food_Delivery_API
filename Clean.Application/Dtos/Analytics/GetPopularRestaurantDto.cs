namespace Clean.Application.Dtos.Analytics;


public class GetPopularRestaurantDto
{
    public int RestaurantId { get; set; }
    public string Name { get; set; }
    public decimal Rating { get; set; }
    public int OrderCount { get; set; }
}