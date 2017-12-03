using System.ComponentModel.DataAnnotations;

namespace DebtsRegister.Core
{
    public class PersonStatisticsRow
    {
        [Timestamp]
        public byte[] Timestamp { get; set; }

        [Key, MaxLength(TablesDbConstants.MaxPersonIDLength)]
        public string PersonId { get; set; }

        public decimal HistoricalDebtAverageThroughCasesOfTaking { get; set; }
    }
}