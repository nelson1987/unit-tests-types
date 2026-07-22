using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Shouldly;
using UnitTestsTypes.Api.Extensions;

namespace UnitTestsTypes.UnitTests;

public class ExceptionHandlingExtensionsTests
{
    [Fact]
    public async Task UseApiExceptionHandler_ReturnsProblemDetailsForUnhandledException()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();

        var app = builder.Build();
        app.UseApiExceptionHandler();
        app.MapGet("/boom", (HttpContext context) => throw new InvalidOperationException("boom"));

        await app.StartAsync();
        try
        {
            var client = app.GetTestClient();
            var response = await client.GetAsync("/boom");

            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.InternalServerError);
            response.Content.Headers.ContentType!.MediaType.ShouldBe("application/problem+json");
            var content = await response.Content.ReadAsStringAsync();
            content.ShouldContain("boom");
            content.ShouldContain("An unexpected error occurred");
        }
        finally
        {
            await app.StopAsync();
        }
    }
}
