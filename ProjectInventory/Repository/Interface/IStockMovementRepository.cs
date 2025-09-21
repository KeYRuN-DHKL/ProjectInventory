using ProjectInventory.Dto;
using ProjectInventory.Entities;
using ProjectInventory.Enum;

namespace ProjectInventory.Repository.Interface;

public interface IStockMovementRepository
{
    Task<List<StockMovement>> FindByIdAsync(Guid id);
    Task<List<StockMovement>> FindByInvoiceNumberAsync(string invoiceNumber,MovementType type);
    Task<List<StockMovementApiDto>> GetItemAsync(Guid id);
}