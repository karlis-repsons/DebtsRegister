using System;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Mongo2Go;

namespace DebtsRegister.Tests.Core
{
    using DebtsRegister.Core;

    public class TestDocumentDbProvider : IDisposable
    {
        private readonly string dbNameSettingsKey = "Documents:MainDB:Name";

        public TestDocumentDbProvider(IConfiguration documentDbSettings) {
            this.documentDbSettings = documentDbSettings
                ?? throw new ArgumentException(
                    "documentDbSettings are required");
        }

        public IMongoDatabase DocumentDB {
            get {
                if (this.mongoDb == null)
                    this.Initialize();

                return this.mongoDb;
            }
        }

        /// <summary>
        /// This caused trouble when called repeatedly
        /// (other MongoDB operations froze unpredictably after some calls).
        /// </summary>
        public void DeleteDB() {
            if (this.mongoDb != null && this.mongoClient != null) {
                this.mongoClient.DropDatabase(
                    this.documentDbSettings[dbNameSettingsKey]);

                this.mongoDb = null;
            }
        }

        public virtual void Dispose() {
            if (this.mongoRunner != null)
                this.mongoRunner.Dispose();

            GC.SuppressFinalize(this);
        }


        private void Initialize() {
            if (this.mongoRunner == null) {
                MongoDbTypesInitializer.Initialize();
                this.mongoRunner = MongoDbRunner.Start();
            }
            
            if (this.mongoClient == null)
                this.mongoClient = new MongoClient(
                    this.mongoRunner.ConnectionString);

            if (this.mongoDb == null)
                this.mongoDb = this.mongoClient.GetDatabase(
                    this.documentDbSettings[dbNameSettingsKey]);
        }


        private readonly IConfiguration documentDbSettings;
        private MongoDbRunner mongoRunner;
        private IMongoClient mongoClient;
        private IMongoDatabase mongoDb;
    }
}