using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using Xunit.Sdk;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

    public class ExtendedProblemDetails : ProblemDetails
    {
        public bool Success { get; set; } = false;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            context.Response.StatusCode = ex.StatusCode;
            await context.Response.WriteAsJsonAsync(new ExtendedProblemDetails
            {
                Title = "Lỗi nghiệp vụ",
                Success = false,
                Detail = ex.Message

            });
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new ExtendedProblemDetails
            {
                Title = "Lỗi hệ thống",
                Success = false,
                Detail = ex.Message

            });
        }
    }
}
