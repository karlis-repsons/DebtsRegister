using System;
using System.Linq;
using Xunit;

namespace DebtsRegister.Tests.Core.Register
{
    using DebtsRegister.Core;

    public class EmptyRegisterAccessTests
    {
        [Fact]
        public void HasNoDataAtAll() {
            using (var wrapper = new RegistersWrapper()) {
                IRegister register = wrapper.New.Register();
                var rdbc = wrapper.DB.Tables.DbContextForReader;

                Assert.True(rdbc.People.Count() == 0);
                Assert.True(rdbc.DebtDeals.Count() == 0);
                Assert.True(rdbc.DebtDealsAnalysis.Count() == 0);
                Assert.True(rdbc.CurrentDebts.Count() == 0);
                Assert.True(rdbc.CurrentTotalsPerPerson.Count() == 0);
                Assert.True(rdbc.CurrentStatisticsPerPerson.Count() == 0);
                Assert.True(rdbc.CurrentPeopleStatisticsDocuments.Count() == 0);

                Assert.True(register.HasNoData);
                Assert.True(((IRegisterReader)register).HasNoData);

                // achievers register
                IAchieversRegisterReader achieversReader
                    = ((IRegisterReader)register).Debts.Achievers;
                Assert.Empty(achieversReader.ByID.WithMaxDueDebtsTotal);
                Assert.Empty(achieversReader.ByID.WhoHistoricallyCreditedMaxTotal);
                Assert.Empty(achieversReader.ByID.BestDebtorByRepaidFractionThenTotal);
            }
        }

        [Fact]
        public void HasPeopleButHasNoDeals() {
            using (var wrapper = new RegistersWrapper()) {
                IRegister register = wrapper.New.Register();
                var rdbc = wrapper.DB.Tables.DbContextForReader;
                var populator
                    = new TablesDbPopulator(wrapper.DB.Tables.DbContext);

                long count = new Random().Next(5, 20);
                populator.AddPeople(count);

                Assert.True(rdbc.People.Count() == count);
                Assert.False(register.HasNoData);
                Assert.False(((IRegisterReader)register).HasNoData);
            }
        }
    }
}