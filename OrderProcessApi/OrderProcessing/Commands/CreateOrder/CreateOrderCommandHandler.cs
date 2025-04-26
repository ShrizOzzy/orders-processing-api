using FluentValidation;
using MediatR;
using OrderProcessApi.OrderProcessing.Models;
using OrderProcessApi.OrderProcessing.Models.DTOs;
using OrderProcessApi.OrderProcessing.Repositories;

namespace OrderProcessApi.OrderProcessing.Commands.CreateOrder;

public class CreateOrderCommandHandler(
    IOrderRepository repository,
    IValidator<CreateOrderCommand> validator)
    : IRequestHandler<CreateOrderCommand, OrderResponseDto>
{
    public async Task<OrderResponseDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var orderItems = new List<OrderItem>();
        decimal totalPrice = 0;

        foreach (var item in request.Items)
        {
            var product = await repository.GetProductByIdAsync(item.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {item.ProductId} not found");
            }

            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            };

            orderItems.Add(orderItem);
            totalPrice += product.Price * item.Quantity;
        }

        var order = new Order
        {
            OrderDate = DateTime.UtcNow,
            TotalPrice = totalPrice
        };

        var orderId = await repository.CreateOrderAsync(order, orderItems);

        var createdOrder = await repository.GetOrderByIdAsync(orderId);

        return new OrderResponseDto
        {
            OrderId = createdOrder.OrderId,
            OrderDate = createdOrder.OrderDate,
            TotalPrice = createdOrder.TotalPrice,
            Items = createdOrder.OrderItems.Select(oi => new OrderItemDto
            {
                ProductId = oi.ProductId,
                ProductName = oi.Product.ProductName,
                UnitPrice = oi.Product.Price,
                Quantity = oi.Quantity,
                ItemTotal = oi.Product.Price * oi.Quantity
            }).ToList()
        };
    }
}