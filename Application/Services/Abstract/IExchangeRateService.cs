using BambooExchangeRateService.Helpers;
using BambooExchangeRateService.Application;
using BambooExchangeRateService;
using BambooExchangeRateService.Application.Models;
using BambooExhangeRateService.Helpers;
using ExchangeRateService.Application.Models;
using BambooExhangeRateService.Application.Models;

namespace BambooExchangeRateService.Application.Services.Abstract
{
    public interface IExchangeRateService
    {
        Task<GenericResponseModel<List<string>>> GetHistoricalRatesAsync(HistoricalRatesRequestModel model, string providerKey);
        Task<GenericResponseModel<ExchangeRateResponseModel>> GetExchangeRate(string currency, string providerKey);
        Task<GenericResponseModel<List<string>>> ConvertAsync(ConvertRequestModel model, string providerKey);
    }
}
