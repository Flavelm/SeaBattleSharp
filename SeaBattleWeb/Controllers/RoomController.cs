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
public class RoomController(UsersContext usersContext, ProfileContext profiles, IRoomsService roomsService, ILogger<RoomController> logger) : ControllerBase
{
    [HttpGet("Create")]
    public async Task<IActionResult> CreateRoom()
    { //Todo Prevent memory leak
        return Ok(new {Create = roomsService.Create()});
    }
    
    [HttpGet("{id}/TestConnection")]
    public async Task<IActionResult> TestConnection(Guid id)
    {
        UserModel? userModel = usersContext.GetCurrentUser(User.Identity);
        IProfileModel? profileModel = 
            userModel is null 
                ? IProfileModel.Null 
                : await profiles.Profiles.FindAsync(userModel.IdUsername);
        if (profileModel == null)
            return BadRequest(new { Error = "Profile not found!" });
        
        if (!roomsService.Has(id))
            return BadRequest(new { Error = "Room not found!" });
        
        return null!;
    }
    
    [HttpGet("{id}/Connection")]
    public async Task OpenConnection(Guid id)
    {
        var webSockets = HttpContext.WebSockets;
        if (!webSockets.IsWebSocketRequest)
            return;
        logger.LogInformation("Opening websocket");
        
        UserModel? userModel = usersContext.GetCurrentUser(User.Identity);
        IProfileModel? profileModel = 
            userModel is null 
            ? IProfileModel.Null 
            : await profiles.Profiles.FindAsync(userModel.IdUsername);
        
        WebSocket socket = await webSockets.AcceptWebSocketAsync();
        logger.LogInformation("Accepted websocket");
        
        await roomsService[id].ProcessSocket(profileModel, socket);
    }
}