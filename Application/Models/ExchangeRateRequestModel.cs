namespace BambooExchangeRateService.Application.Models
{
    public class ExchangeRateRequestModel
    {
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public decimal Amount { get; set; }
    }
}
