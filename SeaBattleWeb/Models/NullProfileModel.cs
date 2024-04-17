using System.Net.WebSockets;

namespace SeaBattleWeb.Models;

public class NullProfileModel(string? idUsername = null) : IProfileModel
{
    public static NullProfileModel Null => new();
    
    public bool IsNull => true;
    public string IdUsername { get; } = idUsername ?? Guid.NewGuid().ToString();
    public Guid Id { get; } = Guid.NewGuid();

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