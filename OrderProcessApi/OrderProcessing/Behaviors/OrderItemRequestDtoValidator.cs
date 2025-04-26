using FluentValidation;
using OrderProcessApi.OrderProcessing.Models.DTOs;

namespace OrderProcessApi.OrderProcessing.Behaviors;

public class OrderItemRequestDtoValidator : AbstractValidator<OrderItemRequestDto>
{
    public OrderItemRequestDtoValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}