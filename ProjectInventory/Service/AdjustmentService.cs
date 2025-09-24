using Microsoft.EntityFrameworkCore;
using ProjectInventory.Data;
using ProjectInventory.Dto;
using ProjectInventory.Entities;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Service;

public class AdjustmentService(ApplicationDbContext context) : IAdjustmentService
{
    public async Task<string> GetInvoiceNumber()
    {
        var lastInvoiceNo = await context.Adjustments.OrderByDescending(k => k.InvoiceNo).FirstOrDefaultAsync();
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
        context.Adjustments.Add(adjustment);
        await context.SaveChangesAsync();
        return adjustment;
    }
}