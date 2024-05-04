using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using SeaBattleWeb.Contexts;
using SeaBattleWeb.Models;
using SeaBattleWeb.Models.Play.Models.Play;

namespace SeaBattleWeb.Services.Play;

public class RoomService : IRoomService, IDisposable
{
    private readonly ApplicationDbContext _competitionContext;
    private readonly FieldServiceFactory _fieldServiceFactory;

    private readonly RoomModel _competitionModel;
    private readonly ConcurrentBag<FieldService> _fieldServices;

    private int CurrentShot;

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

    public Guid RoomId => _competitionModel.RoomId;

    public ReadOnlyCollection<FieldService> FieldServices => new(new List<FieldService>(_fieldServices));

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
        CurrentShot = CurrentShot.Reverse();
        FieldUpdated?.Invoke(this, eventArgs);
        LastActivity = DateTime.Now;
    }

    public FieldService GetByOwner(ProfileModel profileModel)
    {
        return _fieldServices.First(f => f.Field.OwnedProfile.Equals(profileModel));
    }
    
    public FieldService GetByOther(ProfileModel profileModel)
    {
        return _fieldServices.First(f => !f.Field.OwnedProfile.Equals(profileModel));
    }

    public bool CanShot(ProfileModel profileModel)
    {
        if (_fieldServices.Count != 2)
            return false;

        int i = 0;
        foreach (var service in _fieldServices)
        {
            if (service.Field.OwnedProfile.Equals(profileModel))
            {
                return CurrentShot == i;
            }
            i++;
        }
        
        LastActivity = DateTime.Now;
                
        return false;
    }

    public void Join(FieldModel fieldModel)
    {
        if (_fieldServices.Count >= 2 || RoomState == RoomState.GameStarted)
            throw new Exception("Game already started");
        
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