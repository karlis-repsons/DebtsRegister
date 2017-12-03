using System.ComponentModel.DataAnnotations;

namespace DebtsRegister.Core
{
    public class PersonTotalsRow
    {
        [Timestamp]
        public byte[] Timestamp { get; set; }

        [Key, MaxLength(TablesDbConstants.MaxPersonIDLength)]
        public string PersonId { get; set; }

        public decimal DueDebtsTotal { get; set; }

        public decimal HistoricallyCreditedInTotal { get; set; }

        public decimal HistoricallyOwedInTotal { get; set; }

        public int HistoricalCountOfCreditsTaken { get; set; }
    }
}