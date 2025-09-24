using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectInventory.Data;
using ProjectInventory.Entities;
using ProjectInventory.Repository.Interface;

namespace ProjectInventory.Repository;

public class UnitRepository(ApplicationDbContext context) : IUnitRepository
{
    public async Task<List<Unit>> GetAllAsync()
    {
        var units = await context.Units.ToListAsync();
        return units;
    }

    public async Task<Unit?> GetByIdAsync(Guid id)
    {
        var units = await context.Units.FindAsync(id);
        return units;
    }

    public async Task<List<SelectListItem>> GetAllSelectListAsync()
    {
        return await context.Units
            .Where(c => c.IsActive)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToListAsync();
    }

}