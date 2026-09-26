using ECommerce.Application.Interfaces.Payment;
using Microsoft.Extensions.Options;
using Stripe;

namespace ECommerce.Infrastructure.ExternalServices.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly StripeSettings _stripeSettings;
        private readonly IStripeClient _stripeClient;

        public PaymentService(IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings.Value;
            _stripeClient = new StripeClient(_stripeSettings.SecretKey);
        }

        public async Task<PaymentResponseDto> CreateOrUpdatePaymentIntentAsync(
            PaymentRequestDto request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var service = new PaymentIntentService(_stripeClient);
                PaymentIntent paymentIntent;

                if (string.IsNullOrEmpty(request.PaymentIntentId))
                {
                    var options = new PaymentIntentCreateOptions
                    {
                        Amount = (long)(request.TotalAmount * 100),
                        Currency = "usd",
                        ReceiptEmail = request.CustomerEmail,
                        Description = $"Order ID: {request.OrderId} for {request.CustomerName}",
                        Metadata = new Dictionary<string, string>
                        {
                            { "OrderId", request.OrderId.ToString() }
                        }
                    };

                    var requestOptions = new RequestOptions
                    {
                        IdempotencyKey = $"order-{request.OrderId}"
                    };

                    paymentIntent = await service.CreateAsync(options, requestOptions, cancellationToken);
                }
                else
                {
                    var options = new PaymentIntentUpdateOptions
                    {
                        Amount = (long)(request.TotalAmount * 100)
                    };

                    paymentIntent = await service.UpdateAsync(request.PaymentIntentId, options, null, cancellationToken);
                }

                return PaymentResponseDto.Success(paymentIntent.Id, paymentIntent.ClientSecret);
            }
            catch (StripeException ex)
            {
                return PaymentResponseDto.Failure(ex.StripeError.Message);
            }
            catch (Exception ex)
            {
                return PaymentResponseDto.Failure(ex.Message);
            }
        }

        public async Task<RefundResponseDto> RefundAsync(
            RefundRequestDto request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var options = new RefundCreateOptions
                {
                    PaymentIntent = request.ProviderTransactionId,
                    Amount = (long)(request.Amount * 100),
                    Reason = request.Reason
                };

                var service = new RefundService(_stripeClient);
                var refund = await service.CreateAsync(options, null, cancellationToken);

                return RefundResponseDto.Success(refund.Id);
            }
            catch (StripeException ex)
            {
                return RefundResponseDto.Failure(ex.StripeError.Message);
            }
            catch (Exception ex)
            {
                return RefundResponseDto.Failure(ex.Message);
            }
        }
    }
}