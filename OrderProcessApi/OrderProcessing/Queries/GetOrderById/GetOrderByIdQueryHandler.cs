using MediatR;
using OrderProcessApi.OrderProcessing.Models.DTOs;
using OrderProcessApi.OrderProcessing.Repositories;

namespace OrderProcessApi.OrderProcessing.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler(
        IOrderRepository repository)
        : IRequestHandler<GetOrderByIdQuery, OrderResponseDto>
    {
        public async Task<OrderResponseDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await repository.GetOrderByIdAsync(request.OrderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {request.OrderId} not found");
            }

            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.ProductName,
                    UnitPrice = oi.Product.Price,
                    Quantity = oi.Quantity,
                    ItemTotal = oi.Product.Price * oi.Quantity
                }).ToList()
            };
        }
    }
}
