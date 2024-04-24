using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.SignalR;
using NuGet.Frameworks;

namespace SeaBattleWeb.Services;

public class RoomsService(IServiceProvider provider) : IRoomsService
{
    private const int InactiveTime = 3;
    private const double InactiveTimeCheck = InactiveTime + 0.1;

    private readonly object _blocker = new();
    
    private readonly Dictionary<Guid, IServiceScope> _scopes = new();
    private readonly Dictionary<Guid, CancellationTokenSource> _cancellationTokens = new();
    private readonly Dictionary<Guid, CancellationTokenRegistration> _cancellationTokensRegistrations = new();


    public IRoomService this[Guid index]
    {
        get
        {
            lock (_scopes)
            {
                return _scopes[index].ServiceProvider.GetRequiredService<IRoomService>();
            }
        }
    }

    public bool Has(Guid id)
    {
        lock (_blocker)
        {
            return _scopes.ContainsKey(id);
        }
    }

    public bool Has(Guid id, out IRoomService roomService)
    {
        lock (_blocker)
        {
            if (_scopes.TryGetValue(id, out var scope))
            {
                roomService = scope.ServiceProvider.GetRequiredService<IRoomService>();
                return true;
            }
            else
            {
                roomService = null;
                return false;
            }
        }
    }

    public Guid Create()
    {
        Guid gen = Guid.NewGuid();
        lock (_blocker)
        {
            _scopes.Add(gen, provider.CreateScope());
        }
        RenewCancellationTokenSource(gen, false);
        return gen;
    }

    private void UpdateOrDispose(object? id)
    {
        if (id is not Guid guid)
            return;
        
        if (this[guid].LastActivity - DateTime.Now > TimeSpan.FromMinutes(InactiveTime))
        {
            lock (_blocker)
            {
                _scopes[guid].Dispose();
                _scopes.Remove(guid);
                
                _cancellationTokens.Remove(guid);
                var reg = _cancellationTokensRegistrations[guid];
                reg.Unregister();
                reg.Dispose();
                _cancellationTokensRegistrations.Remove(guid);
            }
        }
        else
            RenewCancellationTokenSource(guid);
    }

    private void RenewCancellationTokenSource(Guid uid, bool removeOld = true)
    {
        TimeSpan checkTime = TimeSpan.FromMinutes(InactiveTimeCheck);
        var cancelToken = new CancellationTokenSource(checkTime);
        var cancelRegister = cancelToken.Token.Register(UpdateOrDispose, uid);

        lock (_blocker)
        {
            _cancellationTokens[uid] = cancelToken;
            if (removeOld)
            {
                var oldReg = _cancellationTokensRegistrations[uid];
                oldReg.Unregister();
                oldReg.Dispose();
            }
            _cancellationTokensRegistrations[uid] = cancelRegister;
        }
    }
}