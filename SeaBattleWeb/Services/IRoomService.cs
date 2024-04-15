using System.Net.WebSockets;
using SeaBattleWeb.Models;

namespace SeaBattleWeb.Services;

public interface IRoomService
{
    Task HandleViewer(ProfileModel profile, WebSocket socket);
}