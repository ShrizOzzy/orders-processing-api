using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderProcess.Application.Services.Commands.CreateOrder;
using OrderProcess.Application.Services.Queries.GetOrderById;
using OrderProcess.Core.Models.DTOs;

namespace OrderProcessApi.OrderProcessing.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController(IMediator mediator, ILogger<OrdersController> logger) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] List<OrderItemRequestDto> items)
    {
        try
        {
            var command = new CreateOrderCommand(items);
            var result = await mediator.Send(command);

            return CreatedAtAction(nameof(GetOrder), new { id = result.OrderId }, result);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(ex, "Validation error in order creation");
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "Product not found in order creation");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating order");
            return StatusCode(500, "An error occurred while processing your order");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderResponseDto>> GetOrder(int id)
    {
        try
        {
            var query = new GetOrderByIdQuery(id);
            var result = await mediator.Send(query);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "Order not found");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving order");
            return StatusCode(500, "An error occurred while retrieving your order");
        }
    }
}