using Clean.Application.Dtos.Role;
using Clean.Application.Responses;

namespace Clean.Application.Services.Permission;

public interface IPermissionService
{
    Task<PaginatedResponse<RoleClaimDto>> GetPermissionsByRoleId(GetRolePermissionFilter filter);
    Task<Response<RoleClaimDto>> UpdatePermission(RoleClaimDto permission);
    Task<Response<List<RoleDto>>> GetRoles();
}