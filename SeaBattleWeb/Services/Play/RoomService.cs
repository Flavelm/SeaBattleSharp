using System.Collections.Concurrent;
using SeaBattleWeb.Contexts;
using SeaBattleWeb.Models.Play.Models.Play;

namespace SeaBattleWeb.Services.Play;

public class RoomService : IRoomService, IDisposable
{
    private readonly ApplicationDbContext _competitionContext;
    private readonly FieldServiceFactory _fieldServiceFactory;

    private readonly RoomModel _competitionModel;
    private readonly ConcurrentBag<FieldService> _fieldServices;

    public event EventHandler<FieldServiceEventArgs>? FieldUpdated;
    
    public DateTime LastActivity { get; private set; }

    public RoomState RoomState
    {
        get
        {
            if (_fieldServices.Count <= 1)
                return RoomState.Preparation;
            return _fieldServices.All(e => !e.IsEnded) ? RoomState.GameStarted : RoomState.GameEnded;
        }
    }

    public RoomService(ApplicationDbContext competitionContext, FieldServiceFactory fieldServiceFactory)
    {
        _competitionContext = competitionContext;
        _fieldServiceFactory = fieldServiceFactory;
        _competitionModel = new RoomModel
        {
            RoomId = Guid.NewGuid(),
            Fields = new()
        };
        _competitionContext.Competitions.Add(_competitionModel);
        _fieldServices = new ConcurrentBag<FieldService>();
    }

    private void Notify(object? sender, FieldServiceEventArgs eventArgs)
    {
        FieldUpdated?.Invoke(this, eventArgs);
    }

    public void Join(FieldModel fieldModel)
    {
        lock (_competitionModel)
        {
            _competitionModel.Fields.Add(fieldModel);
            _competitionContext.Competitions.Update(_competitionModel);
        }

        FieldService service = _fieldServiceFactory.Create(fieldModel);
        service.FieldUpdated += Notify;
        _fieldServices.Add(service);
        
        LastActivity = DateTime.Now;
    }

    public void Dispose()
    {
        _competitionContext.Competitions.Remove(_competitionModel);
    }
}