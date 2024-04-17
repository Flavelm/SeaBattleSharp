using System.Net.WebSockets;
using SeaBattleWeb.Models;

namespace SeaBattleWeb.Services;

public interface IRoomsService
{
    IRoomService this[Guid index]
    {
        get;
    }

    Guid Create();
    bool Has(Guid id);
}