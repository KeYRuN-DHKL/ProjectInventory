using Microsoft.EntityFrameworkCore;
using ProjectInventory.Data;
using ProjectInventory.Entities;
using ProjectInventory.Repository.Interface;

namespace ProjectInventory.Repository;

public class SalesRepository(ApplicationDbContext context) : ISalesRepository
{
    public async Task<List<Sale>> GetAllAsync()
    {
        var saleItems = await context.Sales.
            Include(s=> s.StakeHolder)
            .ToListAsync();
        return saleItems;
    }

    public async Task<Sale> FindById(Guid id)
    {
        var saleItem = await context.Sales.FindAsync(id);
        if (saleItem == null)
            throw new KeyNotFoundException("Item not found");
        return saleItem;
    }
}