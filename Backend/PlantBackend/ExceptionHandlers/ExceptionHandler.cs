using Microsoft.AspNetCore.Diagnostics;
using PlantApp.Domain.Utils.Exceptions;

namespace PlantBackend.ExceptionHandlers;

public class ExceptionHandler(ILogger<ExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        int statusCode;
        string message;

        if (exception is AppException ex)
        {
            statusCode = ex switch
            {
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                NotFoundException => StatusCodes.Status404NotFound,
                InvalidOperationAppException => StatusCodes.Status400BadRequest,
                DtoIdMismatchException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status400BadRequest
            };
            message = ex.UserMessage;
        }
        else if (exception is ArgumentNullException argEx)
        {
            statusCode = StatusCodes.Status400BadRequest;
            message = "A required value was null: " + argEx.ParamName;
            logger.LogError(argEx, "A required value was null");
        }
        else if (exception is UnauthorizedAccessException accEx)
        {
            statusCode = StatusCodes.Status401Unauthorized;
            message = "Access denied";
            logger.LogWarning(accEx, "Unauthorized access attempt: {Message}", accEx.Message);
        }
        else
        {
            return false;
        }

        var response = new ErrorResponse(message, statusCode);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
