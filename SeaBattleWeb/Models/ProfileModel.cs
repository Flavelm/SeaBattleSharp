using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.WebSockets;
using SeaBattleWeb.Context;

namespace SeaBattleWeb.Models;

[Table("Profiles")]
public class ProfileModel(ProfileContext profileContext) : IProfileModel
{
    public bool IsNull => false;
    [Required] [Key] public string IdUsername { get; set; }
    [Required] public Guid Id { get; set; } = Guid.NewGuid();
    
    public void Update()
    {
        profileContext.Profiles.Update(this);
    }
    
    public override bool Equals(object? obj)
    {
        return obj is ProfileModel otherModel
               && otherModel.Id == Id 
               && otherModel.IdUsername == IdUsername;
    }
}