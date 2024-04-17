using System.Net.WebSockets;

namespace SeaBattleWeb.Models;

public class NullProfileModel : IProfileModel
{
    public bool IsNull => true;
    public string IdUsername { get; } = Guid.NewGuid().ToString();
    public Guid Id { get; } = Guid.NewGuid();
    
    public WebSocket? Connection { get; set; }

    public void Update()
    {
        throw new NotImplementedException();
    }

    public override bool Equals(object? obj)
    {
        return obj is NullProfileModel otherModel
               && otherModel.Id == Id 
               && otherModel.IdUsername == IdUsername;
    }
}