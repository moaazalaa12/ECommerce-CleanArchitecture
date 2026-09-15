using Microsoft.AspNetCore.Identity;

using ECommerce.Application.Interfaces.Identity;

namespace ECommerce.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string?> GetUserEmailAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user?.Email;
    }

    public async Task<bool> CreateUserAsync(string email, string password)
    {
        var user = new ApplicationUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);
        return result.Succeeded;
    }
}
