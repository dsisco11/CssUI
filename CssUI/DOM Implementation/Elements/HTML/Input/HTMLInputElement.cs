using CssUI.DOM.CustomElements;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Nodes;
using CssUI.DOM.Serialization;
using System;
using System.Collections.Generic;

namespace CssUI.DOM
{

    public class HTMLInputElement : FormAssociatedElement, IListedElement, ISubmittableElement, IResettableElement, ILableableElement, IAutoCapitalizeInheritingElement
    {/* Docs: https://html.spec.whatwg.org/multipage/input.html#the-input-element */
        #region Backing Values
        private string _value = string.Empty;
        private List<FileBlob> selected_files = new List<FileBlob>();
        #endregion

        #region Properties
        /// <summary>
        /// input and textarea elements have a dirty value flag. This is used to track the interaction between the value and default value. If it is false, value mirrors the default value. If it is true, the default value is ignored.
        /// </summary>
        private bool bDirtyValueFlag = false;
        private bool bDirtyCheckednessFlag = false;
        private bool checkedness = false;

        public bool indeterminate;

        /* Docs: https://html.spec.whatwg.org/multipage/input.html#concept-input-type-image-coordinate */
        public int selected_coordinate_x { get; private set; } = 0;
        public int selected_coordinate_y { get; private set; } = 0;
        #endregion

        #region Constructors
        public HTMLInputElement(Document document) : base(document, "input")
        {
        }

        public HTMLInputElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Allows manipulating the checkedness of the input element
        /// </summary>
        public bool Checked
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#dom-input-checked */
            get => checkedness;
            set
            {
                checkedness = value;
                bDirtyCheckednessFlag = true;
            }
        }

        public DateTime? valueAsDate
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#dom-input-valueasdate */
            get
            {
                switch (type)
                {
                    case EInputType.Date:
                    case EInputType.Month:
                    case EInputType.Week:
                    case EInputType.Time:
                        break;
                    default:
                        return null;
                }

                convert_string_to_date(_value.AsMemory(), out DateTime outDateTime);
                return outDateTime;
            }

