using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using UnitTestsTypes.Api.Controllers;

namespace UnitTestsTypes.IntegrationTests;

public class CustomersApiTests : IClassFixture<ContainerFixture>, IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ContainerFixture _containerFixture;

    public CustomersApiTests(WebApplicationFactory<Program> factory, ContainerFixture containerFixture)
    {
        _containerFixture = containerFixture;
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Postgres"] = _containerFixture.ConnectionString,
                    ["RabbitMq:HostName"] = _containerFixture.RabbitHost,
                    ["RabbitMq:Port"] = _containerFixture.RabbitPort.ToString()
                });
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/api/customers");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_ShouldReturnCreated()
    {
        var request = new CustomersController.CustomerRequest("Ana", "ana@email.com", "123");
        var response = await _client.PostAsJsonAsync("/api/customers", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
