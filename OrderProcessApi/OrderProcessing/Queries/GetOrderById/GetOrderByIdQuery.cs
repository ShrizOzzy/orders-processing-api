using MediatR;
using OrderProcessApi.OrderProcessing.Models.DTOs;

namespace OrderProcessApi.OrderProcessing.Queries.GetOrderById
{
    public class GetOrderByIdQuery(int orderId) : IRequest<OrderResponseDto>
    {
        public int OrderId { get; } = orderId;
    }
}
