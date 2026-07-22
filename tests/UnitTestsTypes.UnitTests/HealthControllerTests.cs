using Microsoft.AspNetCore.Mvc;
using Shouldly;
using UnitTestsTypes.Api.Controllers;

namespace UnitTestsTypes.UnitTests;

public class HealthControllerTests
{
    [Fact]
    public void Get_ReturnsOkWithHealthyStatus()
    {
        var controller = new HealthController();

        var result = controller.Get();

        result.ShouldBeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.ShouldNotBeNull();
        okResult.Value!.ToString()!.ShouldContain("status");
    }
}
