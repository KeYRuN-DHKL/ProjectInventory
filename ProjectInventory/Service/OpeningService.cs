using ProjectInventory.Data;
using ProjectInventory.Dto;
using ProjectInventory.Entities;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Service;

public class OpeningService(ApplicationDbContext context) : IOpeningService
{
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
        context.Openings.Add(opening);
        await context.SaveChangesAsync();
        return opening;
    }
}