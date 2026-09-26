using Application.Exceptions;
using ECommerce.Application.Features.Payments.Commands.CreatePaymentIntent;
using ECommerce.Application.Interfaces.Payment;
using ECommerce.Domain.Entities.OrderEntities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces.OrderInterfaces;
using FluentAssertions;
using Moq;

namespace ECommerce.Application.UnitTests.Features.Payments;

public class CreatePaymentIntentCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly CreatePaymentIntentCommandHandler _handler;

    public CreatePaymentIntentCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _paymentServiceMock = new Mock<IPaymentService>();
        _handler = new CreatePaymentIntentCommandHandler(
            _orderRepositoryMock.Object,
            _paymentServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new CreatePaymentIntentCommand(
            Guid.NewGuid(), Guid.NewGuid(), "test@test.com", "Test User");

        _orderRepositoryMock
            .Setup(r => r.GetOrderWithDetailsByIdAsync(command.OrderId))
            .ReturnsAsync((Order?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Order with ID {command.OrderId} was not found.");
    }

    [Fact]
    public async Task Handle_WhenOrderBelongsToAnotherUser_ShouldThrowForbiddenException()
    {
        // Arrange
        var command = new CreatePaymentIntentCommand(
            Guid.NewGuid(), Guid.NewGuid(), "test@test.com", "Test User");

        var order = new Order { Id = command.OrderId, UserId = Guid.NewGuid() };

        _orderRepositoryMock
            .Setup(r => r.GetOrderWithDetailsByIdAsync(command.OrderId))
            .ReturnsAsync(order);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_WhenOrderAlreadyPaid_ShouldThrowBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreatePaymentIntentCommand(
            Guid.NewGuid(), userId, "test@test.com", "Test User");

        var order = new Order
        {
            Id = command.OrderId,
            UserId = userId,
            PaymentStatus = PaymentStatus.Success
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderWithDetailsByIdAsync(command.OrderId))
            .ReturnsAsync(order);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("This order has already been paid.");
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldCreateNewPaymentAndReturnSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreatePaymentIntentCommand(
            Guid.NewGuid(), userId, "test@test.com", "Test User");

        var order = new Order
        {
            Id = command.OrderId,
            UserId = userId,
            TotalAmount = 100m,
            PaymentStatus = PaymentStatus.Pending,
            OrderStatus = OrderStatus.Pending,
            Payments = new List<Payment>()
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderWithDetailsByIdAsync(command.OrderId))
            .ReturnsAsync(order);

        _paymentServiceMock
            .Setup(s => s.CreateOrUpdatePaymentIntentAsync(
                It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(PaymentResponseDto.Success("pi_test_123", "pi_test_123_secret"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.PaymentIntentId.Should().Be("pi_test_123");
        result.ClientSecret.Should().Be("pi_test_123_secret");
        order.Payments.Should().HaveCount(1);
        order.Payments.First().TransactionId.Should().Be("pi_test_123");
        order.Payments.First().PaymentStatus.Should().Be(PaymentStatus.Pending);
    }

    [Fact]
    public async Task Handle_WhenExistingPendingPayment_ShouldUpdateInsteadOfCreate()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreatePaymentIntentCommand(
            Guid.NewGuid(), userId, "test@test.com", "Test User");

        var existingPayment = new Payment
        {
            Id = Guid.NewGuid(),
            TransactionId = "pi_old_123",
            PaymentStatus = PaymentStatus.Pending,
            Amount = 50m
        };

        var order = new Order
        {
            Id = command.OrderId,
            UserId = userId,
            TotalAmount = 100m,
            PaymentStatus = PaymentStatus.Pending,
            OrderStatus = OrderStatus.Pending,
            Payments = new List<Payment> { existingPayment }
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderWithDetailsByIdAsync(command.OrderId))
            .ReturnsAsync(order);

        _paymentServiceMock
            .Setup(s => s.CreateOrUpdatePaymentIntentAsync(
                It.Is<PaymentRequestDto>(r => r.PaymentIntentId == "pi_old_123"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(PaymentResponseDto.Success("pi_old_123", "pi_old_123_secret"));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        order.Payments.Should().HaveCount(1); // No new payment added
        existingPayment.TransactionId.Should().Be("pi_old_123");
        existingPayment.Amount.Should().Be(100m);
    }

    [Fact]
    public async Task Handle_WhenPaymentServiceFails_ShouldThrowBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreatePaymentIntentCommand(
            Guid.NewGuid(), userId, "test@test.com", "Test User");

        var order = new Order
        {
            Id = command.OrderId,
            UserId = userId,
            TotalAmount = 100m,
            PaymentStatus = PaymentStatus.Pending,
            OrderStatus = OrderStatus.Pending,
            Payments = new List<Payment>()
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderWithDetailsByIdAsync(command.OrderId))
            .ReturnsAsync(order);

        _paymentServiceMock
            .Setup(s => s.CreateOrUpdatePaymentIntentAsync(
                It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(PaymentResponseDto.Failure("Stripe error"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("*Failed to initialize payment*");
    }
}