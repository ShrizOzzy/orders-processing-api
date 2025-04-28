using MediatR;
using OrderProcess.Core.Models.DTOs;

namespace OrderProcess.Application.Services.Commands.CreateOrder;

public class CreateOrderCommand(List<OrderItemRequestDto> items) : IRequest<OrderResponseDto>
{
    public List<OrderItemRequestDto> Items { get; } = items;
}