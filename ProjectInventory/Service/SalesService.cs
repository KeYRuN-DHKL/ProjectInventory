using Microsoft.EntityFrameworkCore;
using ProjectInventory.Data;
using ProjectInventory.Dto;
using ProjectInventory.Entities;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Service;

public class SalesService : ISalesService
{
    private readonly ApplicationDbContext _context;

    public SalesService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Sale> CreateAsync(SalesDto dto)
    {
        var sales = new Sale()
        {
            Id = Guid.NewGuid(),
            Description = dto.Description,
            StakeHolderId = dto.StakeHolderId,
            Status = dto.Status,
            InvoiceNo = dto.InvoiceNumber,
            TransactionDate = dto.TransactionDate,
            TotalAmount = dto.TotalAmount,
            TaxableAmount = dto.TaxableAmount,
            TaxAmount = dto.TaxAmount,
            DiscountAmount = dto.DiscountAmount,

        };
        _context.Sales.Add(sales);
        await _context.SaveChangesAsync();
        return sales;
    }

    public async Task<Sale> UpdateAsync(SalesDto dto)
    {
        var sales = await _context.Sales.FirstOrDefaultAsync(s => s.Id == dto.Id);
        if (sales == null)
            throw new KeyNotFoundException("Item not found...");
        sales.Id = dto.Id;
        sales.InvoiceNo = dto.InvoiceNumber;
        sales.Description = dto.Description;
        sales.DiscountAmount = dto.DiscountAmount;
        sales.StakeHolderId = dto.StakeHolderId;
        sales.TaxableAmount = dto.TaxableAmount;
        sales.TaxAmount = dto.TaxAmount;
        sales.TransactionDate = dto.TransactionDate;
        sales.TotalAmount = dto.TotalAmount;
        _context.Sales.Update(sales);
        await _context.SaveChangesAsync();
        return sales;
    }
}