using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace DebtsRegister.Initialization.Simplified
{
    using DebtsRegister.Core;

    public static class SimplifiedAchieversCollectionInitializer
    {
        public static void AddConfiguration(IConfigurationBuilder builder) {
            builder.AddJsonFile(
                "Initialization/Core/db/documents/documentsDbSettings.json",
                optional: false, reloadOnChange: true);
        }

        public static void AddDIServices(
            IServiceCollection services,
            IConfiguration config
        ) {
            MongoDbTypesInitializer.Initialize();

            var mongoClient = new MongoClient();
            IMongoDatabase db = mongoClient
                .GetDatabase(config["Documents:MainDB:Name"]);

            IMongoCollection<AchieversDoc> collection
                = db.GetCollection<AchieversDoc>(
                    config["Documents:MainDB:Collections:Achievers:Name"]);

            services.AddSingleton<IMongoCollection<AchieversDoc>>(collection);
        }
    }
}