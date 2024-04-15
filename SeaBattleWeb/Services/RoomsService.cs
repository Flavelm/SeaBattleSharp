namespace SeaBattleWeb.Services;

public class RoomsService(IServiceProvider provider) : IRoomsService
{
    private readonly List<IServiceScope> _scopes = new();

    public IRoomService this[int index]
    {
        get
        {
            lock (_scopes)
            {
                return _scopes[index].ServiceProvider.GetRequiredService<IRoomService>();
            }
        }
    }

    public int Create()
    {
        lock (_scopes)
        {
            _scopes.Add(provider.CreateScope());
            return _scopes.Count - 1;
        }
    }
}