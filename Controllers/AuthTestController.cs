using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthTestController : ControllerBase
{
    [HttpGet("jwt")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public IActionResult Jwt()
    {
        return Ok(new
        {
            message = "JWT authentication successful.",
            username = User.Identity?.Name,
            role = User.FindFirst(
                System.Security.Claims.ClaimTypes.Role)?.Value
        });
    }

    [HttpGet("basic")]
    [Authorize(AuthenticationSchemes = "Basic")]
    public IActionResult Basic()
    {
        return Ok(new
        {
            message = "Basic authentication successful.",
            username = User.Identity?.Name,
            role = User.FindFirst(
                System.Security.Claims.ClaimTypes.Role)?.Value
        });
    }

    [HttpGet("admin")]
    [Authorize(
        AuthenticationSchemes = "Bearer",
        Roles = "Admin")]
    public IActionResult Admin()
    {
        return Ok(new
        {
            message = "Admin access successful."
        });
    }
}