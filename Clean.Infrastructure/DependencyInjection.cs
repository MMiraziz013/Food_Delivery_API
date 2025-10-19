using Clean.Application.Abstractions;
using Clean.Infrastructure.Data;
using Clean.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clean.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IDataContext>(provider => provider.GetRequiredService<DataContext>());

        
        services.AddScoped<DomainSeeder>();
        services.AddScoped<IdentitySeeder>();

        return services;
    }
}