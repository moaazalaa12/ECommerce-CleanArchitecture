using Application.Exceptions;
using ECommerce.Application.Features.Orders.Commands.CreateOrder;
using ECommerce.Application.Orders.Commands.CreateOrder;
using ECommerce.Domain.Entities.CatalogEntities;
using ECommerce.Domain.Entities.OrderEntities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Interfaces.CatalogInterfaces;
using ECommerce.Domain.Interfaces.OrderInterfaces;
using ECommerce.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler(IOrderRepository _orderRepo,
        IProductRepository _productRepo,
        ICouponRepository _couponRepo,
        ICouponUsageRepository _couponUsageRepo,
        IStockReservationRepository _stockReservationRepo) : IRequestHandler<CreateOrderCommand, Guid>
    {
        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            if (request.Items == null || !request.Items.Any())
                throw new ArgumentException("Order must contain at least one item.");

            // 1. Fetch all requested products in a single DB trip
            var productIds = request.Items.Select(i => i.ProductId).ToList();
            var products = (await _productRepo.FindAsync(p => productIds.Contains(p.Id)))
                           .ToDictionary(p => p.Id);

            decimal subTotal = 0;
            var orderItems = new List<OrderItem>();
            var stockReservations = new List<StockReservation>();

            // 2. Validate products and calculate subtotal
            foreach (var item in request.Items)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                    throw new KeyNotFoundException($"Product with Id {item.ProductId} not found.");

                if (!product.IsActive)
                    throw new InvalidOperationException($"Product '{product.Name}' is not active.");

                if (product.StockQuantity < item.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for '{product.Name}'. Available: {product.StockQuantity}");

                // Prefer DiscountPrice if available
                var priceAtPurchase = product.DiscountPrice ?? product.Price;
                subTotal += priceAtPurchase * item.Quantity;

                // Deduct stock temporarily
                product.StockQuantity -= item.Quantity;
                _productRepo.Update(product);

                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceAtPurchase = priceAtPurchase
                });

                // 15-minute industry standard reservation
                stockReservations.Add(new StockReservation
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    ExpiryDate = DateTime.UtcNow.AddMinutes(15),
                    IsReleased = false
                });
            }

            // 3. Calculate Shipping Fee based on DeliveryMethod
            decimal shippingFee = request.DeliveryMethod switch
            {
                DeliveryMethod.Express => 100m,
                DeliveryMethod.Standard => 50m,
                _ => 50m
            };
            // 4. Validate and calculate Coupon discount
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

                // Prevent discount from exceeding the subtotal
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

            // 5. Create Order
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

            // 6. Link and save Stock Reservations
            foreach (var reservation in stockReservations)
            {
                reservation.Order = order;
                await _stockReservationRepo.AddAsync(reservation);
            }

            // 7. Save Coupon Usage
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

            // TransactionBehavior handles SaveChangesAsync & Commit.
            return order.Id;
        }
    }
}