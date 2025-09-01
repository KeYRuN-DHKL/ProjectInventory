namespace ProjectInventory.Models;

public class PurchaseIndexVm
{ 
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly TransactionDate { get; set; }
    public string? StakeHolderName { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal GrandTotal => TotalAmount + TaxAmount - DiscountAmount;
}
