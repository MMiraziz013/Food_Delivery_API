using System.Net;
using Clean.Application.Abstractions;
using Clean.Application.Dtos.Filters;
using Clean.Application.Dtos.Menu;
using Clean.Application.Dtos.Restaurant;
using Clean.Application.Responses;
using Microsoft.EntityFrameworkCore;

namespace Clean.Application.Services.Restaurant;

public class RestaurantService : IRestaurantService
{
    private readonly IDataContext _context;

    public RestaurantService(IDataContext context)
    {
        _context = context;
    }
    
    public async Task<PaginatedResponse<GetRestaurantDto>> GetRestaurantsAsync(RestaurantPaginationFilter pagination)
    {
        var query = _context.Restaurants.AsQueryable();
        var total = await query.CountAsync();

        if (pagination.MaxRating > 0)
        {
            query = query.Where(r => r.Rating <= pagination.MaxRating);
        }

        if (pagination.MinRating > 0)
        {
            query = query.Where(r => r.Rating >= pagination.MinRating);
        }

        query = query.Skip((pagination.PageNumber - 1) * pagination.PageSize).Take(pagination.PageSize);

        query = query.OrderBy(b => b.Id);

        var restaurants = await query.Select(r => new GetRestaurantDto
        {
            Name = r.Name,
            Address = r.Address,
            ContactPhone = r.ContactPhone,
            Description = r.Description,
            DeliveryPrice = r.DeliveryPrice,
            IsActive = r.IsActive,
            MinOrderAmount = r.MinOrderAmount,
            Rating = r.Rating,
            WorkingHours = r.WorkingHours,
            Menus = r.Menus.Select(m => new GetMenuDto
            {
                Name = m.Name,
                Category = m.Category,
                Description = m.Description,
                IsAvailable = m.IsAvailable,
                PhotoUrl = m.PhotoUrl,
                PreparationTime = m.PreparationTime,
                Weight = m.Weight,
                Price = m.Price
            }).ToList(),
        }).ToListAsync();

        return new PaginatedResponse<GetRestaurantDto>(restaurants, pagination.PageNumber, pagination.PageSize, total);
    }

    public async Task<Response<GetRestaurantDto>> GetRestaurantByIdAsync(int id)
    {
        var restaurant = await _context.Restaurants.FindAsync(id);
        if (restaurant is null)
        {
            return new Response<GetRestaurantDto>(HttpStatusCode.BadGateway,
                message: "No such restaurant in the system");
        }

        var restaurantDto = new GetRestaurantDto
        {
            Name = restaurant.Name,
            Address = restaurant.Address,
            ContactPhone = restaurant.ContactPhone,
            Description = restaurant.Description,
            DeliveryPrice = restaurant.DeliveryPrice,
            IsActive = restaurant.IsActive,
            MinOrderAmount = restaurant.MinOrderAmount,
            Rating = restaurant.Rating,
            WorkingHours = restaurant.WorkingHours,
            Menus = restaurant.Menus.Select(m => new GetMenuDto
            {
                Name = m.Name,
                Category = m.Category,
                Description = m.Description,
                IsAvailable = m.IsAvailable,
                PhotoUrl = m.PhotoUrl,
                PreparationTime = m.PreparationTime,
                Weight = m.Weight,
                Price = m.Price
            }).ToList(),
        };

        return new Response<GetRestaurantDto>(HttpStatusCode.OK, "Restaurant retrieved successfully", restaurantDto);
    }

    public async Task<Response<GetRestaurantDto>> AddRestaurantAsync(AddRestaurantDto dto)
    {
        var exists = await _context.Restaurants.AnyAsync(r => r.Name.ToLower() == dto.Name.ToLower());
        if (exists)
        {
            return new Response<GetRestaurantDto>(HttpStatusCode.BadGateway, "This restaurant already exists!");
        }

        var restaurant = new Domain.Entities.Restaurant
        {
            Name = dto.Name,
            Address = dto.Address,
            ContactPhone = dto.ContactPhone,
            Description = dto.Description,
            DeliveryPrice = dto.DeliveryPrice,
            IsActive = true,
            WorkingHours = dto.WorkingHours,
            Rating = 0m,
            MinOrderAmount = dto.MinOrderAmount,
        };

        await _context.Restaurants.AddAsync(restaurant);
        var isAdded = await _context.SaveChangesAsync();
        if (isAdded > 0)
        {
            return new Response<GetRestaurantDto>(HttpStatusCode.OK, "Restaurant Added!");
        }

        return new Response<GetRestaurantDto>(HttpStatusCode.BadGateway, "Error while adding the restaurant");
    }

    public async Task<Response<string>> UpdateRestaurantAsync(UpdateRestaurantDto dto)
    {
        var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == dto.Id);
        
        if (restaurant is null)
        {
            return new Response<string>(HttpStatusCode.BadGateway, "This restaurant doesn't exists!");
        }

        restaurant.Name = dto.Name;
        restaurant.Description = dto.Description!;
        restaurant.Address = dto.Address;
        restaurant.ContactPhone = dto.ContactPhone;
        restaurant.DeliveryPrice = dto.DeliveryPrice;
        restaurant.IsActive = dto.IsActive;
        restaurant.WorkingHours = dto.WorkingHours;
        restaurant.MinOrderAmount = dto.MinOrderAmount;

        var isUpdated = await _context.SaveChangesAsync();
        if (isUpdated > 0)
        {
            return new Response<string>(HttpStatusCode.OK, "Restaurant Updated!");
        }

        return new Response<string>(HttpStatusCode.BadGateway, "Failed to Update the Restaurant");
    }

    public async Task<Response<string>> DeactivateRestaurantByIdAsync(int id)
    {
        var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == id);
        if (restaurant is null)
        {
            return new Response<string>(HttpStatusCode.BadGateway, "This restaurant doesn't exists!");
        }

        restaurant.IsActive = false;
        var isDeactivated = await _context.SaveChangesAsync();
        if (isDeactivated > 0)
        {
            return new Response<string>(HttpStatusCode.OK, "Restaurant Deactivated!");
        }

        return new Response<string>(HttpStatusCode.BadGateway, "Failed to Deactivate the Restaurant");
    }
}