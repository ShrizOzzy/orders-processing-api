using FluentValidation;
using OrderProcessApi.OrderProcessing.Commands.CreateOrder;

namespace OrderProcessApi.OrderProcessing.Behaviors;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Items).NotEmpty().WithMessage("Order must contain at least one item");
        RuleForEach(x => x.Items).SetValidator(new OrderItemRequestDtoValidator());
    }
}