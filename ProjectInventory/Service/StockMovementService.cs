using ProjectInventory.Data;
using ProjectInventory.Dto;
using ProjectInventory.Entities;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Service;

public class StockMovementService : IStockMovementService
{
    private readonly ApplicationDbContext _context;

    public StockMovementService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddAsync(List<StockMovementDto> dto)
    {
        var stockMovementEntity = dto.Select(d => new StockMovement
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            ProductId = d.ProductId,
            Quantity = d.Quantity,
            MovementType = d.MovementType,
            InvoiceNumber = d.InvoiceNumber,
            Rate = d.Rate,
            Date = d.Date,
            VatPercentage = d.VatPercentage,
            TypeId = d.TypeId,
            Stock = d.Stock,
            
        }).ToList();

        await _context.StockMovements.AddRangeAsync(stockMovementEntity);
        return await _context.SaveChangesAsync() > 0;
    }
}
