using BambooExchangeRateService.Application.Services.External.Concrete;
using BambooExhangeRateService.Application.External.Abstract;
using BambooExhangeRateService.Application.Services.Abstract;
namespace BambooExhangeRateService.Application.Services.Concrete
{
    public class ExchangeRateProviderFactory : IExchangeRateProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, Type> _providers = new();

        public ExchangeRateProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _providers["frankfurter"] = typeof(FrankfurterExchangeRateService);
        }

        public IExternalExchangeRateService GetProvider(string providerKey)
        {
            if (!_providers.TryGetValue(providerKey.ToLower(), out var type))
                throw new ArgumentException($"Provider '{providerKey}' is not supported.");

            return (IExternalExchangeRateService)_serviceProvider.GetRequiredService(type);
        }

        
    }
}
