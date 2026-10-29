using Domain.Exceptions;

namespace Presentation.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            int statusCode = ex switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                ValidationException => StatusCodes.Status400BadRequest,
                InvalidCredentialsException => StatusCodes.Status401Unauthorized,
                ConflictException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            string mensaje = statusCode == StatusCodes.Status500InternalServerError
                ? "Ocurrió un error inesperado"
                : ex.Message;

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(new { error = mensaje });
        }
    }
}