using System;
using System.Linq;

namespace DebtsRegister.Core
{
    public interface IDebtDealsRegisterReader
    {
        /// <summary>
        /// Query all debt deals.
        /// Changes will not be saved in database.
        /// </summary>
        IQueryable<DebtDealRow> All { get; }

        /// <summary>
        /// Query debt deals, which are credit, not paybacks.
        /// Changes will not be saved in database.
        /// </summary>
        IQueryable<DebtDealRow> Credits { get; }

        /// <summary>
        /// Query debt deals, which are paybacks.
        /// Changes will not be saved in database.
        /// </summary>
        IQueryable<DebtDealRow> Paybacks { get; }

        bool IsPayback(long debtDealId);
    }
}