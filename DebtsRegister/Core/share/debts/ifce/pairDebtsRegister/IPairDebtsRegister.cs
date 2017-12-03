namespace DebtsRegister.Core
{
    /*
     * Manages data of tables: CurrentDebts.
     * Listens to event: IDebtDealsRegister.DebtDealReceived.
     */
    public interface IPairDebtsRegister : IPairDebtsRegisterReader
    {
    }
}