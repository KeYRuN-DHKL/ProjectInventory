using System.ComponentModel.DataAnnotations;

namespace ProjectInventory.Models;

public class UnitVm
{
    [Required]
    [StringLength(40, MinimumLength = 4, ErrorMessage = "String must be 4 to 40 character long")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(5, MinimumLength = 1, ErrorMessage = "Symbol must contain 1 to 5 characters")]
    public string Symbol { get; set; } = string.Empty;
    public string? Description { get; set; }
}