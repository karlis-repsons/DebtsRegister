using System.ComponentModel.DataAnnotations;

namespace DebtsRegister.Core
{
    public class PeopleStatisticsDocumentRow
    {
        [Timestamp]
        public byte[] Timestamp { get; set; }

        [Key, MaxLength(SharedDbConstants.MaxDocumentNameLength)]
        public string DocumentName { get; set; }

        [MaxLength(SharedDbConstants.MaxDocumentIdLength)]
        public string DocumentId { get; set; }
    }
}