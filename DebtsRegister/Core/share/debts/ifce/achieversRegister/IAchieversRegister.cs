namespace DebtsRegister.Core
{
    /*
     * Manages data of document AchieversDoc
     * and table CurrentPeopleStatisticsDocuments.
     * Listens to IDebtDealsRegister events: DebtDealReceived,
     * DebtDealAdded.
     */
    public interface IAchieversRegister : IAchieversRegisterReader
    {
    }
}