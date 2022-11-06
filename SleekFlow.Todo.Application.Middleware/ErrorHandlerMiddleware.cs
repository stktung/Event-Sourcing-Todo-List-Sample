using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SleekFlow.Todo.Domain.Common;

namespace SleekFlow.Todo.Application.Middleware;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            switch (error)
            {
                case DomainException:
                case AggregateWrongExpectedVersionException:
                case ProjectionException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case KeyNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                default:
                    // unhandled error
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            await response.WriteAsync(
                JsonSerializer.Serialize(new ErrorResponse(error.GetType().Name, error.Message, error.ToString())));
        }
    }
}