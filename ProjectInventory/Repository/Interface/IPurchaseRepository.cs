using ProjectInventory.Entities;

namespace ProjectInventory.Repository.Interface;

public interface IPurchaseRepository
{
    Task<List<Purchase>> GetAllAsync();
    IQueryable<Purchase> GetQueryAbleData();
    Task<Purchase> FindById(Guid id);
    IQueryable<Purchase> FindWithQueryable(Guid id);
}