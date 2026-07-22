using UnitTestsTypes.Application.Abstractions;
using UnitTestsTypes.Domain.Entities;
using UnitTestsTypes.Domain.Repositories;
using UnitTestsTypes.Domain.Services;

namespace UnitTestsTypes.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly IMessagePublisher _messagePublisher;

    public CustomerService(ICustomerRepository repository, IMessagePublisher messagePublisher)
    {
        _repository = repository;
        _messagePublisher = messagePublisher;
    }

    public Task<Customer?> GetAsync(Guid id, CancellationToken cancellationToken)
        => _repository.GetByIdAsync(id, cancellationToken);

    public Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken)
        => _repository.GetAllAsync(cancellationToken);

    public async Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(customer.Name))
        {
            throw new ArgumentException("Name is required", nameof(customer));
        }

        var createdCustomer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = customer.Name,
            Email = customer.Email,
            Document = customer.Document,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(createdCustomer, cancellationToken);
        await _messagePublisher.PublishAsync("customers", createdCustomer, cancellationToken);
        return createdCustomer;
    }
}
