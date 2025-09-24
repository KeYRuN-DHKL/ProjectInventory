using Microsoft.AspNetCore.Mvc;
using ProjectInventory.Repository.Interface;

namespace ProjectInventory.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class ApiController(IStockMovementRepository repository) : ControllerBase
{
    [HttpGet("{id}")]
    public async  Task<IActionResult> GetItem(Guid id)

    {
        try
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "Id is required..." });
            }

            var items = await repository.GetItemAsync(id);
            if (items.Count == 0)
            {
                return NotFound("Items not found...");
            }
            
            return Ok(items);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}