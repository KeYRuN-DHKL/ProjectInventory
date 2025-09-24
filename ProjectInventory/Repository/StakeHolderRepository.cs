using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectInventory.Repository.Interface;
using ProjectInventory.Data;
using ProjectInventory.Entities;

namespace ProjectInventory.Repository;

public class StakeHolderRepository(ApplicationDbContext context) : IStakeHolderRepository
{
    public async Task<List<StakeHolder>> GetAllAsync()
    {
        var items = await context.StakeHolders.ToListAsync();
        return items;
    }

    public async Task<StakeHolder?> GetByIdAsync(Guid id)
    {
        var item = await context.StakeHolders.FindAsync(id);
        return item;
    }

    public async Task<List<SelectListItem>> GetAllSelectListAsync()
    {
        return await context.StakeHolders
            .Where(s => s.IsActive)
            .Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToListAsync();
    }

    public async Task<string> GetStakeHolderName(Guid id)
    {
        var stakeHolder = await context.StakeHolders.FindAsync(id);
        if (stakeHolder == null)
            throw new KeyNotFoundException("stakeholder not found...");
        return stakeHolder.Name;
    }
}