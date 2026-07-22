namespace UnitTestsTypes.Application.Abstractions;

public interface IMessagePublisher
{
    Task PublishAsync(string topic, object payload, CancellationToken cancellationToken);
}
