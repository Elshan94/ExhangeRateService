namespace BambooExhangeRateService.Application.Models
{
    public class ConvertRequestModel
    {
        public string BaseCurrency { get; set; }

        public List<string> ToCurrencies { get; set; } = new List<string>();
        public decimal Amount { get; set; }
    }
}
