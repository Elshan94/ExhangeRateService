using System.Data.Common;

namespace BambooExchangeRateService.Application.Models
{
    public class DatabaseConfig
    {
        public string DataSource { get; set; }
        public string InitialCatalog { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }

        public string GetConnectionString()
        {
            var connectionStringBuilder = new DbConnectionStringBuilder()
    {
        { "Data Source", DataSource },
        { "Initial Catalog", InitialCatalog },
        { "User ID", UserID },
        { "Password", Password }
    };

            return connectionStringBuilder.ConnectionString;
        }
    }
}
