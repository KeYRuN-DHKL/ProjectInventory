using Microsoft.EntityFrameworkCore;
using ProjectInventory.Data;
using ProjectInventory.Dto;
using ProjectInventory.Entities;
using ProjectInventory.Enum;
using ProjectInventory.Repository.Interface;

namespace ProjectInventory.Repository;

public class StockMovementRepository(ApplicationDbContext context) : IStockMovementRepository
{
    public async Task<List<StockMovement>> FindByIdAsync(Guid id)
    {
        return await context.StockMovements
            .Where(s => s.TypeId == id)
            .Include(s => s.Product)
            .ThenInclude(s => s.Unit)
            .ToListAsync();
    }

    public async Task<List<StockMovement>> FindByInvoiceNumberAsync(string invoiceNumber, MovementType type)
    {
        return await context.StockMovements
            .Where(s => s.InvoiceNumber == invoiceNumber && s.MovementType == type)
            .ToListAsync();
    }
    
    public async Task<List<StockMovementApiDto>> GetItemAsync(Guid id)
    {
        return await context.StockMovements
            .Where(sm => sm.TypeId == id)
            .Include(sm => sm.Product) 
            .ThenInclude(p => p.Unit)  
            .Select(sm => new StockMovementApiDto
            {
                ProductName = sm.Product.Name,
                Unit = sm.Product.Unit.Symbol,
                Quantity = sm.Quantity,
                Rate = sm.Rate,
                Amount = sm.Quantity * sm.Rate,
                VatPercentage = sm.VatPercentage 
            })
            .ToListAsync();
    }
}