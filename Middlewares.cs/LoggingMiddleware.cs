using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Threading.Tasks;

namespace UserManagementApi.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log HTTP method and request path
            var method = context.Request.Method;
            var path = context.Request.Path;

            // Use Stopwatch to measure request processing time (optional)
            var stopwatch = Stopwatch.StartNew();

            // Pass the request to the next middleware
            await _next(context);

            stopwatch.Stop();

            // Log response status code
            var statusCode = context.Response.StatusCode;

            // Output log
            Console.WriteLine($"[{method}] {path} responded with {statusCode} in {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}