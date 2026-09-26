using ECommerce.Application.Interfaces;
using ECommerce.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ECommerce.Application.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse>(
        IUnitOfWork _unitOfWork,
        ILogger<TransactionBehavior<TRequest, TResponse>> _logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var isTransactional = typeof(TRequest).GetInterfaces()
                .Any(i => i == typeof(ITransactionalCommand) ||
                          (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITransactionalCommand<>)));

            if (!isTransactional)
                return await next();

            var requestName = typeof(TRequest).Name;
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("Starting transaction for {RequestName}", requestName);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var response = await next();
                await _unitOfWork.CommitAsync(cancellationToken);

                stopwatch.Stop();

                _logger.LogInformation(
                    "Committed transaction for {RequestName} in {ElapsedMilliseconds}ms",
                    requestName, stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogWarning(ex,
                    "Rolled back transaction for {RequestName} after {ElapsedMilliseconds}ms. Reason: {ErrorMessage}",
                    requestName, stopwatch.ElapsedMilliseconds, ex.Message);

                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}