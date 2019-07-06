
namespace CssUI.CSS
{
    /// <summary>
    /// A simple selector is either a type selector, universal selector, attribute selector, class selector, ID selector, or pseudo-class.
    /// </summary>
    public abstract class CssSimpleSelector
    {
        public ECssSimpleSelectorType Type { get; protected set; }

        public CssSimpleSelector(ECssSimpleSelectorType Type)
        {
            this.Type = Type;
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        abstract public bool Matches(cssElement E);
    }
}
