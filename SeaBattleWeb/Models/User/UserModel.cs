using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeaBattleWeb.Models.User;

[Table("Users")]
public class UserModel
{
    [Required] [Key] public string IdUsername { get; set; }
    [Required] public byte[] Password { get; set; }
    [Required] public string EmailAddress { get; set; }
    [Required] public string Role { get; set; }
    [Required] public string GivenName { get; set; }
}