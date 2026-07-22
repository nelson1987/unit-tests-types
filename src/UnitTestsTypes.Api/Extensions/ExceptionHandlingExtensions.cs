using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace UnitTestsTypes.Api.Extensions;

public static class ExceptionHandlingExtensions
{
    public static IApplicationBuilder UseApiExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appBuilder =>
        {
            appBuilder.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/problem+json";

                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                var ex = exceptionHandlerFeature?.Error;

                var problem = new ProblemDetails
                {
                    Title = "An unexpected error occurred",
                    Detail = ex?.Message ?? "No additional details were provided.",
                    Status = StatusCodes.Status500InternalServerError
                };

                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
            });
        });

        return app;
    }
}
