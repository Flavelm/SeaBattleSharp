using System.Net.WebSockets;

namespace SeaBattleWeb.Models.Play;

public class PlayFieldModel
{
    public Dictionary<IProfileModel, FieldModel> Fields = new();

    public PlayFieldModel(IProfileModel[] profiles)
    {
        foreach (var profile in profiles)
            Fields.Add(profile, new FieldModel());
    }

    public async Task RefreshField()
    {
        
    }

    public async Task SyncMineField(IProfileModel model)
    {
        
    }

    public async Task SyncOtherFields(IProfileModel model)
    {
        
    }
}