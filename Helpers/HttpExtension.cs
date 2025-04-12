using Polly;
using Serilog;

namespace BambooExhangeRateService.Helpers
{
    public static class HttpClientExtensions
    {
        public static IServiceCollection AddFrankfurterHttpClient(this IServiceCollection services, string address)
        {
            services.AddHttpClient("FrankfurterClient", client =>
            {
                client.BaseAddress = new Uri(address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
                .AddTransientHttpErrorPolicy(policy =>
                    policy.WaitAndRetryAsync(
                        retryCount: 3,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        onRetry: (outcome, timespan, retryAttempt, context) =>
                        {
                            Log.Warning($"Retry {retryAttempt} after {timespan.TotalSeconds}s due to: {outcome.Exception?.Message}");
                        }))
                .AddPolicyHandler(Policy<HttpResponseMessage>
                    .Handle<HttpRequestException>()
                    .OrResult(msg => !msg.IsSuccessStatusCode)
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: 3,
                        durationOfBreak: TimeSpan.FromSeconds(30),
                        onBreak: (result, timespan) =>
                        {
                            Log.Warning($"Circuit broken for {timespan.TotalSeconds}s due to: {result.Exception?.Message ?? result.Result?.StatusCode.ToString()}");
                        },
                        onReset: () => Log.Information("Circuit reset."),
                        onHalfOpen: () => Log.Information("Circuit half-open, next call is a trial.")
                    ));

            return services;
        }
    }
}
