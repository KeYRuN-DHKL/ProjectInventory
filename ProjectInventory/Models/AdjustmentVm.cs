using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectInventory.Models;

public class AdjustmentVm
{
    public string InvoiceNo { get; set; }
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }

    public List<SelectListItem> Products { get; set; } = new List<SelectListItem>();

    public List<StockMovementVm> StockMovements { get; set; } = new List<StockMovementVm>();

    public Dictionary<string, string>? ProductUnitMap { get; set; } = new Dictionary<string, string>();
}