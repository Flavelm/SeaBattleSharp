using SeaBattleWeb.Models.Base;

namespace SeaBattleWeb.Models;

public class PublicProfileDto(ProfileModel profileModel) : Serializable
{
    public string Username { get; } = profileModel.IdUsername;
}