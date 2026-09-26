using ECommerce.Application.Features.Payments.Commands.UpdatePaymentStatus;
using ECommerce.Infrastructure.ExternalServices.Payment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class WebhooksController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly StripeSettings _stripeSettings;
        private readonly ILogger<WebhooksController> _logger;

        public WebhooksController(
            IMediator mediator,
            IOptions<StripeSettings> stripeSettings,
            ILogger<WebhooksController> logger)
        {
            _mediator = mediator;
            _stripeSettings = stripeSettings.Value;
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                _logger.LogInformation(
                "Stripe webhook received. HasSignatureHeader: {HasHeader}, BodyLength: {Length}",
                Request.Headers.ContainsKey("Stripe-Signature"),
                json.Length);
                _logger.LogInformation(">>> Body Length: {Length}", json.Length);

                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _stripeSettings.WebhookSecret,
                    throwOnApiVersionMismatch: false
                );

                _logger.LogInformation(
                    "Stripe webhook received: {EventType} ({EventId})",
                    stripeEvent.Type, stripeEvent.Id);

                switch (stripeEvent.Type)
                {
                    case EventTypes.PaymentIntentSucceeded:
                        {
                            var intent = stripeEvent.Data.Object as PaymentIntent;
                            if (intent != null)
                            {
                                await _mediator.Send(
                                    new UpdatePaymentStatusCommand(intent.Id, true));
                            }
                            break;
                        }

                    case EventTypes.PaymentIntentPaymentFailed:
                        {
                            var intent = stripeEvent.Data.Object as PaymentIntent;
                            if (intent != null)
                            {
                                await _mediator.Send(
                                    new UpdatePaymentStatusCommand(intent.Id, false));
                            }
                            break;
                        }

                    default:
                        _logger.LogInformation(
                            "Unhandled Stripe event type: {EventType}",
                            stripeEvent.Type);
                        break;
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex,
                    "Stripe webhook signature validation failed.");
                return BadRequest();
            }
        }
    }
}