namespace Docentify.Domain.Entities;

public class UserNotificationEntity
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Text { get; set; } = null!;

    public string? NotificationDate { get; set; }

    public virtual UserEntity User { get; set; } = null!;
}
