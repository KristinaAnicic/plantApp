using Microsoft.AspNetCore.Diagnostics;

namespace PlantBackend.ExceptionHandlers;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An unexpected error occurred.");
        var response = new ErrorResponse()
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Error = "An unexpected error occurred. Please try again later."
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
