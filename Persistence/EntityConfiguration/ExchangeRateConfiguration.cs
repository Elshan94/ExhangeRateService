using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using BambooExchangeRateService.Persistence.Entities;

namespace BambooExchangeRateService.Persistence.EntityConfiguration
{
    public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
    {
        public void Configure(EntityTypeBuilder<ExchangeRate> builder)
        {
            builder.ToTable("ExchangeRates");
            builder.Property(m => m.Currency).HasMaxLength(10).IsRequired(true);
            builder.Property(m => m.Rate).HasPrecision(18, 2).IsRequired(true);
            builder.Property(m => m.Date).IsRequired(true);
        }
    }
}
