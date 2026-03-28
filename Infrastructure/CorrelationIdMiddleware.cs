using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace AngularWithNET.Infrastructure
{
    public class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-Id";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next) { _next = next; }

        public async Task Invoke(HttpContext context)
        {
            var correlationId = context.Request.Headers[HeaderName].ToString();
            if (string.IsNullOrEmpty(correlationId))
                correlationId = Guid.NewGuid().ToString("N");

            context.Items["CorrelationId"] = correlationId;
            context.Response.Headers[HeaderName] = correlationId;

            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
            var path = context.Request.Path.Value;

            using (LogContext.PushProperty("CorrelationId", correlationId))
            using (LogContext.PushProperty("UserId", userId))
            using (LogContext.PushProperty("Path", path))
            {
                await _next(context);
            }
        }
    }
}
