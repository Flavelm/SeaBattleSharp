namespace SeaBattleWeb.Models;

public interface IProfileModel
{
    public bool IsNull { get; }
    public string IdUsername { get; }
    public Guid Id { get; }

    public void Update() {}

    public static NullProfileModel Null => new();
}