using CssUI.CSS.Enums;
using CssUI.DOM;

namespace CssUI.CSS.Selectors
{
    /// <summary>
    /// A simple selector is either a type selector, universal selector, attribute selector, class selector, ID selector, or pseudo-class.
    /// </summary>
    public abstract class SimpleSelector
    {
        public ECssSimpleSelectorType Type { get; protected set; }

        public SimpleSelector(ECssSimpleSelectorType Type)
        {
            this.Type = Type;
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        abstract public bool Matches(Element E);
    }
}
