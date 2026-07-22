namespace UnitTestsTypes.Domain.Entities;

public class Customer
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Document { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
