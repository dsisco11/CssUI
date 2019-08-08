using CssUI.DOM;
using CssUI.DOM.Events;
using System.Text;

namespace CssUI.HTML
{
    public abstract class FormAssociatedElement : HTMLElement
    {
        #region Backing Values
        HTMLFormElement _form = null;
        EValidityState _validity = 0x0;
        #endregion

        #region Internal Properties
        internal bool bParserInserted { get; set; }

        /// <summary>
        /// This is used to allow <see cref="ElementInternals"/> to set their elements validationMessage
        /// </summary>
        internal string custom_validity_message { get; set; } = null;

        /// <summary>
        /// The custom validity error message will be used when alerting the user to the problem with the control.
        /// </summary>
        internal string custom_validity_error_message { get; set; } = null;

        /// <summary>
        /// Each form-associated custom element has a validation anchor element. It is null initially.
        /// </summary>
        internal Element validation_anchor { get; set; }

        /// <summary>
        /// Each form-associated custom element has a state. 
        /// It is information with which the user agent can restore a user's input for the element. 
        /// The initial value of state is null, and state can be null, a string, a File, or a list of entries.
        /// </summary>
        internal FormSubmissionValue state { get; set; }

        /// <summary>
        /// Each form-associated custom element has submission value. 
        /// It is used to provide one or more entries on form submission, and The initial value of submission value is null, and submission value can be null, a string, a File, or a list of entries.
        /// </summary>
        internal FormSubmissionValue submission_value { get; set; }
        #endregion

        #region Properties
        public virtual HTMLFormElement form
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#form-owner */
            get
            {
                /* If a form has been explicitly set then return it */
                if (_form != null)
                {
                    return _form;
                }

                /* if the 'form' content attribute has been specified then find the form by the id given by the attribute */
                AttributeValue attr = getAttribute(EAttributeName.Form);
                if (attr != null)
                {
                    Element formElement = ownerDocument?.getElementByID(attr.Get_Atomic());
                    if (!(formElement is HTMLFormElement))
                        return null;

                    return (HTMLFormElement)formElement;
                }

                return null;
            }

            set
            {
                _form = value;
            }
        }
        public virtual string value { get; set; }
        public virtual string type { get; set; } = null;
        public EValidityState validity
        {
            get => _validity | query_validity();
            internal set => _validity = value;
        }
        #endregion

        #region Constructors
        public FormAssociatedElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Accessors
        public virtual bool isMutable => false;
        public bool willValidate => !FormCommon.Is_Barred_From_Validation(this);
        internal string validationMessage
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#dom-cva-validationmessage */
            get
            {
                if (FormCommon.Is_Barred_From_Validation(this) || 0x0 == validity)
                {
                    return string.Empty;
                }

                /* 2) Return a suitably localized message that the user agent would show the user if this were the only form control with a validity constraint problem. 
                 * If the user agent would not actually show a textual message in such a situation (e.g., it would show a graphical cue instead), 
                 * then return a suitably localized message that expresses (one or more of) the validity constraint(s) that the control does not satisfy. 
                 * If the element is a candidate for constraint validation and is suffering from a custom error, then the custom validity error message should be present in the return value. */

                StringBuilder sb = new StringBuilder();
                EValidityState flags = validity;

                if (!ReferenceEquals(null, custom_validity_message) && custom_validity_message.Length > 0)
                {
                    sb.AppendLine(custom_validity_message);
                }

                if (0 != (validity & EValidityState.customError))
                {
                    sb.AppendLine(custom_validity_error_message);
                }

                if (0 != (validity & EValidityState.valueMissing))
                {
                    sb.AppendLine($"A value is required for this field.");
                }

                if (0 != (validity & EValidityState.tooLong))
                {
                    sb.AppendLine($"Value exceeds maximum length.");
                }

                if (0 != (validity & EValidityState.tooShort))
                {
                    sb.AppendLine($"Value is under minimum length.");
                }

                if (0 != (validity & EValidityState.typeMismatch))
                {
                    sb.AppendLine($"Value is not in the correct format for this element.");
                }

                if (0 != (validity & EValidityState.stepMismatch))
                {
                    sb.AppendLine($"Value does not adhere to the step rules for this element.");
                }

                if (0 != (validity & EValidityState.patternMismatch))
                {
                    sb.AppendLine($"Value does not adhere to the pattern rules for this element.");
                }

                if (0 != (validity & EValidityState.rangeUnderflow))
                {
                    sb.AppendLine($"Value exceeds the max range for this element.");
                }

                if (0 != (validity & EValidityState.rangeOverflow))
                {
                    sb.AppendLine($"Value is below the minimum range for this element.");
                }

                if (0 != (validity & EValidityState.badInput))
                {
                    sb.AppendLine($"Value is incomplete.");
                }

                return sb.ToString();
            }

