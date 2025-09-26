using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectInventory.Enum;

namespace ProjectInventory.Models;

public class PurchaseVm
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly TransactionDate { get; set; }
    public Guid StakeHolderId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal GrandTotal => TotalAmount + TaxAmount - DiscountAmount;
    public Status Status { get; set; } = Status.Completed;
    
    public List<SelectListItem>? Products { get; set; } = new List<SelectListItem>();

    public List<SelectListItem>? StakeHolders { get; set; } = new List<SelectListItem>();
    public Dictionary<string, ProductUnitVm>? ProductUnitMap { get; set; } = new Dictionary<string, ProductUnitVm>();
    
    public List<StockMovementVm> StockMovements { get; set; } = new List<StockMovementVm>();
}