using OrderProcessApi.OrderProcessing.Models.DTOs;
using MediatR;

namespace OrderProcessApi.OrderProcessing.Commands.CreateOrder
{
    public class CreateOrderCommand(List<OrderItemRequestDto> items) : IRequest<OrderResponseDto>
    {
        public List<OrderItemRequestDto> Items { get; } = items;
    }
}