            set
            {
                custom_validity_message = value;
            }
        }
        #endregion

        /// <summary>
        /// Returns true if the element's value has no validity problems; false otherwise. Fires an invalid event at the element in the latter case.
        /// </summary>
        /// <returns></returns>
        public virtual bool checkValidity()
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#dom-cva-checkvalidity */
            if (!FormCommon.Is_Barred_From_Validation(this) && !check_satisfies_constraints())
            {
                dispatchEvent(new Event(EEventName.Invalid, new EventInit() { cancelable = true }));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if the element's value has no validity problems; otherwise, returns false, fires an invalid event at the element, and (if the event isn't canceled) reports the problem to the user.
        /// </summary>
        /// <returns></returns>
        public virtual bool reportValidity()
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#report-validity-steps */
            if (!FormCommon.Is_Barred_From_Validation(this) && !check_satisfies_constraints())
            {
                /* 1) Let report be the result of firing an event named invalid at element, with the cancelable attribute initialized to true. */

                var report = dispatchEvent(new Event(EEventName.Invalid, new EventInit() { cancelable = true }));
                /* 2) If report is true, then report the problems with the constraints of this element to the user. 
                 * When reporting the problem with the constraints to the user, the user agent may run the focusing steps for element, 
                 * and may change the scrolling position of the document, or perform some other action that brings element to the user's attention. 
                 * User agents may report more than one constraint violation, if element suffers from multiple problems at once. 
                 * If element is not being rendered, then the user agent may, instead of notifying the user, report the error for the running script. */

                if (report)
                {
                    ScrollIntoView();
                    Focus();

                    /* XXX: Show some kind of popup or something to notify the user why this element is invalid */
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sets a custom error, so that the element would fail to validate. The given message is the message to be shown to the user when reporting the problem to the user.
        /// If the argument is the empty string, clears the custom error.
        /// </summary>
        /// <param name="error"></param>
        internal virtual void setCustomValidity(string error)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#dom-cva-setcustomvalidity */
            custom_validity_error_message = error;
        }


        /// <summary>
        /// Checks the constraints for this element and returns the validity flags it is violating(if any)
        /// </summary>
        /// <returns></returns>
        /* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#validity-states */
        internal virtual EValidityState query_validity()
        {
            EValidityState flags = 0x0;

            /* When a control has no value but has a required attribute (input required, textarea required); or, more complicated rules for select elements and controls in radio button groups, as specified in their sections. */
            if (hasAttribute(EAttributeName.Required))
            {
                if (string.IsNullOrEmpty(value))
                {
                    flags |= EValidityState.valueMissing;
                }
            }

            if (!string.IsNullOrEmpty(custom_validity_error_message))
            {
                flags |= EValidityState.customError;
            }

            return flags;
        }

        internal virtual bool check_satisfies_constraints()
        {
            if (DOMCommon.Is_Form_Associated_Custom_Element(this))
            {
                EValidityState flags = validity;
                if (0 != (flags & (EValidityState.valueMissing | EValidityState.typeMismatch | EValidityState.patternMismatch | EValidityState.tooLong | EValidityState.tooShort | EValidityState.rangeUnderflow | EValidityState.rangeOverflow | EValidityState.stepMismatch | EValidityState.badInput)))
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(custom_validity_error_message))
                {
                    return false;
                }
            }

            if (0x0 != query_validity())
            {
                return false;
            }

            return true;
        }


    }
}
