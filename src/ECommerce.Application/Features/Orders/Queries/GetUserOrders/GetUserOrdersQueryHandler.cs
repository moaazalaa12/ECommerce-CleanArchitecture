using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.Orders;
using ECommerce.Domain.Interfaces.OrderInterfaces;
using MediatR;

namespace ECommerce.Application.Features.Orders.Queries.GetUserOrders
{
    public class GetUserOrdersQueryHandler : IRequestHandler<GetUserOrdersQuery, PagedResponse<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetUserOrdersQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<PagedResponse<OrderDto>> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
        {
            var (orders, totalCount) = await _orderRepository.GetPagedUserOrdersAsync(
                request.UserId,
                request.PageNumber,
                request.PageSize,
                request.SortBy,
                request.IsDescending,
                cancellationToken);

            var orderDtos = _mapper.Map<IReadOnlyList<OrderDto>>(orders);

            return new PagedResponse<OrderDto>(
                Data: orderDtos,
                PageNumber: request.PageNumber,
                PageSize: request.PageSize,
                TotalRecords: totalCount
            );
        }
    }
}
