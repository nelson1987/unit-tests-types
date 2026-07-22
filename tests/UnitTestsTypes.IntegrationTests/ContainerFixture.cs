using DotNet.Testcontainers.Containers;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace UnitTestsTypes.IntegrationTests;

public sealed class ContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase("unitdb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly RabbitMqContainer _rabbitMq = new RabbitMqBuilder()
        .WithImage("rabbitmq:3-management")
        .Build();

    public string ConnectionString => _postgres.GetConnectionString();
    public string RabbitHost => _rabbitMq.Hostname;
    public int RabbitPort => _rabbitMq.GetMappedPublicPort(5672);

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        await _rabbitMq.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _rabbitMq.DisposeAsync();
        await _postgres.DisposeAsync();
    }
}
