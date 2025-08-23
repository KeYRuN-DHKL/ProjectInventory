using Microsoft.EntityFrameworkCore;
using ProjectInventory.Data;
using ProjectInventory.Dto;
using ProjectInventory.Entities;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Service;

public class AdjustmentService : IAdjustmentService
{
    private readonly ApplicationDbContext _context;

    public AdjustmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> GetInvoiceNumber()
    {
        var lastInvoiceNo = await _context.Adjustments.OrderByDescending(k => k.InvoiceNo).FirstOrDefaultAsync();
        long newInvoiceNo = 1;
        if (lastInvoiceNo != null)
        {
            newInvoiceNo = long.Parse(lastInvoiceNo.InvoiceNo) + 1;
        }

        return newInvoiceNo.ToString();
    }

    public async Task<Adjustment> AddAsync(AdjustmentDto dto)
    {
        var adjustment = new Adjustment();
        adjustment.Id = Guid.NewGuid();
        adjustment.InvoiceNo = dto.InvoiceNo;
        adjustment.Date = dto.Date;
        adjustment.Amount = dto.Amount;
        adjustment.Description = dto.Description;
        _context.Adjustments.Add(adjustment);
        await _context.SaveChangesAsync();
        return adjustment;
    }
}