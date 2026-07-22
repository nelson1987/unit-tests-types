using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UnitTestsTypes.Application.Abstractions;
using UnitTestsTypes.Application.Services;
using UnitTestsTypes.Domain.Repositories;
using UnitTestsTypes.Domain.Services;
using UnitTestsTypes.Infrastructure.MessageBus;
using UnitTestsTypes.Infrastructure.Repositories;

namespace UnitTestsTypes.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICustomerRepository, DapperCustomerRepository>();
        services.AddSingleton<ICustomerService, CustomerService>();
        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
        return services;
    }
}
