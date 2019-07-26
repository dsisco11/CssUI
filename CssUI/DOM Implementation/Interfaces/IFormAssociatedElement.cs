
namespace CssUI.DOM
{
    internal interface IFormAssociatedElement
    {/* Docs: https://html.spec.whatwg.org/multipage/forms.html#form-associated-element */

        /// <summary>
        /// A form-associated element can have a relationship with a form element, which is called the element's form owner. If a form-associated element is not associated with a form element, its form owner is said to be null.
        /// </summary>
        HTMLFormElement form { get; set; }
        bool bParserInserted { get; set; }

        /// <summary>
        /// Each form-associated custom element has a state. 
        /// It is information with which the user agent can restore a user's input for the element. 
        /// The initial value of state is null, and state can be null, a string, a File, or a list of entries.
        /// </summary>
        object state { get; set; }
        /// <summary>
        /// Each form-associated custom element has submission value. 
        /// It is used to provide one or more entries on form submission, and The initial value of submission value is null, and submission value can be null, a string, a File, or a list of entries.
        /// </summary>
        object submission_value { get; set; }

        EValidityState validity_flags { get; set; }
        /// <summary>
        /// Each form-associated custom element has a validation message string. It is the empty string initially.
        /// </summary>
        string validation_message { get; set; }
        /// <summary>
        /// Each form-associated custom element has a validation anchor element. It is null initially.
        /// </summary>
        Element validation_anchor { get; set; }

        /// <summary>
        /// The custom validity error message will be used when alerting the user to the problem with the control.
        /// </summary>
        string custom_validity_error_message { get; set; }
    }
}
