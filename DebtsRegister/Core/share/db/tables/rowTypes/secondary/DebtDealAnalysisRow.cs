using System.ComponentModel.DataAnnotations;

namespace DebtsRegister.Core
{
    public class DebtDealAnalysisRow
    {
        [Key]
        public long DebtDealId { get; set; }

        [Required]
        public bool IsPayback { get; set; }
    }
}