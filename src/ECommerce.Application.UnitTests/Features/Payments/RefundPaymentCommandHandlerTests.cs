using Application.Exceptions;
using ECommerce.Application.Features.Payments.Commands.RefundPayment;
using ECommerce.Application.Interfaces.Payment;
using ECommerce.Domain.Entities.OrderEntities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces.OrderInterfaces;
using FluentAssertions;
using Moq;

namespace ECommerce.Application.UnitTests.Features.Payments;

public class RefundPaymentCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly RefundPaymentCommandHandler _handler;

    public RefundPaymentCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _paymentServiceMock = new Mock<IPaymentService>();
        _handler = new RefundPaymentCommandHandler(
            _orderRepositoryMock.Object,
            _paymentServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new RefundPaymentCommand(Guid.NewGuid(), "Customer request");

        _orderRepositoryMock
            .Setup(r => r.GetOrderWithDetailsByIdAsync(command.OrderId))
            .ReturnsAsync((Order?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenOrderAlreadyRefunded_ShouldThrowBadRequestException()
    {
        // Arrange
        var command = new RefundPaymentCommand(Guid.NewGuid(), "Customer request");

        var order = new Order
        {
            Id = command.OrderId,
            PaymentStatus = PaymentStatus.Refunded
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderWithDetailsByIdAsync(command.OrderId))
            .ReturnsAsync(order);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("This order has already been refunded.");
    }

    [Fact]
    public async Task Handle_WhenNoSuccessfulPayment_ShouldThrowBadRequestException()
    {
        // Arrange
        var command = new RefundPaymentCommand(Guid.NewGuid(), "Customer request");

        var order = new Order
        {
            Id = command.OrderId,
            PaymentStatus = PaymentStatus.Pending,
            Payments = new List<Payment>
            {
                new() { PaymentStatus = PaymentStatus.Pending }
            }
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderWithDetailsByIdAsync(command.OrderId))
            .ReturnsAsync(order);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("*does not have a successful payment*");
    }

    [Fact]
    public async Task Handle_WhenValidRefund_ShouldUpdateStatusesAndReturnSuccess()
    {
        // Arrange
        var command = new RefundPaymentCommand(Guid.NewGuid(), "Defective product");

        var successfulPayment = new Payment
        {
            TransactionId = "pi_success_123",
            PaymentStatus = PaymentStatus.Success,
            Amount = 100m
        };

        var order = new Order
        {
            Id = command.OrderId,
            PaymentStatus = PaymentStatus.Success,
            Payments = new List<Payment> { successfulPayment }
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderWithDetailsByIdAsync(command.OrderId))
            .ReturnsAsync(order);

        _paymentServiceMock
            .Setup(s => s.RefundAsync(
                It.IsAny<RefundRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(RefundResponseDto.Success("re_test_123"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.RefundTransactionId.Should().Be("re_test_123");
        successfulPayment.PaymentStatus.Should().Be(PaymentStatus.Refunded);
        order.PaymentStatus.Should().Be(PaymentStatus.Refunded);
    }

    [Fact]
    public async Task Handle_WhenRefundServiceFails_ShouldThrowBadRequestException()
    {
        // Arrange
        var command = new RefundPaymentCommand(Guid.NewGuid(), "Customer request");

        var successfulPayment = new Payment
        {
            TransactionId = "pi_success_123",
            PaymentStatus = PaymentStatus.Success,
            Amount = 100m
        };

        var order = new Order
        {
            Id = command.OrderId,
            PaymentStatus = PaymentStatus.Success,
            Payments = new List<Payment> { successfulPayment }
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderWithDetailsByIdAsync(command.OrderId))
            .ReturnsAsync(order);

        _paymentServiceMock
            .Setup(s => s.RefundAsync(
                It.IsAny<RefundRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(RefundResponseDto.Failure("Stripe refund failed"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("*Refund failed*");
    }
}