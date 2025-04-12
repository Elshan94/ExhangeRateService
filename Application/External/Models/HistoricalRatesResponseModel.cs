using System.Text.Json.Serialization;

namespace BambooExhangeRateService.Application.External.Models
{
    public class HistoricalRatesResponseModel
    {
        public List<ExternalExchangeRateResponseModel> Data { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class HistoricalExternalExchangeRateResponseModel
    {
        public string Base { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; }
    }
}
