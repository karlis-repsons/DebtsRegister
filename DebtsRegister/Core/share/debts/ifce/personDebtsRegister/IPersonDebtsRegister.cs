namespace DebtsRegister.Core
{
    /*
     * Manages data of tables: CurrentTotalsPerPerson, StatisticsPerPerson.
     * Listens to event: IDebtDealsRegister.DebtDealReceived.
     */
    public interface IPersonDebtsRegister : IPersonDebtsRegisterReader
    {
    }
}