namespace ECommerce.Domain.Entities.UserEntities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? Revoked { get; set; }
    public string UserId { get; set; } = string.Empty;

    // Navigation Property
    public ApplicationUser User { get; set; } = null!;

    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsActive => Revoked == null && !IsExpired;
}