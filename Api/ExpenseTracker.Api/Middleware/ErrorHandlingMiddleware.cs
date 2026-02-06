using System;
using System.Net;
using System.Text.Json;
using ExpenseTracker.Api.Models;
using ExpenseTracker.Api.Errors;

namespace ExpenseTracker.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next=next;
    }

    public async Task Invoke(HttpContext context)
    {
        try // jalan normal jika tdk ada masalah
        {
            await _next(context);
        }
        catch (ApiException ex) // handle api exception
        {
            await WriteError(context, ex.StatusCode, ex.ErrorType, ex.Message);
        }
        catch(Exception) // handle exception lainya jika ada
        {
            await WriteError(
                context, (int)HttpStatusCode.InternalServerError,
                "ServerError", "Something went wrong"
            );
        }
    }
    private static Task WriteError(HttpContext context, int StatusCode, string type, string message)
    {
        context.Response.StatusCode = StatusCode;
        context.Response.ContentType= "application/json";

        var response = new ErrorResponse
        {
            Type = type,
            Message = message
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
    private static Task HandleException(HttpContext context)
    {
        var response = new ErrorResponse
        {
            Type = "ServerError",
            Message = "Something went wrong"
        };

        context.Response.StatusCode= (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType="application/json";

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
