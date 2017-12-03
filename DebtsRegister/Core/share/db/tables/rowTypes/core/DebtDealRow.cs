using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DebtsRegister.Core
{
    public class DebtDealRow
    {
        public long Id { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [MaxLength(TablesDbConstants.MaxPersonIDLength)]
        public string GiverId { get; set; }

        [MaxLength(TablesDbConstants.MaxPersonIDLength)]
        public string TakerId { get; set; }

        public decimal Amount { get; set; }
    }
}