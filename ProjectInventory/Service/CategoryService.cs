using Microsoft.EntityFrameworkCore;
using ProjectInventory.Data;
using ProjectInventory.Dto;
using ProjectInventory.Entities;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Service;

public class CategoryService(ApplicationDbContext context) : ICategoryService
{
    public async Task<bool> CreateAsync(CategoryDto dto)
    {
        var category = new Category
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            IsActive = dto.IsActive
        };
        context.Categories.Add(category);
        bool isSaved = await context.SaveChangesAsync() > 0;
        if (!isSaved)
            throw new InvalidOperationException("Unable to insert into the database...");
        return isSaved;
    }

    public async Task<bool> EditAsync(Guid id,CategoryDto dto)
    {
         var items = await context.Categories.FirstOrDefaultAsync(e => e.Id == id);
         if (items == null)
             throw new InvalidOperationException("Category not found with the id: {id}");
         var isAvailable = await context.Categories.AnyAsync(c => (c.Name == dto.Name && c.Description == dto.Description && c.IsActive == dto.IsActive));
         if (isAvailable)
             throw new InvalidOperationException("Category already exists");
         items.Name = dto.Name;
         items.Description = dto.Description ;
         items.IsActive = dto.IsActive;
         context.Categories.Update(items);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var items = await context.Categories.FirstOrDefaultAsync(e => e.Id == id);
        if (items == null)
            throw new InvalidOperationException("Cannot delete an item with the given Id: {id}");
        context.Categories.Remove(items);
       return await context.SaveChangesAsync() > 0;
    }
}