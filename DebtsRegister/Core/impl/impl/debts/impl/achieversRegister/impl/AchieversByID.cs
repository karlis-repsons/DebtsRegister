using System;
using System.Collections.Generic;

namespace DebtsRegister.Core
{
    public class AchieversByID : IAchieversByID {
        public AchieversByID(
            Func<AchieversDoc> achieversDocGetter
        ) {
            this.achieversDocGetter = achieversDocGetter;
        }

        public IReadOnlyCollection<string> WithMaxDueDebtsTotal
            => this.achieversDocGetter()?.MaxDueDebtsTotalPersonIds
            ?? new HashSet<string>();

        public IReadOnlyCollection<string> WhoHistoricallyCreditedMaxTotal
            => this.achieversDocGetter()?.HistoricallyCreditedMaxTotalPersonIds
            ?? new HashSet<string>();

        public IReadOnlyCollection<string> BestDebtorByRepaidFractionThenTotal
            => this.achieversDocGetter()?.BestDebtorIdsByRepaidFractionThenTotal
            ?? new HashSet<string>();


        private readonly Func<AchieversDoc> achieversDocGetter;
    }
}