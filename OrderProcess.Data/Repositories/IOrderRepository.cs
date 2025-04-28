using OrderProcess.Core.Models;

namespace OrderProcess.Data.Repositories;

public interface IOrderRepository
{
    Task<Product> GetProductByIdAsync(int productId);
    Task<int> CreateOrderAsync(Order order, List<OrderItem> orderItems);
    Task<Order> GetOrderByIdAsync(int orderId);
}