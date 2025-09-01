using Microsoft.EntityFrameworkCore;
using ProjectInventory.Data;
using ProjectInventory.Entities;
using ProjectInventory.Repository.Interface;

namespace ProjectInventory.Repository;

public class PurchaseRepository : IPurchaseRepository
{
   private readonly ApplicationDbContext _context;

   public PurchaseRepository(ApplicationDbContext context)
   {
      _context = context;
   }

   public async Task<List<Purchase>> GetAllAsync()
   {
      var purchases = await _context.Purchases
         .Include(s => s.StakeHolder).ToListAsync();
      return purchases;
   }

   public IQueryable<Purchase> GetQueryAbleData()
   {
      return _context.Purchases;
   }

   public async Task<Purchase> FindById(Guid id)
   {
      return await _context.Purchases.FindAsync(id);
   }
}