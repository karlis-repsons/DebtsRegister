using System.ComponentModel.DataAnnotations;

namespace DebtsRegister.Core
{
    public class PersonRow
    {
        [Timestamp]
        public byte[] Timestamp { get; set; }

        [MaxLength(TablesDbConstants.MaxPersonIDLength)]
        public string Id { get; set; }

        [Required, MaxLength(TablesDbConstants.MaxPersonNamepartLength)]
        public string Name { get; set; }

        [Required, MaxLength(TablesDbConstants.MaxPersonNamepartLength)]
        public string Surname { get; set; }
    }
}