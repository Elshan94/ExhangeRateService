﻿namespace BambooExhangeRateService.Application.External.Models
{
    public class HistoricalRateRequestModel
    {
        public string Base { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
