using System.Net.WebSockets;
using SeaBattleWeb.Models;

namespace SeaBattleWeb.Services;

public interface IRoomsService
{
    IRoomService this[int index]
    {
        get;
    }

    int Create();
}