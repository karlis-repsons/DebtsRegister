using System;

using DebtsRegister.Core;

namespace DebtsRegister.Tests.Core
{
    public class TestTablesDbContextsProvider : IDisposable
    {
        public TestTablesDbContextsProvider(
            TestTablesDbConnection dbConnection
        ) {
            this.dbConnection = dbConnection;
        }

        public TablesDbContext DbContext {
            get {
                if (this.dbc == null) {
                    this.InitializeIfNecessary();
                    this.dbc = new TablesDbContext(
                        this.dbConnection.ReadWriteDbContextOptions);
                }

                return this.dbc;
            }
        }

        public TablesDbContextForReader DbContextForReader {
            get {
                if (this.rdbc == null) {
                    this.InitializeIfNecessary();
                    this.rdbc = new TablesDbContextForReader(
                        this.dbConnection.ReaderDbContextOptions);
                }

                return this.rdbc;
            }
        }

        public virtual void Dispose() {
            if (this.dbc != null)
                this.dbc.Dispose();
            if (this.rdbc != null)
                this.rdbc.Dispose();

            GC.SuppressFinalize(this);
        }


        private bool initializeDone = false;
        private void InitializeIfNecessary() {
            if (this.initializeDone == false)
                using (var tmpDbContext = new TablesDbContext(
                    dbConnection.ReadWriteDbContextOptions)
                )
                    tmpDbContext.Database.EnsureCreated();

            this.initializeDone = true;
        }


        private readonly TestTablesDbConnection dbConnection;
        private TablesDbContext dbc;
        private TablesDbContextForReader rdbc;
    }
}