using System.Text.Json.Serialization;

namespace BambooExhangeRateService.Application.External.Models
{
    public class ExternalExchangeRateResponseModel
    {
        [JsonPropertyName("base")]
        public string Base { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("rates")]
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
