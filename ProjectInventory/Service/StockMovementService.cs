using Microsoft.EntityFrameworkCore;
using ProjectInventory.Data;
using ProjectInventory.Dto;
using ProjectInventory.Entities;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Service;

public class StockMovementService(ApplicationDbContext context) : IStockMovementService
{
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

        await context.StockMovements.AddRangeAsync(stockMovementEntity);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(StockMovementDto dto)
    {
        var stockMovement = await context.StockMovements.FindAsync(dto.Id);
        if (stockMovement == null)
        {
            throw new KeyNotFoundException($"StockMovement not found");
        }

        stockMovement.Date = dto.Date;
        stockMovement.ProductId = dto.ProductId;
        stockMovement.Quantity = dto.Quantity;
        stockMovement.Rate = dto.Rate;
        stockMovement.VatPercentage = dto.VatPercentage;
        stockMovement.Stock = dto.Stock;
        stockMovement.InvoiceNumber = dto.InvoiceNumber ?? stockMovement.InvoiceNumber;

        context.StockMovements.Update(stockMovement);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var stockMovement = await context.StockMovements.FindAsync(id);
        if (stockMovement == null)
            throw new KeyNotFoundException($"Unable to find the stockMovement");
        context.StockMovements.Remove(stockMovement);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddAsync(StockMovementDto dto)
    {
        var stockMovement = new StockMovement
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            MovementType = dto.MovementType,
            InvoiceNumber = dto.InvoiceNumber ?? "",
            Rate = dto.Rate,
            Date = dto.Date,
            VatPercentage = dto.VatPercentage,
            TypeId = dto.TypeId,
            Stock = dto.Stock,
        };
        context.StockMovements.Add(stockMovement);
        return await context.SaveChangesAsync() > 0;
    }
    
}
