using Moq;
using OrderProcessApi.OrderProcessing.Models;
using OrderProcessApi.OrderProcessing.Queries.GetOrderById;
using OrderProcessApi.OrderProcessing.Repositories;

namespace Tests.OrderProcessing.Tests.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandlerTests
    {
        private readonly Mock<IOrderRepository> _mockRepository;
        private readonly GetOrderByIdQueryHandler _handler;

        public GetOrderByIdQueryHandlerTests()
        {
            _mockRepository = new Mock<IOrderRepository>();
            _handler = new GetOrderByIdQueryHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_ValidOrderId_ReturnsOrderResponseDto()
        {
            // Arrange
            var orderId = 1;
            var testOrder = CreateTestOrder(orderId);

            _mockRepository.Setup(r => r.GetOrderByIdAsync(orderId))
                .ReturnsAsync(testOrder);

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
                .ReturnsAsync((Order)null);

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

            _mockRepository.Setup(r => r.GetOrderByIdAsync(orderId))
                .ReturnsAsync(testOrder);

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
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = 1,
                        Quantity = 1,
                        Product = new Product
                        {
                            ProductId = 1,
                            ProductName = "Laptop",
                            Price = 100m
                        }
                    },
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
                }
            };
        }
    }
}