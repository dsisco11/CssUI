
namespace CssUI.CSS
{

    /// <summary>
    /// A universal selector matches any element in any namespace
    /// <para>Universal-selectors MUST be seperate from the Type-selector class because they are ignores when calculating the selectors specificity!</para>
    /// </summary>
    public class CssUniversalSelector : CssSimpleSelector
    {
        public CssUniversalSelector() : base(ECssSimpleSelectorType.UniversalSelector)
        {
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        override public bool Matches(cssElement E)
        {
            return true;
        }
    }
}
