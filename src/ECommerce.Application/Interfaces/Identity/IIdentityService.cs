namespace ECommerce.Application.Interfaces.Identity;

public interface IIdentityService
{
    Task<string?> GetUserEmailAsync(Guid userId);
    Task<bool> CreateUserAsync(string email, string password);
}