using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Middleware;

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            InvalidOperationException => (StatusCodes.Status409Conflict, "Conflict"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        if (statusCode >= StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled exception while processing {Path}", httpContext.Request.Path);
        else
            logger.LogWarning(exception, "Handled API exception for {Path}", httpContext.Request.Path);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = statusCode >= StatusCodes.Status500InternalServerError
                ? "Une erreur interne est survenue."
                : exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
