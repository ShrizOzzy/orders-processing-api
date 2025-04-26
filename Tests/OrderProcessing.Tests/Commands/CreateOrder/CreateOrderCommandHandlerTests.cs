using FluentValidation;
using FluentAssertions;
using OrderProcessApi.OrderProcessing.Behaviors;
using OrderProcessApi.OrderProcessing.Commands.CreateOrder;
using OrderProcessApi.OrderProcessing.Models;
using OrderProcessApi.OrderProcessing.Models.DTOs;
using OrderProcessApi.OrderProcessing.Repositories;
using Moq;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace Tests.OrderProcessing.Tests.Commands.CreateOrder
{
    public class CreateOrderCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _repositoryMock;
        private readonly CreateOrderCommandHandler _handler;

        public CreateOrderCommandHandlerTests()
        {
            _repositoryMock = new Mock<IOrderRepository>();
            IValidator<CreateOrderCommand> validator = new CreateOrderCommandValidator();

            _handler = new CreateOrderCommandHandler(
                _repositoryMock.Object,
                validator);
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesOrderAndReturnsResponse()
        {
            // Arrange
            var productId = 1;
            var quantity = 2;
            var price = 100m;
            var totalPrice = price * quantity;

            var command = new CreateOrderCommand(new List<OrderItemRequestDto>
            {
                new OrderItemRequestDto { ProductId = productId, Quantity = quantity }
            });

            _repositoryMock.Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync(new Product { ProductId = productId, Price = price });

            _repositoryMock.Setup(x => x.CreateOrderAsync(It.IsAny<Order>(), It.IsAny<List<OrderItem>>()))
                .ReturnsAsync(1);

            _repositoryMock.Setup(x => x.GetOrderByIdAsync(1))
                .ReturnsAsync(new Order
                {
                    OrderId = 1,
                    OrderDate = DateTime.UtcNow,
                    TotalPrice = totalPrice,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            ProductId = productId,
                            Quantity = quantity,
                            Product = new Product { ProductId = productId, Price = price }
                        }
                    }
                });

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
            var productId = 999;
            var command = new CreateOrderCommand(new List<OrderItemRequestDto>
            {
                new OrderItemRequestDto { ProductId = productId, Quantity = 1 }
            });

            _repositoryMock.Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync(new Product());

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}