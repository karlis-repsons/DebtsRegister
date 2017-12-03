using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DebtsRegister.Initialization.Simplified
{
    using DebtsRegister.Core;

    public static class SimplifiedTablesDbContextInitializer
    {
        public static void AddConfiguration(IConfigurationBuilder builder) {
            builder.AddJsonFile(
                "Initialization/Core/db/tables/tablesDbSettings.json",
                optional: false, reloadOnChange: true);
        }

        public static void AddDIServices(
            IServiceCollection services,
            IConfiguration config
        ) {
            var dbConnectionString = config["Tables:MainDB:ConnectionString"];

            services.AddDbContext<TablesDbContext>(
                options => options.UseSqlServer(dbConnectionString));

            services.AddDbContext<TablesDbContextForReader>(
                options => options.UseSqlServer(dbConnectionString));
        }
    }
}