using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderProcess.Core.Models;

public class OrderItem
{
    public OrderItem()
    {
    }

    public OrderItem(int productId, int quantity, Product product)
    {
        ProductId = productId;
        Quantity = quantity;
        Product = product;
    }

    [Key]
    public int OrderItemId { get; set; }

    [Required]
    [ForeignKey("Order")]
    public int OrderId { get; set; }

    [Required]
    [ForeignKey("Product")]
    public int ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }

    public Order Order { get; set; }
    public Product Product { get; set; }
}