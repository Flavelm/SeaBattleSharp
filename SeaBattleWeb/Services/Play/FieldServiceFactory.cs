using SeaBattleWeb.Contexts;
using SeaBattleWeb.Models.Play.Models.Play;

namespace SeaBattleWeb.Services.Play;

public class FieldServiceFactory(IServiceProvider provider)
{
    public FieldService Create(FieldModel fieldModel)
    {
        ILogger<FieldService> logger = provider.GetRequiredService<ILogger<FieldService>>();
        ApplicationDbContext profileContext = provider.GetRequiredService<ApplicationDbContext>();
        RoomService roomService = provider.GetRequiredService<RoomService>();

        return new FieldService(logger, profileContext, roomService, fieldModel);
    }
}