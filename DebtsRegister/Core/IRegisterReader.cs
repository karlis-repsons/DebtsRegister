namespace DebtsRegister.Core
{
    public interface IRegisterReader
    {
        bool HasNoData { get; }

        IPeopleRegisterReader People { get; }

        IDebtsRegisterReader Debts { get; }
    }
}