            set
            {
                switch (type)
                {
                    case EInputType.Date:
                    case EInputType.Month:
                    case EInputType.Week:
                    case EInputType.Time:
                        break;
                    default:
                        throw new InvalidStateError($"The input-type({Enum.GetName(typeof(EInputType), type)}) value cannot be converted to the requested type");
                }

                if (value.HasValue || value.Value.Ticks == 0)
                {
                    _value = string.Empty;
                }
                else
                {
                    convert_date_to_string(value.Value, out string outValue);
                    _value = outValue;
                }
            }
        }

        public double valueAsNumber
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#dom-input-valueasnumber */
            get
            {
                switch (type)
                {
                    case EInputType.Date:
                    case EInputType.Month:
                    case EInputType.Week:
                    case EInputType.Time:
                    case EInputType.Local:
                    case EInputType.Number:
                    case EInputType.Radio:
                        break;
                    default:
                        return double.NaN;
                }

                convert_string_to_number(_value.AsMemory(), out double outValue);
                return outValue;
            }
            set
            {
                switch (type)
                {
                    case EInputType.Date:
                    case EInputType.Month:
                    case EInputType.Week:
                    case EInputType.Time:
                    case EInputType.Local:
                    case EInputType.Number:
                    case EInputType.Radio:
                        break;
                    default:
                        throw new InvalidStateError($"The input-type({Enum.GetName(typeof(EInputType), type)}) value cannot be converted to the requested type");
                }

                if (double.IsInfinity(value)) throw new TypeError("Value cannot be infinity");
                if (double.IsNaN(value)) _value = string.Empty;
                else
                {
                    convert_number_to_string(value, out string outValue);
                    _value = outValue;
                }
            }
        }
        #endregion

        #region Content Attributes
        /// <summary>
        /// Hint for expected file type in file upload controls
        /// <para>The accept attribute may be specified to provide user agents with a hint of what file types will be accepted.</para>
        /// </summary>
        [CEReactions] public string accept
        {
            get => getAttribute(EAttributeName.Accept).Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Accept, AttributeValue.From_String(value)));
        }

        /// <summary>
        /// Replacement text for use when images are not available
        /// <para>The alt attribute provides the textual label for the button for users and user agents who cannot use the image. The alt attribute must be present, and must contain a non-empty string giving the label that would be appropriate for an equivalent button if the image was unavailable.</para>
        /// </summary>
        [CEReactions] public string alt
        {
            get
            {
                HTMLCommon.Resolve_Autofill(this, out _, out _, out _, out string outValue);
                return outValue;
            }
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Alt, AttributeValue.From_String(value)));
        }

        /// <summary>
        /// Hint for form autofill feature
        /// </summary>
        [CEReactions] public string autocomplete
        {
            get => getAttribute(EAttributeName.Autocomplete).Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Autocomplete, AttributeValue.From_String(value)));
        }

        /// <summary>
        /// Automatically focus the form control when the page is loaded
        /// </summary>
        [CEReactions] public bool autofocus
        {
            get => hasAttribute(EAttributeName.Autofocus);
            set => CEReactions.Wrap_CEReaction(this, () => toggleAttribute(EAttributeName.Autofocus, value));
        }

        [CEReactions] public bool defaultChecked
        {
            get => hasAttribute(EAttributeName.Checked);
            set => CEReactions.Wrap_CEReaction(this, () => toggleAttribute(EAttributeName.Checked, value));
        }

        [CEReactions] public string dirName
        {
            get => getAttribute(EAttributeName.Dirname)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Dirname, AttributeValue.From_String(value)));
        }

        //[CEReactions] public bool disabled; /* Redundant */

        /// <summary>
        /// Returns the list of selected files
        /// </summary>
        public IReadOnlyList<FileBlob> files
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#dom-input-files */
            get
            {
                if (type != EInputType.File) return null;

                return selected_files;
            }
            set
            {
                if (type != EInputType.File) return;
                if (value == null) return;

                selected_files = new List<FileBlob>(value);
            }
        }

        [CEReactions] public string formAction
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#dom-fs-formaction */
            get
            {
                var attrValue = getAttribute(EAttributeName.FormAction)?.Get_String();
                if (string.IsNullOrEmpty(attrValue))
                {
                    return nodeDocument.URL;
                }

                return attrValue;
            }
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.FormAction, AttributeValue.From_String(value)));
        }

        [CEReactions] public EEncType formEnctype
        {
            get => getAttribute(EAttributeName.FormEncType).Get_Enum<EEncType>();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.FormEncType, AttributeValue.From_Enum(value)));
        }

        [CEReactions] public EFormMethod formMethod
        {
            get => getAttribute(EAttributeName.FormMethod).Get_Enum<EFormMethod>();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.FormMethod, AttributeValue.From_Enum(value)));
        }

        [CEReactions] public bool formNoValidate
        {
            get => hasAttribute(EAttributeName.FormNoValidate);
            set => CEReactions.Wrap_CEReaction(this, () => toggleAttribute(EAttributeName.NoValidate, value));
        }

        [CEReactions] public string formTarget
        {
            get => getAttribute(EAttributeName.FormTarget).Get_String();
            set
            {
                if (HTMLCommon.Is_Valid_Browsing_Context_Name_Or_Keyword(value.AsMemory()))
                {
                    throw new DOMException($"formtarget cannot accept the invalid value: \"{value}\"");
                }

                CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.FormAction, AttributeValue.From_String(value)));
            }
        }

        [CEReactions] public uint height
        {
            ...
        }

        public HTMLDataListElement list
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#concept-input-list */
            get
            {
                /* The suggestions source element is the first element in the tree in tree order to have an ID equal to the value of the list attribute, if that element is a datalist element. 
                 * If there is no list attribute, or if there is no element with that ID, or if the first element with that ID is not a datalist element, then there is no suggestions source element. */
                if (hasAttribute(EAttributeName.List, out Attr outList))
                {
                    string idValue = outList?.Value?.Get_Atomic();
                    if (!string.IsNullOrEmpty(idValue))
                    {
                        var found = nodeDocument.getElementByID(idValue);
                        if (found is HTMLDataListElement datalist)
                        {
                            return datalist;
                        }
                    }
                }

                return null;
            }
            private set => setAttribute(EAttributeName.List, value==null ? null : AttributeValue.From_String(value?.id));
        }

        [CEReactions] public string max
        {
            get => getAttribute(EAttributeName.Max)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Max, AttributeValue.From_String(value)));
        }

        [CEReactions] public int maxLength
        {
            get => getAttribute(EAttributeName.MaxLength).Get_Int();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.MaxLength, AttributeValue.From_Integer(value)));
        }

        [CEReactions] public string min
        {
            get => getAttribute(EAttributeName.Min)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Min, AttributeValue.From_String(value)));
        }

        [CEReactions] public int minLength
        {
            get => getAttribute(EAttributeName.MinLength).Get_Int();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.MinLength, AttributeValue.From_Integer(value)));
        }

        [CEReactions] public bool multiple
        {
            get => hasAttribute(EAttributeName.Multiple);
            set => CEReactions.Wrap_CEReaction(this, () => toggleAttribute(EAttributeName.Multiple, value));
        }

        [CEReactions] public string name
        {
            get => getAttribute(EAttributeName.Name)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Name, AttributeValue.From_String(value)));
        }

        [CEReactions] public string pattern
        {
            get => getAttribute(EAttributeName.Pattern)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Pattern, AttributeValue.From_String(value)));
        }

        [CEReactions] public string placeholder
        {
            get => getAttribute(EAttributeName.Placeholder)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Placeholder, AttributeValue.From_String(value)));
        }

        [CEReactions] public bool readOnly
        {
            get => hasAttribute(EAttributeName.ReadOnly);
            set => CEReactions.Wrap_CEReaction(this, () => toggleAttribute(EAttributeName.ReadOnly, value));
        }

        [CEReactions] public bool required
        {
            get => hasAttribute(EAttributeName.Required);
            set => CEReactions.Wrap_CEReaction(this, () => toggleAttribute(EAttributeName.Required, value));
        }

        [CEReactions] public uint size
        {
            get => getAttribute(EAttributeName.Size).Get_UInt();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Size, AttributeValue.From_Integer(value)));
        }

        [CEReactions] public string src
        {
            get => getAttribute(EAttributeName.Src)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Src, AttributeValue.From_String(value)));
        }

        [CEReactions] public new EInputType type
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#dom-input-type */
            get => getAttribute(EAttributeName.Type).Get_Enum<EInputType>();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Type, AttributeValue.From_Enum(value)));
        }

        [CEReactions] public string defaultValue
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#dom-input-defaultvalue */
            get => getAttribute(EAttributeName.Value)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Value, AttributeValue.From_String(value)));
        }

        [CEReactions] public uint width
        {
            ...
        }
        #endregion

        #region Input Step
        [CEReactions] public string step
        {
            get => getAttribute(EAttributeName.Step)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Step, AttributeValue.From_String(value)));
        }

        double default_step
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#concept-input-step */
            get
            {
                ...
            }
        }

        double? default_step_base
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#concept-input-step */
            get
            {
                ...
            }
        }

        double step_scale_factor
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#concept-input-step */
            get
            {
                ...
            }
        }

        double step_base
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#concept-input-min-zero */
            get
            {
                /* 1) If the element has a min content attribute, and the result of applying the algorithm to convert a string to a number to the value of the min content attribute is not an error, then return that result. */
                if (hasAttribute(EAttributeName.Min, out Attr outMinAttr) && outMinAttr.Value != null && convert_string_to_number(outMinAttr.Value.Get_String().AsMemory(), out double outMin))
                {
                    return outMin;
                }
                /* 2) If the element has a value content attribute, and the result of applying the algorithm to convert a string to a number to the value of the value content attribute is not an error, then return that result. */
                if (hasAttribute(EAttributeName.Value, out Attr outValueAttr) && outValueAttr.Value != null && convert_string_to_number(outValueAttr.Value.Get_String().AsMemory(), out double outValue))
                {
                    return outValue;
                }
                /* 3) If a default step base is defined for this element given its type attribute's state, then return it. */
                double? dsb = default_step_base;
                if (dsb.HasValue)
                {
                    return dsb.Value;
                }

                /* 4) Return zero. */
                return 0;
            }
        }

        double? allowed_value_step
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#concept-input-step */
            get
            {
                /* 1) If the attribute is absent, then the allowed value step is the default step multiplied by the step scale factor. */
                if (!hasAttribute(EAttributeName.Step, out Attr outAttr) || ReferenceEquals(null, outAttr?.Value?.Get_String()))
                    return default_step * step_scale_factor;

                string value = outAttr.Value.Get_String();
                /* 2) Otherwise, if the attribute's value is an ASCII case-insensitive match for the string "any", then there is no allowed value step. */
                if (StringCommon.StriEq(@"any".AsSpan(), value.AsSpan()))
                    return null;

                /* 3) Otherwise, if the rules for parsing floating-point number values, when they are applied to the attribute's value, 
                 * return an error, zero, or a number less than zero, then the allowed value step is the default step multiplied by the step scale factor. */
                if (!DOMParser.Parse_FloatingPoint(value.AsMemory(), out double outParsed) || MathExt.Flteq(outParsed, 0.0))
                {
                    return default_step * step_scale_factor;
                }

                /* 4) Otherwise, the allowed value step is the number returned by the rules for parsing floating-point number values when they are applied to the attribute's value, multiplied by the step scale factor. */
                return outParsed * step_scale_factor;
            }
        }
        #endregion

        #region Input Value
        private enum EInputValueMode { Value, Default, Default_ON, Filename };

        private EInputValueMode Get_Value_Mode()
        {
            switch (type)
            {
                case EInputType.Hidden:
                    return EInputValueMode.Default;
                case EInputType.Text:
                case EInputType.Search:
                case EInputType.Url:
                case EInputType.Telephone:
                case EInputType.Email:
                case EInputType.Password:
                case EInputType.Date:
                case EInputType.Month:
                case EInputType.Week:
                case EInputType.Time:
                case EInputType.Local:
                case EInputType.Number:
                case EInputType.Range:
                case EInputType.Color:
                    return EInputValueMode.Value;
                case EInputType.Checkbox:
                case EInputType.Radio:
                    return EInputValueMode.Default_ON;
                case EInputType.File:
                    return EInputValueMode.Filename;
                case EInputType.Submit:
                case EInputType.Image:
                case EInputType.Reset:
                case EInputType.Button:
                    return EInputValueMode.Default;
            }

            return EInputValueMode.Default;
        }

        [CEReactions] public override string value
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#dom-input-value */
            get
            {
                switch (Get_Value_Mode())
                {
                    case EInputValueMode.Value:
                        {
                            return _value;
                        }
                    default:
                    case EInputValueMode.Default:
                        {
                            return getAttribute(EAttributeName.Value)?.Get_String() ?? string.Empty;
                        }
                    case EInputValueMode.Default_ON:
                        {
                            return getAttribute(EAttributeName.Value)?.Get_String() ?? "on";
                        }
                    case EInputValueMode.Filename:
                        {
                            const string fakepath = "C:\\fakepath\\";
                            string fileName = selected_files.Count > 0 ? selected_files[0].name : string.Empty;
                            return StringCommon.Concat(fakepath.AsMemory(), fileName.AsMemory());
                        }
                }
            }

            set
            {
                switch (Get_Value_Mode())
                {
                    case EInputValueMode.Value:
                        {
                            var oldValue = _value;
                            _value = value;
                            bDirtyValueFlag = true;
                            run_value_sanitization();
                            if (!StringCommon.StrEq(_value.AsSpan(), oldValue.AsSpan()))
                            {
                                text_entry_cursor_position = value.Length;
                                selection?.Collapse();
                                selection.direction = ESelectionDirection.None;
                            }
                        }
                        break;
                    default:
                    case EInputValueMode.Default:
                        {
                            setAttribute(EAttributeName.Value, AttributeValue.From_String(value));
                        }
                        break;
                    case EInputValueMode.Default_ON:
                        {
                            setAttribute(EAttributeName.Value, AttributeValue.From_String(value));
                        }
                        break;
                    case EInputValueMode.Filename:
                        {/* On setting, if the new value is the empty string, empty the list of selected files; otherwise, throw an "InvalidStateError" DOMException. */
                            if (value.Length <= 0)
                            {
                                selected_files.Clear();
                            }
                            else
                            {
                                throw new InvalidStateError("The only valid value for this type of input element is an empty string");
                            }
                        }
                        break;
                }
            }
        }
        #endregion


        public void stepUp(long n = 1)
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#dom-input-stepup */
            switch (type)
            {
                case EInputType.Date:
                case EInputType.Month:
                case EInputType.Week:
                case EInputType.Time:
                case EInputType.Local:
                case EInputType.Number:
                case EInputType.Radio:
                    break;
                default:
                    throw new InvalidStateError($"The input-type({Enum.GetName(typeof(EInputType), type)}) does not implement the stepUp() method");
            }

            if (!allowed_value_step.HasValue)
            {
                throw new InvalidStateError("Input element has no allowed-value-step, step cannot be \"any\"");
            }
            /* 3) If the element has a minimum and a maximum and the minimum is greater than the maximum, then return. */
            var min = get_minimum();
            var max = get_maximum();
            if (min.HasValue && max.HasValue && min > max)
            {
                if (min > max)
                    return;

                /* 4) If the element has a minimum and a maximum and there is no value greater than or equal to the element's minimum and less than or equal to the element's maximum that, 
                 * when subtracted from the step base, is an integral multiple of the allowed value step, then return. */
                var allowed_step = allowed_value_step.Value;
                var mn = step_base - min.Value;
                var mx = step_base - max.Value;
                var minMult = allowed_step / mn;
                var maxMult = allowed_step / mx;
                var delta = maxMult - minMult;
                if ((int)delta == 0) // these two values fall within the same multiple of the allowed-step
                {
                    if ((allowed_step % mn) != 0)
                    {
                        return;
                    }
                }
            }

            double value = 0;
            /* 5) If applying the algorithm to convert a string to a number to the string given by the element's value does not result in an error, 
             * then let value be the result of that algorithm. Otherwise, let value be zero. */
            if (convert_string_to_number(_value.AsMemory(), out double outValue))
            {
                value = outValue;
            }

            var valueBeforeStepping = value;
            /* 7) If value subtracted from the step base is not an integral multiple of the allowed value step, 
             * then set value to the nearest value that, when subtracted from the step base, is an integral multiple of the allowed value step, 
             * and that is less than value if the method invoked was the stepDown() method, and more than value otherwise. */


        }

        public void stepDown(long n = 1)
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#dom-input-stepup */
            switch (type)
            {
                case EInputType.Date:
                case EInputType.Month:
                case EInputType.Week:
                case EInputType.Time:
                case EInputType.Local:
                case EInputType.Number:
                case EInputType.Radio:
                    break;
                default:
                    throw new InvalidStateError($"The input-type({Enum.GetName(typeof(EInputType), type)}) does not implement the stepDown() method");
            }
        }

        public IReadOnlyCollection<HTMLLabelElement> labels
        {
            get
            {
                if (type == EInputType.Hidden)
                    return null;

                return (IReadOnlyCollection<HTMLLabelElement>)DOMCommon.Get_Descendents(form, new FilterLabelFor(this), Enums.ENodeFilterMask.SHOW_ELEMENT);
            }
        }


        #region State Determined Algorithms
        double? get_minimum()
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#attr-input-min */
            /* If the element has a min attribute, 
             * and the result of applying the algorithm to convert a string to a number to the value of the min attribute is a number, 
             * then that number is the element's minimum; otherwise, 
             * if the type attribute's current state defines a default minimum, then that is the minimum; otherwise, 
             * the element has no minimum. */
            if (hasAttribute(EAttributeName.Min, out Attr outAttr) && outAttr.Value != null && convert_string_to_number(outAttr.Value.Get_String().AsMemory(), out double outValue))
            {
                return outValue;
            }

            switch (type)
            {
                default:
                    return null;
            }
        }

        double? get_maximum()
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#attr-input-max */
         /* If the element has a max attribute, 
          * and the result of applying the algorithm to convert a string to a number to the value of the max attribute is a number, 
          * then that number is the element's maximum; otherwise, 
          * if the type attribute's current state defines a default maximum, then that is the maximum; otherwise, 
          * the element has no maximum. */
            if (hasAttribute(EAttributeName.Max, out Attr outAttr) && outAttr.Value != null && convert_string_to_number(outAttr.Value.Get_String().AsMemory(), out double outValue))
            {
                return outValue;
            }

            switch (type)
            {
                default:
                    return null;
            }
        }

        bool convert_string_to_number(ReadOnlyMemory<char> buffMem, out double outValue)
        {
        }

        bool convert_number_to_string(double num, out string outValue)
        {
        }

        bool convert_string_to_date(ReadOnlyMemory<char> buffMem, out DateTime outValue)
        {
        }

        bool convert_date_to_string(DateTime date, out string outValue)
        {
        }

        void run_value_sanitization()
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#value-sanitization-algorithm */

        }
        #endregion

        #region Overrides
        internal override void run_attribute_change_steps(Element element, AtomicName<EAttributeName> localName, AttributeValue oldValue, AttributeValue value, string Namespace)
        {
            base.run_attribute_change_steps(element, localName, oldValue, value, Namespace);

            /* When the value content attribute is added, set, or removed, 
             * if the control's dirty value flag is false, the user agent must set the value of the element to the value of the value content attribute, 
             * if there is one, or the empty string otherwise, and then run the current value sanitization algorithm, if one is defined. */
            if (localName == EAttributeName.Value)
            {
                if (!bDirtyValueFlag)
                {
                    this.value = defaultValue ?? string.Empty;
                    run_value_sanitization();
                }
            }
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

            /* Constraint validation: When the element has an allowed value step, 
             * and the result of applying the algorithm to convert a string to a number to the string given by the element's value is a number, 
             * and that number subtracted from the step base is not an integral multiple of the allowed value step, 
             * the element is suffering from a step mismatch. */
            if (allowed_value_step.HasValue)
            {
                if (convert_string_to_number(_value.AsMemory(), out double outValueNumber))
                {
                    double delta = outValueNumber - step_base;
                    double multiple = allowed_value_step.Value % delta;
                    if (multiple > 0)
                    {
                        flags |= EValidityState.stepMismatch;
                    }
                }
            }

            if (!ReferenceEquals(null, value) && value.Length > 0)
            {
                /* When a control has a value that is not the empty string and is too low for the min attribute. */
            }
        }
        #endregion

        #region Text Selection
        /* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#textFieldSelection:concept-textarea/input-selection */

        private readonly TextSelection selection = new TextSelection();
        private int text_entry_cursor_position = 0;

        public void select()
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#dom-textarea/input-select */
            /* 1) If this element is an input element, and either select() does not apply to this element or the corresponding control has no selectable text, return. */
            if (this is HTMLInputElement inputElement)
            {
                var ty = inputElement.type;
                switch (inputElement.type)
                {
                    case EInputType.Text:
                    case EInputType.Search:
                    case EInputType.Url:
                    case EInputType.Telephone:
                    case EInputType.Email:
                    case EInputType.Password:
                    case EInputType.Date:
                    case EInputType.Month:
                    case EInputType.Week:
                    case EInputType.Time:
                    case EInputType.Local:
                    case EInputType.Number:
                    case EInputType.Color:
                    case EInputType.File:
                        break;
                    default:
                        return;
                }
            }

            if (!StringCommon.Scan(value.AsSpan(), Filters.FilterCharSelectable.Instance))
            {
                return;
            }

            /* 2) Set the selection range with 0 and infinity. */
            setSelectionRange(0, int.MaxValue);
        }

        public int? selectionStart
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#dom-textarea/input-selectionstart */
            get
            {
                /* 1) If this element is an input element, and selectionStart does not apply to this element, return null. */
                if (this is HTMLInputElement inputElement)
                {
                    switch (inputElement.type)
                    {
                        case EInputType.Text:
                        case EInputType.Search:
                        case EInputType.Url:
                        case EInputType.Telephone:
                        case EInputType.Password:
                            break;
                        default:
                            return null;
                    }
                }
                /* 2) If there is no selection, return the offset (in logical order) within the relevant value to the character that immediately follows the text entry cursor. */
                if (!selection.HasSelection)
                    return text_entry_cursor_position;

                return selection.start;
            }

            set
            {
                /* 1) If this element is an input element, and selectionStart does not apply to this element, throw an "InvalidStateError" DOMException. */
                if (this is HTMLInputElement inputElement)
                {
                    switch (inputElement.type)
                    {
                        case EInputType.Text:
                        case EInputType.Search:
                        case EInputType.Url:
                        case EInputType.Telephone:
                        case EInputType.Password:
                            break;
                        default:
                            throw new InvalidStateError($"This property is not valid for input elements of type \"{Lookup.Keyword(inputElement.type)}\"");
                    }
                }
                var end = selectionEnd;
                if (end < value)
                    end = value;

                setSelectionRange(value, end, selectionDirection);
            }
        }

        public int? selectionEnd
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#dom-textarea/input-selectionend */
            get
            {
                /* 1) If this element is an input element, and selectionStart does not apply to this element, return null. */
                if (this is HTMLInputElement inputElement)
                {
                    switch (inputElement.type)
                    {
                        case EInputType.Text:
                        case EInputType.Search:
                        case EInputType.Url:
                        case EInputType.Telephone:
                        case EInputType.Password:
                            break;
                        default:
                            return null;
                    }
                }
                /* 2) If there is no selection, return the offset (in logical order) within the relevant value to the character that immediately follows the text entry cursor. */
                if (!selection.HasSelection)
                    return text_entry_cursor_position;

                return selection.end;
            }

            set
            {
                /* 1) If this element is an input element, and selectionStart does not apply to this element, throw an "InvalidStateError" DOMException. */
                if (this is HTMLInputElement inputElement)
                {
                    switch (inputElement.type)
                    {
                        case EInputType.Text:
                        case EInputType.Search:
                        case EInputType.Url:
                        case EInputType.Telephone:
                        case EInputType.Password:
                            break;
                        default:
                            throw new InvalidStateError($"This property is not valid for input elements of type \"{Lookup.Keyword(inputElement.type)}\"");
                    }
                }
                setSelectionRange(selectionStart, value, selectionDirection);
            }
        }

        public ESelectionDirection? selectionDirection
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#dom-textarea/input-selectiondirection */
            get
            {
                /* 1) If this element is an input element, and selectionStart does not apply to this element, return null. */
                if (this is HTMLInputElement inputElement)
                {
                    switch (inputElement.type)
                    {
                        case EInputType.Text:
                        case EInputType.Search:
                        case EInputType.Url:
                        case EInputType.Telephone:
                        case EInputType.Password:
                            break;
                        default:
                            return null;
                    }
                }
                return selection.direction;
            }
            set
            {
                /* 1) If this element is an input element, and selectionStart does not apply to this element, throw an "InvalidStateError" DOMException. */
                if (this is HTMLInputElement inputElement)
                {
                    switch (inputElement.type)
                    {
                        case EInputType.Text:
                        case EInputType.Search:
                        case EInputType.Url:
                        case EInputType.Telephone:
                        case EInputType.Password:
                            break;
                        default:
                            throw new InvalidStateError($"This property is not valid for input elements of type \"{Lookup.Keyword(inputElement.type)}\"");
                    }
                }
                setSelectionRange(selectionStart, selectionEnd, value);
            }
        }

        public void setSelectionRange(int? start, int? end, ESelectionDirection? direction = null)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#dom-textarea/input-setselectionrange */
         /* 1) If this element is an input element, and selectionStart does not apply to this element, throw an "InvalidStateError" DOMException. */
            if (this is HTMLInputElement inputElement)
            {
                switch (inputElement.type)
                {
                    case EInputType.Text:
                    case EInputType.Search:
                    case EInputType.Url:
                    case EInputType.Telephone:
                    case EInputType.Password:
                        break;
                    default:
                        throw new InvalidStateError($"This method is not valid for input elements of type \"{Lookup.Keyword(inputElement.type)}\"");
                }
            }

            if (!start.HasValue)
            {
                start = 0;
            }

            if (!end.HasValue)
            {
                end = 0;
            }

            /* 3) Set the selection of the text control to the sequence of characters within the relevant value starting with the character at the startth position (in logical order) and ending with the character at the (end-1)th position. 
             * \Arguments greater than the length of the relevant value of the text control (including the special value infinity) must be treated as pointing at the end of the text control. 
             * If end is less than or equal to start then the start of the selection and the end of the selection must both be placed immediately before the character with offset end. 
             * In UAs where there is no concept of an empty selection, this must set the cursor to be just before the character with offset end. */

            TextSelection oldSelection = new TextSelection(selection);
            var maxIndex = value.Length - 1;
            if (end <= start)
            {/* If end is less than or equal to start then the start of the selection and the end of the selection must both be placed immediately before the character with offset end. */
                var pos = MathExt.Clamp(end.Value - 1, 0, maxIndex);
                selection.start = pos;
                selection.end = pos;
            }
            else
            {
                selection.start = MathExt.Clamp(start.Value, 0, maxIndex);
                selection.end = MathExt.Clamp(end.Value, 0, maxIndex);
            }

            /* 4) If direction is not a case-sensitive match for either the string "backward" or "forward", or if the direction argument was omitted, set direction to "none". */
            if (!direction.HasValue)
            {
                direction = ESelectionDirection.None;
            }

            selection.direction = direction.Value;

            /* 6) If the previous steps caused the selection of the text control to be modified (in either extent or direction), then queue a task, using the user interaction task source, to fire an event named select at the element, with the bubbles attribute initialized to true. */
            if (!oldSelection.Equals(selection))
            {
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    dispatchEvent(new Event(EEventName.Select, new EventInit() { bubbles = true }));
                }).ConfigureAwait(continueOnCapturedContext: false);
            }
        }

        public void setRangeText(string replacement)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#dom-textarea/input-setrangetext */
         /* 1) If this element is an input element, and selectionStart does not apply to this element, throw an "InvalidStateError" DOMException. */
            if (this is HTMLInputElement inputElement)
            {
                switch (inputElement.type)
                {
                    case EInputType.Text:
                    case EInputType.Search:
                    case EInputType.Url:
                    case EInputType.Telephone:
                    case EInputType.Password:
                        break;
                    default:
                        throw new InvalidStateError($"This method is not valid for input elements of type \"{Lookup.Keyword(inputElement.type)}\"");
                }
            }

            bDirtyValueFlag = true;

            /* 3) If the method has only one argument, then let start and end have the values of the selectionStart attribute and the selectionEnd attribute respectively. */
            int start = selectionStart;
            int end = selectionEnd;
            /* 4) If start is greater than end, then throw an "IndexSizeError" DOMException. */
            if (start > end)
            {
                throw new IndexSizeError("Start cannot be greater than end");
            }

            string relevantValue = value;
            var valueLength = relevantValue.Length;

            if (start > valueLength) start = maxLength;
            if (end > valueLength) end = maxLength;

            var selection_start = selectionStart;
            var selection_end = selectionEnd;

            /* 9) If start is less than end, delete the sequence of characters within the element's relevant value starting with the character at the startth position (in logical order) and ending with the character at the (end-1)th position. */
            if (start < end)
            {
                relevantValue = relevantValue.Substring(start, (end - start) - 1);
            }

            /* 10) Insert the value of the first argument into the text of the relevant value of the text control, immediately before the startth character. */
            relevantValue.Insert(start, replacement);

            var newLength = replacement.Length;
            var newEnd = start + newLength;

            var oldLength = end - start;
            var delta = newLength - oldLength;
            if (selection_start > end)
            {
                selection_start += delta;
            }
            else if (selection_start > start)
            {
                selection_start = start;
            }

            if (selection_end > end)
            {
                selection_end += delta;
            }
            else if (selection_end > start)
            {
                selection_end = newEnd;
            }

            setSelectionRange(selection_start, selection_end);
        }

        public void setRangeText(string replacement, int start, int end, ESelectionMode selectionMode = ESelectionMode.Preserve)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#dom-textarea/input-setrangetext */
         /* 1) If this element is an input element, and selectionStart does not apply to this element, throw an "InvalidStateError" DOMException. */
            if (this is HTMLInputElement inputElement)
            {
                switch (inputElement.type)
                {
                    case EInputType.Text:
                    case EInputType.Search:
                    case EInputType.Url:
                    case EInputType.Telephone:
                    case EInputType.Password:
                        break;
                    default:
                        throw new InvalidStateError($"This method is not valid for input elements of type \"{Lookup.Keyword(inputElement.type)}\"");
                }
            }

            bDirtyValueFlag = true;
            /* 4) If start is greater than end, then throw an "IndexSizeError" DOMException. */
            if (start > end)
            {
                throw new IndexSizeError("Start cannot be greater than end");
            }

            string relevantValue = value;
            var valueLength = relevantValue.Length;

            if (start > valueLength) start = maxLength;
            if (end > valueLength) end = maxLength;

            var selection_start = selectionStart;
            var selection_end = selectionEnd;

            /* 9) If start is less than end, delete the sequence of characters within the element's relevant value starting with the character at the startth position (in logical order) and ending with the character at the (end-1)th position. */
            if (start < end)
            {
                relevantValue = relevantValue.Substring(start, (end - start) - 1);
            }

            /* 10) Insert the value of the first argument into the text of the relevant value of the text control, immediately before the startth character. */
            relevantValue.Insert(start, replacement);

            var newLength = replacement.Length;
            var newEnd = start + newLength;

            switch (selectionMode)
            {
                case ESelectionMode.Select:
                    {
                        selection_start = start;
                        selection_end = newEnd;
                    }
                    break;
                case ESelectionMode.Start:
                    {
                        selection_start = selection_end = start;
                    }
                    break;
                case ESelectionMode.End:
                    {
                        selection_start = selection_end = newEnd;
                    }
                    break;
                case ESelectionMode.Preserve:
                    {
                        var oldLength = end - start;
                        var delta = newLength - oldLength;
                        if (selection_start > end)
                        {
                            selection_start += delta;
                        }
                        else if (selection_start > start)
                        {
                            selection_start = start;
                        }

                        if (selection_end > end)
                        {
                            selection_end += delta;
                        }
                        else if (selection_end > start)
                        {
                            selection_end = newEnd;
                        }
                    }
                    break;
            }

            setSelectionRange(selection_start, selection_end);
        }
        #endregion
    }
}
