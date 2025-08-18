using ProjectInventory.Entities;
using ProjectInventory.Dto;

namespace ProjectInventory.Service.Interface;

public interface IStockMovementService
{
    Task<bool> AddAsync(List<StockMovementDto> dto);
}