using System.Collections.Generic;

namespace DebtsRegister.Core
{
    public interface IAchieversByID
    {
        IReadOnlyCollection<string> WithMaxDueDebtsTotal { get; }

        IReadOnlyCollection<string> WhoHistoricallyCreditedMaxTotal { get; }

        IReadOnlyCollection<string> BestDebtorByRepaidFractionThenTotal { get; }
    }
}