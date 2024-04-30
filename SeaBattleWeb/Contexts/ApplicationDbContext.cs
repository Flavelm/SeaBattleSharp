using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SeaBattleWeb.Models;
using SeaBattleWeb.Models.Play.Models.Play;

namespace SeaBattleWeb.Contexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : DbContext(options)
{
    public DbSet<RoomModel> Competitions { get; private set; } = null!;
    
    public DbSet<ProfileModel> Profiles { get; private set; } = null!;
    
    public ProfileModel GenerateProfile(string? username)
    {
        ProfileModel profileModel = new ProfileModel(this)
        {
            Id = Guid.NewGuid(),
            IdUsername = username ?? Guid.NewGuid().ToString()
        };
        return profileModel;
    }
    
    public string GenerateToken(ProfileModel profileModel)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, profileModel.Id.ToString()),
            new Claim(ClaimTypes.GivenName, profileModel.IdUsername),
        };

        var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
            configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public ProfileModel? GetCurrentUser(IIdentity? identity)
    {
        if (identity as ClaimsIdentity is { IsAuthenticated: true, Claims: not null } claimsIdentity)
        {
            dynamic dyn = new { };

            foreach (var o in claimsIdentity.Claims)
            {
                switch (o)
                {
                    case { Type: ClaimTypes.NameIdentifier } claim:
                        dyn.Id = Guid.Parse(claim.Value);
                        break;
                    case { Type: ClaimTypes.GivenName } claim:
                        dyn.IdUsername = claim.Value;
                        break;
                }
            }

            return new ProfileModel(this)
            {
                Id = dyn.Id,
                IdUsername = dyn.IdUsername
            };
        }
        return null;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PositionModel>(
            b =>
            {
                b.Property(e => e.X);
                b.Property(e => e.Y);
            });

        modelBuilder.Entity<ShipModel>(
            b =>
            {
                b.Property(e => e.X);
                b.Property(e => e.Y);
            });
    }
}