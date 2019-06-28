
namespace CssUI.CSS
{
    public class CssPseudoElementSelector : CssSimpleSelector
    {
        protected readonly string Name;

        public CssPseudoElementSelector(string Name) : base(ECssSimpleSelectorType.PseudoElementSelector)
        {
            this.Name = Name;
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        override public bool Matches(cssElement E)
        {
            switch (Name)
            {
                default:
                    throw new CssSelectorException("Selector pseudo-element (", Name, ") logic not implemented!");
            }
        }
    }
}
