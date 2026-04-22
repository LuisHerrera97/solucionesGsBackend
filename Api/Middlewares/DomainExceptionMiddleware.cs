using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FinancieraSoluciones.Application.DTOs.Shared;
using FinancieraSoluciones.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace FinancieraSoluciones.Api.Middlewares
{
    public class DomainExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public DomainExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                await WriteResponseAsync(context, 400, string.Join(" | ", ex.Errors.Select(e => e.ErrorMessage)));
            }
            catch (DomainException ex)
            {
                await WriteResponseAsync(context, ex.HttpCode, ex.Message);
            }
        }

        private static async Task WriteResponseAsync(HttpContext context, int httpCode, string message)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(message, httpCode));
        }
    }
}
