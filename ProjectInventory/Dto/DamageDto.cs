namespace ProjectInventory.Dto;

public class DamageDto
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}