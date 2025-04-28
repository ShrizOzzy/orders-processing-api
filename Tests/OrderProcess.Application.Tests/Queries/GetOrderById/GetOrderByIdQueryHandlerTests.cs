using AutoMapper;
using Moq;
using OrderProcess.Application.Services.Queries.GetOrderById;
using OrderProcess.Core.Models;
using OrderProcess.Core.Models.DTOs;
using OrderProcess.Data.Repositories;

namespace Tests.OrderProcess.Application.Tests.Queries.GetOrderById;

public class GetOrderByIdQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetOrderByIdQueryHandler _handler;

    public GetOrderByIdQueryHandlerTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _mockMapper = new Mock<IMapper>();

        _handler = new GetOrderByIdQueryHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ValidOrderId_ReturnsOrderResponseDto()
    {
        // Arrange
        var orderId = 1;
        var testOrder = CreateTestOrder(orderId);

        var expectedResponse = CreateExpectedOrderResponseDto(orderId);

        _mockRepository.Setup(r => r.GetOrderByIdAsync(orderId))
            .ReturnsAsync(testOrder);

        _mockMapper.Setup(m => m.Map<OrderResponseDto>(It.IsAny<Order>()))
            .Returns(expectedResponse);

        var query = new GetOrderByIdQuery(orderId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderId, result.OrderId);
        Assert.Equal(150m, result.TotalPrice);
        Assert.Equal(2, result.Items.Count);

        _mockRepository.Verify(r => r.GetOrderByIdAsync(orderId), Times.Once);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var orderId = 999;

        _mockRepository.Setup(r => r.GetOrderByIdAsync(orderId))
            .ReturnsAsync((Order)null!);

        var query = new GetOrderByIdQuery(orderId);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None));

        _mockRepository.Verify(r => r.GetOrderByIdAsync(orderId), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var orderId = 1;
        var expectedException = new Exception("Database error");

        _mockRepository.Setup(r => r.GetOrderByIdAsync(orderId))
            .ThrowsAsync(expectedException);

        var query = new GetOrderByIdQuery(orderId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(query, CancellationToken.None));

        Assert.Equal(expectedException.Message, exception.Message);
    }

    [Fact]
    public async Task Handle_ValidOrder_MapsItemsCorrectly()
    {
        // Arrange
        var orderId = 1;
        var testOrder = CreateTestOrder(orderId);

        var expectedResponse = CreateExpectedOrderResponseDto(orderId);

        _mockRepository.Setup(r => r.GetOrderByIdAsync(orderId))
            .ReturnsAsync(testOrder);

        _mockMapper.Setup(m => m.Map<OrderResponseDto>(It.IsAny<Order>()))
            .Returns(expectedResponse);

        var query = new GetOrderByIdQuery(orderId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var firstItem = result.Items.First();
        Assert.Equal(1, firstItem.ProductId);
        Assert.Equal("Laptop", firstItem.ProductName);
        Assert.Equal(100m, firstItem.UnitPrice);
        Assert.Equal(1, firstItem.Quantity);
        Assert.Equal(100m, firstItem.ItemTotal);
    }

    private static Order CreateTestOrder(int orderId)
    {
        return new Order
        {
            OrderId = orderId,
            OrderDate = DateTime.UtcNow,
            TotalPrice = 150m,
            OrderItems =
            [
                new(productId: 1, quantity: 1, product: new Product
                {
                    ProductId = 1,
                    ProductName = "Laptop",
                    Price = 100m
                }),
                new OrderItem
                {
                    ProductId = 2,
                    Quantity = 1,
                    Product = new Product
                    {
                        ProductId = 2,
                        ProductName = "Mouse",
                        Price = 50m
                    }
                }
            ]
        };
    }

    private static OrderResponseDto CreateExpectedOrderResponseDto(int orderId)
    {
        return new OrderResponseDto
        {
            OrderId = orderId,
            OrderDate = DateTime.UtcNow,
            TotalPrice = 150m,
            Items =
            [
                new OrderItemDto
                {
                    ProductId = 1,
                    ProductName = "Laptop",
                    UnitPrice = 100m,
                    Quantity = 1,
                    ItemTotal = 100m
                },

                new OrderItemDto
                {
                    ProductId = 2,
                    ProductName = "Mouse",
                    UnitPrice = 50m,
                    Quantity = 1,
                    ItemTotal = 50m
                }
            ]
        };
    }
}
