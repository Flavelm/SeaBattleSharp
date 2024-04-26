using Newtonsoft.Json;

namespace SeaBattleWeb.Models.Base;

public abstract class Serializable
{
    public virtual string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }

    public string ToString()
    {
        return Serialize();
    }
}