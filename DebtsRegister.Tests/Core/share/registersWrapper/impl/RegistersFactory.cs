using MongoDB.Driver;

using DebtsRegister.Core;
using DebtsRegisterImpl = DebtsRegister.Core.DebtsRegister;
using RegisterImpl = DebtsRegister.Core.Register;

namespace DebtsRegister.Tests.Core
{
    public class RegistersFactory
    {
        public RegistersFactory(
            TablesDbContext dbc, TablesDbContextForReader rdbc,
            IMongoCollection<AchieversDoc> achieversDocCollection
        ) {
            this.dbc = dbc;
            this.rdbc = rdbc;
            this.achieversCollection = achieversDocCollection;
        }

        public IRegister Register() {
            var peopleRegister = new PeopleRegister(dbc, rdbc);

            var debtDealsRegister = new DebtDealsRegister(dbc, rdbc);
            var personDebtsRegister = new PersonDebtsRegister(dbc, rdbc, debtDealsRegister);
            var pairDebtsRegister = new PairDebtsRegister(dbc, rdbc, debtDealsRegister);
            var achieversRegister = new AchieversRegister(dbc, rdbc, achieversCollection, debtDealsRegister, personDebtsRegister);

            var debtsRegister = new DebtsRegisterImpl(dbc, rdbc,
                personDebtsRegister, personDebtsRegister,
                pairDebtsRegister, pairDebtsRegister,
                debtDealsRegister, debtDealsRegister,
                achieversRegister, achieversRegister);

            return new RegisterImpl(
                    peopleRegister, peopleRegister,
                    debtsRegister, debtsRegister);
        }

        public IPeopleRegister PeopleRegister() => this.Register().People;

        public IDebtsRegister DebtsRegister() => this.Register().Debts;

        public IDebtDealsRegister DebtDealsRegister()
            => this.DebtsRegister().Deals;

        public IPersonDebtsRegister PersonDebtsRegister()
            => this.DebtsRegister().Person;

        public IPairDebtsRegister PairDebtsRegister()
            => this.DebtsRegister().Pairs;

        public IAchieversRegister AchieversRegister()
            => this.DebtsRegister().Achievers;


        private readonly TablesDbContext dbc;
        private readonly TablesDbContextForReader rdbc;
        private readonly IMongoCollection<AchieversDoc> achieversCollection;
    }
}