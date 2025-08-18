using ProjectInventory.Entities;
using ProjectInventory.Enum;

namespace ProjectInventory.Dto;

public class StockMovementDto
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal? VatPercentage { get; set; }
    public MovementType MovementType { get; set; }
    public Stock Stock { get; set; }    
    public string? InvoiceNumber { get; set; }
    public Guid TypeId { get; set; }
}