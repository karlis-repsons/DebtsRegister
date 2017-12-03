using System;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

using DebtsRegister.Core;

namespace DebtsRegister.Tests.Core
{
    public class TestDBsProvider : IDisposable {
        public TestTablesDbContextsProvider Tables {
            get {
                if (this.tablesContextsProvider == null)
                    this.InitializeTables();
                return this.tablesContextsProvider;
            }
        }

        public TestDocumentCollectionsProvider Documents {
            get {
                if (this.documentCollectionsProvider == null)
                    this.InitializeDocuments();
                return this.documentCollectionsProvider;
            }
        }

        public void DeleteAllContent() {
            this.DeleteTablesDB();
            this.DeleteDocumentDbContent();
        }

        public virtual void Dispose() {
            if (this.tablesContextsProvider != null)
                this.tablesContextsProvider.Dispose();
            if (this.tablesDbConnection != null)
                this.tablesDbConnection.Dispose();

            if (this.documentDbProvider != null)
                this.documentDbProvider.Dispose();

            GC.SuppressFinalize(this);
        }


        private void InitializeTables() {
            if (this.tablesDbConnection == null)
                this.tablesDbConnection = new TestTablesDbConnection();

            if (this.tablesContextsProvider == null)
                this.tablesContextsProvider
                    = new TestTablesDbContextsProvider(this.tablesDbConnection);
        }

        private void DeleteTablesDB() {
            if (this.tablesContextsProvider != null) {
                this.tablesContextsProvider.Dispose();
                this.tablesContextsProvider = null;
            }

            if (this.tablesDbConnection != null) {
                this.tablesDbConnection.Dispose();
                this.tablesDbConnection = null;
            }
        }

        private void InitializeDocuments() {
            if (this.documentDbProvider == null)
                this.documentDbProvider
                    = new TestDocumentDbProvider(this.DocumentDbSettings);

            if (this.documentCollectionsProvider == null)
                this.documentCollectionsProvider
                    = new TestDocumentCollectionsProvider(
                        this.documentDbProvider.DocumentDB,
                        this.DocumentDbSettings);
        }

        private IConfiguration DocumentDbSettings {
            get {
                if (this.documentDbSettings == null) {
                    var builder = new ConfigurationBuilder()
                    .SetBasePath(PathsGetter.ConfigurationRoot);

                    builder.AddJsonFile(
                        "core/db/documents/documentsDbSettings.json",
                        optional: false, reloadOnChange: true);

                    this.documentDbSettings = builder.Build();
                }

                return this.documentDbSettings;
            }
        }

        private void DeleteDocumentDbContent() {
            if (this.documentCollectionsProvider != null)
                this.documentCollectionsProvider.DeleteContent();
        }


        private TestTablesDbConnection tablesDbConnection;
        private TestTablesDbContextsProvider tablesContextsProvider;
        private IConfiguration documentDbSettings;
        private TestDocumentDbProvider documentDbProvider;
        private TestDocumentCollectionsProvider documentCollectionsProvider;
    }
}