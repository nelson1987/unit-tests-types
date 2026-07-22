using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Shouldly;
using UnitTestsTypes.Api.Controllers;
using UnitTestsTypes.Domain.Common;
using UnitTestsTypes.Domain.Entities;
using UnitTestsTypes.Domain.Services;

namespace UnitTestsTypes.UnitTests;

public class CustomersControllerTests
{
    [Fact]
    public async Task Get_WhenCustomerExists_ReturnsOkResult()
    {
        var customerService = Substitute.For<ICustomerService>();
        var customer = new Customer { Id = Guid.NewGuid(), Name = "Ana", Email = "ana@email.com", Document = "123" };
        customerService.GetAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);

        var controller = new CustomersController(customerService);

        var result = await controller.Get(customer.Id, CancellationToken.None);

        result.Result.ShouldBeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.ShouldBe(customer);
    }

    [Fact]
    public async Task Get_WhenCustomerDoesNotExist_ReturnsNotFound()
    {
        var customerService = Substitute.For<ICustomerService>();
        customerService.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Customer?)null);

        var controller = new CustomersController(customerService);

        var result = await controller.Get(Guid.NewGuid(), CancellationToken.None);

        result.Result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetAll_ReturnsAllCustomers()
    {
        var customerService = Substitute.For<ICustomerService>();
        var customers = new List<Customer> { new() { Id = Guid.NewGuid(), Name = "Ana" } };
        customerService.GetAllAsync(Arg.Any<CancellationToken>()).Returns(customers);

        var controller = new CustomersController(customerService);

        var result = await controller.GetAll(CancellationToken.None);

        result.Result.ShouldBeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.ShouldBe(customers);
    }

    [Fact]
    public async Task Post_WhenRequestIsNull_ReturnsBadRequest()
    {
        var customerService = Substitute.For<ICustomerService>();
        var controller = new CustomersController(customerService);

        var result = await controller.Post(null!, CancellationToken.None);

        result.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Post_WhenServiceReturnsFailure_ReturnsBadRequestWithProblemDetails()
    {
        var customerService = Substitute.For<ICustomerService>();
        customerService.CreateAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>())
            .Returns(Result<Customer>.Failure("validation_error", "Name is required"));

        var controller = new CustomersController(customerService);

        var result = await controller.Post(new CustomersController.CustomerRequest("", "ana@email.com", "123"), CancellationToken.None);

        result.Result.ShouldBeOfType<BadRequestObjectResult>();
        var badRequest = result.Result as BadRequestObjectResult;
        badRequest!.Value.ShouldBeOfType<ProblemDetails>();
        var problemDetails = badRequest.Value as ProblemDetails;
        problemDetails!.Detail.ShouldBe("Name is required");
        problemDetails.Status.ShouldBe(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Post_WhenServiceSucceeds_ReturnsCreatedAtAction()
    {
        var customerService = Substitute.For<ICustomerService>();
        var createdCustomer = new Customer { Id = Guid.NewGuid(), Name = "Ana", Email = "ana@email.com", Document = "123" };
        customerService.CreateAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>())
            .Returns(Result<Customer>.Success(createdCustomer));

        var controller = new CustomersController(customerService);

        var result = await controller.Post(new CustomersController.CustomerRequest("Ana", "ana@email.com", "123"), CancellationToken.None);

        result.Result.ShouldBeOfType<CreatedAtActionResult>();
        var createdAtResult = result.Result as CreatedAtActionResult;
        createdAtResult!.Value.ShouldBe(createdCustomer);
    }
}
