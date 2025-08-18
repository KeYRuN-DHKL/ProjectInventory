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
}