using Xunit;
using System.Linq;

using DebtsRegister.Core;

namespace DebtsRegister.Tests.Core.DB.Tables
{
    public class TablesDbContextTests
    {
        public const string Id1 = "id1";
        public const string Name1 = "Rowan";
        public const string Surname1 = "Atkinson";

        public const string Id2 = "id2";
        public const string Name2 = "Susan";
        public const string Surname2 = "McGrath";

        [Fact]
        public void CanUpdateReferencedEntityThroughReadWriteContext() {
            const string newName = "RenamedSusan";

            using (var connection = new TestTablesDbConnection()) {
                using (var cp = new TestTablesDbContextsProvider(connection)) {
                    var dbc = cp.DbContext;
                    this.TryAddingPeople(dbc);
                    
                    PersonRow person = dbc.People
                        .Where(p => p.Id == Id2).First();
                    person.Name = newName;

                    Assert.True(
                        dbc.People.Where(p => p.Id == Id2)
                            .Select(p => p.Name).First() == Name2);

                    dbc.SaveChanges();
                }

                using (var cp = new TestTablesDbContextsProvider(connection))
                    Assert.True(
                        cp.DbContext.People.Where(p => p.Id == Id2)
                            .Select(p => p.Name).First() == newName);
            }
        }
        
        [Fact]
        public void CannotUpdateReferencedEntityThroughReaderContext() {
            const string newName = "RenamedSusan";

            using (var connection = new TestTablesDbConnection()) {
                using (var cp = new TestTablesDbContextsProvider(connection)) {
                    this.TryAddingPeople(cp.DbContext);

                    var rdbc = cp.DbContextForReader;
                    PersonRow person = rdbc.People
                        .Where(p => p.Id == Id2).First();

                    person.Name = newName;
                    rdbc.SaveChanges();
                }

                using (var cp = new TestTablesDbContextsProvider(connection))
                    Assert.True(
                        cp.DbContextForReader
                            .People.Where(p => p.Id == Id2)
                            .Select(p => p.Name).First() == Name2);
            }
        }
        

        private void TryAddingPeople(TablesDbContext dbContext) {
            dbContext.People.Add(new PersonRow {
                Id = Id1, Name = Name1, Surname = Surname1 });

            dbContext.People.Add(new PersonRow {
                Id = Id2, Name = Name2, Surname = Surname2 });

            dbContext.SaveChanges();
        }
    }
}