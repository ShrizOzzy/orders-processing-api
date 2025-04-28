using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Moq;
using OrderProcess.Application.Services.Commands.CreateOrder;
using OrderProcess.Core.Models;
using OrderProcess.Core.Models.DTOs;
using OrderProcess.Data.Repositories;
using OrderProcessApi.OrderProcessing.Validations;

namespace Tests.OrderProcess.Application.Tests.Commands.CreateOrder;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _mapperMock = new Mock<IMapper>();
        IValidator<CreateOrderCommand> validator = new CreateOrderCommandValidator();

        _handler = new CreateOrderCommandHandler(
            _repositoryMock.Object,
            validator,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesOrderAndReturnsResponse()
    {
        // Arrange
        const int productId = 1;
        const int quantity = 2;
        const decimal price = 100m;
        const decimal totalPrice = price * quantity;

        var command = new CreateOrderCommand(
                [
                    new OrderItemRequestDto { ProductId = productId, Quantity = quantity }
                ]
            );

        var orderEntity = new Order
        {
            OrderId = 1,
            OrderDate = DateTime.UtcNow,
            TotalPrice = totalPrice,
            OrderItems = new List<OrderItem>
            {
                new()
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Product = new Product { ProductId = productId, Price = price }
                }
            }
        };

        var expectedResponse = new OrderResponseDto
        {
            OrderId = 1,
            OrderDate = orderEntity.OrderDate,
            TotalPrice = totalPrice,
            Items =
            [
                new OrderItemDto
                {
                    ProductId = productId,
                    ProductName = "Test Product",
                    UnitPrice = price,
                    Quantity = quantity,
                    ItemTotal = price * quantity
                }
            ]
        };

        _repositoryMock.Setup(x => x.GetProductByIdAsync(productId))
            .ReturnsAsync(new Product { ProductId = productId, Price = price });

        _repositoryMock.Setup(x => x.CreateOrderAsync(It.IsAny<Order>(), It.IsAny<List<OrderItem>>()))
            .ReturnsAsync(1);

        _repositoryMock.Setup(x => x.GetOrderByIdAsync(1))
            .ReturnsAsync(orderEntity);

        _mapperMock.Setup(x => x.Map<OrderResponseDto>(It.IsAny<Order>()))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.OrderId.Should().Be(1);
        result.TotalPrice.Should().Be(totalPrice);
        result.Items.Should().HaveCount(1);
        result.Items[0].ProductId.Should().Be(productId);
        result.Items[0].Quantity.Should().Be(quantity);
        result.Items[0].ItemTotal.Should().Be(totalPrice);

        _repositoryMock.Verify(x => x.CreateOrderAsync(It.IsAny<Order>(), It.IsAny<List<OrderItem>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidProduct_Exception()
    {
        // Arrange
        const int productId = 999;
        var command = new CreateOrderCommand(
            [
                new OrderItemRequestDto { ProductId = productId, Quantity = 1 }
            ]
        );

        _repositoryMock.Setup(x => x.GetProductByIdAsync(productId))
            .ReturnsAsync((Product)null!); // simulate product not found

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
