using ProjectInventory.Dto;
using ProjectInventory.Entities;

namespace ProjectInventory.Service.Interface;

public interface IAdjustmentService
{
    Task<String> GetInvoiceNumber();

    Task<Adjustment> AddAsync(AdjustmentDto dto);
}