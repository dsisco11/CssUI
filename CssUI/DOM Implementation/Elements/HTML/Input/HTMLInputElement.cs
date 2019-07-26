using CssUI.DOM.CustomElements;
using CssUI.DOM.Nodes;
using System;
using System.Collections.Generic;

namespace CssUI.DOM
{
    public class HTMLInputElement : FormAssociatedElement, IListedElement, ISubmittableElement, IResettableElement, ILableableElement, IAutoCapitalizeInheritingElement
    {/* Docs:  */
        #region Properties

        #endregion

        #region Constructors
        public HTMLInputElement(Document document) : base(document, "input")
        {
        }

        public HTMLInputElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion


        [CEReactions] public string accept;
        [CEReactions] public string alt;
        [CEReactions] public string autocomplete;
        [CEReactions] public bool autofocus;
        [CEReactions] public bool defaultChecked;
        public bool Checked;
        [CEReactions] public string dirName;
        [CEReactions] public bool disabled;
        public List<FileBlob> files;
        [CEReactions] public string formAction;
        [CEReactions] public string formEnctype;
        [CEReactions] public string formMethod;
        [CEReactions] public bool formNoValidate;
        [CEReactions] public string formTarget;
        [CEReactions] public uint height;
        public bool indeterminate;
        public HTMLElement list { get; private set; }
        [CEReactions] public string max;
        [CEReactions] public long maxLength;
        [CEReactions] public string min;
        [CEReactions] public long minLength;
        [CEReactions] public bool multiple;
        [CEReactions] public string name;
        [CEReactions] public string pattern;
        [CEReactions] public string placeholder;
        [CEReactions] public bool readOnly;
        [CEReactions] public bool required;
        [CEReactions] public uint size;
        [CEReactions] public string src;
        [CEReactions] public string step;

        [CEReactions]
        public EInputType type
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#dom-input-type */
            get => getAttribute(EAttributeName.Type).Get_Enum<EInputType>();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Type, AttributeValue.From_Enum(value)));
        }

        [CEReactions] public string defaultValue;
        [CEReactions] public string value;
        public object valueAsDate;
        public double valueAsNumber;
        [CEReactions] public uint width;

        public void stepUp(long n = 1);
        public void stepDown(long n = 1);

        public IReadOnlyCollection<HTMLLabelElement> labels
        {
            get
            {
                if (type == EInputType.Hidden)
                    return null;

                return (IReadOnlyCollection<HTMLLabelElement>)DOMCommon.Get_Descendents(form, new FilterLabelFor(this), Enums.ENodeFilterMask.SHOW_ELEMENT);
            }
        }

        public void select();
        public uint selectionStart;
        public uint selectionEnd;
        public string selectionDirection;

        /* Docs: https://html.spec.whatwg.org/multipage/input.html#concept-input-type-image-coordinate */
        public int selected_coordinate_x { get; private set; } = 0;
        public int selected_coordinate_y { get; private set; } = 0;


        #region State Determined Algorithms
        long get_minimum()
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#attr-input-min */
        }

        long get_maximum()
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#attr-input-max */
        }

        long convert_string_to_number(ReadOnlyMemory<char> buffMem)
        {
        }

        string convert_number_to_string(long num)
        {
        }
        #endregion

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


        public void setRangeText(string replacement);
        public void setRangeText(string replacement, uint start, uint end, SelectionMode selectionMode = "preserve");
        public void setSelectionRange(uint start, uint end, string direction);

    }
}
