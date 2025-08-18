using ProjectInventory.Enum;

namespace ProjectInventory.Models;

public class StockMovementVm
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal? VatPercentage { get; set; }
    public Stock? Stock{ get; set; }
}