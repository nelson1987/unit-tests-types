using UnitTestsTypes.Domain.Entities;

namespace UnitTestsTypes.Domain.Services;

public interface ICustomerService
{
    Task<Customer?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken);
    Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken);
}
