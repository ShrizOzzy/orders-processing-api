using AutoMapper;
using MediatR;
using OrderProcess.Core.Models.DTOs;
using OrderProcess.Data.Repositories;

namespace OrderProcess.Application.Services.Queries.GetOrderById;

public class GetOrderByIdQueryHandler(
    IOrderRepository orderRepository, IMapper mapper)
    : IRequestHandler<GetOrderByIdQuery, OrderResponseDto>
{
    private readonly IMapper _mapper = mapper;
    public async Task<OrderResponseDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {

        var order = await orderRepository.GetOrderByIdAsync(request.OrderId);

        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {request.OrderId} not found");
        }

        var responseDto = _mapper.Map<OrderResponseDto>(order);
        return responseDto;
    }
}