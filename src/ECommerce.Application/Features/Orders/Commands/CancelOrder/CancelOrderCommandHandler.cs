using Application.Exceptions;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces.CatalogInterfaces;
using ECommerce.Domain.Interfaces.OrderInterfaces;
using MediatR;

namespace ECommerce.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommandHandler(
        IOrderRepository _orderRepository,
        IStockReservationRepository _stockReservationRepo,
        IProductRepository _productRepo,
        ICouponUsageRepository _couponUsageRepo, 
        ICouponRepository _couponRepo) : IRequestHandler<CancelOrderCommand, bool>
    {

        public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderWithDetailsByIdAsync(request.OrderId, cancellationToken);

            if (order == null || order.UserId != request.UserId)
                throw new UnauthorizedException("Order not found or you are not authorized to cancel it.");


            if (order.OrderStatus != OrderStatus.Pending && order.OrderStatus != OrderStatus.Processing)
                throw new BadRequestException($"Cannot cancel order because it is in '{order.OrderStatus}' status.");


            order.OrderStatus = OrderStatus.Cancelled;
            _orderRepository.Update(order);


            var reservations = await _stockReservationRepo.FindAsync(sr => sr.OrderId == request.OrderId);
            foreach (var reservation in reservations)
            {

                reservation.IsReleased = true;
                _stockReservationRepo.Update(reservation);


                var product = await _productRepo.GetByIdAsync(reservation.ProductId);
                if (product != null)
                {
                    product.StockQuantity += reservation.Quantity;
                    _productRepo.Update(product);
                }
            }

            if (order.CouponId.HasValue)
            {
                var couponUsages = await _couponUsageRepo.FindAsync(cu => cu.OrderId == request.OrderId);
                var usage = couponUsages.FirstOrDefault();
                if (usage != null)
                {
                    _couponUsageRepo.Delete(usage);
                }

                var coupon = await _couponRepo.GetByIdAsync(order.CouponId.Value);
                if (coupon != null)
                {
                    coupon.CurrentUsageCount -= 1;
                    _couponRepo.Update(coupon);
                }
            }

            return true;
        }
    }
}
