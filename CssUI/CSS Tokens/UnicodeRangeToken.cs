
namespace CssUI.CSS
{
    public sealed class UnicodeRangeToken : CssToken
    {
        /// <summary>
        /// Starting point of the range
        /// </summary>
        public readonly int Start;
        /// <summary>
        /// Ending point of the range
        /// </summary>
        public readonly int End;

        public UnicodeRangeToken(int Start, int End) : base(ECssTokenType.Unicode_Range)
        {
            this.Start = Start;
            this.End = End;
        }

        public override string Encode()
        {
            string StartRange = Start.ToString("X2");
            string EndRange = End.ToString("X2");
            return string.Concat("U+", StartRange, "-", "U+", EndRange);
        }

        #region Equality Operators
        public static bool operator ==(UnicodeRangeToken A, UnicodeRangeToken B)
        {
            return (A.Type == B.Type && A.Start == B.Start && A.End == B.End);
        }

        public static bool operator !=(UnicodeRangeToken A, UnicodeRangeToken B)
        {
            return (A.Type != B.Type && A.Start != B.Start && A.End != B.End);
        }

        public override bool Equals(object o)
        {

            if (o is UnicodeRangeToken)
            {
                return this == (UnicodeRangeToken)o;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Start ^ End;
        }

        #endregion
    }
}
