namespace ECommerce.Application.Interfaces.Notifications
{
    public interface IRealtimeNotifier
    {
        Task SendToUserAsync(Guid userId, string eventName, object payload,
            CancellationToken cancellationToken = default);
    }
}
