using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectInventory.Data;
using ProjectInventory.Entities;
using ProjectInventory.Repository.Interface;

namespace ProjectInventory.Repository;

public class CategoryRepository(ApplicationDbContext context) : ICategoryRepository
{
    public async Task<List<Category?>> GetAllAsync()
    {
        var items = await context.Categories.ToListAsync();
        return items;
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        var item = await context.Categories.FindAsync(id);
        return item;
    }

    public async Task<List<SelectListItem>> GetAllSelectListAsync()
    {
        return await context.Categories
            .Where(c => c.IsActive)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .ToListAsync();
    }
}