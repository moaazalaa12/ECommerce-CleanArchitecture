using ECommerce.Application.Interfaces;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionBehavior(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var isTransactional = typeof(TRequest).GetInterfaces()
                .Any(i => i == typeof(ITransactionalCommand) ||
                          (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITransactionalCommand<>)));

            if (!isTransactional)
                return await next();

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var response = await next();
                await _unitOfWork.CommitAsync(cancellationToken);
                return response;
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}