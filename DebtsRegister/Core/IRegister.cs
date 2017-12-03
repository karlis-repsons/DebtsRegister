namespace DebtsRegister.Core
{
    public interface IRegister : IRegisterReader
    {
        new IPeopleRegister People { get; }

        new IDebtsRegister Debts { get; }
    }
}