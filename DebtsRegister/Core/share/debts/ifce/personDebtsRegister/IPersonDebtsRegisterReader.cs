namespace DebtsRegister.Core
{
    public interface IPersonDebtsRegisterReader
    {
        PersonTotalsRow GetTotals(string personId);

        PersonStatisticsRow GetStatistics(string personId);

        decimal? GetRepaidFraction(string personId);
    }
}