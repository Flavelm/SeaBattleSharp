using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeaBattleWeb.Context;
using SeaBattleWeb.Models;
using SeaBattleWeb.Models.User;

namespace SeaBattleWeb.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "user")]
public class ProfileController(UsersContext usersContext, ProfileContext profileContext) : ControllerBase
{
    [HttpPost]
    public IActionResult Create()
    {
        UserModel model = usersContext.GetCurrentUser(User.Identity);
        ProfileModel? profileModel = profileContext.Profiles.Find(model.IdUsername);

        if (profileModel == null)
        {
            profileContext.Add(new ProfileModel(profileContext)
            {
                IdUsername = model.IdUsername
            });
            profileContext.SaveChanges();
            return Ok(profileModel);
        }

        return NotFound(new { Error = "Profile already exists" });
    }
    
    [HttpGet]
    public IActionResult Get()
    {
        UserModel model = usersContext.GetCurrentUser(User.Identity);
        ProfileModel? context = profileContext.Profiles.Find(model.IdUsername);

        if (context is { })
        {
            return Ok(context);
        }

        return NotFound(new { Error = "Profile not found" });
    }
}