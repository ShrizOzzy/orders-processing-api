using FluentValidation;
using OrderProcess.Application.Services.Commands.CreateOrder;

namespace OrderProcessApi.OrderProcessing.Validations;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Items).NotEmpty().WithMessage("Order must contain at least one item");
        RuleForEach(x => x.Items).SetValidator(new OrderItemRequestDtoValidator());
    }
}