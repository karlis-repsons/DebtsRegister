using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DebtsRegister.Initialization.Simplified
{
    using DebtsRegister.Core;

    public static class SimplifiedCoreInitializer
    {
        public static void AddConfiguration(IConfigurationBuilder builder) {
            SimplifiedTablesDbContextInitializer.AddConfiguration(builder);
            SimplifiedAchieversCollectionInitializer.AddConfiguration(builder);
        }

        public static void AddDIServices(
            IServiceCollection services,
            IConfiguration config
        ) {
            SimplifiedTablesDbContextInitializer.AddDIServices(services, config);
            SimplifiedAchieversCollectionInitializer.AddDIServices(services, config);
            AddRegistersDIServices(services);
        }


        private static void AddRegistersDIServices(
            IServiceCollection services
        ) {
            // people
            services.AddScoped<IPeopleRegister, PeopleRegister>();
            services.AddScoped<IPeopleRegisterReader, PeopleRegister>();

            // debts
            services.AddScoped<IDebtsRegister, DebtsRegister>();
            services.AddScoped<IDebtsRegisterReader, DebtsRegister>();
            {
                // person
                services.AddScoped<
                    IPersonDebtsRegister, PersonDebtsRegister
                >();
                services.AddScoped<
                    IPersonDebtsRegisterReader, PersonDebtsRegister
                >();

                // pair
                services.AddScoped<IPairDebtsRegister, PairDebtsRegister>();
                services.AddScoped<
                    IPairDebtsRegisterReader, PairDebtsRegister
                >();

                // debt deals
                services.AddScoped<IDebtDealsRegister, DebtDealsRegister>();
                services.AddScoped<
                    IDebtDealsRegisterReader, DebtDealsRegister
                >();

                // achievers
                services.AddScoped<IAchieversRegister, AchieversRegister>();
                services.AddScoped<
                    IAchieversRegisterReader, AchieversRegister
                >();
            }
        }
    }
}