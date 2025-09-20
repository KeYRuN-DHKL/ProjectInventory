using ProjectInventory.Dto;
using ProjectInventory.Entities;

namespace ProjectInventory.Service.Interface;

public interface ISalesService
{
   Task<Sale> CreateAsync(SalesDto dto);
   Task<Sale> UpdateAsync(SalesDto dto);
}