using Microsoft.EntityFrameworkCore;
using SeaBattleWeb.Models;

namespace SeaBattleWeb.Context;

public class ProfileContext(DbContextOptions<ProfileContext> options) : DbContext(options)
{
    public DbSet<ProfileModel> Profiles { get; private set; } = null!;
}