using System;
using System.Linq;
using Xunit;
using MongoDB.Driver;

namespace DebtsRegister.Tests.Core.Register.Debts.DealSequences.Basic
{
    using DebtsRegister.Core;

    public class SingleDebtDealTest
    {
        [Fact]
        public void FinalStateMatches() {
            const int peopleCount = 10,
                      firstIdNumber = 0,
                      secondIdNumber = 3,
                      thirdIdNumber = 7;
            decimal amount = 20.123m;

            string giverId, takerId, thirdId;
            long debtDealId;

            using (var dbsProvider = new TestDBsProvider()) {
                using (var w = new RegistersWrapper(dbsProvider)) {
                    var dbc = w.DB.Tables.DbContext;

                    new TablesDbPopulator(dbc).AddPeople(peopleCount);

                    IRegister rwRoot = w.New.Register();
                    var roRoot = (IRegisterReader)rwRoot;

                    IPeopleRegisterReader roPeople = roRoot.People;
                    giverId = roPeople.All.OrderBy(p => p.Id)
                                .Skip(firstIdNumber).First().Id;
                    takerId = roPeople.All.OrderBy(p => p.Id)
                                .Skip(secondIdNumber).First().Id;
                    thirdId = roPeople.All.OrderBy(p => p.Id)
                                .Skip(thirdIdNumber).First().Id;

                    debtDealId = rwRoot.Debts.Deals.Add(new DebtDealRow {
                        Time = DateTime.Now,
                        GiverId = giverId, TakerId = takerId,
                        Amount = amount
                    });
                }

                using (var w = new RegistersWrapper(dbsProvider)) {
                    var rdbc = w.DB.Tables.DbContextForReader;
                    var roRoot = (IRegisterReader)w.New.Register();
                    
                    this.CheckPairDebtsRegister(
                        roRoot.Debts.Pairs, rdbc,
                        giverId, takerId, thirdId, amount);

                    this.CheckPersonDebtsRegister(
                        roRoot.Debts.Person, rdbc,
                        giverId, takerId, thirdId, amount);

                    this.CheckDebtDealsRegister(
                        roRoot.Debts.Deals, rdbc,
                        debtDealId,
                        giverId, takerId, thirdId, amount);

                    this.CheckAchieversRegister(
                        roRoot.Debts.Achievers, w.DB.Documents.AchieversCollection,
                        giverId, takerId, amount);
                }
            }
        }


        private void CheckPairDebtsRegister(
            IPairDebtsRegisterReader roPairs,
            TablesDbContextForReader rdbc,
            string giverId, string takerId, string thirdId,
            decimal amount
        ) {
            Assert.InRange(
                (roPairs.GetCurrentDebt(giverId, takerId) - amount) / amount,
                0, DebtConstants.ValueRelativeError);

            Assert.InRange(
                roPairs.GetCurrentDebt(takerId, giverId),
                0, DebtConstants.MaxZeroEquivalent);

            Assert.InRange(
                roPairs.GetCurrentDebt(giverId, thirdId),
                0, DebtConstants.MaxZeroEquivalent);

            Assert.Equal(1, rdbc.CurrentDebts.Count());
        }

        private void CheckPersonDebtsRegister(
            IPersonDebtsRegisterReader roPerson,
            TablesDbContextForReader rdbc,
            string giverId, string takerId, string thirdId,
            decimal amount
        ) {
            // totals
            PersonTotalsRow giverTotals = roPerson.GetTotals(giverId);
            Assert.Equal(0, giverTotals.DueDebtsTotal);
            Assert.Equal(0, giverTotals.HistoricalCountOfCreditsTaken);
            Assert.Equal(amount, giverTotals.HistoricallyCreditedInTotal);
            Assert.Equal(0, giverTotals.HistoricallyOwedInTotal);

            PersonTotalsRow takerTotals = roPerson.GetTotals(takerId);
            Assert.Equal(amount, takerTotals.DueDebtsTotal);
            Assert.Equal(1, takerTotals.HistoricalCountOfCreditsTaken);
            Assert.Equal(0, takerTotals.HistoricallyCreditedInTotal);
            Assert.Equal(amount, takerTotals.HistoricallyOwedInTotal);

            PersonTotalsRow thirdTotals = roPerson.GetTotals(thirdId);
            Assert.Equal(0, thirdTotals.DueDebtsTotal);
            Assert.Equal(0, thirdTotals.HistoricalCountOfCreditsTaken);
            Assert.Equal(0, thirdTotals.HistoricallyCreditedInTotal);
            Assert.Equal(0, thirdTotals.HistoricallyOwedInTotal);

            Assert.Equal(2, rdbc.CurrentTotalsPerPerson.Count());

            // statistics
            PersonStatisticsRow giverStatistics
                = roPerson.GetStatistics(giverId);
            Assert.Equal(0,
                   giverStatistics.HistoricalDebtAverageThroughCasesOfTaking);

            PersonStatisticsRow takerStatistics
                = roPerson.GetStatistics(takerId);
            Assert.Equal(amount,
                   takerStatistics.HistoricalDebtAverageThroughCasesOfTaking);

            PersonStatisticsRow thirdStatistics
                = roPerson.GetStatistics(thirdId);
            Assert.Equal(0,
                   thirdStatistics.HistoricalDebtAverageThroughCasesOfTaking);

            Assert.Equal(1, rdbc.CurrentStatisticsPerPerson.Count());

            // repaid fraction
            Assert.Null(roPerson.GetRepaidFraction(giverId));
            Assert.Equal(0, roPerson.GetRepaidFraction(takerId));
            Assert.Null(roPerson.GetRepaidFraction(thirdId));
        }

        private void CheckDebtDealsRegister(
            IDebtDealsRegisterReader roDebtDeals,
            TablesDbContextForReader rdbc,
            long debtDealId,
            string giverId, string takerId, string thirdId,
            decimal amount
        ) {
            // queryables
            Assert.Equal(1, roDebtDeals.All.Count());
            Assert.Equal(1, roDebtDeals.All.Count(dd => dd.Id == debtDealId));

            Assert.Equal(1, roDebtDeals.Credits.Count());
            Assert.Equal(1,
                roDebtDeals.Credits.Count(dd => dd.Id == debtDealId));

            Assert.Equal(0, roDebtDeals.Paybacks.Count());

            // is payback
            Assert.False(roDebtDeals.IsPayback(debtDealId));

            // count of deals
            Assert.Equal(1, rdbc.DebtDeals.Count());
        }

        private void CheckAchieversRegister(
            IAchieversRegisterReader roAchievers,
            IMongoCollection<AchieversDoc> mongoCollection,
            string giverId, string takerId, decimal amount
        ) {
            var byId = roAchievers.ByID;

            Assert.Contains(byId.WithMaxDueDebtsTotal, id => id == takerId);
            Assert.Equal(1, byId.WithMaxDueDebtsTotal.Count);

            Assert.Contains(
                byId.WhoHistoricallyCreditedMaxTotal, id => id == giverId);
            Assert.Equal(1, byId.WhoHistoricallyCreditedMaxTotal.Count);

            Assert.Equal(0, byId.BestDebtorByRepaidFractionThenTotal.Count);
        }
    }
}