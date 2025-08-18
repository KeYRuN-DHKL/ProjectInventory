using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectInventory.Models;

public class ProductVm
{
    [Required(ErrorMessage = "Name of the Product is required")]
    [StringLength(50,MinimumLength=3,ErrorMessage="String must be between 3 to 50 characters")]
    [RegularExpression(@"^[A-Za-z\s]+$", 
        ErrorMessage = "Name is required.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Code is required.")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Code must be numbers only.")]
    public string Code { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Cost Price is required.")]
    [RegularExpression(@"^\d{1,4}$", ErrorMessage = "Code must be exactly 4 digits.")]
    public decimal CostPrice { get; set; } 
    
    public Guid UnitId { get; set; }
    
    public Guid CategoryId { get; set; }
    [StringLength(100,ErrorMessage="Description is required")]
    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> Units { get; set; }= new List<SelectListItem>();
}