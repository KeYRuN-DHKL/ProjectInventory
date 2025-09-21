namespace ProjectInventory.Dto;

public class StockMovementApiDto
{
    public string ProductName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public decimal? VatPercentage { get; set; }
}