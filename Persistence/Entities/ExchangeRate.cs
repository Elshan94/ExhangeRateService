﻿namespace BambooExchangeRateService.Persistence.Entities
{
    public class ExchangeRate
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public decimal Rate { get; set; }
    }
}
