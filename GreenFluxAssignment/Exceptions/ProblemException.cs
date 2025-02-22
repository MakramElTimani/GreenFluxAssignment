using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
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

    public ProblemException(Exception exception) : base(exception.Message)
    {
        (int statusCode, string message) = exception switch
        {
            ArgumentException => ((int)HttpStatusCode.BadRequest, exception.Message),
            InvalidOperationException => ((int)HttpStatusCode.BadRequest, exception.Message),
            DbUpdateException => ((int)HttpStatusCode.UnprocessableEntity, "An error occurred while updating the database!"),
            DbException => ((int)HttpStatusCode.UnprocessableEntity, "An error occurred while accessing the database!"),
            _ => ((int)HttpStatusCode.InternalServerError, "An unexpected error occurred!")
        };
        Error = message;
        StatusCode = statusCode;
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
            problemException = new ProblemException(exception);
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
