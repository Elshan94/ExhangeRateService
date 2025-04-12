using BambooExhangeRateService.Application.Services.Abstract;
using ExchangeRateService.Application.Models;
using System.Text.Json;
using StackExchange.Redis;

namespace BambooExhangeRateService.Application.Services.Concrete
{
    public class RedisService : ICacheService
    {
        private readonly IDatabase _redis;
        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        public async Task<ExchangeRateResponseModel> GetAsync(string cuurency)
        {
            var cached = await _redis.StringGetAsync(cuurency);
            if (!cached.HasValue) return null;

            return JsonSerializer.Deserialize<ExchangeRateResponseModel>(cached);
        }

        public async Task SetAsync(string cuurency, ExchangeRateResponseModel data)
        {
            var ttl = GetTimeUntilNextUpdate();
            var json = JsonSerializer.Serialize(data);
            await _redis.StringSetAsync(cuurency, json, ttl);
        }
 
        private TimeSpan GetTimeUntilNextUpdate()
        {
            var now = DateTime.UtcNow;
            var nextUpdate = now.Date.AddHours(16);
            if (now >= nextUpdate)
                nextUpdate = nextUpdate.AddDays(1);
            return nextUpdate - now;
        }
    }
}
