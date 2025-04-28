using MediatR;
using OrderProcess.Core.Models.DTOs;

namespace OrderProcess.Application.Services.Queries.GetOrderById;

public class GetOrderByIdQuery(int orderId) : IRequest<OrderResponseDto>
{
    public int OrderId { get; } = orderId;
}