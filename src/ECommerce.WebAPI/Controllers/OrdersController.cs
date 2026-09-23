using ECommerce.Application.Features.Orders.Commands.CancelOrder;
using ECommerce.Application.Features.Orders.Commands.CreateOrder;
using ECommerce.Application.Features.Orders.Commands.UpdateOrderStatus;
using ECommerce.Application.Features.Orders.Queries.GetOrderById;
using ECommerce.Application.Features.Orders.Queries.GetUserOrders;
using ECommerce.Domain.Enums;
using ECommerce.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            command.UserId = User.GetUserId();
            var orderId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetOrderById), new { id = orderId }, new { OrderId = orderId });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery(id));

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserOrders([FromQuery] GetUserOrdersQuery query)
        {
            query.UserId = User.GetUserId();
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var userId = User.GetUserId();
            await _mediator.Send(new CancelOrderCommand(id, userId));

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] OrderStatus newStatus)
        {
            await _mediator.Send(new UpdateOrderStatusCommand(id, newStatus));

            return NoContent();
        }
    }
}