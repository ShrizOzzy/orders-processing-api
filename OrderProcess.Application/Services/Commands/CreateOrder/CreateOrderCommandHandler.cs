using AutoMapper;
using FluentValidation;
using MediatR;
using OrderProcess.Core.Models;
using OrderProcess.Core.Models.DTOs;
using OrderProcess.Data.Repositories;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace OrderProcess.Application.Services.Commands.CreateOrder;

public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IValidator<CreateOrderCommand> validator,
    IMapper mapper)
    : IRequestHandler<CreateOrderCommand, OrderResponseDto>
{
    private readonly IMapper _mapper = mapper;

    public async Task<OrderResponseDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors.ToString());
        }

        var productFetchTasks = request.Items
            .Select(item => orderRepository.GetProductByIdAsync(item.ProductId))
            .ToList();

        var products = await Task.WhenAll(productFetchTasks);

        var orderItems = new List<OrderItem>();
        decimal totalPrice = 0;

        for (var i = 0; i < request.Items.Count; i++)
        {
            var item = request.Items[i];
            var product = products[i];

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

        var orderId = await orderRepository.CreateOrderAsync(order, orderItems);

        var createdOrder = await orderRepository.GetOrderByIdAsync(orderId);

        var responseDto = _mapper.Map<OrderResponseDto>(createdOrder);
        return responseDto;
    }
}
