using System;
using System.Linq;

namespace DebtsRegister.Core
{
    /*
     * Manages data of tables: CurrentTotalsPerPerson, StatisticsPerPerson.
     * Listens to event: IDebtDealsRegister.DebtDealReceived.
     */
    public class PersonDebtsRegister : IPersonDebtsRegister
    {
        public PersonDebtsRegister(
            TablesDbContext dbc,
            TablesDbContextForReader rdbc,
            IDebtDealsRegister debtDealsRegister
        ) {
            this.dbc = dbc;
            this.rdbc = rdbc;

            debtDealsRegister.DebtDealReceived
                += this.OnDebtDealReceived;
        }

        public PersonTotalsRow GetTotals(string personId)
            => this.rdbc.CurrentTotalsPerPerson
                .Where(ct => ct.PersonId == personId).FirstOrDefault()
            ?? new PersonTotalsRow();

        public PersonStatisticsRow GetStatistics(string personId)
            => this.rdbc.CurrentStatisticsPerPerson
                .Where(s => s.PersonId == personId).FirstOrDefault()
            ?? new PersonStatisticsRow();

        public decimal? GetRepaidFraction(string personId) {
            PersonTotalsRow totals
                = this.rdbc.CurrentTotalsPerPerson
                    .Where(ct =>
                           ct.PersonId == personId
                        && ct.HistoricallyOwedInTotal > 0)
                    .FirstOrDefault();

            if (totals == null)
                return null;

            return (  totals.HistoricallyOwedInTotal
                    - totals.DueDebtsTotal
                   )
                    / totals.HistoricallyOwedInTotal;
        }


        protected virtual void OnDebtDealReceived(
            object o_debtDealsRegister, DebtDealReceivedEventData evData
        ) {
            this.UpdateTotalsPerPerson(evData);
            this.UpdateStatisticsPerPerson(evData);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="evData"></param>
        /// <returns>new taker totals</returns>
        private void UpdateTotalsPerPerson(DebtDealReceivedEventData evData) {
            /*
             *     *** Notes on parallel execution ***
             * 
             *  Need to consider:
             *      * parallel changes of the same
             *        giverTotals and takerTotals red from
             *        CurrentTotalsPerPerson table.
             *      
             *      * parallel attempts to add new totals
             *        for the same giver or taker.
             *  
             *  Result - serialize for operations in CurrentTotalsPerPerson
             *           table with the same giverId or takerId.
             */

            DebtDealRow deal = evData.Deal;

            PersonTotalsRow giverTotals
                = this.dbc.CurrentTotalsPerPerson
                    .Where(ct => ct.PersonId == deal.GiverId)
                    .FirstOrDefault()
                ?? new PersonTotalsRow();
            giverTotals.PersonId = deal.GiverId;

            PersonTotalsRow takerTotals
                = this.dbc.CurrentTotalsPerPerson
                    .Where(ct => ct.PersonId == deal.TakerId)
                    .FirstOrDefault()
                ?? new PersonTotalsRow();
            takerTotals.PersonId = deal.TakerId;

            if (evData.Analysis.IsPayback)
                giverTotals.DueDebtsTotal = Math.Max(
                    0, giverTotals.DueDebtsTotal - deal.Amount);
            else {
                giverTotals.HistoricallyCreditedInTotal += deal.Amount;
                takerTotals.DueDebtsTotal += deal.Amount;
                takerTotals.HistoricallyOwedInTotal += deal.Amount;
                takerTotals.HistoricalCountOfCreditsTaken++;
            }

            if (this.dbc.CurrentTotalsPerPerson
                    .Count(ct => ct.PersonId == deal.GiverId) == 0
            )
                this.dbc.CurrentTotalsPerPerson.Add(giverTotals);

            if (this.dbc.CurrentTotalsPerPerson
                    .Count(ct => ct.PersonId == deal.TakerId) == 0
            )
                this.dbc.CurrentTotalsPerPerson.Add(takerTotals);

            this.dbc.SaveChanges();
        }

        private void UpdateStatisticsPerPerson(
            DebtDealReceivedEventData evData
        ) {
            if (evData.Analysis.IsPayback)
                return;

            /*
             *     *** Notes on parallel execution ***
             * 
             *  Need to consider:
             *      * parallel changes of:
             *          + takerStatistics from CurrentStatisticsPerPerson table
             *          + takerTotals from CurrentTotalsPerPerson table
             *  
             *  Result - serialize for operations with the same takerId in:
             *              + CurrentStatisticsPerPerson table
             *              + CurrentTotalsPerPerson table
             */

            string takerId = evData.Deal.TakerId;

            PersonTotalsRow takerTotals
                = this.rdbc.CurrentTotalsPerPerson
                    .Where(ct => ct.PersonId == takerId)
                    .FirstOrDefault()
                ?? new PersonTotalsRow();
            PersonStatisticsRow takerStatistics
                = this.dbc.CurrentStatisticsPerPerson
                    .Where(s => s.PersonId == takerId)
                    .FirstOrDefault()
                ?? new PersonStatisticsRow();

            takerStatistics.PersonId = takerId;
            takerStatistics.HistoricalDebtAverageThroughCasesOfTaking
                = takerTotals.HistoricallyOwedInTotal
                / takerTotals.HistoricalCountOfCreditsTaken;

            if (this.dbc.CurrentStatisticsPerPerson
                    .Count(s => s.PersonId == takerId) == 0
            )
                this.dbc.CurrentStatisticsPerPerson.Add(takerStatistics);

            this.dbc.SaveChanges();
        }


        private readonly TablesDbContext dbc;
        private readonly TablesDbContextForReader rdbc;
    }
}