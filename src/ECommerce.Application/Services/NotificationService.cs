using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.Notifications;
using ECommerce.Application.Interfaces.Notifications;
using ECommerce.Domain.Entities.UserEntities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Interfaces.UserInterfaces;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Services
{
    public class NotificationService(INotificationRepository _notificationRepo,
        IUnitOfWork _unitOfWork,
        IRealtimeNotifier _realtimeNotifier, IMapper _mapper, ILogger<NotificationService> _logger) : INotificationService
    {
        public async Task SendNotificationAsync(Guid userId, string title, string message,
            NotificationType type, Guid? relatedEntityId = null,
            CancellationToken cancellationToken = default)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                RelatedEntityId = relatedEntityId,
                IsRead = false
            };

            await _notificationRepo.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            try
            {
                var dto = _mapper.Map<NotificationDto>(notification);
                await _realtimeNotifier.SendToUserAsync(
                    userId, "ReceiveNotification", dto, cancellationToken);

                _logger.LogInformation("Notification pushed to user {UserId}", userId);
            }
            catch (Exception ex)
            {
                // Don't fail the main operation if SignalR push fails
                _logger.LogWarning(ex, "Failed to push notification to user {UserId}", userId);
            }
        }

        public async Task<PagedResponse<NotificationDto>> GetUserNotificationsAsync(
            Guid userId,
            PagedRequest<NotificationSortColumn> request,
            bool unreadOnly = false,
            CancellationToken cancellationToken = default)
        {
            var (items, totalCount) = await _notificationRepo.GetPagedUserNotificationsAsync(
                userId,
                unreadOnly,
                request.PageNumber,
                request.PageSize,
                request.SortBy,
                request.IsDescending,
                cancellationToken);

            var dtos = _mapper.Map<IReadOnlyList<NotificationDto>>(items);

            return new PagedResponse<NotificationDto>(
                Data: dtos,
                PageNumber: request.PageNumber,
                PageSize: request.PageSize,
                TotalRecords: totalCount
            );
        }

        public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _notificationRepo.CountUnreadAsync(userId, cancellationToken);

        public async Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId,
            CancellationToken cancellationToken = default)
        {
            var notification = await _notificationRepo.GetByIdAsync(notificationId);
            if (notification == null || notification.UserId != userId)
                return false;

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            _notificationRepo.Update(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _notificationRepo.MarkAllAsReadAsync(userId, cancellationToken);
    }
}
