using Microsoft.EntityFrameworkCore;
using ProjectInventory.Data;
using ProjectInventory.Entities;
using ProjectInventory.Enum;
using ProjectInventory.Repository.Interface;

namespace ProjectInventory.Repository;

public class StockMovementRepository : IStockMovementRepository
{
    private readonly ApplicationDbContext _context;

    public StockMovementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<StockMovement>> FindByIdAsync(Guid id)
    {
        return await _context.StockMovements
            .Where(s => s.TypeId == id)
            .ToListAsync();
    }

    public async Task<List<StockMovement>> FindByInvoiceNumberAsync(string invoiceNumber, MovementType type)
    {
        return await _context.StockMovements
            .Where(s => s.InvoiceNumber == invoiceNumber && s.MovementType == type)
            .ToListAsync();
    }
}