using SeaBattleWeb.Models.Base;

namespace SeaBattleWeb.Models;

public class NotificationDto(NotificationType? notificationType = null) : Serializable
{
    public NotificationType? NotificationType { get; set; } = notificationType;
}