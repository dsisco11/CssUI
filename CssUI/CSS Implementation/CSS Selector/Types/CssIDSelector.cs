
namespace CssUI.CSS
{
    public class CssIDSelector : CssSimpleSelector
    {
        readonly string MatchID;

        public CssIDSelector(string MatchID) : base(ECssSimpleSelectorType.IDSelector)
        {
            this.MatchID = MatchID;
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        override public bool Matches(cssElement E)
        {
            return string.Compare(E.ID.ToLowerInvariant(), MatchID) == 0;
        }
    }
}
