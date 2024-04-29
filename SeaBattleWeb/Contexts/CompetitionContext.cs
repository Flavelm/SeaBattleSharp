using Microsoft.EntityFrameworkCore;
using SeaBattleWeb.Models.Play;
using SeaBattleWeb.Models.Play.Models.Play;

namespace SeaBattleWeb.Contexts;

public class CompetitionContext(DbContextOptions<ProfileContext> options) : DbContext(options)
{
    public DbSet<RoomModel> Competitions { get; private set; } = null!;
}