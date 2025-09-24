using ProjectInventory.Data;
using ProjectInventory.Dto;
using ProjectInventory.Entities;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Service;

public class UnitService(ApplicationDbContext context) : IUnitService
{
    public async Task AddAsync(UnitDto dto)
    {
        var unit = new Unit
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Symbol = dto.Symbol,
            Description = dto.Description,
        };
        context.Units.Add(unit);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, UnitDto dto)
    {
        var item = await context.Units.FindAsync(id);
        if (item == null)
            throw new InvalidOperationException($"Item with the Id: {id} not found");

        item.Name = dto.Name;
        item.Symbol = dto.Symbol;
        item.Description = dto.Description;
        item.IsActive = dto.IsActive;

        context.Units.Update(item);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var item = await context.Units.FindAsync(id);
        if (item == null)
        {
            throw new InvalidOperationException($"Item with the Id: {id} not found...");
        }

        context.Units.Remove(item);
        await context.SaveChangesAsync();
    }
}