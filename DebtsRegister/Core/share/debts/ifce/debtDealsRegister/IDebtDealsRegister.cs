using System;
using System.Linq;

namespace DebtsRegister.Core
{
    /*
     * Manages data of tables: DebtDeals, DebtDealsAnalysis.
     */
    public interface IDebtDealsRegister : IDebtDealsRegisterReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deal"></param>
        /// <returns>debt deal ID</returns>
        long Add(DebtDealRow deal);

        new IQueryable<DebtDealRow> All { get; }

        new IQueryable<DebtDealRow> Credits { get; }

        new IQueryable<DebtDealRow> Paybacks { get; }

        event EventHandler<DebtDealReceivedEventData> DebtDealReceived;

        event EventHandler<DebtDealAddedEventData> DebtDealAdded;
    }
}