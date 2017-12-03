using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using DebtsRegister.Core;

namespace DebtsRegister.Tests.Core
{
    public class TestTablesDbConnection : IDisposable
    {
        public DbContextOptions<TablesDbContext> ReadWriteDbContextOptions {
            get {
                if (this.readWriteDbContextOptions == null) {
                    this.InitializeIfNecessary();
                    this.readWriteDbContextOptions
                        = new DbContextOptionsBuilder<TablesDbContext>()
                            .UseSqlite(this.dbConnection)
                            .Options;
                }

                return this.readWriteDbContextOptions;
            }
        }

        public DbContextOptions<TablesDbContextForReader>
            ReaderDbContextOptions {
            get {
                if (this.readerDbContextOptions == null) {
                    this.InitializeIfNecessary();
                    this.readerDbContextOptions
                        = new DbContextOptionsBuilder<TablesDbContextForReader>()
                            .UseSqlite(this.dbConnection)
                            .Options;
                }

                return this.readerDbContextOptions;
            }
        }
        
        public virtual void Dispose() {
            if (this.dbConnection != null)
                this.dbConnection.Dispose();

            GC.SuppressFinalize(this);
        }


        private void InitializeIfNecessary() {
            if (this.dbConnection == null) {
                this.dbConnection = new SqliteConnection("DataSource=:memory:");
                this.dbConnection.Open();
            }
        }


        private SqliteConnection dbConnection;
        private DbContextOptions<TablesDbContext> readWriteDbContextOptions;
        private DbContextOptions<TablesDbContextForReader>
            readerDbContextOptions;
    }
}