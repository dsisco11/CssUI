using CssUI.DOM;
using CssUI.DOM.Nodes;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Selectors
{

    public class ClassSelector : SimpleSelector
    {
        readonly string ClassName;

        public ClassSelector(string ClassName) : base(ESimpleSelectorType.ClassSelector)
        {
            this.ClassName = ClassName;
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        override public bool Matches(Element E, params Node[] scopeElements)
        {
            return E.classList.Contains(ClassName);
        }
    }
}
