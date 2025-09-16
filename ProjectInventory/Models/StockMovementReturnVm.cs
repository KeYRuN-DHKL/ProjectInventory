using ProjectInventory.Enum;

namespace ProjectInventory.Models;

public class StockMovementReturnVm
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal? VatPercentage { get; set; }
    public Stock? stock { get; set; }
    public string? UnitName { get; set; }
    public string ProductName { get; set; }
}