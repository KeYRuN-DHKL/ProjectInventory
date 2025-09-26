using System.ComponentModel.DataAnnotations;
using ProjectInventory.Enum;

namespace ProjectInventory.Models;

public class StakeHolderDeleteVm
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Full name must be between 6 and 100 characters.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "VatNo is required")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Only digits are allowed")]
    public string VatNo { get; set; } = null!;

    [Required(ErrorMessage = "Address is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Address must be between 6 and 100 characters.")]
    public string Address { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^(98|97)\d{8}$",
        ErrorMessage = "Phone number must be exactly 10 digits and start with '98' or '97'.")]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; } = null!;
    
    public StakeHolderType Type { get; set; }
    
    public bool IsActive { get; set; }
}