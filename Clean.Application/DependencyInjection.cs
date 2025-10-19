using Clean.Application.Abstractions;
using Clean.Application.Services.Jwt;
using Clean.Application.Services.Menu;
using Clean.Application.Services.Permission;
using Clean.Application.Services.Restaurant;
using Clean.Application.Services.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clean.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IRestaurantService, RestaurantService>();
        services.AddTransient<IMenuService, MenuService>();
        
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddTransient<IJwtTokenService, JwtTokenService>();
        
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<JwtTokenService>(configuration.GetSection(JwtOptions.SectionName));
        
        return services;
    }
}