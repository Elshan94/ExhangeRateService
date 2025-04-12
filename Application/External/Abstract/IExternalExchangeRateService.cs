using BambooExhangeRateService.Application.External.Models;
using BambooExhangeRateService.Application.External.Models;
using System.Threading.Tasks;

namespace BambooExhangeRateService.Application.External.Abstract
{
    public interface IExternalExchangeRateService
    {
        Task<ExternalExchangeRateResponseModel> GetExchangeRateAsync(string currency);
        Task<ExternalExchangeRateResponseModel> GetExhangeRateBySymbolFilter(string from, List<string> to);
        Task<HistoricalRatesResponseModel> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate, int page, int pageSize);
    }
}
