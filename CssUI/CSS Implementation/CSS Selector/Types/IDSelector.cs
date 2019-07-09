using CssUI.DOM;
using CssUI.DOM.Nodes;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Selectors
{
    public class IDSelector : SimpleSelector
    {
        readonly string MatchID;

        public IDSelector(string MatchID) : base(ESimpleSelectorType.IDSelector)
        {
            this.MatchID = MatchID;
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        override public bool Matches(Element E, params Node[] scopeElements)
        {
            return string.Compare(E.id.ToLowerInvariant(), MatchID) == 0;
        }
    }
}
