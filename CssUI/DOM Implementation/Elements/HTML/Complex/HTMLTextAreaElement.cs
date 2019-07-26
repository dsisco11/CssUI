using CssUI.DOM.CustomElements;
using CssUI.DOM.Nodes;
using System.Collections.Generic;

namespace CssUI.DOM
{
    public class HTMLTextAreaElement : FormAssociatedElement, IListedElement, ILableableElement, ISubmittableElement, IResettableElement, IAutoCapitalizeInheritingElement
    {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#the-textarea-element */
        #region Properties
        public string value;
        #endregion

        #region Constructors

        #endregion


        [CEReactions] string autocomplete;
        [CEReactions] bool autofocus;
        [CEReactions] long cols;
        [CEReactions] string dirName;
        [CEReactions] bool disabled;
        [CEReactions] long maxLength;
        [CEReactions] long minLength;
        [CEReactions] string name;
        [CEReactions] string placeholder;
        [CEReactions] bool readOnly;
        [CEReactions] bool required;
        [CEReactions] ulong rows;
        [CEReactions] string wrap;

        readonly string type;
        [CEReactions] string defaultValue;
        public readonly ulong textLength;

        public readonly bool willValidate;
        public bool checkValidity();
        public bool reportValidity();
        public void setCustomValidity(string error);


        #region Form-Associated Element Overrides
        internal override EValidityState query_validity()
        {
            EValidityState flags = base.query_validity();

            /* When a control has no value but has a required attribute (input required, textarea required); or, more complicated rules for select elements and controls in radio button groups, as specified in their sections. */
            if (hasAttribute(EAttributeName.Required))
            {
                if (ReferenceEquals(null, value) || value.Length <= 0)
                {
                    flags |= EValidityState.valueMissing;
                }
            }

            /* When a control has a value that is too long for the form control maxlength attribute (input maxlength, textarea maxlength). */
            if (value.Length > maxLength)
            {
                flags |= EValidityState.tooLong;
            }

            /* When a control has a value that is too short for the form control minlength attribute (input minlength, textarea minlength). */
            if (value.Length < minLength)
            {
                flags |= EValidityState.tooShort;
            }

            if (!ReferenceEquals(null, value) && value.Length > 0)
            {
                /* When a control has a value that is not the empty string and is too low for the min attribute. */
            }
        }
        #endregion



        public IReadOnlyCollection<HTMLLabelElement> labels
        {
            get => (IReadOnlyCollection<HTMLLabelElement>)DOMCommon.Get_Descendents(form, new FilterLabelFor(this), Enums.ENodeFilterMask.SHOW_ELEMENT);
        }

        public void select();
        public long selectionStart;
        public long selectionEnd;
        public string selectionDirection;
        public void setRangeText(string replacement);
        public void setRangeText(string replacement, long start, long end, SelectionMode selectionMode = "preserve");
        public void setSelectionRange(ulong start, long end, string direction);
    }
}
