using ECommerce.Application.Interfaces.Wishlist;
using ECommerce.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyWishlist(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _wishlistService.GetUserWishlistAsync(userId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("check/{productId}")]
    public async Task<IActionResult> IsInWishlist(Guid productId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var isInWishlist = await _wishlistService.IsInWishlistAsync(userId, productId, cancellationToken);
        return Ok(new { isInWishlist });
    }

    [HttpPost("{productId}")]
    public async Task<IActionResult> AddToWishlist(Guid productId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _wishlistService.AddToWishlistAsync(userId, productId, cancellationToken);
        return Ok(new { message = "Product added to wishlist successfully." });
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> RemoveFromWishlist(Guid productId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _wishlistService.RemoveFromWishlistAsync(userId, productId, cancellationToken);
        return NoContent();
    }
}