using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SeaBattleWeb.Contexts;

namespace SeaBattleWeb.Models;

[Table("Profiles")]
public class ProfileModel(ProfileContext profileContext)
{
    [Required] [Key] public string IdUsername { get; init; }
    [Required] public Guid Id { get; init; }
    
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

    public PublicProfileDto ToPublicDto()
    {
        return new PublicProfileDto(this);
    }
}