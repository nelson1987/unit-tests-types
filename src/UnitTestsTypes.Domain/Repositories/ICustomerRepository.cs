using UnitTestsTypes.Domain.Entities;

namespace UnitTestsTypes.Domain.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken);
    Task<Guid> AddAsync(Customer customer, CancellationToken cancellationToken);
}
