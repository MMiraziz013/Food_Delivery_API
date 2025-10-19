using System.ComponentModel.DataAnnotations;
using Clean.Application.Dtos.Filters;

namespace Clean.Application.Dtos.Role;

public class GetRolePermissionFilter : PaginationFilter
{
    [Required]
    public string RoleId { get; set; }

    public GetRolePermissionFilter() : base()
    {

    }
    public string? Search { get; set; }
    public GetRolePermissionFilter(int pageNumber, int pageSize) : base(pageNumber, pageSize)
    {

    }
}