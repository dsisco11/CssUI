
namespace CssUI.CSS
{
    public sealed class PercentageToken : ValuedTokenBase
    {
        /// <summary>
        /// Holds the numeric representation of this token value
        /// </summary>
        public readonly double Number;

        public PercentageToken(string Value, double Number) : base(ECssTokenType.Percentage, Value)
        {
            this.Number = Number;
        }

        /// <summary>
        /// Encodes the token back to it's CSS representation
        /// </summary>
        /// <returns>Css string</returns>
        public override string Encode()
        {
            return string.Concat(Value, "%");
        }
    }
}
