using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace SeaBattleWeb.Services;

public class RoomsService(IServiceProvider provider) : IRoomsService
{
    private const int InactiveTime = 3; //minutes
    private const double InactiveTimeCheck = InactiveTime + 0.1;
    
    private readonly ConcurrentDictionary<Guid, IServiceScope> _scopes = new();
    private readonly ConcurrentDictionary<Guid, Timer> _timers = new();

    public IRoomService this[Guid index] => _scopes[index].ServiceProvider.GetRequiredService<IRoomService>();

    public bool Has(Guid id) => _scopes.ContainsKey(id);
    
    public bool Has(Guid id, [NotNullWhen(true)] out IRoomService? roomService)
    {
        if (_scopes.TryGetValue(id, out var scope))
        {
            roomService = scope.ServiceProvider.GetRequiredService<IRoomService>();
            return true;
        }
        roomService = null;
        return false;
    }

    public Guid Create()
    {
        Guid gen = Guid.NewGuid();
        _scopes.TryAdd(gen, provider.CreateScope());
        Timer timer = new Timer(TryDispose, gen, TimeSpan.Zero, TimeSpan.FromMinutes(InactiveTimeCheck));
        _timers.TryAdd(gen, timer);
        return gen;
    }

    private void TryDispose(object? id)
    {
        if (id is not Guid gen) 
            return;

        IRoomService room = this[gen];
        if (DateTime.Now - room.LastActivity > TimeSpan.FromMinutes(InactiveTime))
        {
            if (_timers.TryRemove(gen, out var timer))
                timer.DisposeAsync();
            if (_scopes.TryRemove(gen, out var scope))
                scope.Dispose();
        }
    }
}