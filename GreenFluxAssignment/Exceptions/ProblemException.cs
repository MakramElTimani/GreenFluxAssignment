using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GreenFluxAssignment.Exceptions;

[Serializable]
public class ProblemException : Exception
{
    public string Error { get; set; }

    public int StatusCode { get; init; }

    public ProblemException(HttpStatusCode statusCode, string error, string message) : base(message)
    {
        Error = error;
        StatusCode = (int)statusCode;
    }
}

public class ProblemExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public ProblemExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ProblemException problemException)
        {
            return true;
        }

        var problemDetails = new ProblemDetails
        {
            Title = problemException.Error,
            Detail = exception.Message,
            Status = problemException.StatusCode,
            Instance = httpContext.TraceIdentifier
        };
        httpContext.Response.StatusCode = problemException.StatusCode;
        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
            });
    }
}
