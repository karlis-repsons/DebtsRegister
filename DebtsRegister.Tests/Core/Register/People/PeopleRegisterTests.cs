using Xunit;
using System.Linq;

using DebtsRegister.Core;

namespace DebtsRegister.Tests.Core.Register.People
{
    public class PeopleRegisterTests {
        public const string Id1 = "id1";
        public const string Name1 = "Rowan";
        public const string Surname1 = "Atkinson";

        public const string Id2 = "id2";
        public const string Name2 = "Susan";
        public const string Surname2 = "McGrath";

        public const string Id3 = "id3";
        public const string Name3 = "Safia";
        public const string Surname3 = "Zenoth";

        [Fact]
        public void HasNoDataWhenEmpty() {
            using (var w = new RegistersWrapper()) {
                IPeopleRegister register = w.New.PeopleRegister();

                Assert.True(register.HasNoData);
                Assert.True(((IPeopleRegisterReader)register).HasNoData);
            }
        }

        [Fact]
        public void CanTryToLookupPeopleWhenEmpty() {
            using (var w = new RegistersWrapper()) {
                IPeopleRegister rwRegister = w.New.PeopleRegister();
                var roRegister = (IPeopleRegisterReader)rwRegister;

                Assert.True(roRegister.GetById("123") == null);
                Assert.True(roRegister.All
                    .Count(p => p.Id == "123") == 0);

                Assert.True(rwRegister.GetById("123") == null);
                Assert.True(rwRegister.All
                    .Count(p => p.Id == "123") == 0);
            }
        }

        [Fact]
        public void CanAddPerson() {
            using (var dbsProvider = new TestDBsProvider()) {
                this.AddTwoPersons(dbsProvider);

                using (var w = new RegistersWrapper(dbsProvider)) {
                    var rdbc = w.DB.Tables.DbContextForReader;

                    Assert.True(rdbc.People.Count() == 2);

                    Assert.True(rdbc.People.Where(
                               p => p.Id == Id1
                            && p.Name == Name1 && p.Surname == Surname1
                        ).Count() == 1
                    );

                    Assert.True(rdbc.People.Where(
                               p => p.Id == Id2
                            && p.Name == Name2 && p.Surname == Surname2
                        ).Count() == 1
                    );
                }
            }
        }

        [Fact]
        public void CanGetPersonById() {
            using (var dbsProvider = new TestDBsProvider()) {
                this.AddThreePersons(dbsProvider);

                using (var w = new RegistersWrapper(dbsProvider)) {
                    var rdbc = w.DB.Tables.DbContextForReader;

                    Assert.True(rdbc.People.Where(
                               p => p.Id == Id2
                            && p.Name == Name2 && p.Surname == Surname2
                        ).Count() == 1
                    );

                    IPeopleRegister register = w.New.PeopleRegister();
                    var regReader = (IPeopleRegisterReader)register;

                    foreach (PersonRow person in new[] {
                        register.GetById(Id2),
                        regReader.GetById(Id2) }
                    )
                        Assert.True( person.Id == Id2
                                  && person.Name == Name2
                                  && person.Surname == Surname2 );
                }
            }
        }

        [Fact]
        public void CanQueryPeople() {
            using (var dbsProvider = new TestDBsProvider()) {
                this.AddThreePersons(dbsProvider);

                using (var w = new RegistersWrapper(dbsProvider)) {
                    var dbc = w.DB.Tables.DbContext;
                    Assert.True(dbc.People.Where(
                               p => p.Id == Id2
                            && p.Name == Name2 && p.Surname == Surname2
                        ).Count() == 1
                    );

                    var rdbc = w.DB.Tables.DbContextForReader;
                    Assert.True(rdbc.People.Where(
                               p => p.Id == Id2
                            && p.Name == Name2 && p.Surname == Surname2
                        ).Count() == 1
                    );
                }
            }
        }

        
        private void AddTwoPersons(TestDBsProvider dbsProvider) {
            using (var w = new RegistersWrapper(dbsProvider)) {
                IPeopleRegister register = w.New.PeopleRegister();

                register.Add(new PersonRow {
                    Id = Id1, Name = Name1, Surname = Surname1
                });
                register.Add(new PersonRow {
                    Id = Id2, Name = Name2, Surname = Surname2
                });
            }
        }

        private void AddThreePersons(TestDBsProvider dbsProvider) {
            this.AddTwoPersons(dbsProvider);

            using (var w = new RegistersWrapper(dbsProvider)) {
                IPeopleRegister peopleRegister = w.New.PeopleRegister();
                peopleRegister.Add(new PersonRow {
                    Id = Id3, Name = Name3, Surname = Surname3 });
            }
        }
    }
}