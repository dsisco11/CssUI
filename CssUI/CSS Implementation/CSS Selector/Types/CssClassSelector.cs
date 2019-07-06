
namespace CssUI.CSS
{

    public class CssClassSelector : CssSimpleSelector
    {
        readonly string ClassName;

        public CssClassSelector(string ClassName) : base(ECssSimpleSelectorType.ClassSelector)
        {
            this.ClassName = ClassName;
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        override public bool Matches(cssElement E)
        {
            return E.Has_Class(ClassName);
        }
    }
}
