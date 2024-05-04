using SeaBattleWeb.Contexts;
using SeaBattleWeb.Models.Play.Models.Play;

namespace SeaBattleWeb.Services.Play;

public class FieldService : IDisposable
{
    private readonly ILogger<FieldService> _logger;
    private readonly IRoomService _roomService;
    private readonly ApplicationDbContext _profileContext;
    private readonly FieldModel _field;
    
    public bool IsEnded => _field.FieldForOther.Count == 100; //TODO Just placeholder
    
    public FieldService(
        ILogger<FieldService> logger,
        ApplicationDbContext profileContext, 
        IRoomService roomService,
        FieldModel fieldModel)
    {
        _logger = logger;
        _profileContext = profileContext;
        _roomService = roomService;
        _field = fieldModel;
    }
    
    public FieldModel Field => _field;

    public event EventHandler<FieldServiceEventArgs>? FieldUpdated;
    
    public void Shot(int x, int y)
    {
        Field.OpenedPositions.Add(new PositionModel(x, y));
        FieldUpdated?.Invoke(this, new FieldServiceEventArgs
        {
            Instance = this,
            Type = FieldServiceEventType.PositionOpened
        });
    }

    public void Dispose()
    {
        _profileContext.Profiles.Remove(_field.OwnedProfile);
    }
}