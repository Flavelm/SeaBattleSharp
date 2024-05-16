using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SeaBattleWeb.Contexts;

namespace SeaBattleWeb.Hubs;

[AllowAnonymous]
public class TokenHub(ApplicationDbContext profiles) : Hub
{
    public Task GenerateToken(string? name)
    {
        var caller = Clients.Caller;

        if (profiles.Profiles.Find(name) != null)
            throw new Exception("Profile already registered");
        
        var profile = profiles.GenerateProfile(name);
        profiles.Profiles.Add(profile);
        
        var token = profiles.GenerateToken(profile);
        caller.SendAsync("setToken", token);
        
        profiles.SaveChanges();
        return Task.CompletedTask;
    }
}