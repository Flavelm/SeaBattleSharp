using System.Net.WebSockets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
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
    { //Todo Prevent memory leak
        return Ok(new {Create = roomsService.Create()});
    }
    
    [HttpPost("{id}/TestConnection")]
    public async Task<IActionResult> TestConnection(Guid id, string? name)
    {
        UserModel? userModel = usersContext.GetCurrentUser(User.Identity);
        IProfileModel? profileModel = 
            userModel is null 
                ? new NullProfileModel(name) 
                : await profiles.Profiles.FindAsync(userModel.IdUsername);
        
        if (profileModel == null)
            return BadRequest(new { Error = "Profile not found!" });

        if (!roomsService.Has(id, out var room))
            return BadRequest(new { Error = "Room not found!" });
        else
            if (room.IsReady || room.CanConnect)
                return BadRequest(new { Error = $"Room ready {room.IsReady} {room.CanConnect}" });
        
        return Ok(new { Message = $"Welcome {(profileModel is NullProfileModel ? "anonymous" : "known")}, {profileModel.IdUsername}" });
    }
    
    [HttpGet("{id}/Connection")]
    public async Task OpenConnection(Guid id, string? name)
    {
        var webSockets = HttpContext.WebSockets;
        if (!webSockets.IsWebSocketRequest)
            return;
        
        if (!roomsService.Has(id, out var room))
        {
            logger.LogDebug("Room not found");
            return;
        }
        if (room.IsReady || room.CanConnect)
        {
            logger.LogDebug($"Room ready {room.IsReady} {room.CanConnect}");
            return;
        }
        
        logger.LogInformation("Opening websocket");
        
        UserModel? userModel = usersContext.GetCurrentUser(User.Identity);
        IProfileModel? profileModel = 
            userModel is null 
            ? new NullProfileModel(name)
            : await profiles.Profiles.FindAsync(userModel.IdUsername);
        
        WebSocket socket = await webSockets.AcceptWebSocketAsync();
        logger.LogInformation("Accepted websocket");
        
        await roomsService[id].ProcessSocket(profileModel, socket);
    }
}