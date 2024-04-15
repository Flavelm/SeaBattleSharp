using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SeaBattleWeb.Models;
using SeaBattleWeb.Models.User;

namespace SeaBattleWeb.Context;

public class UsersContext(DbContextOptions<UsersContext> options, IConfiguration configuration) : DbContext(options)
{
    public DbSet<UserModel> Users { get; private set; } = null!;

    public UserModel? FindByDto(UserLoginDto userDto)
    {
        string lowerUsername = userDto.Username.ToLower();

        return Users.Find(lowerUsername);
    }
    
    public string GenerateToken(UserModel user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.IdUsername),
            new Claim(ClaimTypes.Email, user.EmailAddress),
            new Claim(ClaimTypes.GivenName, user.GivenName),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
            configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public UserModel GetCurrentUser(IIdentity? identity)
    {
        if (identity as ClaimsIdentity is { IsAuthenticated: true, Claims: not null } claimsIdentity)
        {
            var user = new UserModel();
            
            foreach (var o in claimsIdentity.Claims)
            {
                switch (o)
                {
                    case { Type: ClaimTypes.NameIdentifier } claim:
                        user.IdUsername = claim.Value;
                        break;
                    case { Type: ClaimTypes.Email } claim:
                        user.EmailAddress = claim.Value;
                        break;
                    case { Type: ClaimTypes.GivenName } claim:
                        user.GivenName = claim.Value;
                        break;
                    case { Type: ClaimTypes.Role } claim:
                        user.Role = claim.Value;
                        break;
                }
            }

            return user;
        }
        return null;
    }
}