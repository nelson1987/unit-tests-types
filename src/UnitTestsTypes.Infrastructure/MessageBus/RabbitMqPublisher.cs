using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using UnitTestsTypes.Application.Abstractions;

namespace UnitTestsTypes.Infrastructure.MessageBus;

public class RabbitMqPublisher : IMessagePublisher
{
    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _password;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        _hostName = configuration["RabbitMq:HostName"] ?? "localhost";
        _userName = configuration["RabbitMq:UserName"] ?? "guest";
        _password = configuration["RabbitMq:Password"] ?? "guest";
    }

    public Task PublishAsync(string topic, object payload, CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory { HostName = _hostName, UserName = _userName, Password = _password };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: topic, type: ExchangeType.Fanout, durable: true, autoDelete: false);
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));
        channel.BasicPublish(exchange: topic, routingKey: string.Empty, basicProperties: null, body: body);
        return Task.CompletedTask;
    }
}
