using NSubstitute;
using Shouldly;
using UnitTestsTypes.Application.Abstractions;
using UnitTestsTypes.Application.Services;
using UnitTestsTypes.Domain.Common;
using UnitTestsTypes.Domain.Entities;
using UnitTestsTypes.Domain.Repositories;

namespace UnitTestsTypes.UnitTests;

public class CustomerServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldPopulateIdAndCreatedAt()
    {
        var repository = Substitute.For<ICustomerRepository>();
        var publisher = Substitute.For<IMessagePublisher>();
        repository.AddAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromResult(callInfo.Arg<Customer>().Id));

        var service = new CustomerService(repository, publisher);

        var result = await service.CreateAsync(new Customer { Name = "Ana", Email = "ana@email.com", Document = "123" }, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Id.ShouldNotBe(Guid.Empty);
        result.Value.CreatedAt.ShouldBeGreaterThan(DateTime.MinValue);

        await repository.Received(1).AddAsync(Arg.Is<Customer>(customer => customer.Name == "Ana" && customer.Id != Guid.Empty), Arg.Any<CancellationToken>());
        await publisher.Received(1).PublishAsync("customers", Arg.Is<Customer>(customer => customer.Id == result.Value!.Id), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFailure_WhenNameIsMissing()
    {
        var repository = Substitute.For<ICustomerRepository>();
        var publisher = Substitute.For<IMessagePublisher>();
        var service = new CustomerService(repository, publisher);

        var result = await service.CreateAsync(new Customer { Email = "ana@email.com", Document = "123" }, CancellationToken.None);

        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error!.Code.ShouldBe("validation_error");
        result.Error.Message.ShouldBe("Name is required");
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFailure_WhenNameIsWhitespaceAndNotPersistAnything()
    {
        var repository = Substitute.For<ICustomerRepository>();
        var publisher = Substitute.For<IMessagePublisher>();
        var service = new CustomerService(repository, publisher);

        var result = await service.CreateAsync(new Customer { Name = "   ", Email = "ana@email.com", Document = "123" }, CancellationToken.None);

        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error!.Code.ShouldBe("validation_error");
        result.Error.Message.ShouldBe("Name is required");

        await repository.DidNotReceive().AddAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>());
        await publisher.DidNotReceive().PublishAsync(Arg.Any<string>(), Arg.Any<Customer>(), Arg.Any<CancellationToken>());
    }
}
