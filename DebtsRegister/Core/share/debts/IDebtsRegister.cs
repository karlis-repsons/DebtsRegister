namespace DebtsRegister.Core
{
    public interface IDebtsRegister : IDebtsRegisterReader
    {
        new IPersonDebtsRegister Person { get; }

        new IPairDebtsRegister Pairs { get; }

        new IDebtDealsRegister Deals { get; }

        new IAchieversRegister Achievers { get; }
    }
}