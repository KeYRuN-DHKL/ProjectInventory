using ProjectInventory.Data;
using ProjectInventory.Dto;
using ProjectInventory.Entities;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Service;

public class OpeningService : IOpeningService
{
    private readonly ApplicationDbContext _context;

    public OpeningService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Opening> AddAsync(OpeningDto dto)
    {
        var opening = new Opening
        {
            Id = Guid.NewGuid(),
            IsDeleted = false,
            Amount = dto.Amount,
            CreatedAt = DateTime.UtcNow,
            Description = dto.Description
        };
        _context.Openings.Add(opening);
        await _context.SaveChangesAsync();
        return opening;
    }
}