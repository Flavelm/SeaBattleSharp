using System.Collections.ObjectModel;
using System.Net.WebSockets;
using SeaBattleWeb.Models;

namespace SeaBattleWeb.Services;

public interface IRoomService
{
    DateTime LastActivity { get; }
    bool IsReady { get; }
    Task ProcessSocket(IProfileModel profile, WebSocket socket);
    
}