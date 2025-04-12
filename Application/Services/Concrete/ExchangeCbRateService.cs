using AutoMapper;
using BambooExchangeRateService.Application.Models;
using BambooExchangeRateService.Application.Services.Abstract;
using BambooExchangeRateService.Helpers;
using BambooExhangeRateService.Application.Models;
using BambooExhangeRateService.Application.Services.Abstract;
using BambooExhangeRateService.Helpers;
using ExchangeRateService.Application.Models;
using Newtonsoft.Json;
using Serilog;
using System.Net.Http;
using System.Text.Json;

namespace BambooExchangeRateService.Application.Services.Concrete
{
    public class ExchangeCbRateService : IExchangeRateService
    {
        private IExchangeRateProviderFactory ProviderFactory { get; }
        private ICacheService CacheService { get; }
        private ForbiddenCurrencies ForbiddenCurrencies { get; }

        public ExchangeCbRateService(IExchangeRateProviderFactory providerFactory, ICacheService cacheService, ForbiddenCurrencies currencies)
        {
            ProviderFactory = providerFactory;
            CacheService = cacheService;
            ForbiddenCurrencies = currencies;
        }

        public async Task<GenericResponseModel<ExchangeRateResponseModel>> GetExchangeRate(string currency, string providerKey)
        {
            GenericResponseModel<ExchangeRateResponseModel> responseModel = new GenericResponseModel<ExchangeRateResponseModel>();

            try
            {
                if (ForbiddenCurrencies.List.Contains(currency))
                {
                    responseModel.Error($"Conversion not allowed for this currency: {currency}", 400);
                    return responseModel;
                }

                var cached = await CacheService.GetAsync($"{currency}{providerKey}");
                if (cached != null)
                {
                    responseModel.Success(cached);
                    return responseModel;
                }

                var provider = ProviderFactory.GetProvider(providerKey);

                var response = await provider.GetExchangeRateAsync(currency);

                ExchangeRateResponseModel rateResponseModel = new ExchangeRateResponseModel();

                rateResponseModel.Rates = response.Rates;
                rateResponseModel.Date = response.Date;
                rateResponseModel.Base = response.Base;

                await CacheService.SetAsync(currency, rateResponseModel);

                responseModel.Success(rateResponseModel);
            }
            catch (Exception ex)
            {
                responseModel.InternalError();
            }


            return responseModel;
        }

        public async Task<GenericResponseModel<List<string>>> ConvertAsync(ConvertRequestModel model, string providerKey)
        {
            GenericResponseModel<List<string>> responseModel = new GenericResponseModel<List<string>>();

            try
            {
                if (ForbiddenCurrencies.List.Contains(model.BaseCurrency))
                {
                    responseModel.Error($"Conversion not allowed for this currency: {model.BaseCurrency}", 400);
                    return responseModel;
                }

                var ifExist = ForbiddenCurrencies.List.Any(x => model.ToCurrencies.Contains(x));

                if (ifExist)
                {
                    responseModel.Error($"Conversion not allowed for this currency: {model.BaseCurrency}", 400);
                    return responseModel;
                }

                List<string> list = new List<string>();

                var cached = await CacheService.GetAsync($"{model.BaseCurrency}{providerKey}");
                if (cached != null)
                {
                    foreach (var kvp in cached.Rates)
                    {
                        if (model.ToCurrencies.Contains(kvp.Key))
                        {
                            var converted = Convert(model.Amount, kvp.Value);
                            list.Add($"{model.Amount} {model.BaseCurrency} = {converted} {kvp.Key}");
                        }
                    }

                    responseModel.Success(list);
                    return responseModel;
                }

                var provider = ProviderFactory.GetProvider(providerKey);

                var response = await provider.GetExhangeRateBySymbolFilter(model.BaseCurrency, model.ToCurrencies);

                foreach (var kvp in response.Rates)
                {
                    var converted = Convert(model.Amount, kvp.Value);
                    list.Add($"{model.Amount} {model.BaseCurrency} = {converted} {kvp.Key}");
                }

                responseModel.Success(list);
            }
            catch (Exception ex)
            {
                responseModel.InternalError();
            }

            return responseModel;
        }

        public async Task<GenericResponseModel<List<string>>> GetHistoricalRatesAsync(HistoricalRatesRequestModel model , string providerKey)
        {
            GenericResponseModel<List<string>> responseModel = new GenericResponseModel<List<string>>();

            try
            {
                if (ForbiddenCurrencies.List.Contains(model.BaseCurrency))
                {
                    responseModel.Error($"Conversion not allowed for this currency: {model.BaseCurrency}", 400);
                    return responseModel;
                }

                List<string> list = new List<string>();

                var cached = await CacheService.GetAsync($"{model.BaseCurrency}{providerKey}");
                if (cached != null)
                {
                    responseModel.Success(list);
                    return responseModel;
                }

                var provider = ProviderFactory.GetProvider(providerKey);

                var response = await provider.GetHistoricalRatesAsync(model.BaseCurrency, model.StartDate, model.EndDate, model.Page, model.PageSize);

                responseModel.Success(list);
            }
            catch (Exception ex)
            {
                responseModel.InternalError();
            }

            return responseModel;
        }

        private decimal Convert(decimal amount, decimal rate) => Math.Round(amount * rate, 2);

    }
}
