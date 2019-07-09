using CssUI.DOM;
using CssUI.DOM.Nodes;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Selectors
{

    /// <summary>
    /// A universal selector matches any element in any namespace
    /// <para>Universal-selectors MUST be seperate from the Type-selector class because they are ignores when calculating the selectors specificity!</para>
    /// </summary>
    public class UniversalSelector : SimpleSelector
    {
        public UniversalSelector() : base(ESimpleSelectorType.UniversalSelector)
        {
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        override public bool Matches(Element E, params Node[] scopeElements)
        {
            return true;
        }
    }
}
