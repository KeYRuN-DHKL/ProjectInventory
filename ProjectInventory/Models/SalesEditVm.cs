using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectInventory.Models;

public class SalesEditVm
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly TransactionDate { get; set; }
    public Guid StakeHolderId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal GrandTotal => TotalAmount + TaxAmount - DiscountAmount;
    
    public List<SelectListItem>? Products { get; set; } = new List<SelectListItem>();

    public List<SelectListItem>? StakeHolders { get; set; } = new List<SelectListItem>();
    public Dictionary<string, string>? ProductUnitMap { get; set; } = new Dictionary<string, string>();
    
    public List<StockMovementEditVm> StockMovements { get; set; } = new List<StockMovementEditVm>();
}