using BambooExhangeRateService.Application.External.Abstract;
using BambooExhangeRateService.Application.External.Models;
using Serilog;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace BambooExchangeRateService.Application.Services.External.Concrete
{
    public class FrankfurterExchangeRateService : IExternalExchangeRateService
    {
        private FrankfurterExchangeRateConfig FrankfurterExchangeRateConfig { get; }
        private IHttpClientFactory HttpClientFactory { get; }
        private ILogger<FrankfurterExchangeRateService> Logger { get; }

        public FrankfurterExchangeRateService(IHttpClientFactory hTttpClientFactory, FrankfurterExchangeRateConfig frankfurterExchangeRateConfig, ILogger<FrankfurterExchangeRateService> logger)
        {
            HttpClientFactory = hTttpClientFactory;
            FrankfurterExchangeRateConfig = frankfurterExchangeRateConfig;
            Logger = logger;
        }

        private string GenerateCorrelationKey()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task<ExternalExchangeRateResponseModel> GetExchangeRateAsync(string currency)
        {
            ExternalExchangeRateResponseModel result = default;
            var CorrelationKey = GenerateCorrelationKey();

            try
            {
                Logger.LogInformation("CorrelationKey: {CorrelationKey} - Fetching exchange rates for currency: {Currency}", CorrelationKey, currency);

                var client = HttpClientFactory.CreateClient("FrankfurterClient");
                string requestUrl = $"{FrankfurterExchangeRateConfig.BaseAddress}latest?base={currency}";

                Logger.LogInformation("CorrelationKey: {CorrelationKey} - Requesting URL: {RequestUrl}", CorrelationKey, requestUrl);

                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Add("X-Correlation-ID", CorrelationKey);

                var response = await client.SendAsync(request);

                Logger.LogInformation("CorrelationKey: {CorrelationKey} - Received response with status code: {StatusCode}", CorrelationKey, response.StatusCode);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                result = JsonSerializer.Deserialize<ExternalExchangeRateResponseModel>(responseContent);

                Logger.LogInformation("CorrelationKey: {CorrelationKey} - Successfully retrieved exchange rates for {Currency}", CorrelationKey, currency);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CorrelationKey: {CorrelationKey} - Exception occurred while retrieving rates for currency {Currency} from central bank.", CorrelationKey, currency);
                throw;
            }

            return result;
        }

        public async Task<ExternalExchangeRateResponseModel> GetExhangeRateBySymbolFilter(string from, List<string> to)
        {
            ExternalExchangeRateResponseModel result = default;
            var CorrelationKey = GenerateCorrelationKey();

            try
            {
                Logger.LogInformation("CorrelationKey: {CorrelationKey} - Fetching exchange rates for {FromCurrency} to {ToCurrencies}", CorrelationKey, from, string.Join(",", to));

                var client = HttpClientFactory.CreateClient("FrankfurterClient");
                string toSymbols = string.Join(",", to);
                var requestUrl = $"{FrankfurterExchangeRateConfig.BaseAddress}latest?base={from}&symbols={toSymbols}";

                Logger.LogInformation("CorrelationKey: {CorrelationKey} - Requesting URL: {RequestUrl}", CorrelationKey, requestUrl);

                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Add("X-Correlation-ID", CorrelationKey); // Add the correlation ID to the request header

                var response = await client.SendAsync(request);

                Logger.LogInformation("CorrelationKey: {CorrelationKey} - Received response with status code: {StatusCode}", CorrelationKey, response.StatusCode);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                result = JsonSerializer.Deserialize<ExternalExchangeRateResponseModel>(responseContent);

                Logger.LogInformation("CorrelationKey: {CorrelationKey} - Successfully retrieved exchange rates for {FromCurrency} to {ToCurrencies}", CorrelationKey, from, string.Join(",", to));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CorrelationKey: {CorrelationKey} - Exception occurred while retrieving rates for {FromCurrency} to {ToCurrencies} from central bank.", CorrelationKey, from, string.Join(",", to));
                throw;
            }

            return result;
        }

        public async Task<HistoricalRatesResponseModel> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            HistoricalRatesResponseModel result = default;
            var CorrelationKey = GenerateCorrelationKey();

            try
            {
                Logger.LogInformation("CorrelationKey: {CorrelationKey} - Fetching historical exchange rates for {BaseCurrency} from {StartDate} to {EndDate}, page {Page} with page size {PageSize}",
                    CorrelationKey, baseCurrency, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), page, pageSize);

                var client = HttpClientFactory.CreateClient("FrankfurterClient");
                var requestUrl = $"{FrankfurterExchangeRateConfig.BaseAddress}{startDate:yyyy-MM-dd}..{endDate:yyyy-MM-dd}?base={baseCurrency}";

                Logger.LogInformation("CorrelationKey: {CorrelationKey} - Requesting URL: {RequestUrl}", CorrelationKey, requestUrl);

                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Add("X-Correlation-ID", CorrelationKey); // Add the correlation ID to the request header

                var response = await client.SendAsync(request);

                Logger.LogInformation("CorrelationKey: {CorrelationKey} - Received response with status code: {StatusCode}", CorrelationKey, response.StatusCode);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var external = JsonSerializer.Deserialize<HistoricalExternalExchangeRateResponseModel>(responseContent);

                if (external?.Rates == null || !external.Rates.Any())
                    throw new Exception("No exchange rate data found.");

                var allRates = external.Rates
                    .Select(r => new ExternalExchangeRateResponseModel
                    {
                        Date = r.Key,
                        Base = baseCurrency,
                        Rates = r.Value
                    })
                    .OrderBy(r => r.Date)
                    .ToList();

                var pagedRates = allRates
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                Logger.LogInformation("CorrelationKey: {CorrelationKey} - Successfully retrieved historical exchange rates for {BaseCurrency} from {StartDate} to {EndDate}, total rates: {TotalRates}",
                    CorrelationKey, baseCurrency, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), external.Rates.Count);

                return new HistoricalRatesResponseModel
                {
                    Data = pagedRates,
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = external.Rates.Count,
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CorrelationKey: {CorrelationKey} - Exception occurred while retrieving historical rates for {BaseCurrency} from {StartDate} to {EndDate} from central bank.",
                    CorrelationKey, baseCurrency, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
                throw;
            }

            return result;
        }
    }




}
