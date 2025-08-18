using ProjectInventory.Enum;

namespace ProjectInventory.Entities;

public class StockMovement:AuditableEntity
{
    public DateOnly Date { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal? VatPercentage { get; set; }
    public MovementType MovementType { get; set; }
    public Stock Stock { get; set; }
    public string InvoiceNumber { get; set; } = null!;
    public Guid TypeId { get; set; }
}