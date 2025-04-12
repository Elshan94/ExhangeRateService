namespace BambooExhangeRateService.Application.Models
{
    public class HistoricalRatesRequestModel
    {
        public string BaseCurrency { get; set; }  
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }  
        public int Page { get; set; } 
        public int PageSize { get; set; }  
        public string ProviderKey { get; set; } 
    }
}
