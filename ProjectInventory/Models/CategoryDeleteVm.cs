using System.ComponentModel.DataAnnotations;

namespace ProjectInventory.Models;

public class CategoryDeleteVm
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(50,MinimumLength=3,ErrorMessage="String must be between 3 to 50 characters")]
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public bool IsActive { get; set; }
}