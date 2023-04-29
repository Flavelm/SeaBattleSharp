using System.Text;
using Newtonsoft.Json;

namespace SeaBattle.Server;

public static class Errors
{
    public static readonly string BufferDropped = JsonConvert.SerializeObject(new { message = "Buffer dropped" });
    public static readonly byte[] BufferDroppedBytes = Encoding.UTF8.GetBytes(BufferDropped);
}