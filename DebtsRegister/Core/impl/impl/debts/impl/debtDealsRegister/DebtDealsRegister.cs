using System;
using System.Linq;

namespace DebtsRegister.Core
{
    /*
     * Manages data of tables: DebtDeals, DebtDealsAnalysis.
     */
    public class DebtDealsRegister : IDebtDealsRegister
    {
        public DebtDealsRegister(
            TablesDbContext dbc,
            TablesDbContextForReader rdbc
        ) {
            this.dbc = dbc;
            this.rdbc = rdbc;
        }

        public long Add(DebtDealRow deal) {
            /*
             *     *** Notes on parallel execution ***
             * 
             * If two debt deals have 4 unique involved people id-s,
             * these same deals may be processed in parallel in this register
             * (but may need serialization while other registers handle 
             * DebtDealAdded event).
             * 
             * Need to consider validity of initialGiverDebtToTaker.
             * 
             * Other cases:
             *  three unique involved id-s:
             *      same giver, different takers: => parallel-ok
             *      
             *      different givers, same taker: => parallel-ok
             *  
             *  two unique involved id-s:
             *      same giver, same taker => serialize (case a)
             *      
             *      D1.giver = D2.taker, D1.taker = D2.giver 
             *          => serialize (case b)
             * 
             * 
             * Here, parallel execution has to be serialized when register
             * has not finished processing two deals which match one of these:
             *     a) if giver and taker are the same for two deals,
             *        later deal must wait till earlier one is fully processed.
             *        (consider small and complete pay-back immediately
             *         followed by a big credit - if D1 is processed after D2
             *         was fully processed, big credit could be considered
             *         as mostly a GIFT, confusing it with a payback)
             *     
             *     b) if D1.giver = D2.taker, D1.taker = D2.giver
             *        (consider pay and repay of the same amount -
             *         in case of parallel execution of both deals,
             *         payback could be missed and DB would get corrupt)
             */

            long addedDebtDealId;
            using (var transaction = this.dbc.Database.BeginTransaction()) {
                this.dbc.DebtDeals.Add(deal);
                this.dbc.SaveChanges(); // need deal.Id
                addedDebtDealId = deal.Id;

                decimal initialGiverDebtToTaker = GiverDebtToTaker();
                decimal? repayGiftAmount = null;
                if (initialGiverDebtToTaker > 0m)
                    repayGiftAmount
                        = Math.Max(0m,
                            deal.Amount - initialGiverDebtToTaker);

                var analysisRow = new DebtDealAnalysisRow() {
                    DebtDealId = deal.Id,
                    IsPayback = initialGiverDebtToTaker > 0m
                };
                this.dbc.DebtDealsAnalysis.Add(analysisRow);
                this.dbc.SaveChanges();

                DebtDealReceived?.Invoke(this,
                    new DebtDealReceivedEventData() {
                        Deal = deal,
                        Analysis = analysisRow,
                        RepayGiftAmount = repayGiftAmount });

                transaction.Commit();
            }

            DebtDealAdded?.Invoke(this,
                new DebtDealAddedEventData() {
                    AddedDebtDealId = addedDebtDealId });

            return addedDebtDealId;

            decimal GiverDebtToTaker() {
                return this.dbc.CurrentDebts.Where(cd =>
                       cd.CreditorId == deal.TakerId
                    && cd.DebtorId == deal.GiverId
                ).Select(cd => cd.DebtTotal).FirstOrDefault();
            }
        }

        public IQueryable<DebtDealRow> All => this.dbc.DebtDeals;
        IQueryable<DebtDealRow> IDebtDealsRegisterReader.All
            => this.rdbc.DebtDeals;

        public IQueryable<DebtDealRow> Credits
            => this.QueryDebtDeals(isPayback: false, returnForReader: false);
        IQueryable<DebtDealRow> IDebtDealsRegisterReader.Credits
            => this.QueryDebtDeals(isPayback: false, returnForReader: true);

        public IQueryable<DebtDealRow> Paybacks
            => this.QueryDebtDeals(isPayback: true, returnForReader: false);
        IQueryable<DebtDealRow> IDebtDealsRegisterReader.Paybacks
            => this.QueryDebtDeals(isPayback: true, returnForReader: true);

        public bool IsPayback(long debtDealId) {
            DebtDealAnalysisRow ar
                = this.rdbc.DebtDealsAnalysis
                    .Where(dda => dda.DebtDealId == debtDealId)
                    .First();

            return ar.IsPayback;
        }

        public event EventHandler<DebtDealReceivedEventData> DebtDealReceived;
        public event EventHandler<DebtDealAddedEventData> DebtDealAdded;


        private IQueryable<DebtDealRow> QueryDebtDeals(
            bool isPayback, bool returnForReader
        ) {
            if (returnForReader)
                return Fetch<TablesDbContextForReader>(this.rdbc);
            else
                return Fetch<TablesDbContext>(this.dbc);

            IQueryable<DebtDealRow> Fetch<TContext>(TContext dbContext)
                where TContext : TablesDbContextBase<TContext>
            {
                return from debtDeal in dbContext.DebtDeals
                       join analysis in dbContext.DebtDealsAnalysis
                       on debtDeal.Id equals analysis.DebtDealId
                       where analysis.IsPayback == isPayback
                       select debtDeal;
            }
        }


        private readonly TablesDbContext dbc;
        private readonly TablesDbContextForReader rdbc;
    }
}