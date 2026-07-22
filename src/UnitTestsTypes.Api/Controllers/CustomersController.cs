using Microsoft.AspNetCore.Mvc;
using UnitTestsTypes.Domain.Entities;
using UnitTestsTypes.Domain.Services;

namespace UnitTestsTypes.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Customer>> Get(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetAsync(id, cancellationToken);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Customer>>> GetAll(CancellationToken cancellationToken)
    {
        var customers = await _customerService.GetAllAsync(cancellationToken);
        return Ok(customers);
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> Post([FromBody] CustomerRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Request body is required",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Document = request.Document,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _customerService.CreateAsync(customer, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid customer data",
                Detail = result.Error!.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, result.Value);
    }

    public record CustomerRequest(string Name, string Email, string Document);
}
