using System.Linq;

namespace DebtsRegister.Core
{
    public class PeopleRegister : IPeopleRegister
    {
        public PeopleRegister(
            TablesDbContext dbc,
            TablesDbContextForReader rdbc
        ) {
            this.dbc = dbc;
            this.rdbc = rdbc;
        }

        public bool HasNoData => this.dbc.People.Any() == false;

        public IQueryable<PersonRow> All => this.dbc.People;
        IQueryable<PersonRow> IPeopleRegisterReader.All => this.rdbc.People;

        public PersonRow GetById(string id)
            => this.dbc.People.Where(p => p.Id == id).FirstOrDefault();
        PersonRow IPeopleRegisterReader.GetById(string id)
           => this.rdbc.People.Where(p => p.Id == id).FirstOrDefault();

        public void Add(PersonRow person) {
            this.dbc.People.Add(person);
            this.dbc.SaveChanges();
        }


        private readonly TablesDbContext dbc;
        private readonly TablesDbContextForReader rdbc;
    }
}