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
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Document = request.Document,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _customerService.CreateAsync(customer, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    public record CustomerRequest(string Name, string Email, string Document);
}
