using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using MongoDB.Driver;
using MongoDB.Bson;

using DebtsRegister.Core;

namespace DebtsRegister.Tests.Core.Register.Debts.DealSequences.Basic
{
    public class PaybackTest
    {
        public PaybackTest(ITestOutputHelper output) {
            this.output = output;
        }

        [Fact]
        public void FinalStateMatches() {
            using (var dbsProvider = new TestDBsProvider()) {
                for (byte c = 0; c < 30; c++) {
                    TakeCreditAndPayBack(dbsProvider, countOfPaybacks: 1);
                    dbsProvider.DeleteAllContent();

                    TakeCreditAndPayBack(dbsProvider, countOfPaybacks: 2);
                    dbsProvider.DeleteAllContent();

                    TakeCreditAndPayBack(dbsProvider,
                        countOfPaybacks: (uint)new Random().Next(3, 70));
                    dbsProvider.DeleteAllContent();
                }
            }
        }


        private void TakeCreditAndPayBack(
            TestDBsProvider dbsProvider, uint countOfPaybacks
        ) {
            const int peopleCount = 10,
                      firstIdNumber = 0,
                      secondIdNumber = 3,
                      thirdIdNumber = 7;
            const decimal maxAmout = 1e6m,
                          minAmount = 1;
            decimal totalAmount
                = (decimal)(new Random().NextDouble())
                * (maxAmout - minAmount)
                + minAmount;

            string logIndent = "   ";
            this.output.WriteLine(
                $"TakeCreditAndPayBack: totalAmount = {totalAmount}, "
              + $"countOfPaybacks = {countOfPaybacks}:");

            using (var w = new RegistersWrapper(dbsProvider)) {
                var dbc = w.DB.Tables.DbContext;

                new TablesDbPopulator(dbc).AddPeople(peopleCount);

                var rdbc = w.DB.Tables.DbContextForReader;
                IRegister rwRoot = w.New.Register();
                var roRoot = (IRegisterReader)rwRoot;

                IPeopleRegisterReader roPeople = roRoot.People;
                string giverId = roPeople.All.OrderBy(p => p.Id)
                                    .Skip(firstIdNumber).First().Id;
                string takerId = roPeople.All.OrderBy(p => p.Id)
                                    .Skip(secondIdNumber).First().Id;
                string thirdId = roPeople.All.OrderBy(p => p.Id)
                                    .Skip(thirdIdNumber).First().Id;

                (long creditDebtDealId, long[] paybackDebtDealIds)
                    = this.MakeDeals(
                        rwRoot, rdbc,
                        giverId, takerId, thirdId,
                        totalAmount, countOfPaybacks, logIndent);

                this.CheckPairDebtsRegister(
                    roRoot.Debts.Pairs, rdbc, giverId, takerId, thirdId);

                this.CheckPersonDebtsRegister(
                    roRoot.Debts.Person, rdbc,
                    giverId, takerId, thirdId, totalAmount);

                this.CheckDebtDealsRegister(
                    roRoot.Debts.Deals, rdbc,
                    creditDebtDealId, paybackDebtDealIds,
                    giverId, takerId, thirdId,
                    totalAmount, countOfPaybacks);

                this.CheckAchieversRegister(
                    roRoot.Debts.Achievers,
                    rdbc, w.DB.Documents.AchieversCollection,
                    giverId, takerId, totalAmount);
            }

            this.output.WriteLine("");
        }

        private
            (long creditDebtDealId, long[] paybackDebtDealIds)

            MakeDeals
            (
                IRegister rwRoot,
                TablesDbContextForReader rdbc,
                string giverId, string takerId, string thirdId,
                decimal totalAmount, uint countOfPaybacks,
                string logIndent
        ) {
            IDebtDealsRegister rwDebtDeals = rwRoot.Debts.Deals;
            long creditDebtDealId = rwDebtDeals.Add(new DebtDealRow {
                Time = DateTime.Now,
                GiverId = giverId, TakerId = takerId,
                Amount = totalAmount
            });

            decimal amountPaidBack = 0;
            decimal accruedRepayError = 0;
            var paybackDebtDealIds = new List<long>();
            for (uint n = 0; n < countOfPaybacks; n++) {
                uint nPaybacksLeft = countOfPaybacks - n;
                decimal dueAmount = totalAmount - amountPaidBack;
                if (dueAmount < DebtConstants.ValueEpsilon)
                    throw new InvalidOperationException(
                        $"got too small dueAmount: {dueAmount}");

                string logIndent2 = logIndent + logIndent;
                this.output.WriteLine($"{logIndent}n = {n}:");
                this.output.WriteLine($"{logIndent2}dueAmount = {dueAmount}");

                decimal minPaybackLimit
                    = Math.Max(
                        dueAmount / nPaybacksLeft / 10
                      , DebtConstants.ValueEpsilon * nPaybacksLeft);
                decimal maxPaybackLimit
                    = dueAmount
                    - minPaybackLimit * (countOfPaybacks - n);

                decimal paybackAmount
                    = (decimal)(new Random().NextDouble())
                    * (maxPaybackLimit - minPaybackLimit)
                    + minPaybackLimit;

                this.output.WriteLine(
                    $"{logIndent2}paybackAmount = {paybackAmount}");

                if (n == countOfPaybacks - 1) {
                    paybackAmount = rwRoot.Debts.Pairs
                        .GetCurrentDebt(giverId, takerId);

                    this.output.WriteLine(
                          $"{logIndent2}corrected last paybackAmount = "
                        + paybackAmount);
                }

                long paybackDebtDealId = rwDebtDeals.Add(new DebtDealRow {
                    Time = DateTime.Now,
                    GiverId = takerId, TakerId = giverId,
                    Amount = paybackAmount
                });
                paybackDebtDealIds.Add(paybackDebtDealId);
                amountPaidBack += paybackAmount;
                accruedRepayError
                        += DebtConstants.ValueRelativeError * amountPaidBack;
            }

            Assert.InRange(
                        Math.Abs(amountPaidBack - totalAmount),
                        0, accruedRepayError);

            return (creditDebtDealId, paybackDebtDealIds.ToArray());
        }

