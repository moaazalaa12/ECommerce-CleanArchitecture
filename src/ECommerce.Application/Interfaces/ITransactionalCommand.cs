using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Interfaces
{
    public interface ITransactionalCommand<out TResponse> : IRequest<TResponse>
    {
    }

    public interface ITransactionalCommand : IRequest
    {
    }
}
