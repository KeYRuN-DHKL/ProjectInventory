using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectInventory.Data;
using ProjectInventory.Entities;
using ProjectInventory.Repository.Interface;

namespace ProjectInventory.Repository;

public class ProductRepository(ApplicationDbContext context) : IProductRepository
{
    public async Task<List<Product>> GetAllAsync()
    {
        var items =await context.Products
            .Include(p => p.Category)
            .Include(p => p.Unit).ToListAsync();
        return items;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        var item = await context.Products.FindAsync(id);
        return item;
    }

    public async Task<List<SelectListItem>> GetAllSelectListAsync()
    {
        return await context.Products
            .Where(p => p.IsActive)
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }).ToListAsync();
    }
    
}