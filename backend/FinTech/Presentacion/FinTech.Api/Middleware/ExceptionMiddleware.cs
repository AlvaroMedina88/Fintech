using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FinTech.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "Recurso no encontrado.");
            await WriteProblem(context, HttpStatusCode.NotFound, "Not Found", ex.Message);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Argumento inválido.");
            await WriteProblem(context, HttpStatusCode.BadRequest, "Bad Request", ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Operación inválida.");
            await WriteProblem(context, HttpStatusCode.UnprocessableEntity, "Unprocessable Entity", ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado.");
            await WriteProblem(context, HttpStatusCode.InternalServerError, "Internal Server Error", "Ocurrió un error inesperado.");
        }
    }

    private static async Task WriteProblem(HttpContext context, HttpStatusCode statusCode, string title, string detail)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";
        var problem = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };
        await context.Response.WriteAsJsonAsync(problem);
    }
}
