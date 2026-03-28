using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Text.Json;

namespace AngularWithNET.Infrastructure
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionHandler(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var correlationId = context.Items["CorrelationId"]?.ToString() ?? context.TraceIdentifier;

                Log.Error(ex, "Unhandled exception for {Method} {Path} [CorrelationId: {CorrelationId}]",
                    context.Request.Method, context.Request.Path, correlationId);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "An unexpected error occurred.",
                    Status = 500,
                };
                problem.Extensions["correlationId"] = correlationId;

                if (_env.IsDevelopment())
                    problem.Detail = ex.Message;

                await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
            }
        }
    }
}
