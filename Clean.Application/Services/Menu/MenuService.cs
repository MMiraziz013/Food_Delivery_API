using System.Net;
using Clean.Application.Abstractions;
using Clean.Application.Dtos.Filters;
using Clean.Application.Dtos.Menu;
using Clean.Application.Dtos.Restaurant;
using Clean.Application.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clean.Application.Services.Menu;

public class MenuService : IMenuService
{
    private readonly IDataContext _context;

    public MenuService(IDataContext context)
    {
        _context = context;
    }
    
    public async Task<PaginatedResponse<GetMenuDto>> GetMenusAsync(MenuPaginationFilter pagination)
    {
        var query = _context.Menus
            .Include(m => m.Restaurant)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Restaurant))
        {
            query = query.Where(r => r.Restaurant != null &&
                                     r.Restaurant.Name.ToLower() == pagination.Restaurant.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(pagination.Category))
        {
            query = query.Where(r => r.Category.ToLower() == pagination.Category.ToLower());
        }

        var total = await query.CountAsync();
        

        query = query.OrderBy(b => b.Id)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize);

        var menuItems = await query.Select(m => new GetMenuDto
        {
            Name = m.Name,
            Description = m.Description,
            Category = m.Category,
            IsAvailable = m.IsAvailable,
            PreparationTime = m.PreparationTime,
            Weight = m.Weight,
            PhotoUrl = m.PhotoUrl,
            Price = m.Price,
            RestaurantName = m.Restaurant.Name
        }).ToListAsync();

        return new PaginatedResponse<GetMenuDto>(menuItems, pagination.PageNumber, pagination.PageSize, total);
    }


    public async Task<Response<GetMenuDto>> AddMenuAsync(AddMenuItemDto dto)
    {
        var restaurant=
            await _context.Restaurants.FirstOrDefaultAsync(r => r.Name.ToLower() == dto.Restaurant.ToLower());
        if (restaurant is null)
        {
            return new Response<GetMenuDto>(HttpStatusCode.BadGateway,
                "No such restaurant to add menu item to in the system");
        }

        var menu = new Domain.Entities.Menu
        {
            Name = dto.Name,
            Description = dto.Description,
            Category = dto.Category,
            IsAvailable = true,
            PreparationTime = dto.PreparationTime,
            Weight = dto.Weight,
            PhotoUrl = dto.PhotoUrl,
            Price = dto.Price,
            RestaurantId = restaurant.Id
        };

        await _context.Menus.AddAsync(menu);
        var isAdded = await _context.SaveChangesAsync();
        if (isAdded > 0)
        {
            return new Response<GetMenuDto>(HttpStatusCode.OK, "Menu Item Added Successfully!");
        }

        return new Response<GetMenuDto>(HttpStatusCode.BadGateway, "Error while adding menu item");
    }

    public async Task<Response<string>> UpdateMenuAsync(UpdateMenuItemDto dto)
    {
        var currentMenu = await _context.Menus.FirstOrDefaultAsync(m=> m.Id == dto.Id);

        if (currentMenu is null)
        {
            return new Response<string>(HttpStatusCode.BadGateway,
                "No such menu to update in the system");
        }

        var restaurant =
            await _context.Restaurants.FirstOrDefaultAsync(r => dto.Restaurant.ToLower() == r.Name.ToLower());

        if (restaurant is null)
        {
            return new Response<string>(HttpStatusCode.BadGateway,
                "No such restaurant to update menu item in the system");
        }
        
        currentMenu.Name = dto.Name;
        currentMenu.Description = dto.Description;
        currentMenu.Category = dto.Category;
        currentMenu.IsAvailable = true;
        currentMenu.PreparationTime = dto.PreparationTime;
        currentMenu.Weight = dto.Weight;
        currentMenu.PhotoUrl = dto.PhotoUrl;
        currentMenu.Price = dto.Price;
        currentMenu.RestaurantId = restaurant.Id;

        var isUpdated = await _context.SaveChangesAsync();
        if (isUpdated > 0)
        {
            return new Response<string>(HttpStatusCode.OK, "Menu Item Updated!");
        }
        
        return new Response<string>(HttpStatusCode.BadGateway, "Error while updating menu item");
    }

    public async Task<Response<string>> DeactivateMenuByIdAsync(int id)
    {
        var menu = await _context.Menus.FirstOrDefaultAsync(m => m.Id == id);
        if (menu is null)
        {
            return new Response<string>(HttpStatusCode.BadGateway, "No such menu item to make unavailable");
        }

        menu.IsAvailable = false;
        var isDeactivated = await _context.SaveChangesAsync();
        if (isDeactivated > 0)
        {
            return new Response<string>(HttpStatusCode.OK, "Menu Item Made Unavailable");
        }
        
        return new Response<string>(HttpStatusCode.BadGateway, "Error while deactivating menu item");
    }
}