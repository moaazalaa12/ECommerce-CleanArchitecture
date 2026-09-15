using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities.UserEntities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; } = Guid.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; } = false;
    public Guid? RelatedEntityId { get; set; }
    public DateTime? ReadAt { get; set; }

    // Navigation Property
    public ApplicationUser User { get; set; } = null!;
}