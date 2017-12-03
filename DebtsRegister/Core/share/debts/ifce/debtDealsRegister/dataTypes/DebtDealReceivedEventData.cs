namespace DebtsRegister.Core
{
    public class DebtDealReceivedEventData
    {
        public DebtDealRow Deal { get; set; }

        public DebtDealAnalysisRow Analysis { get; set; }

        public decimal? RepayGiftAmount { get; set; }
    }
}