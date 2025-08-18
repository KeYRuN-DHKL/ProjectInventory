using ProjectInventory.Entities;
namespace ProjectInventory.Repository.Interface;

public interface ISalesRepository
{
    Task<List<Sale>> GetAllAsync();
}