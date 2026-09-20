using ECommerce.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommand : ITransactionalCommand<bool>
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }

        public CancelOrderCommand(Guid orderId, Guid userId)
        {
            OrderId = orderId;
            UserId = userId;
        }
    }
}
