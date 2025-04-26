using Microsoft.EntityFrameworkCore;
using OrderProcessApi.OrderProcessing.Data;
using OrderProcessApi.OrderProcessing.Models;

namespace OrderProcessApi.OrderProcessing.Repositories
{
    public class OrderRepository(OrderDbContext context) : IOrderRepository
    {
        public async Task<Product> GetProductByIdAsync(int productId)
        {
           var product = await context.Products.FindAsync(productId);
           return product;
        }

        public async Task<int> CreateOrderAsync(Order order, List<OrderItem> orderItems)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                context.Orders.Add(order);
                await context.SaveChangesAsync();

                foreach (var item in orderItems)
                {
                    item.OrderId = order.OrderId;
                    context.OrderItems.Add(item);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order.OrderId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            var order = await context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
            return order;
        }
    }
}