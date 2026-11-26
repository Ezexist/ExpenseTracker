
using System.Net;
using System.Text.Json;
using ExpenseTracker.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            await WriteProblemAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Bad Request",
                ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await WriteProblemAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Invalid Operation",
                ex.Message);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Database update conflict.");

            await WriteProblemAsync(
                context,
                StatusCodes.Status409Conflict,
                "Conflict",
                "The request conflicts with existing data.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");

            await WriteProblemAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteProblemAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        await JsonSerializer.SerializeAsync(
            context.Response.Body,
            problem,
            cancellationToken: context.RequestAborted);
    }
}
