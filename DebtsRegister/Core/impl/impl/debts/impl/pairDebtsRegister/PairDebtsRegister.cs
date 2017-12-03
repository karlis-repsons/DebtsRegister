using System;
using System.Linq;

namespace DebtsRegister.Core
{
    /*
     * Manages data of tables: CurrentDebts.
     * Listens to event: IDebtDealsRegister.DebtDealReceived.
     */
    public class PairDebtsRegister : IPairDebtsRegister {
        public PairDebtsRegister(
            TablesDbContext dbc,
            TablesDbContextForReader rdbc,
            IDebtDealsRegister debtDealsRegister
        ) {
            this.dbc = dbc;
            this.rdbc = rdbc;

            debtDealsRegister.DebtDealReceived += this.OnDebtDealReceived;
        }

        public decimal GetCurrentDebt(string creditorId, string debtorId) {
            DebtRow debt = this.rdbc.CurrentDebts
                .Where(cd => cd.CreditorId == creditorId
                          && cd.DebtorId == debtorId).FirstOrDefault();

            return debt?.DebtTotal ?? 0;
        }


        protected virtual void OnDebtDealReceived(
            object o_debtDealsRegister, DebtDealReceivedEventData evData
        ) {
            /*
             *     *** Notes on parallel execution ***
             * 
             * Similar to DebtDealsRegister Add method,
             * consider only these cases:
             * 
             *  two unique involved id-s:
             *      * same giver, same taker
             *      * D1.giver = D2.taker, D1.taker = D2.giver
             *  
             *  Need to consider parallel changes of the same debt
             *  in CurrentDebts table.
             *  
             *  Result - serialize always when
             *  two deals have only two unique involved id-s.
             */

            DebtDealRow deal = evData.Deal;
            DebtRow initialGiverDebtToTaker
                = this.dbc.CurrentDebts.Where(cd =>
                       cd.CreditorId == deal.TakerId
                    && cd.DebtorId == deal.GiverId).FirstOrDefault();
            if (initialGiverDebtToTaker == null)
                initialGiverDebtToTaker = new DebtRow {
                    CreditorId = deal.TakerId, DebtorId = deal.GiverId };

            DebtRow initialTakerDebtToGiver
                = this.dbc.CurrentDebts.Where(cd =>
                       cd.CreditorId == deal.GiverId
                    && cd.DebtorId == deal.TakerId).FirstOrDefault();
            if (initialTakerDebtToGiver == null)
                initialTakerDebtToGiver = new DebtRow {
                    CreditorId = deal.GiverId, DebtorId = deal.TakerId };

            DebtRow debtToChange;
            if (evData.Analysis.IsPayback)
                debtToChange = initialGiverDebtToTaker;
            else
                debtToChange = initialTakerDebtToGiver;

            decimal initialDebtAmount = debtToChange?.DebtTotal ?? 0m;
            decimal resultingDebtAmount;
            if (evData.Analysis.IsPayback)
                resultingDebtAmount = Math.Max(0m,
                    initialDebtAmount - deal.Amount);
            else
                resultingDebtAmount = initialDebtAmount + deal.Amount;

            debtToChange.DebtTotal = resultingDebtAmount;

            if (resultingDebtAmount > DebtConstants.MaxZeroEquivalent) {
                if (this.dbc.CurrentDebts.Count(cd =>
                           cd.CreditorId == debtToChange.CreditorId
                        && cd.DebtorId == debtToChange.DebtorId
                    )
                        == 0
                )
                    this.dbc.CurrentDebts.Add(debtToChange);
            }
            else
                this.dbc.CurrentDebts.Remove(debtToChange);

            this.dbc.SaveChanges();
        }


        private readonly TablesDbContext dbc;
        private readonly TablesDbContextForReader rdbc;
    }
}