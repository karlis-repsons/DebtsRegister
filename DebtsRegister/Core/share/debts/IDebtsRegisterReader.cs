namespace DebtsRegister.Core
{
    public interface IDebtsRegisterReader
    {
        bool HasNoData { get; }

        IPersonDebtsRegisterReader Person { get; }

        IPairDebtsRegisterReader Pairs { get; }

        IDebtDealsRegisterReader Deals { get; }

        IAchieversRegisterReader Achievers { get; }
    }
}