        private void CheckPairDebtsRegister(
            IPairDebtsRegisterReader roPairs,
            TablesDbContextForReader rdbc,
            string giverId, string takerId, string thirdId
        ) {
            decimal gtd = roPairs.GetCurrentDebt(giverId, takerId);
            Assert.Equal(0, gtd);

            decimal tgd = roPairs.GetCurrentDebt(takerId, giverId);
            Assert.Equal(0, tgd);

            decimal gth = roPairs.GetCurrentDebt(giverId, thirdId);
            Assert.Equal(0, gth);

            Assert.Equal(0, rdbc.CurrentDebts.Count());
        }

        private void CheckPersonDebtsRegister(
            IPersonDebtsRegisterReader roPerson,
            TablesDbContextForReader rdbc,
            string giverId, string takerId, string thirdId,
            decimal total
        ) {
            // totals
            PersonTotalsRow giverTotals = roPerson.GetTotals(giverId);
            Assert.Equal(0, giverTotals.DueDebtsTotal);
            Assert.Equal(0, giverTotals.HistoricalCountOfCreditsTaken);
            Assert.Equal(total, giverTotals.HistoricallyCreditedInTotal);
            Assert.Equal(0, giverTotals.HistoricallyOwedInTotal);

            PersonTotalsRow takerTotals = roPerson.GetTotals(takerId);
            Assert.Equal(0, takerTotals.DueDebtsTotal);
            Assert.Equal(1, takerTotals.HistoricalCountOfCreditsTaken);
            Assert.Equal(0, takerTotals.HistoricallyCreditedInTotal);
            Assert.Equal(total, takerTotals.HistoricallyOwedInTotal);

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
            Assert.Equal(total, 
                  takerStatistics.HistoricalDebtAverageThroughCasesOfTaking);

            PersonStatisticsRow thirdStatistics
                = roPerson.GetStatistics(thirdId);
            Assert.Equal(0,
                thirdStatistics.HistoricalDebtAverageThroughCasesOfTaking);

            Assert.Equal(1, rdbc.CurrentStatisticsPerPerson.Count());

            // repaid fraction
            Assert.True(roPerson.GetRepaidFraction(giverId) == null);
            Assert.Equal(1, roPerson.GetRepaidFraction(takerId));
            Assert.True(roPerson.GetRepaidFraction(thirdId) == null);
        }

        private void CheckDebtDealsRegister(
            IDebtDealsRegisterReader roDebtDeals,
            TablesDbContextForReader rdbc,
            long creditDebtDealId, long[] paybackDebtDealIds,
            string giverId, string takerId, string thirdId,
            decimal total, uint countOfPaybacks
        ) {
            // queryables
            Assert.Equal(total,
                roDebtDeals.All
                    .Where(dd => dd.Id == creditDebtDealId)
                    .First().Amount);

            Assert.Equal(1 + (int)countOfPaybacks, roDebtDeals.All.Count());
            Assert.Equal(1, roDebtDeals.Credits.Count());
            Assert.Equal((int)countOfPaybacks, roDebtDeals.Paybacks.Count());

            // is payback
            Assert.False(roDebtDeals.IsPayback(creditDebtDealId));
            foreach (long id in paybackDebtDealIds)
                Assert.True(roDebtDeals.IsPayback(id));

            // count of deals
            Assert.Equal(1 + (int)countOfPaybacks, rdbc.DebtDeals.Count());
        }

        private void CheckAchieversRegister(
            IAchieversRegisterReader roAchievers,
            TablesDbContextForReader rdbc,
            IMongoCollection<AchieversDoc> mongoCollection,
            string giverId, string takerId, decimal total
        ) {
            var byId = roAchievers.ByID;

            string achieversDocIdString
                = rdbc.CurrentPeopleStatisticsDocuments
                    .Where(csd => csd.DocumentName == AchieversDoc.Name)
                    .First().DocumentId;
            Assert.False(string.IsNullOrEmpty(achieversDocIdString));

            ObjectId achieversDocId = new ObjectId(achieversDocIdString);
            AchieversDoc achieversDoc
                = mongoCollection.AsQueryable()
                    .Where(doc => doc.Id == achieversDocId)
                    .First();

            Action<IReadOnlyCollection<string>> maxDebtorChecker
                = set => {
                    if (set != null)
                        Assert.Equal(0, set.Count);
                };

            maxDebtorChecker(byId.WithMaxDueDebtsTotal);
            maxDebtorChecker(achieversDoc.MaxDueDebtsTotalPersonIds);

            Action<IReadOnlyCollection<string>> maxCreditorChecker
                = set => {
                    Assert.Contains(set, id => id == giverId);
                    Assert.Equal(1, set.Count);
                };

            maxCreditorChecker(byId.WhoHistoricallyCreditedMaxTotal);
            maxCreditorChecker(
                achieversDoc.HistoricallyCreditedMaxTotalPersonIds);

            Action<IReadOnlyCollection<string>> bestDebtorChecker
                = set => {
                    Assert.Contains(set, id => id == takerId);
                    Assert.Equal(1, set.Count);
                };

            bestDebtorChecker(byId.BestDebtorByRepaidFractionThenTotal);
            bestDebtorChecker(
                achieversDoc.BestDebtorIdsByRepaidFractionThenTotal);
        }


        private readonly ITestOutputHelper output;
    }
}