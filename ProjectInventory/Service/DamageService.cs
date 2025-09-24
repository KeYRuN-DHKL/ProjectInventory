using ProjectInventory.Data;
using ProjectInventory.Dto;
using ProjectInventory.Entities;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Service;

public class DamageService(ApplicationDbContext context) : IDamageService
{
    public async Task<Damage> AddAsync(DamageDto dto)
    {
        var damage= new Damage
        {
            Id = Guid.NewGuid(),
            IsDeleted = false,
            Amount = dto.Amount,
            CreatedAt = DateTime.UtcNow,
            Description = dto.Description
        };
        context.Damages.Add(damage);
        await context.SaveChangesAsync();
        return damage;
    }
}