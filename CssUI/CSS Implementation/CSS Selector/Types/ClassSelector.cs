using CssUI.CSS.Enums;
using CssUI.DOM;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Selectors
{

    public class ClassSelector : SimpleSelector
    {
        readonly string ClassName;

        public ClassSelector(string ClassName) : base(ECssSimpleSelectorType.ClassSelector)
        {
            this.ClassName = ClassName;
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        override public bool Matches(Element E)
        {
            return E.classList.Contains(ClassName);
        }
    }
}
