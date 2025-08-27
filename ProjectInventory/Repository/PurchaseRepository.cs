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
      var purchases = await _context.Purchases.ToListAsync();
      return purchases;
   }
}