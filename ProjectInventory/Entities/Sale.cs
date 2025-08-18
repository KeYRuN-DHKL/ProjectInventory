using ProjectInventory.Enum;

namespace ProjectInventory.Entities;

public class Sale : AuditableEntity
{
    public string InvoiceNo { get; set; } = null!;
    
    public DateOnly TransactionDate { get; set;}
    
    public Guid StakeHolderId { get; set; }
    public StakeHolder StakeHolder { get; set; } = null!;

    public decimal TotalAmount { get; set; }
    
    public decimal TaxableAmount { get; set; }
    
    public decimal TaxAmount { get; set; }
    
    public decimal DiscountAmount { get; set; }
    
    public string? Description { get; set; }

    public decimal GrandTotal => TotalAmount + TaxableAmount - DiscountAmount;

    public Status Status { get; set; } = Status.Completed;
}