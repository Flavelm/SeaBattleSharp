using System.Collections.Concurrent;
using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SeaBattleWeb.Contexts;
using SeaBattleWeb.Models;
using SeaBattleWeb.Models.Play.Models.Play;
using SeaBattleWeb.Services;
using SeaBattleWeb.Services.Play;

namespace SeaBattleWeb.Hubs;

[Authorize]
public class RoomHub(ILogger<RoomHub> logger, ApplicationDbContext profiles, RoomsService roomsService) : Hub<IClientRoomHub>
{
    //Id, List<connectionId>
    private static readonly ConcurrentDictionary<Guid, List<string>> UserConnections = new();
    
    private IRoomService? _roomService;
    private ProfileModel? _profile;

    public override async Task OnConnectedAsync()
    {
        logger.LogTrace("MapHub started. ID: {0}", Context.ConnectionId);
        
        if (Context.User == null)
            return;

        ProfileModel profile = profiles.GetCurrentUser(Context.User.Identity)!;
        
        logger.LogTrace("User {} identified", profile.IdUsername);
        
        if (UserConnections.TryGetValue(profile.Id, out var connections))
            connections.Add(Context.ConnectionId);
        else
            UserConnections.TryAdd(profile.Id, [Context.ConnectionId]);

        _profile = profile;
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_profile == null)
            return;

        if (UserConnections.TryGetValue(_profile.Id, out var value))
        {
            value.Remove(Context.ConnectionId);
            if (value.Count == 0)
                UserConnections.TryRemove(_profile.Id, out _);
        }
    }

    public async Task Create(FieldDto fieldDto)
    {
        if (_profile == null)
            return;

        Guid room = roomsService.Create();
        await Groups.AddToGroupAsync(Context.ConnectionId, room.ToLowerString());

        await Join(room.ToLowerString(), fieldDto);
    }
    
    public async Task Join(string roomId, FieldDto fieldDto)
    {
        if (_profile == null)
            return;
        
        Guid roomGuid = Guid.Parse(roomId);

        if (!roomsService.Has(roomGuid, out var room))
        {
            Clients.Caller.HandleError("room.notfound");
            return;
        }
        
        _roomService = room;
        
        if (_roomService.RoomState != RoomState.Preparation)
        {
            Clients.Caller.HandleError("room.full");
            return;
        }
            
        _roomService.Join(new FieldModel(_profile, fieldDto.Ships)
        {
            FieldId = Guid.NewGuid(),
            OpenedPositions = new List<PositionModel>(),
        });
        
        _roomService.FieldUpdated += RoomOnFieldUpdated;
    }

    public async Task Shot(int x, int y)
    {
        if (_profile == null || _roomService == null)
            return;
    }

    public async Task Notify()
    {
        if (_profile == null || _roomService == null)
            return;
        
        //TODO Notify
    }

    private void RoomOnFieldUpdated(object? sender, FieldServiceEventArgs e)
    {
        Notify();
    }

    protected override void Dispose(bool disposing)
    {
        if (_roomService != null)
            _roomService.FieldUpdated -= RoomOnFieldUpdated;
        
        base.Dispose(disposing);
    }
}