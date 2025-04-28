using System.ComponentModel.DataAnnotations;

namespace OrderProcess.Core.Models.DTOs;

/// <summary>
/// Data transfer object for creating an order item
/// </summary>
public class OrderItemRequestDto
{
    /// <summary>
    /// ID of the product being ordered
    /// </summary>
    [Required(ErrorMessage = "Product ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Product ID must be a positive number")]
    public int ProductId { get; set; }

    /// <summary>
    /// Quantity of the product being ordered
    /// </summary>
    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
}