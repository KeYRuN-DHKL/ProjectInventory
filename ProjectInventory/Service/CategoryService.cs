using Microsoft.EntityFrameworkCore;
using ProjectInventory.Data;
using ProjectInventory.Dto;
using ProjectInventory.Entities;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Service;

public class CategoryService :ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(CategoryDto dto)
    {
        var category = new Category
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            IsActive = dto.IsActive
        };
        _context.Categories.Add(category);
        bool isSaved = await _context.SaveChangesAsync() > 0;
        if (!isSaved)
            throw new InvalidOperationException("Unable to insert into the database...");
        return isSaved;
    }

    public async Task<bool> EditAsync(Guid id,CategoryDto dto)
    {
         var items = await _context.Categories.FirstOrDefaultAsync(e => e.Id == id);
         if (items == null)
             throw new InvalidOperationException("Category not found with the id: {id}");
         var isAvailable = await _context.Categories.AnyAsync(c => (c.Name == dto.Name && c.Description == dto.Description && c.IsActive == dto.IsActive));
         if (isAvailable)
             throw new InvalidOperationException("Category already exists");
         items.Name = dto.Name;
         items.Description = dto.Description ;
         items.IsActive = dto.IsActive;
         _context.Categories.Update(items);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var items = await _context.Categories.FirstOrDefaultAsync(e => e.Id == id);
        if (items == null)
            throw new InvalidOperationException("Cannot delete an item with the given Id: {id}");
        _context.Categories.Remove(items);
       return await _context.SaveChangesAsync() > 0;
    }
}