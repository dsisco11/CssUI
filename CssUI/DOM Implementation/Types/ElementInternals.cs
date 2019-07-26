using CssUI.DOM.Exceptions;
using CssUI.DOM.Nodes;
using System;
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
        /// <param name="value">New value to assign to the element, must be <see cref="string"/>, <see cref="FileBlob"/>, or <see cref="FormData"/></param>
        /// <param name="state">New state to give the element, must be <see cref="string"/>, <see cref="FileBlob"/>, or <see cref="FormData"/></param>
        public void setFormValue(object value, object state = null)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#dom-elementinternals-setformvalue */

            if (!DOMCommon.Is_Form_Associated_Custom_Element(TargetElement))
            {
                throw new NotSupportedError($"Element internals may only function on custom form-associated elements");
            }

            if (!(value is string) && !(value is FileBlob) && !(value is FormData))
            {
                throw new ArgumentException($"The Value argument must be one of string, FileBlob, or FormData");
            }

            if (!(state is null) && !(state is string) && !(state is FileBlob) && !(state is FormData))
            {
                throw new ArgumentException($"The State argument (if specified) must be one of string, FileBlob, or FormData");
            }

            var element = (FormAssociatedElement)TargetElement;
            /* 3) Set target element's submission value to value if value is not a FormData object, or to a clone of the entry list associated with value otherwise. */
            if (value is FormData valueFormData)
            {
                element.submission_value = new FormData(valueFormData);
            }
            else
            {
                element.submission_value = value;
            }

            /* 4) If the state argument of the function is omitted, set element's state to its submission value. */
            if (state is null)
            {
                element.state = element.submission_value;
            }
            else if (state is FormData stateFormData)
            {
                element.state = new FormData(stateFormData);
            }
            else
            {
                element.state = state;
            }

        }

        /// <summary>
        /// Marks internals's target element as suffering from the constraints indicated by the flags argument, and sets the element's validation message to message. 
        /// If anchor is specified, the user agent might use it to indicate problems with the constraints of internals's target element when the form owner is validated interactively or reportValidity() is called.
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="message"></param>
        /// <param name="anchor"></param>
        public void setValidity(EValidityState flags, string message, HTMLElement anchor)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#dom-elementinternals-setvalidity */
            if (!DOMCommon.Is_Form_Associated_Custom_Element(TargetElement))
            {
                throw new NotSupportedError($"Element internals may only function on custom form-associated elements");
            }

            if (flags != 0 && (ReferenceEquals(null, message) || message.Length <= 0))
            {
                throw new TypeError("A message must be given when setting validity flags");
            }

            var element = (FormAssociatedElement)TargetElement;
            element.validity = flags;
            element.validationMessage = ReferenceEquals(null, message) ? string.Empty : message;

            if (0 != (flags & EValidityState.customError))
            {
                element.custom_validity_error_message = element.validationMessage;
            }
            else
            {
                element.custom_validity_error_message = string.Empty;
            }

            if (anchor == null)
            {
                element.validation_anchor = anchor;
            }
            else
            {
                if (!DOMCommon.Is_Shadow_Including_Descendant(anchor, TargetElement))
                {
                    throw new NotFoundError("validity anchor element must be a descendant of the element it is an anchor for");
                }

                element.validation_anchor = anchor;
            }
        }

        /// <summary>
        /// Returns true if internals's target element will be validated when the form is submitted; false otherwise.
        /// </summary>
        public bool willValidate
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#dom-elementinternals-willvalidate */
            get
            {
                if (!DOMCommon.Is_Form_Associated_Custom_Element(TargetElement))
                {
                    throw new NotSupportedError($"Element internals may only function on custom form-associated elements");
                }

                if (FormCommon.Is_Barred_From_Validation((FormAssociatedElement)TargetElement))
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Returns the ValidityState object for internals's target element.
        /// </summary>
        public EValidityState validity => ((FormAssociatedElement)TargetElement).validity;

        /// <summary>
        /// Returns the error message that would be shown to the user if internals's target element was to be checked for validity.
        /// </summary>
        public string validationMessage => ((FormAssociatedElement)TargetElement).validationMessage;

        /// <summary>
        /// Returns true if internals's target element has no validity problems; false otherwise. Fires an invalid event at the element in the latter case.
        /// </summary>
        /// <returns></returns>
        public bool checkValidity()
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#dom-cva-checkvalidity */
            if (!DOMCommon.Is_Form_Associated_Custom_Element(TargetElement))
            {
                throw new NotSupportedError($"Element internals may only function on custom form-associated elements");
            }

            var element = (FormAssociatedElement)TargetElement;
            return element.checkValidity();
        }

        /// <summary>
        /// Returns true if internals's target element has no validity problems; otherwise, returns false, fires an invalid event at the element, and (if the event isn't canceled) reports the problem to the user.
        /// </summary>
        /// <returns></returns>
        public bool reportValidity()
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#dom-elementinternals-reportvalidity */
            if (!DOMCommon.Is_Form_Associated_Custom_Element(TargetElement))
            {
                throw new NotSupportedError($"Element internals may only function on custom form-associated elements");
            }

            var element = (FormAssociatedElement)TargetElement;
            return element.reportValidity();
        }

        public IReadOnlyCollection<HTMLLabelElement> labels
        {/* Docs: https://html.spec.whatwg.org/multipage/forms.html#dom-elementinternals-labels */
            get
            {
                if (!DOMCommon.Is_Form_Associated_Custom_Element(TargetElement))
                {
                    throw new NotSupportedError($"Element internals may only function on custom form-associated elements");
                }

                return (IReadOnlyCollection<HTMLLabelElement>)DOMCommon.Get_Descendents(form, new FilterLabelFor(TargetElement), Enums.ENodeFilterMask.SHOW_ELEMENT);
            }
        }

    }
}
