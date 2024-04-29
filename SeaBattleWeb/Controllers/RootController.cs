using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SeaBattleWeb.Controllers;

[ApiController]
[Route("/api")]
[AllowAnonymous]
public class RootController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(new { Ok = 200 });
    }
}