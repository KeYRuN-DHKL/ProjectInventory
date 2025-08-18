using ProjectInventory.Data;
using ProjectInventory.Dto;
using ProjectInventory.Entities;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Service;

public class PurchaseService : IPurchaseService
{
    private readonly ApplicationDbContext _context;

    public PurchaseService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Purchase> CreateAsync(PurchaseDto dto)
    {
        var purchaseEntity = new Purchase
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            InvoiceNumber = dto.InvoiceNumber,
            TransactionDate = dto.TransactionDate,
            StakeHolderId = dto.StakeHolderId,
            TotalAmount = dto.TotalAmount,
            TaxableAmount = dto.TaxableAmount,
            TaxAmount = dto.TaxAmount,
            DiscountAmount = dto.DiscountAmount,
            Description = dto.Description,
            Status = dto.Status,
        };
        _context.Purchases.Add(purchaseEntity);
        await _context.SaveChangesAsync();
        return purchaseEntity;
    }
}