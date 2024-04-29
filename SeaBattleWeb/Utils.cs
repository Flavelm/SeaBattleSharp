using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using SeaBattleWeb.Models;
using SeaBattleWeb.Models.Base;

namespace SeaBattleWeb;

public static class Utils
{
    public static string ToLowerString(this Guid s)
    {
        return s.ToString().ToLower();
    }

    public static string Err(string err)
    {
        return JsonConvert.SerializeObject(new { Error = err });
    }

    public static int Reverse(this int num) => num == 0 ? 1 : 0;
}