using SeaBattleWeb.Contexts;
using SeaBattleWeb.Models.Play.Models.Play;

namespace SeaBattleWeb.Services.Play;

public class FieldService : IDisposable
{
    private readonly ILogger<FieldService> _logger;
    private readonly RoomService _roomService;
    private readonly ApplicationDbContext _profileContext;
    private readonly FieldModel _field;

    public bool IsEnded => _field.GetField(_field.OwnedProfile).Count == 100; //TODO Just placeholder
    
    public FieldService(
        ILogger<FieldService> logger,
        ApplicationDbContext profileContext, 
        RoomService roomService,
        FieldModel fieldModel)
    {
        _logger = logger;
        _profileContext = profileContext;
        _roomService = roomService;
        _field = fieldModel;
    }
    
    public FieldModel Field => _field;

    public event EventHandler<FieldServiceEventArgs>? FieldUpdated;

    public void Dispose()
    {
        _profileContext.Profiles.Remove(_field.OwnedProfile);
    }
}