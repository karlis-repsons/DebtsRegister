namespace DebtsRegister.Core
{
    public class DebtConstants
    {
        public const decimal MaxZeroEquivalent = 1e-28m;

        public const decimal ValueRelativeError = 1e-28m;

        /// <summary>
        /// The smallest supported p - 1 value, where p > 1.
        /// </summary>
        public const decimal ValueEpsilon = 2e-28m;
    }
}