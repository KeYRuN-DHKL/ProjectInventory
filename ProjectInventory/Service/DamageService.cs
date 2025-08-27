using ProjectInventory.Data;
using ProjectInventory.Dto;
using ProjectInventory.Entities;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Service;

public class DamageService : IDamageService
{
    private readonly ApplicationDbContext _context;

    public DamageService(ApplicationDbContext context)
    {
        _context = context;
    }

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
        _context.Damages.Add(damage);
        await _context.SaveChangesAsync();
        return damage;
    }
}