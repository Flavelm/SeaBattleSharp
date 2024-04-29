using System.ComponentModel.DataAnnotations;

namespace SeaBattleWeb.Models.Play.Models.Play;

public class RoomModel
{
    [Key] public Guid RoomId { get; init; }
    
    public List<FieldModel> Fields { get; init; }
}