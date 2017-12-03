namespace DebtsRegister.Core
{
    public class Register : IRegister
    {
        public Register(
            IPeopleRegister peopleRegister,
            IPeopleRegisterReader peopleRegisterReader,
            IDebtsRegister debtsRegister,
            IDebtsRegisterReader debtsRegisterReader
        ) {
            this.People = peopleRegister;
            this.peopleRegisterReader = peopleRegisterReader;
            this.Debts = debtsRegister;
            this.debtsRegisterReader = debtsRegisterReader;
        }

        public bool HasNoData
            =>    this.peopleRegisterReader.HasNoData
               && this.debtsRegisterReader.HasNoData;

        public IPeopleRegister People { get; }

        public IDebtsRegister Debts { get; }

        IPeopleRegisterReader IRegisterReader.People
            => this.peopleRegisterReader;

        IDebtsRegisterReader IRegisterReader.Debts
            => this.debtsRegisterReader;


        private readonly IPeopleRegisterReader peopleRegisterReader;
        private readonly IDebtsRegisterReader debtsRegisterReader;
    }
}