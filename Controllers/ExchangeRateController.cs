using BambooExchangeRateService.Application.Models;
using BambooExchangeRateService.Application.Services.Abstract;
using BambooExhangeRateService.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BambooExchangeRateService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
    public class ExchangeRateController : ControllerBase
    {
        public IExchangeRateService ExchangeRateService { get; }

        public ExchangeRateController(IExchangeRateService exchangeRateService)
        {
            ExchangeRateService = exchangeRateService;
        }

        [HttpPost("convertAmount")]
     //   [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> ConvertAmount([FromBody] ConvertRequestModel model, [FromQuery] string providerKey)
        {
            var result = await ExchangeRateService.ConvertAsync(model, providerKey);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("getExchangeRates")]
   //    [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetExchangeRates([FromQuery] string currency, string providerKey)
        {
            var result = await ExchangeRateService.GetExchangeRate(currency, providerKey);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("getHistoricalExchangeRates")]
   //     [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetHistoricalExchangeRates([FromBody] HistoricalRatesRequestModel model, string providerKey)
        {
            var result = await ExchangeRateService.GetHistoricalRatesAsync(model, providerKey);
            return StatusCode(result.StatusCode, result);
        }
    }
}
