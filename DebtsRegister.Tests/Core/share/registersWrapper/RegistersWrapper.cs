using System;

namespace DebtsRegister.Tests.Core
{
    public class RegistersWrapper : IDisposable
    {
        public RegistersWrapper(TestDBsProvider dbsProvider = null) {
            if (dbsProvider != null) {
                this.dbsProvider = dbsProvider;
                this.hasExternalDbsProvider = true;
            }
        }

        public RegistersFactory New {
            get {
                if (this.factory == null)
                    this.factory = new RegistersFactory(
                        this.DB.Tables.DbContext,
                        this.DB.Tables.DbContextForReader,
                        this.DB.Documents.AchieversCollection);

                return this.factory;
            }
        }

        public TestDBsProvider DB {
            get {
                if (this.dbsProvider == null)
                    this.dbsProvider = new TestDBsProvider();

                return this.dbsProvider;
            }
        }

        public virtual void Dispose() {
            if (   this.dbsProvider != null
                && this.hasExternalDbsProvider == false
            )
                this.dbsProvider.Dispose();

            GC.SuppressFinalize(this);
        }


        private readonly bool hasExternalDbsProvider = false;
        private TestDBsProvider dbsProvider;
        private RegistersFactory factory;
    }
}