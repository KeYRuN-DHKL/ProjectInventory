using ProjectInventory.Entities;

namespace ProjectInventory.Repository.Interface;

public interface IPurchaseRepository
{
    Task<List<Purchase>> GetAllAsync();
}