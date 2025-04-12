namespace ExchangeRateService.Application.Models
{
    public class ExchangeRateResponseModel
    {
        public string Base { get; set; }
        public string Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
