namespace DebtsRegister.Tests.Core
{
    using DebtsRegister.Core;

    public class TablesDbPopulator
    {
        public TablesDbPopulator(TablesDbContext dbc) {
            this.dbc = dbc;
        }

        public void AddPeople(long count) {
            for (long i = 1; i <= count; i++) {
                string FirstName = $"First{i}", LastName = $"Last{i}";
                dbc.People.Add(
                    new PersonRow {
                        Id = $"xyz-{i}",
                        Name = FirstName,
                        Surname = LastName
                    });
            }

            dbc.SaveChanges();
        }


        private readonly TablesDbContext dbc;
    }
}