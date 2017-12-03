namespace DebtsRegister.Core
{
    public interface IPairDebtsRegisterReader
    {
        decimal GetCurrentDebt(string creditorId, string debtorId);
    }
}