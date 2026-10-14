using ECommerce.Application.Interfaces.Notifications;
using ECommerce.WebAPI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ECommerce.WebAPI.Services
{
    public class SignalRRealtimeNotifier : IRealtimeNotifier
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRRealtimeNotifier(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToUserAsync(Guid userId, string eventName, object payload,
            CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients
                .Group($"user-{userId}")
                .SendAsync(eventName, payload, cancellationToken);
        }
    }
}
