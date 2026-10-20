using ECommerce.Application.DTOs.Reviews;
using ECommerce.Application.Interfaces.Reviews;
using ECommerce.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("product/{productId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductReviews(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _reviewService.GetProductReviewsAsync(productId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddReview(CreateReviewDto dto, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var userName = User.Identity?.Name ?? "Unknown User";

        var result = await _reviewService.AddReviewAsync(userId, userName, dto, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _reviewService.DeleteReviewAsync(userId, id, cancellationToken);
        return NoContent();
    }
}