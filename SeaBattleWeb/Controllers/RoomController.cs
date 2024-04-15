using System.Net.WebSockets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeaBattleWeb.Context;
using SeaBattleWeb.Models;
using SeaBattleWeb.Models.User;
using SeaBattleWeb.Services;

namespace SeaBattleWeb.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "user")]
public class RoomController(UsersContext usersContext, ProfileContext profiles, IRoomsService roomsService) : ControllerBase
{
    [HttpGet("{id}/Connection")]
    public async Task<IActionResult> OpenConnection(int id)
    {
        var webSockets = HttpContext.WebSockets;
        if (!webSockets.IsWebSocketRequest)
            return BadRequest();
        
        WebSocket socket = await webSockets.AcceptWebSocketAsync();

        UserModel userModel = usersContext.GetCurrentUser(User.Identity);
        ProfileModel? profileModel = profiles.Profiles.Find(userModel.IdUsername);

        if (profileModel == null)
            return NotFound(new { Error = "Not found your profile" });
        
        roomsService[id].HandleViewer(profileModel, socket);
        
        return Ok();
    }
}