using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Home.Core.Logging {

    public class RequestLogger {

        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLogger> _logger;

        public RequestLogger(RequestDelegate next, ILogger<RequestLogger> logger) {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context) {
            var req = context.Request;
            _logger.LogInformation($"Incoming request {req.Method} {req.Path}");
            await _next(context);
            _logger.LogInformation($"Outgoing response for {req.Method} {req.Path} with status code {context.Response.StatusCode}"); 
        }
        
    }

    public static class RequestLoggerExtensions {

        public static IApplicationBuilder UseRequestLogger(this IApplicationBuilder builder) {
            return builder.UseMiddleware<RequestLogger>();
        }

    }

}
