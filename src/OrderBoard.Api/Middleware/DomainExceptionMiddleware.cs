using System.Net;
using Microsoft.AspNetCore.Mvc;
using OrderBoard.Core.Exceptions;

namespace OrderBoard.Api.Middleware;

public sealed class DomainExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (DomainException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Title = "Domain validation error",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = ex.Message
            };

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}