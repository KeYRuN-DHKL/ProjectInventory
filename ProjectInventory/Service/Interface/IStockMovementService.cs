using ProjectInventory.Entities;
using ProjectInventory.Dto;

namespace ProjectInventory.Service.Interface;

public interface IStockMovementService
{
    Task<bool> AddAsync(List<StockMovementDto> dto);
    Task<bool> UpdateAsync(StockMovementDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> AddAsync(StockMovementDto dto);
}