using ECommerce.Application.Features.Payments.Commands.UpdatePaymentStatus;
using ECommerce.Domain.Entities.OrderEntities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces.OrderInterfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ECommerce.Application.UnitTests.Features.Payments;

public class UpdatePaymentStatusCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<ILogger<UpdatePaymentStatusCommandHandler>> _loggerMock;
    private readonly UpdatePaymentStatusCommandHandler _handler;

    public UpdatePaymentStatusCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _loggerMock = new Mock<ILogger<UpdatePaymentStatusCommandHandler>>();
        _handler = new UpdatePaymentStatusCommandHandler(
            _orderRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ShouldReturnWithoutChanges()
    {
        // Arrange
        var command = new UpdatePaymentStatusCommand("pi_unknown_123", true);

        _orderRepositoryMock
            .Setup(r => r.GetOrderByPaymentIntentIdAsync(command.PaymentIntentId))
            .ReturnsAsync((Order?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _orderRepositoryMock.Verify(
            r => r.GetOrderByPaymentIntentIdAsync(command.PaymentIntentId),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSuccessfulPayment_ShouldUpdateOrderAndPaymentStatus()
    {
        // Arrange
        var payment = new Payment
        {
            TransactionId = "pi_success_123",
            PaymentStatus = PaymentStatus.Pending
        };

        var order = new Order
        {
            Id = Guid.NewGuid(),
            PaymentStatus = PaymentStatus.Pending,
            OrderStatus = OrderStatus.Pending,
            Payments = new List<Payment> { payment }
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderByPaymentIntentIdAsync("pi_success_123"))
            .ReturnsAsync(order);

        var command = new UpdatePaymentStatusCommand("pi_success_123", true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        payment.PaymentStatus.Should().Be(PaymentStatus.Success);
        order.PaymentStatus.Should().Be(PaymentStatus.Success);
        order.OrderStatus.Should().Be(OrderStatus.Processing);
    }

    [Fact]
    public async Task Handle_WhenAlreadyProcessed_ShouldNotUpdateAgain()
    {
        // Arrange - Idempotency Test
        var payment = new Payment
        {
            TransactionId = "pi_already_processed",
            PaymentStatus = PaymentStatus.Success // Already Success
        };

        var order = new Order
        {
            Id = Guid.NewGuid(),
            PaymentStatus = PaymentStatus.Success,
            OrderStatus = OrderStatus.Processing,
            Payments = new List<Payment> { payment }
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderByPaymentIntentIdAsync("pi_already_processed"))
            .ReturnsAsync(order);

        var command = new UpdatePaymentStatusCommand("pi_already_processed", true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert - لا يزال نفس الحالة (لم تتغير)
        payment.PaymentStatus.Should().Be(PaymentStatus.Success);
        order.PaymentStatus.Should().Be(PaymentStatus.Success);
    }

    [Fact]
    public async Task Handle_WhenFailedPayment_ShouldUpdateToFailed()
    {
        // Arrange
        var payment = new Payment
        {
            TransactionId = "pi_failed_123",
            PaymentStatus = PaymentStatus.Pending
        };

        var order = new Order
        {
            Id = Guid.NewGuid(),
            PaymentStatus = PaymentStatus.Pending,
            OrderStatus = OrderStatus.Pending,
            Payments = new List<Payment> { payment }
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderByPaymentIntentIdAsync("pi_failed_123"))
            .ReturnsAsync(order);

        var command = new UpdatePaymentStatusCommand("pi_failed_123", false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        payment.PaymentStatus.Should().Be(PaymentStatus.Failed);
        order.PaymentStatus.Should().Be(PaymentStatus.Failed);
    }
}