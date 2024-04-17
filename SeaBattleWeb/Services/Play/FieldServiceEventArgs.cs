namespace SeaBattleWeb.Services.Play;

public class FieldServiceEventArgs
{
    public required FieldServiceEventType Type { get; init; }
    public required FieldService Instance { get; init; }
}