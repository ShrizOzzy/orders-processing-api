using FluentValidation;
using OrderProcess.Core.Models.DTOs;

namespace OrderProcessApi.OrderProcessing.Validations;

public class OrderItemRequestDtoValidator : AbstractValidator<OrderItemRequestDto>
{
    public OrderItemRequestDtoValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}