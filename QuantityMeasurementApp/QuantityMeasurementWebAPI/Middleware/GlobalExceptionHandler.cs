using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Npgsql;
using QuantityMeasurementBusinessLayer;
using QuantityMeasurementModel.Dto;

namespace QuantityMeasurementWebAPI.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _env    = env;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception   exception,
            CancellationToken cancellationToken)
        {
            string fullMessage = BuildFullMessage(exception);

            int status;
            if (FindPostgresException(exception) is { } pgEx)
            {
                status = 503;
                fullMessage =
                    $"{pgEx.Message} (PostgreSQL error {pgEx.SqlState}). " +
                    "Ensure PostgreSQL is running and ConnectionStrings:QuantityMeasurementDb matches your instance. " +
                    "Apply schema: dotnet ef database update --project QuantityMeasurementRepository --startup-project QuantityMeasurementWebAPI";
            }
            else
            {
                (status, fullMessage) = exception switch
                {
                    UnauthorizedAccessException  => (401, exception.Message),
                    InvalidOperationException    => (409, exception.Message),
                    ArgumentException            => (400, exception.Message),
                    KeyNotFoundException         => (404, exception.Message),
                    QuantityMeasurementException => (400, exception.Message),
                    _                            => (500, fullMessage)
                };
            }

            _logger.LogError(exception,
                "[GlobalExceptionHandler] {Status} | {ExType} | {FullMsg} | {Method} {Path}",
                status,
                exception.GetType().FullName,
                fullMessage,
                httpContext.Request.Method,
                httpContext.Request.Path);

            httpContext.Response.StatusCode  = status;
            httpContext.Response.ContentType = "application/json";

            var body = new ErrorResponseDto
            {
                Timestamp = DateTime.UtcNow,
                Status    = status,
                Error = status switch
                {
                    400 => "Bad Request",
                    401 => "Unauthorized",
                    403 => "Forbidden",
                    404 => "Not Found",
                    409 => "Conflict",
                    503 => "Service Unavailable",
                    _   => "Internal Server Error"
                },
                Message = fullMessage,
                Path    = httpContext.Request.Path
            };

            await httpContext.Response.WriteAsync(
                JsonSerializer.Serialize(body,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }),
                cancellationToken);

            return true;
        }

        private static string BuildFullMessage(Exception ex)
        {
            var parts = new List<string>();
            var current = ex;
            int depth = 0;
            while (current != null && depth < 5)
            {
                parts.Add($"[{current.GetType().Name}] {current.Message}");
                current = current.InnerException;
                depth++;
            }
            return string.Join(" → ", parts);
        }

        private static PostgresException? FindPostgresException(Exception? ex)
        {
            while (ex != null)
            {
                if (ex is PostgresException pg) return pg;
                ex = ex.InnerException;
            }
            return null;
        }
    }
}