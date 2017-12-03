using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

using DebtsRegister.Core;

namespace DebtsRegister.Tests.Core
{
    public class TestDocumentCollectionsProvider
    {
        public TestDocumentCollectionsProvider(
            IMongoDatabase mongoDb, IConfiguration documentDbSettings
        ) {
            this.mongoDb = mongoDb;
            this.documentDbSettings = documentDbSettings;
        }

        public IMongoDatabase MongoDB => this.mongoDb;

        public IMongoCollection<AchieversDoc> AchieversCollection {
            get {
                if (this.achieversCollection == null)
                    this.achieversCollection
                        = this.mongoDb.GetCollection<AchieversDoc>(
                            this.documentDbSettings[
                                "Documents:MainDB:Collections:Achievers:Name"]);

                return this.achieversCollection;
            }
        }

        public void DeleteContent() {
            DeleteAllDocumentsIn(this.achieversCollection);

            void DeleteAllDocumentsIn<DocType>(
                IMongoCollection<DocType> collection
            ) {
                collection.DeleteMany(Builders<DocType>.Filter.Empty);
            }
        }


        private readonly IMongoDatabase mongoDb;
        private readonly IConfiguration documentDbSettings;
        private IMongoCollection<AchieversDoc> achieversCollection;
    }
}