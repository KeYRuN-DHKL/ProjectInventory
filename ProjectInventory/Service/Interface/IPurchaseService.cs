using ProjectInventory.Dto;
using ProjectInventory.Entities;

namespace ProjectInventory.Service.Interface;

public interface IPurchaseService
{
    Task<Purchase> CreateAsync(PurchaseDto dto);
    Task<Purchase> UpdateAsync(PurchaseDto dto);
    Task<Purchase> PurchaseReturnAsync(Guid id);
}