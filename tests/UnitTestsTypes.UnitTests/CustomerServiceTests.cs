using UnitTestsTypes.Application.Abstractions;
using UnitTestsTypes.Application.Services;
using UnitTestsTypes.Domain.Entities;
using UnitTestsTypes.Domain.Repositories;

namespace UnitTestsTypes.UnitTests;

public class CustomerServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldPopulateIdAndCreatedAt()
    {
        var repository = new FakeCustomerRepository();
        var service = new CustomerService(repository, new FakePublisher());

        var result = await service.CreateAsync(new Customer { Name = "Ana", Email = "ana@email.com", Document = "123" }, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.True(result.CreatedAt > DateTime.MinValue);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenNameIsMissing()
    {
        var repository = new FakeCustomerRepository();
        var service = new CustomerService(repository, new FakePublisher());

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(new Customer { Email = "ana@email.com", Document = "123" }, CancellationToken.None));
    }

    private sealed class FakeCustomerRepository : ICustomerRepository
    {
        public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult<Customer?>(null);
        public Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Customer>>(Array.Empty<Customer>());
        public Task<Guid> AddAsync(Customer customer, CancellationToken cancellationToken) => Task.FromResult(customer.Id);
    }

    private sealed class FakePublisher : IMessagePublisher
    {
        public Task PublishAsync(string topic, object payload, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
