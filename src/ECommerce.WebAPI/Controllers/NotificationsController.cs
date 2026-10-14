using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces.Notifications;
using ECommerce.Domain.Enums;
using ECommerce.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController(INotificationService _notificationService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications(
            [FromQuery] PagedRequest<NotificationSortColumn> request,
            [FromQuery] bool unreadOnly = false,
            CancellationToken cancellationToken = default)
        {
            var userId = User.GetUserId();
            var result = await _notificationService.GetUserNotificationsAsync(
                userId, request, unreadOnly, cancellationToken);
            return Ok(result);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken = default)
        {
            var userId = User.GetUserId();
            var count = await _notificationService.GetUnreadCountAsync(userId, cancellationToken);
            return Ok(new { count });
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken = default)
        {
            var userId = User.GetUserId();
            var result = await _notificationService.MarkAsReadAsync(id, userId, cancellationToken);

            if (!result)
                return NotFound(new { message = "Notification not found." });

            return NoContent();
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken = default)
        {
            var userId = User.GetUserId();
            await _notificationService.MarkAllAsReadAsync(userId, cancellationToken);
            return NoContent();
        }
    }
}
