using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace LibraryManagement.API.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title, detail) = exception switch
        {
            NotFoundException or KeyNotFoundException =>
                (404, "Not Found", exception.Message),

            BusinessException or ConflictException
                or InvalidOperationException =>
                (409, "Business Conflict", exception.Message),

            ArgumentException =>
                (400, "Bad Request", exception.Message),

            UnauthorizedAccessException =>
                (401, "Unauthorized", "Invalid credentials."),

            DbUpdateConcurrencyException =>
                (409, "Concurrency Conflict",
                    "Data has changed. Please reload and retry."),

            DbUpdateException
            {
                InnerException: PostgresException
                {
                    SqlState: PostgresErrorCodes.UniqueViolation
                }
            } =>
                (409, "Duplicate Data",
                    "A record with the same unique value already exists."),

            PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            } =>
                (409, "Duplicate Data",
                    "A record with the same unique value already exists."),

            DbUpdateException
            {
                InnerException: PostgresException
                {
                    SqlState: PostgresErrorCodes.ForeignKeyViolation
                }
            } =>
                (409, "Reference Conflict",
                    "This operation violates a data relationship."),

            _ =>
                (500, "Internal Server Error",
                    "An unexpected error occurred.")
        };

        if (status >= 500)
        {
            _logger.LogError(
                exception,
                "Unhandled error. TraceId: {TraceId}",
                context.TraceIdentifier);
        }
        else
        {
            _logger.LogWarning(
                exception,
                "Request failed with status {StatusCode}",
                status);
        }

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        problem.Extensions["traceId"] =
            context.TraceIdentifier;

        context.Response.StatusCode = status;

        await context.Response.WriteAsJsonAsync(
            problem,
            cancellationToken);

        return true;
    }
}
