using SeaBattleWeb.Models.Base;

namespace SeaBattleWeb.Models;

public class NotificationDto(NotificationType? notificationType = null, object? description = null) : Serializable
{
    public NotificationType? NotificationType { get; set; } = notificationType;
    public object? Description { get; set; }
}