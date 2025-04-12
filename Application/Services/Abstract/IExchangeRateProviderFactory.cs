using BambooExhangeRateService.Application.External.Abstract;

namespace BambooExhangeRateService.Application.Services.Abstract
{
    public interface IExchangeRateProviderFactory
    {
        IExternalExchangeRateService GetProvider(string providerKey);
    }
}
