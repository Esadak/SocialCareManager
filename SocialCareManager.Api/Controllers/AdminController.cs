using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SocialCareManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok("You are an Admin.");
    }
}