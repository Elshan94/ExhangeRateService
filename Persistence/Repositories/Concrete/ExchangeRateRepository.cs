using BambooExchangeRateService.Persistence.Entities;
using BambooExchangeRateService.Persistence.Repositories.Abstract;

namespace BambooExchangeRateService.Persistence.Repositories.Concrete
{
    public class ExchangeRateRepository : GenericRepository<ExchangeRate>, IExchangeRateRepository
    {
        public ExchangeRateRepository(ExchangeRateDbContext exchangeRateDbContext): base(exchangeRateDbContext)
        {

        }

    }
}
