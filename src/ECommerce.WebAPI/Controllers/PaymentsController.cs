using Application.Exceptions;
using ECommerce.Application.DTOs.Payments;
using ECommerce.Application.Features.Payments.Commands.CreatePaymentIntent;
using ECommerce.Application.Features.Payments.Commands.RefundPayment;
using ECommerce.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace ECommerce.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController(IMediator _mediator) : ControllerBase
    {
        [HttpPost("{orderId}")]
        public async Task<IActionResult> CreateOrUpdatePaymentIntent(
            Guid orderId,
            CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var email = User.FindFirstValue(ClaimTypes.Email);
            var name = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
            {
                throw new UnauthorizedException("User claims are missing.");
            }

            var command = new CreatePaymentIntentCommand(orderId, userId, email, name);
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new PaymentIntentResponseDto(result.ClientSecret));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{orderId}/refund")]
        public async Task<IActionResult> RefundPayment(
            Guid orderId,
            [FromBody] RefundRequestApiDto requestDto,
            CancellationToken cancellationToken)
        {
            var command = new RefundPaymentCommand(orderId, requestDto.Reason);
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }
    }
}