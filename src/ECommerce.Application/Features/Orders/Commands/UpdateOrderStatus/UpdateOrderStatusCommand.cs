using ECommerce.Application.Interfaces;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommand : ITransactionalCommand<bool>
    {
        public Guid OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }

        public UpdateOrderStatusCommand(Guid orderId, OrderStatus newStatus)
        {
            OrderId = orderId;
            NewStatus = newStatus;
        }
    }
}
