using Microsoft.AspNetCore.Mvc;

namespace ProjectInventory.Controllers;

public class CookieController : Controller
{
    public IActionResult SetCookie()
    {
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddMinutes(30),
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
        };
        Response.Cookies.Append("MyCookie", "Kiran", cookieOptions);
        return Ok("Cookie have been set");
    }

    public IActionResult GetCookie()
    {
        if (Request.Cookies.TryGetValue("MyCookie", out string? cookieValue))
        {
            return Ok($"Cookie value: {cookieValue}");
        }

        return NotFound("Cookie not found.");
    }
    
    [HttpGet("delete-cookie")]
    public IActionResult DeleteCookie()
    {
        Response.Cookies.Delete("MyCookie");
        return Ok("Cookie deleted.");
    }
}