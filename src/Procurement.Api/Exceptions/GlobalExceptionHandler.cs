using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Inventory.Exceptions;

namespace Procurement.Api.Exceptions;
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is ValidationException validationException)
        {
            await HandleValidationExceptionAsync(httpContext, validationException, cancellationToken);
            return true;
        }

        if (exception is InsufficientStockException)
        {
            await HandleInsufficientStockExceptionAsync(httpContext, cancellationToken);
            return true;
        }

        if (exception is DbUpdateConcurrencyException)
        {
            await HandleConcurrencyExceptionAsync(httpContext, cancellationToken);
            return true;
        }

        _logger.LogError(exception, "An unhandled exception occurred while processing the request.");
        await HandleDefaultExceptionAsync(httpContext, exception, cancellationToken);
        
        return true;
    }

    private static async Task HandleValidationExceptionAsync(
        HttpContext httpContext, 
        ValidationException exception, 
        CancellationToken cancellationToken)
    {
        var errors = exception.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                g => g.Key, 
                g => g.Select(e => e.ErrorMessage).ToArray());

        var problemDetails = new ValidationProblemDetails(errors)
        {
            Type = "https://httpstatuses.com/400",
            Title = "Validation failed.",
            Status = StatusCodes.Status400BadRequest,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }

    private static async Task HandleInsufficientStockExceptionAsync(
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Type = "https://httpstatuses.com/409",
            Title = "Conflict.",
            Status = StatusCodes.Status409Conflict,
            Detail = "Insufficient stock for the requested quantity.",
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }

    private static async Task HandleConcurrencyExceptionAsync(
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Type = "https://httpstatuses.com/409",
            Title = "Conflict.",
            Status = StatusCodes.Status409Conflict,
            Detail = "The inventory record was modified by another user. Please reload and try again.",
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }

    private static async Task HandleDefaultExceptionAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Type = $"https://httpstatuses.com/{statusCode}",
            Title = GetTitle(statusCode),
            Status = statusCode,
            Detail = GetDetail(statusCode),
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }

    private static string GetTitle(int statusCode) => statusCode switch
    {
        StatusCodes.Status401Unauthorized => "Unauthorized.",
        StatusCodes.Status404NotFound => "Resource not found.",
        _ => "An unexpected error occurred."
    };

    private static string GetDetail(int statusCode) => statusCode switch
    {
        StatusCodes.Status401Unauthorized => "You are not authorized to perform this operation.",
        StatusCodes.Status404NotFound => "The requested resource was not found.",
        _ => "An unexpected error occurred while processing your request."
    };
}