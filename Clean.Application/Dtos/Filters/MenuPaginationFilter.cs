namespace Clean.Application.Dtos.Filters;

public class MenuPaginationFilter : PaginationFilter
{
    public string? Category { get; set; }
    public string? Restaurant { get; set; }
}