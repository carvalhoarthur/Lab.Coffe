using MediatR;
using Microsoft.AspNetCore.Mvc;
using Lab.Coffe.Application.DTOs;
using Lab.Coffe.Application.UseCases.Coffee;

namespace Lab.Coffe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CoffeeController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CoffeeController> _logger;

    public CoffeeController(IMediator mediator, ILogger<CoffeeController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all coffees
    /// </summary>
    /// <returns>List of all coffees</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CoffeeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CoffeeDto>>> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all coffees");
        var result = await _mediator.Send(new GetAllCoffeesQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get coffee by ID
    /// </summary>
    /// <param name="id">Coffee ID</param>
    /// <returns>Coffee details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CoffeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CoffeeDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting coffee with ID: {CoffeeId}", id);
        var result = await _mediator.Send(new GetCoffeeByIdQuery(id), cancellationToken);
        
        if (result == null)
        {
            return NotFound($"Coffee with ID {id} not found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Create a new coffee
    /// </summary>
    /// <param name="request">Coffee creation data</param>
    /// <returns>Created coffee</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CoffeeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CoffeeDto>> Create([FromBody] CreateCoffeeRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new coffee: {CoffeeName}", request.Name);
        var result = await _mediator.Send(new CreateCoffeeCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing coffee
    /// </summary>
    /// <param name="id">Coffee ID</param>
    /// <param name="request">Coffee update data</param>
    /// <returns>Updated coffee</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CoffeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CoffeeDto>> Update(Guid id, [FromBody] UpdateCoffeeRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating coffee with ID: {CoffeeId}", id);
        
        try
        {
            var result = await _mediator.Send(new UpdateCoffeeCommand(id, request), cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Delete a coffee
    /// </summary>
    /// <param name="id">Coffee ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting coffee with ID: {CoffeeId}", id);
        var result = await _mediator.Send(new DeleteCoffeeCommand(id), cancellationToken);
        
        if (!result)
        {
            return NotFound($"Coffee with ID {id} not found");
        }

        return NoContent();
    }
}
