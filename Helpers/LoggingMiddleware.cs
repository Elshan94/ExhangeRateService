using System.Diagnostics;
using System.Security.Claims;

namespace BambooExhangeRateService.Helpers
{
    public class LoggingMiddleware
    {
        private RequestDelegate Next { get; }
        private ILogger<LoggingMiddleware> Logger { get; }

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            Next = next;
            Logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            var clientIp = context.Connection.RemoteIpAddress?.ToString();

            var clientId = context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Anonymous";

            Logger.LogInformation("Request received: Client IP = {ClientIp}, ClientId = {ClientId}, Method = {HttpMethod}, Endpoint = {Endpoint}",
                clientIp, clientId, context.Request.Method, context.Request.Path);

            await Next(context);

            stopwatch.Stop();
            Logger.LogInformation("Response sent: Client IP = {ClientIp}, ClientId = {ClientId}, Method = {HttpMethod}, Endpoint = {Endpoint}, StatusCode = {StatusCode}, ResponseTime = {ResponseTime}ms",
                clientIp, clientId, context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
        }
    }
}
