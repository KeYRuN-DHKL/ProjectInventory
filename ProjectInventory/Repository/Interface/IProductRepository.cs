using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectInventory.Data;
using ProjectInventory.Entities;

namespace ProjectInventory.Repository.Interface;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(Guid id);
    Task<List<SelectListItem>> GetAllSelectListAsync();
}