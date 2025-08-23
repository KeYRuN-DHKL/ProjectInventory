namespace ProjectInventory.Dto;

public class AdjustmentDto
{
    public Guid Id { get; set; }
    public string InvoiceNo { get; set; }
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}