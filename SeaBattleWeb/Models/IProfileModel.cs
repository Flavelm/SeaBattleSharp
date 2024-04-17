using System.Net.WebSockets;

namespace SeaBattleWeb.Models;

public interface IProfileModel
{
    public bool IsNull { get; }
    public string IdUsername { get; }
    public Guid Id { get; }
    
    public void Update();
    public bool Equals(object? obj);
}