namespace SeaBattleWeb.Models;

public class NullProfileModel : IProfileModel
{
    public bool IsNull => true;
    public string IdUsername { get; } = Guid.NewGuid().ToString();
    public Guid Id { get; } = Guid.NewGuid();
}