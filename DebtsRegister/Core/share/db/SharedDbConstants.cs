namespace DebtsRegister.Core
{
    public class SharedDbConstants
    {
        public const byte MaxDocumentNameLength = 45;

        /// <summary>
        /// Equal to length of hex string
        /// made from 12 byte MongoDB document ID.
        /// </summary>
        public const byte MaxDocumentIdLength = 24;
    }
}