using System.Security.Claims;
using Clean.Application.Services.Permission;
using Clean.Application.Security.Permission;
using Clean.Domain.Entities;
using Clean.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Clean.Infrastructure.Data.Seed;

public class IdentitySeeder
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public IdentitySeeder(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedAdminUserAsync();
        await SeedPermissionsAsync(); // 👈 Attach permissions to roles
    }

    private async Task SeedRolesAsync()
    {
        var roles = new List<string> { RoleConstants.Admin, RoleConstants.Client, RoleConstants.Courier };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole<int>(role));
        }
    }

    private async Task SeedAdminUserAsync()
    {
        var existing = await _userManager.FindByNameAsync("admin");
        if (existing != null) return;

        var admin = new User
        {
            UserName = "admin",
            Email = "admin@gmail.com",
            PhoneNumber = "123456789",
            Address = "6th Ave, Queens, NYC",
            Name = "System Administrator",
            Role = UserRole.Admin
        };

        var result = await _userManager.CreateAsync(admin, "myPassword1$");
        if (result.Succeeded)
            await _userManager.AddToRoleAsync(admin, RoleConstants.Admin);
    }

    private async Task SeedPermissionsAsync()
    {
        foreach (var roleName in RolePermissionService.GetAllRoles())
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) continue;

            var permissions = RolePermissionService.GetPermissionsByRoles(new[] { roleName });

            var existingClaims = await _roleManager.GetClaimsAsync(role);
            foreach (var permission in permissions)
            {
                if (!existingClaims.Any(c => c.Type == "Permission" && c.Value == permission))
                {
                    await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                }
            }
        }
    }
}
