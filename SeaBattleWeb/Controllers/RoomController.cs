using System.Net.WebSockets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using SeaBattleWeb.Context;
using SeaBattleWeb.Models;
using SeaBattleWeb.Models.User;
using SeaBattleWeb.Services;

namespace SeaBattleWeb.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoomController(UsersContext usersContext, ProfileContext profiles, IRoomsService roomsService, ILogger<RoomController> logger) : ControllerBase
{
    [HttpPost("Create")]
    public async Task<IActionResult> CreateRoom()
    { 
        //Todo Prevent memory leak
        return Ok(new {Create = roomsService.Create()});
    }

    [Route("{id}/Connection")] [HttpGet] [HttpPost]
    public async Task<IActionResult?> Connection(Guid id, string? name)
    {
        if (!roomsService.Has(id, out var room))
            return BadRequest(new { Error = "Room not found!" });
        else
            if (room.IsReady)
                return BadRequest(new { Error = $"Room ready: {room.IsReady}" });
        
        UserModel? userModel = usersContext.GetCurrentUser(User.Identity);
        IProfileModel? profileModel = 
            userModel is null 
                ? new NullProfileModel(name) 
                : await profiles.Profiles.FindAsync(userModel.IdUsername);
        
        if (profileModel == null)
            return BadRequest(new { Error = "Profile not found!" });
        
        if (HttpContext.Request.Method == "POST")
            return Ok(new { Message = $"Welcome {(profileModel is NullProfileModel ? "anonymous" : "known")}, {profileModel.IdUsername}" });
        
        var webSockets = HttpContext.WebSockets;
        if (!webSockets.IsWebSocketRequest)
            return BadRequest(new { Error = "It is not WebSocket" });
        
        logger.LogInformation("Opening websocket");
        
        WebSocket socket = await webSockets.AcceptWebSocketAsync();
        
        logger.LogInformation("Accepted websocket");
        
        await roomsService[id].ProcessSocket(profileModel, socket);
        
        return null;
    }
    
    public override BadRequestObjectResult BadRequest(object? error)
    {
        logger.LogDebug("BadRequest serialize {}", error);
        return base.BadRequest(error);
    }
}