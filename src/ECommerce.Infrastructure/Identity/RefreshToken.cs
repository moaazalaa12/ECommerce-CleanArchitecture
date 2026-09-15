using ECommerce.Domain.Common;

namespace ECommerce.Infrastructure.Identity;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? Revoked { get; set; }
    public Guid UserId { get; set; } = Guid.Empty;


    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsActive => Revoked == null && !IsExpired;

    // Navigational Properties
    public ApplicationUser? User { get; set; }
}