namespace Clean.Application.Dtos.Filters;

public class RestaurantPaginationFilter : PaginationFilter
{
    public decimal MaxRating { get; set; }
    public decimal MinRating { get; set; }

    public RestaurantPaginationFilter()
    {
        
    }
}