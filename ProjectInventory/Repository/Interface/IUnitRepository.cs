using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectInventory.Entities;

namespace ProjectInventory.Repository.Interface;

public interface IUnitRepository
{
    Task<List<Unit>> GetAllAsync();
    Task<Unit?> GetByIdAsync(Guid id);
    Task<List<SelectListItem>> GetAllSelectListAsync();
}