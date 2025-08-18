using Microsoft.EntityFrameworkCore;
using ProjectInventory.Entities;

namespace ProjectInventory.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
    
    public DbSet<Category> Categories { get; set; }
    
    public DbSet<Unit> Units { get; set; }
    
    public DbSet<StakeHolder> StakeHolders { get; set; }
    
    public DbSet<Product> Products { get; set; }
    
    public DbSet<Purchase> Purchases { get; set; }
    
    public DbSet<StockMovement> StockMovements { get; set; }
    
    public DbSet<Sale> Sales { get; set; }
    
}