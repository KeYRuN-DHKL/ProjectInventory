namespace ProjectInventory.Entities;

public class Damage : AuditableEntity
{
    public string InvoiceNo { get; set; }
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}