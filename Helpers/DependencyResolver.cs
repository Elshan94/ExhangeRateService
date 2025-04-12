using BambooExchangeRateService.Application.Services.Abstract;
using BambooExchangeRateService.Application.Services.Concrete;
using BambooExchangeRateService.Application.Services.External.Concrete;
using BambooExhangeRateService.Application.External.Abstract;
using BambooExhangeRateService.Application.Services.Abstract;
using BambooExhangeRateService.Application.Services.Concrete;


namespace BambooExchangeRateService.Helpers
{
    public static class DependencyResolver
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IExternalExchangeRateService), typeof(FrankfurterExchangeRateService));
            services.AddScoped<FrankfurterExchangeRateService>();
            services.AddScoped(typeof(IExchangeRateProviderFactory), typeof(ExchangeRateProviderFactory));
            services.AddScoped(typeof(IExchangeRateService), typeof(ExchangeCbRateService));
            services.AddScoped(typeof(ICacheService), typeof(RedisService));
            services.AddScoped(typeof(ITokenService), typeof(JwtTokenService));
            return services;
        }
    }
}
