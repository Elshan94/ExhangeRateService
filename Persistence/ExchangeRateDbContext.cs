using BambooExchangeRateService.Persistence.Entities;
using BambooExchangeRateService.Persistence.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace BambooExchangeRateService.Persistence
{
    public class ExchangeRateDbContext : DbContext
    {
        public DbSet<ExchangeRate> IBExchangeRates { get; set; }
        public ExchangeRateDbContext(DbContextOptions<ExchangeRateDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var assembly = typeof(ExchangeRateConfiguration).Assembly;

            modelBuilder.ApplyConfigurationsFromAssembly(assembly);

        }
    }
}
