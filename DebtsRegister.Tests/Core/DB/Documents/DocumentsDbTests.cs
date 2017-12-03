using System.Collections.Generic;
using System.Linq;
using Xunit;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DebtsRegister.Tests.Core.DB.Documents
{
    public class TestDoc
    {
        public ObjectId Id { get; set; }

        public byte B { get; set; }

        public string S { get; set; }

        public HashSet<string> HSS { get; set; }
    }

    public class DocumentsDbTests
    {
        [Fact]
        public void CanSaveAndRetrieveDocument() {
            string testCollectionName = "TestDocs_hs523";
            var originalDoc = new TestDoc() {
                Id = new ObjectId(),
                B = 12,
                S = "stri",
                HSS = new HashSet<string>() { "aVal", "bVal" }
            };

            using (var dbsProvider = new TestDBsProvider()) {
                using (var w = new RegistersWrapper(dbsProvider)) {
                    IMongoCollection<TestDoc> collection = w.DB.Documents.MongoDB
                        .GetCollection<TestDoc>(testCollectionName);

                    collection.InsertOne(originalDoc);
                }

                using (var w = new RegistersWrapper(dbsProvider)) {
                    IMongoCollection<TestDoc> collection = w.DB.Documents.MongoDB
                        .GetCollection<TestDoc>(testCollectionName);

                    TestDoc retrievedDoc = collection.AsQueryable()
                        .Where(td => td.Id == originalDoc.Id)
                        .First();

                    Assert.True(retrievedDoc.B == originalDoc.B);
                    Assert.True(retrievedDoc.S == originalDoc.S);
                    Assert.True(retrievedDoc.HSS.SetEquals(originalDoc.HSS));
                }
            }
        }
    }
}