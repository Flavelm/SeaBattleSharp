using System.Net.WebSockets;
using SeaBattleWeb.Models;

namespace SeaBattleWeb.Services;

public interface IRoomService
{
    DateTime LastActivity { get; }
    Task ProcessSocket(IProfileModel profile, WebSocket socket);
}