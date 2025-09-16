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
      var purchase =  await _context.Purchases.FindAsync(id);
      if (purchase == null)
         throw new KeyNotFoundException("Purchase not found");
      return purchase;
   }
   
   public IQueryable<Purchase> FindWithQueryable(Guid id)
   {
      var purchase =  _context.Purchases.Include(p => p.StakeHolder).Where(p => p.Id == id);
      if (purchase == null)
         throw new KeyNotFoundException("Purchase not found");
      return purchase;
   }
}