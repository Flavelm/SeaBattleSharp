using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SeaBattleWeb.Controllers;

[ApiController]
[Route("/")]
[AllowAnonymous]
public class RootController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(new { Ok = 200 });
    }
}