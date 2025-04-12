using BambooExhangeRateService.Application.Models;
using ExchangeRateService.Application.Models;

namespace BambooExhangeRateService.Application.Services.Abstract
{
    public interface ICacheService
    {
        Task<ExchangeRateResponseModel> GetAsync(string baseCurrency);
        Task SetAsync(string baseCurrency, ExchangeRateResponseModel data);
    }
}
