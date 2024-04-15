using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeaBattleWeb.Context;
using SeaBattleWeb.Models;
using SeaBattleWeb.Models.User;

namespace SeaBattleWeb.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly UsersContext _context;
    
    public AuthenticationController(UsersContext context, IConfiguration configuration, ILogger<AuthenticationController> logger)
    {
        _context = context;
        logger.LogInformation("Create");
    }
    
    [HttpPost("Register")]
    [AllowAnonymous] 
    public IActionResult Create([FromBody] UserRegisterDto userDto)
    {
        UserModel? user = _context.FindByDto(userDto);

        if (user != null)
            return Conflict(new { Error = "User exists" });
        
        byte[] passwordBytes = Encoding.UTF8.GetBytes(userDto.Password);
        
        byte[] passwordHash = SHA256.HashData(passwordBytes);
        string idUsername = userDto.Username.ToLower();
        
        UserModel newUser = new UserModel
        {
            IdUsername = idUsername,
            EmailAddress = userDto.EmailAddress,
            GivenName = userDto.Username,
            Password = passwordHash,
            Role = "user"
        };

        _context.Users.Add(newUser);
        _context.SaveChanges();
        
        return Ok(new { token = _context.GenerateToken(newUser) });
    }

    [HttpGet("Login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] UserLoginDto userDto)
    {
        UserModel? user = _context.FindByDto(userDto);

        if (user == null)
            return NotFound(new { Error = "User not exists" });

        return Ok(new { Token = _context.GenerateToken(user) });
    }
}