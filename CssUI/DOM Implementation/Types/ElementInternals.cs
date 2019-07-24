using CssUI.DOM.Nodes;
using System.Collections.Generic;

namespace CssUI.DOM
{
    public class ElementInternals
    {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#elementinternals */

        #region Properties
        public readonly HTMLElement TargetElement = null;
        public HTMLFormElement form { get; internal set; }
        #endregion

        #region Constructor
        public ElementInternals(HTMLElement targetElement)
        {
            TargetElement = targetElement;
        }
        #endregion


        /// <summary>
        /// Sets both the state and submission value of internals's target element to value.
        /// If value is null, the element won't participate in form submission.
        /// </summary>
        /// <param name="USVString"></param>
        /// <param name="FormData"></param>
        public void setFormValue((File or USVString or FormData)? value, optional(File or USVString or FormData)? state)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#dom-elementinternals-setformvalue */
        }


        public void setValidity(EValidityState flags, string message, HTMLElement anchor);
        public readonly bool willValidate;
        public readonly EValidityState validity;
        public readonly string validationMessage;
        public bool checkValidity();
        public bool reportValidity();

        public readonly IReadOnlyCollection<Node> labels;

    }
}
