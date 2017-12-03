using System.Linq;

namespace DebtsRegister.Core
{
    public class DebtsRegister : IDebtsRegister
    {
        public DebtsRegister(
            TablesDbContext dbc,
            TablesDbContextForReader rdbc,
            IPersonDebtsRegister personDebtsRegister,
            IPersonDebtsRegisterReader personDebtsRegisterReader,
            IPairDebtsRegister pairDebtsRegister,
            IPairDebtsRegisterReader pairDebtsRegisterReader,
            IDebtDealsRegister debtDealsRegister,
            IDebtDealsRegisterReader debtDealsRegisterReader,
            IAchieversRegister achieversRegister,
            IAchieversRegisterReader achieversRegisterReader
        ) {
            this.dbc = dbc;
            this.rdbc = rdbc;

            this.Person = personDebtsRegister;
            this.personDebtsRegisterReader = personDebtsRegisterReader;

            this.Pairs = pairDebtsRegister;
            this.pairDebtsRegisterReader = pairDebtsRegisterReader;

            this.Deals = debtDealsRegister;
            this.debtDealsRegisterReader = debtDealsRegisterReader;

            this.Achievers = achieversRegister;
            this.achieversRegisterReader = achieversRegisterReader;
        }

        public bool HasNoData => this.rdbc.DebtDeals.Any() == false;

        public IPersonDebtsRegister Person { get; }
        IPersonDebtsRegisterReader IDebtsRegisterReader.Person
            => this.personDebtsRegisterReader;

        public IPairDebtsRegister Pairs { get; }
        IPairDebtsRegisterReader IDebtsRegisterReader.Pairs
            => this.pairDebtsRegisterReader;

        public IDebtDealsRegister Deals { get; }
        IDebtDealsRegisterReader IDebtsRegisterReader.Deals
            => this.debtDealsRegisterReader;

        public IAchieversRegister Achievers { get; }
        IAchieversRegisterReader IDebtsRegisterReader.Achievers
            => this.achieversRegisterReader;


        private readonly TablesDbContext dbc;
        private readonly TablesDbContextForReader rdbc;
        private readonly IPersonDebtsRegisterReader personDebtsRegisterReader;
        private readonly IPairDebtsRegisterReader pairDebtsRegisterReader;
        private readonly IDebtDealsRegisterReader debtDealsRegisterReader;
        private readonly IAchieversRegisterReader achieversRegisterReader;
    }
}