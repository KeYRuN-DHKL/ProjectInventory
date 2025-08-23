namespace ProjectInventory.Entities;

public class Opening:AuditableEntity
{
    public DateOnly Date { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
}