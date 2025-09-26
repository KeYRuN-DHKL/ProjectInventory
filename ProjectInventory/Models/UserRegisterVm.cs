using System.ComponentModel.DataAnnotations;

namespace ProjectInventory.Models;

public class UserRegisterVm
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = null!;

    // [Required(ErrorMessage = "Full name is required")]
    // [RegularExpression(@"^[A-Za-z]+( [A-Za-z]+){1,2}$",
    //     ErrorMessage = "Full name must contain only alphabets and have 2 or 3 parts (e.g., Ramesh Pathak or Ram Prasad Bimali)")]
    // public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^(98|97)\d{8}$",
        ErrorMessage = "Phone number must be 10 digits and start with 98 or 97")]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Confirm Password is required")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password did not match")]
    public string ConfirmPassword { get; set; } = null!;
}