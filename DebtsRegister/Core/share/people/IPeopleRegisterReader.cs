using System.Linq;

namespace DebtsRegister.Core
{
    public interface IPeopleRegisterReader
    {
        bool HasNoData { get; }

        IQueryable<PersonRow> All { get; }

        PersonRow GetById(string id);
    }
}