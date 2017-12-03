using System.ComponentModel.DataAnnotations;

namespace DebtsRegister.Core
{
    public class DebtRow
    {
        [Timestamp]
        public byte[] Timestamp { get; set; }

        [MaxLength(TablesDbConstants.MaxPersonIDLength)]
        public string CreditorId { get; set; }

        [MaxLength(TablesDbConstants.MaxPersonIDLength)]
        public string DebtorId { get; set; }

        public decimal DebtTotal { get; set; }
    }
}