using Clean.Application.Security.Permission;

namespace Clean.Application.Services.Permission;

public static class RolePermissionService
{
    private static readonly Dictionary<string, List<string>> _rolePermissions = new()
    {
        {
            RoleConstants.Admin, new List<string>
            {
                PermissionConstants.Restaurants.View,
                PermissionConstants.Restaurants.Manage,
                PermissionConstants.Orders.View,
                PermissionConstants.Orders.Create,
                PermissionConstants.Orders.Manage,
                PermissionConstants.Couriers.View,
                PermissionConstants.Couriers.Manage,
                PermissionConstants.Menus.View,
                PermissionConstants.Menus.Manage,
                PermissionConstants.User.Manage
            }
        },
        {
            RoleConstants.Courier, new List<string>
            {
                PermissionConstants.Orders.View,
                PermissionConstants.Orders.Manage,
                PermissionConstants.Couriers.View,
                PermissionConstants.User.Manage

            }
        },
        {
            RoleConstants.Client, new List<string>
            {
                PermissionConstants.Restaurants.View,
                PermissionConstants.Menus.View,
                PermissionConstants.Orders.Create,
                PermissionConstants.Orders.View,
                PermissionConstants.User.Manage

            }
        }
    };

    public static IEnumerable<string> GetPermissionsByRoles(IEnumerable<string> roles)
    {
        return roles
            .SelectMany(role => _rolePermissions.TryGetValue(role, out var permissions)
                ? permissions
                : Enumerable.Empty<string>())
            .Distinct()
            .ToList();
    }
}
