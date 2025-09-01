using Microsoft.EntityFrameworkCore;
using ProjectInventory.Data;
using ProjectInventory.Dto;
using ProjectInventory.Entities;
using ProjectInventory.Enum;
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
            Description = dto.Description ?? "",
            Status = dto.Status,
        };
        _context.Purchases.Add(purchaseEntity);
        await _context.SaveChangesAsync();
        return purchaseEntity;
    }

    public async Task<Purchase> UpdateAsync(PurchaseDto dto)
    {
        var purchase = await _context.Purchases.FirstOrDefaultAsync(p => p.Id == dto.Id);
        if (purchase == null)
            throw new KeyNotFoundException($"purchase not found");
        purchase.InvoiceNumber = dto.InvoiceNumber;
        purchase.Description = dto.Description ?? "";
        purchase.DiscountAmount = dto.DiscountAmount;
        purchase.TaxableAmount = dto.TaxableAmount;
        purchase.TaxAmount = dto.TaxAmount;
        purchase.StakeHolderId = dto.StakeHolderId;
        purchase.TransactionDate = dto.TransactionDate;
        purchase.TotalAmount = dto.TotalAmount;
        _context.Purchases.Update(purchase);
        await _context.SaveChangesAsync();
        return purchase;
    }

    public async Task<Purchase> PurchaseReturnAsync(Guid id)
    {
        var purchase = await _context.Purchases.FirstOrDefaultAsync(p => p.Id == id);
        if (purchase == null)
            throw new KeyNotFoundException($"purchase not found");
        purchase.Status = Status.Returned;
        _context.Purchases.Update(purchase);
        await _context.SaveChangesAsync();
        return purchase;
    }
}