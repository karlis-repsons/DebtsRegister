using System.Linq;

namespace DebtsRegister.Core
{
    public interface IPeopleRegister : IPeopleRegisterReader
    {
        new IQueryable<PersonRow> All { get; }

        new PersonRow GetById(string id);

        void Add(PersonRow person);
    }
}