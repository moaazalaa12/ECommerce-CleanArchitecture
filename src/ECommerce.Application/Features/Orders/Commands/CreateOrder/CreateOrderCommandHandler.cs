using Application.Exceptions;
using ECommerce.Application.Interfaces.Notifications;
using ECommerce.Domain.Entities.OrderEntities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Interfaces.CatalogInterfaces;
using ECommerce.Domain.Interfaces.OrderInterfaces;
using ECommerce.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler(
        IOrderRepository _orderRepo,
        IProductRepository _productRepo,
        ICouponRepository _couponRepo,
        ICouponUsageRepository _couponUsageRepo,
        IStockReservationRepository _stockReservationRepo,
        IUnitOfWork _unitOfWork,
        INotificationService _notificationService,
        ILogger<CreateOrderCommandHandler> _logger) : IRequestHandler<CreateOrderCommand, Guid>
    {
        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            if (request.Items == null || !request.Items.Any())
                throw new BadRequestException("Order must contain at least one item.");

            if (request.Items.Any(i => i.Quantity <= 0))
                throw new BadRequestException("Item quantity must be greater than zero.");

            // Merge duplicate product entries to validate total requested quantity accurately
            var groupedItems = request.Items
                .GroupBy(i => i.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Quantity = g.Sum(i => i.Quantity)
                })
                .ToList();

            var productIds = groupedItems.Select(i => i.ProductId).ToList();
            var products = (await _productRepo.FindAsync(p => productIds.Contains(p.Id)))
                           .ToDictionary(p => p.Id);

            decimal subTotal = 0;
            var orderItems = new List<OrderItem>();
            var stockReservations = new List<StockReservation>();

            foreach (var item in groupedItems)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                    throw new KeyNotFoundException($"Product with Id {item.ProductId} not found.");

                if (!product.IsActive)
                    throw new BadRequestException($"Product '{product.Name}' is not active.");

                if (product.StockQuantity < item.Quantity)
                    throw new BadRequestException($"Insufficient stock for '{product.Name}'. Available: {product.StockQuantity}");

                var priceAtPurchase = product.DiscountPrice ?? product.Price;
                subTotal += priceAtPurchase * item.Quantity;

                product.StockQuantity -= item.Quantity;
                _productRepo.Update(product);

                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceAtPurchase = priceAtPurchase
                });

                stockReservations.Add(new StockReservation
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    ExpiryDate = DateTime.UtcNow.AddMinutes(15),
                    IsReleased = false
                });
            }

            decimal shippingFee = request.DeliveryMethod switch
            {
                DeliveryMethod.Express => 100m,
                DeliveryMethod.Standard => 50m,
                _ => 50m
            };

            decimal discountAmount = 0;
            if (request.CouponId.HasValue)
            {
                var coupon = await _couponRepo.GetByIdAsync(request.CouponId.Value);

                if (coupon == null || !coupon.IsActive || coupon.ExpiryDate < DateTime.UtcNow)
                    throw new BadRequestException("Coupon is invalid or expired.");

                if (coupon.CurrentUsageCount >= coupon.MaxUsageCount)
                    throw new BadRequestException("Coupon usage limit has been reached.");

                if (coupon.MinOrderAmount.HasValue && subTotal < coupon.MinOrderAmount.Value)
                    throw new BadRequestException($"Order subtotal must be at least {coupon.MinOrderAmount.Value} to use this coupon.");

                discountAmount = coupon.DiscountType == DiscountType.FixedAmount
                    ? coupon.DiscountValue
                    : subTotal * (coupon.DiscountValue / 100m);

                if (discountAmount > subTotal)
                    discountAmount = subTotal;

                coupon.CurrentUsageCount += 1;
                _couponRepo.Update(coupon);
            }

            decimal totalAmount = (subTotal + shippingFee) - discountAmount;

            var shippingAddress = new Address
            {
                Street = request.ShippingAddress.Street,
                City = request.ShippingAddress.City,
                State = request.ShippingAddress.State,
                Country = request.ShippingAddress.Country,
                ZipCode = request.ShippingAddress.ZipCode
            };

            var order = new Order
            {
                UserId = request.UserId,
                OrderDate = DateTime.UtcNow,
                OrderStatus = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                ShippingAddress = shippingAddress,
                DeliveryMethod = request.DeliveryMethod,
                ShippingFee = shippingFee,
                DiscountAmount = discountAmount,
                TotalAmount = totalAmount,
                CouponId = request.CouponId,
                OrderItems = orderItems
            };

            await _orderRepo.AddAsync(order);

            foreach (var reservation in stockReservations)
            {
                reservation.Order = order;
                await _stockReservationRepo.AddAsync(reservation);
            }

            if (request.CouponId.HasValue)
            {
                var couponUsage = new CouponUsage
                {
                    UserId = request.UserId,
                    CouponId = request.CouponId.Value,
                    Order = order,
                    UsedAt = DateTime.UtcNow
                };
                await _couponUsageRepo.AddAsync(couponUsage);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Prevent notification failures from rolling back a successfully saved order
            try
            {
                await _notificationService.SendNotificationAsync(
                    userId: order.UserId,
                    title: "Order Placed",
                    message: $"Your order #{order.Id} has been placed successfully.",
                    type: NotificationType.OrderPlaced,
                    relatedEntityId: order.Id,
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send order placement notification for OrderId: {OrderId}", order.Id);
            }

            return order.Id;
        }
    }
}