using CssUI.DOM;
using CssUI.DOM.CustomElements;
using CssUI.DOM.Enums;
using CssUI.DOM.Events;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Nodes;
using CssUI.Filters;
using CssUI.HTML.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CssUI.HTML
{

    /// <summary>
    /// The input element represents a typed data field, usually with a form control to allow the user to edit the data.
    /// </summary>
    [MetaElement("input")]
    public class HTMLInputElement : FormAssociatedElement, IListedElement, ISubmittableElement, IResettableElement, ILableableElement, IAutoCapitalizeInheritingElement
    {/* Docs: https://html.spec.whatwg.org/multipage/input.html#the-input-element */
        const bool THROW_FOR_ATTR_INVALID_TYPES = false;
        const bool THROW_FOR_METHOD_INVALID_TYPES = false;

        #region Definitions
        public override EContentCategories Categories
        {
            get
            {
                var model = EContentCategories.Flow | EContentCategories.Phrasing;
                if (type != EInputType.Hidden)
                {
                    model |= EContentCategories.Interactive | EContentCategories.Palpable;
                }

                return model;
            }
        }
        #endregion

        #region Backing Values
        private string _value = string.Empty;
        private List<FileBlob> selected_files = new List<FileBlob>();
        private bool _checkedness = false;
        private NodeFilter labelFilter;
        private Regex patternCompiled = null;
        #endregion

        #region Properties
        /// <summary>
        /// input and textarea elements have a dirty value flag. This is used to track the interaction between the value and default value. If it is false, value mirrors the default value. If it is true, the default value is ignored.
        /// </summary>
        private bool bDirtyValueFlag = false;
        private bool bDirtyCheckednessFlag = false;

        public bool indeterminate;

        /* Docs: https://html.spec.whatwg.org/multipage/input.html#concept-input-type-image-coordinate */
        public int selected_coordinate_x { get; private set; } = 0;
        public int selected_coordinate_y { get; private set; } = 0;
        #endregion

        #region Constructors
        public HTMLInputElement(Document document) : this(document, "input")
        {
        }

        public HTMLInputElement(Document document, string localName) : base(document, localName)
        {
            labelFilter = new FilterLabelFor(this);
            if (hasAttribute(EAttributeName.Type))
            {
                update_rendering_for_type(type);
                run_value_sanitization();
            }
        }
        #endregion


        #region Accessors
        private bool checkedness
        {
            get => _checkedness;
            set
            {
                _checkedness = value;
                update_radio_button_group();
            }
        }
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
                        {
                            if (THROW_FOR_METHOD_INVALID_TYPES) throw new InvalidStateError($"The input-type({Enum.GetName(typeof(EInputType), type)}) value cannot be converted to the requested type");
                            else return;
                        }
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
                        {
                            if (THROW_FOR_METHOD_INVALID_TYPES) throw new InvalidStateError($"The input-type({Enum.GetName(typeof(EInputType), type)}) value cannot be converted to the requested type");
                            else return;
                        }
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

        public IReadOnlyCollection<HTMLLabelElement> labels
        {
            get
            {
                if (type == EInputType.Hidden)
                    return null;

                return DOMCommon.Get_Descendents_OfType<HTMLLabelElement>(form, labelFilter, ENodeFilterMask.SHOW_ELEMENT);
            }
        }
        #endregion


        #region Content Attributes
        /// <summary>
        /// Hint for expected file type in file upload controls
        /// <para>The accept attribute may be specified to provide user agents with a hint of what file types will be accepted.</para>
        /// </summary>
        [CEReactions]
        public string accept
        {
            get => getAttribute(EAttributeName.Accept).Get_String();
            set
            {
                CEReactions.Wrap_CEReaction(this, () =>
                {
                    switch (type)
                    {
                        case EInputType.File:
                            break;
                        default:
                            {
                                if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                                else return;
                            }
                    }

                    setAttribute(EAttributeName.Accept, AttributeValue.From_String(value));
                });
            }
        }

        /// <summary>
        /// Replacement text for use when images are not available
        /// <para>The alt attribute provides the textual label for the button for users and user agents who cannot use the image. The alt attribute must be present, and must contain a non-empty string giving the label that would be appropriate for an equivalent button if the image was unavailable.</para>
        /// </summary>
        [CEReactions]
        public string alt
        {
            get
            {
                HTMLCommon.Resolve_Autofill(this, out _, out _, out _, out string outValue);
                return outValue;
            }
            set => CEReactions.Wrap_CEReaction(this, () =>
            {
                switch (type)
                {
                    case EInputType.Image:
                        break;
                    default:
                        {
                            if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                            else return;
                        }
                }
                setAttribute(EAttributeName.Alt, AttributeValue.From_String(value));
            });
        }

        /// <summary>
        /// Hint for form autofill feature
        /// </summary>
        [CEReactions]
        public string autocomplete
        {
            get => getAttribute(EAttributeName.Autocomplete).Get_String();
            set => CEReactions.Wrap_CEReaction(this, () =>
            {
                switch (type)
                {
                    case EInputType.Hidden:
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
                        break;
                    default:
                        {
                            if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                            else return;
                        }
                }
                setAttribute(EAttributeName.Autocomplete, AttributeValue.From_String(value));
            });
        }

        /// <summary>
        /// Automatically focus the form control when the page is loaded
        /// </summary>
        [CEReactions]
        public bool autofocus
        {
            get => hasAttribute(EAttributeName.Autofocus);
            set => CEReactions.Wrap_CEReaction(this, () => toggleAttribute(EAttributeName.Autofocus, value));
        }

        [CEReactions]
        public bool defaultChecked
        {
            get => hasAttribute(EAttributeName.Checked);
            set => CEReactions.Wrap_CEReaction(this, () =>
            {
                switch (type)
                {
                    case EInputType.Checkbox:
                    case EInputType.Radio:
                        break;
                    default:
                        {
                            if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                            else return;
                        }
                }
                toggleAttribute(EAttributeName.Checked, value);
            });
        }

        [CEReactions]
        public string dirName
        {
            get => getAttribute(EAttributeName.Dirname)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () =>
            {
                switch (type)
                {
                    case EInputType.Text:
                    case EInputType.Search:
                        break;
                    default:
                        {
                            if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                            else return;
                        }
                }
                setAttribute(EAttributeName.Dirname, AttributeValue.From_String(value));
            });
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

        [CEReactions]
        public string formAction
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
            set => CEReactions.Wrap_CEReaction(this, () =>
            {
                switch (type)
                {
                    case EInputType.Submit:
                    case EInputType.Image:
                        break;
                    default:
                        {
                            if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                            else return;
                        }
                }
                setAttribute(EAttributeName.FormAction, AttributeValue.From_String(value));
            });
        }

        [CEReactions]
        public EEncType? formEnctype
        {
            get => getAttribute(EAttributeName.FormEncType)?.Get_Enum<EEncType>();
            set => CEReactions.Wrap_CEReaction(this, () =>
            {
                switch (type)
                {
                    case EInputType.Submit:
                    case EInputType.Image:
                        break;
                    default:
                        {
                            if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                            else return;
                        }
                }

                setAttribute(EAttributeName.FormEncType, !value.HasValue ? null : AttributeValue.From_Enum(value.Value));
            });
        }

        [CEReactions]
        public EFormMethod? formMethod
        {
            get => getAttribute(EAttributeName.FormMethod)?.Get_Enum<EFormMethod>();
            set => CEReactions.Wrap_CEReaction(this, () =>
            {
                switch (type)
                {
                    case EInputType.Submit:
                    case EInputType.Image:
                        break;
                    default:
                        {
                            if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                            else return;
                        }
                }

                setAttribute(EAttributeName.FormMethod, !value.HasValue ? null : AttributeValue.From_Enum(value.Value));
            });
        }

        [CEReactions]
        public bool formNoValidate
        {
            get => hasAttribute(EAttributeName.FormNoValidate);
            set => CEReactions.Wrap_CEReaction(this, () =>
            {
                switch (type)
                {
                    case EInputType.Submit:
                    case EInputType.Image:
                        break;
                    default:
                        {
                            if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                            else return;
                        }
                }
                toggleAttribute(EAttributeName.NoValidate, value);
            });
        }

        [CEReactions]
        public string formTarget
        {
            get => getAttribute(EAttributeName.FormTarget).Get_String();
            set
            {
                CEReactions.Wrap_CEReaction(this, () =>
                {
                    switch (type)
                    {
                        case EInputType.Submit:
                        case EInputType.Image:
                            break;
                        default:
                            {
                                if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                                else return;
                            }
                    }

                    if (HTMLCommon.Is_Valid_Browsing_Context_Name_Or_Keyword(value.AsMemory()))
                    {
                        throw new DOMException($"formtarget cannot accept the invalid value: \"{value}\"");
                    }

                    setAttribute(EAttributeName.FormAction, AttributeValue.From_String(value));
                });
            }
        }

        [CEReactions]
        public uint height
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#dom-input-height */
            get
            {
                switch (type)
                {
                    case EInputType.Image:
                        break;
                    default:
                        return 0;
                }

                /* =====================================================================
                 * XXX: WIDTH & HEIGHT FOR IMAGES IS THE IMAGE BOUNDS, SEE DOCUMENTATION
                 * =====================================================================
                 */
                return 0;
            }
            set
            {
                CEReactions.Wrap_CEReaction(this, () =>
                {
                    switch (type)
                    {
                        case EInputType.Image:
                            break;
                        default:
                            {
                                if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                                else return;
                            }
                    }

                    setAttribute(EAttributeName.Height, AttributeValue.From_Integer(value));
                });
            }
        }

        public HTMLDataListElement list
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#concept-input-list */
            get
            {
                /* The suggestions source element is the first element in the tree in tree order to have an ID equal to the value of the list attribute, if that element is a datalist element. 
                 * If there is no list attribute, or if there is no element with that ID, or if the first element with that ID is not a datalist element, then there is no suggestions source element. */
                if (hasAttribute(EAttributeName.List, out Attr outList))
                {
                    string idValue = outList?.Value?.Get_String();
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
            private set
            {
                switch (type)
                {
                    case EInputType.Text:
                    case EInputType.Search:
                    case EInputType.Url:
                    case EInputType.Telephone:
                    case EInputType.Email:
                    case EInputType.Date:
                    case EInputType.Month:
                    case EInputType.Week:
                    case EInputType.Time:
                    case EInputType.Local:
                    case EInputType.Number:
                    case EInputType.Range:
                    case EInputType.Color:
                        break;
                    default:
                        {
                            if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                            else return;
                        }
                }

                setAttribute(EAttributeName.List, value == null ? null : AttributeValue.From_String(value?.id));
            }
        }

        [CEReactions]
        public string max
        {
            get => getAttribute(EAttributeName.Max)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () =>
            {
                switch (type)
                {
                    case EInputType.Date:
                    case EInputType.Month:
                    case EInputType.Week:
                    case EInputType.Time:
                    case EInputType.Local:
                    case EInputType.Number:
                    case EInputType.Range:
                        break;
                    default:
                        {
                            if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                            else return;
                        }
                }
                setAttribute(EAttributeName.Max, AttributeValue.From_String(value));
            });
        }

        [CEReactions]
        public int maxLength
        {
            get => getAttribute(EAttributeName.MaxLength).Get_Int();
            set => CEReactions.Wrap_CEReaction(this, () =>
            {
                switch (type)
                {
                    case EInputType.Text:
                    case EInputType.Search:
                    case EInputType.Url:
                    case EInputType.Telephone:
                    case EInputType.Email:
                    case EInputType.Password:
                        break;
                    default:
                        {
                            if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                            else return;
                        }
                }
                setAttribute(EAttributeName.MaxLength, AttributeValue.From_Integer(value));
            });
        }

        [CEReactions]
        public string min
        {
            get => getAttribute(EAttributeName.Min)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () =>
            {
                switch (type)
                {
                    case EInputType.Date:
                    case EInputType.Month:
                    case EInputType.Week:
                    case EInputType.Time:
                    case EInputType.Local:
                    case EInputType.Number:
                    case EInputType.Range:
                        break;
                    default:
                        {
                            if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                            else return;
                        }
                }
                setAttribute(EAttributeName.Min, AttributeValue.From_String(value));
            });
        }

        [CEReactions]
        public int minLength
        {
            get => getAttribute(EAttributeName.MinLength).Get_Int();
            set => CEReactions.Wrap_CEReaction(this, () =>
            {
                switch (type)
                {
                    case EInputType.Text:
                    case EInputType.Search:
                    case EInputType.Url:
                    case EInputType.Telephone:
                    case EInputType.Email:
                    case EInputType.Password:
                        break;
                    default:
                        {
                            if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                            else return;
                        }
                }
                setAttribute(EAttributeName.MinLength, AttributeValue.From_Integer(value));
            });
        }

        [CEReactions]
        public bool multiple
        {
            get => hasAttribute(EAttributeName.Multiple);
            set => CEReactions.Wrap_CEReaction(this, () =>
            {
                switch (type)
                {
                    case EInputType.Email:
                    case EInputType.File:
                        break;
                    default:
                        {
                            if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                            else return;
                        }
                }
                toggleAttribute(EAttributeName.Multiple, value);
            });
        }

        [CEReactions]
        public string name
        {
            get => getAttribute(EAttributeName.Name).Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Name, AttributeValue.From_String(value)));
        }

        [CEReactions]
        public string pattern
        {
            get => getAttribute(EAttributeName.Pattern)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () =>
            {
                switch (type)
                {
                    case EInputType.Text:
                    case EInputType.Search:
                    case EInputType.Url:
                    case EInputType.Telephone:
                    case EInputType.Email:
                    case EInputType.Password:
                        break;
                    default:
                        {
                            if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                            else return;
                        }
                }

                patternCompiled = null;
                setAttribute(EAttributeName.Pattern, AttributeValue.From_String(value));
                /* The compiled pattern regular expression, when matched against a string, must have its start anchored to the start of the string and its end anchored to the end of the string. */
                /* This implies that the regular expression language used for this attribute is the same as that used in JavaScript, except that the pattern attribute is matched against the entire value, 
                 * not just any subset (somewhat as if it implied a ^(?: at the start of the pattern and a )$ at the end). */
                if (!string.IsNullOrEmpty(value))
                {
                    string patternStr = string.Concat("^(?:", value, ")$");
                    patternCompiled = new Regex(patternStr, RegexOptions.ECMAScript | RegexOptions.Compiled);
                }
            });
        }

        [CEReactions]
        public string placeholder
        {
            get => getAttribute(EAttributeName.Placeholder)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () =>
            {
                switch (type)
                {
                    case EInputType.Text:
                    case EInputType.Search:
                    case EInputType.Url:
                    case EInputType.Telephone:
                    case EInputType.Email:
                    case EInputType.Password:
                    case EInputType.Number:
                        break;
                    default:
                        {
                            if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                            else return;
                        }
                }
                setAttribute(EAttributeName.Placeholder, AttributeValue.From_String(value));
            });
        }

        [CEReactions]
        public bool readOnly
        {
            get => hasAttribute(EAttributeName.ReadOnly);
            set
            {
                CEReactions.Wrap_CEReaction(this, () =>
                {
                    switch (type)
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
                            break;
                        default:
                            {
                                if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                                else return;
                            }
                    }

                    toggleAttribute(EAttributeName.ReadOnly, value);
                });
            }
        }

        [CEReactions]
        public bool required
        {
            get => hasAttribute(EAttributeName.Required);
            set => CEReactions.Wrap_CEReaction(this, () =>
            {
                switch (type)
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
                    case EInputType.Checkbox:
                    case EInputType.Radio:
                    case EInputType.File:
                        break;
                    default:
                        {
                            if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                            else return;
                        }
                }
                toggleAttribute(EAttributeName.Required, value);
            });
        }

        [CEReactions]
        public uint size
        {
            get => getAttribute(EAttributeName.Size).Get_UInt();
            set => CEReactions.Wrap_CEReaction(this, () =>
            {
                switch (type)
                {
                    case EInputType.Text:
                    case EInputType.Search:
                    case EInputType.Url:
                    case EInputType.Telephone:
                    case EInputType.Email:
                    case EInputType.Password:
                    case EInputType.Image:
                        break;
                    default:
                        {
                            if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                            else return;
                        }
                }
                setAttribute(EAttributeName.Size, AttributeValue.From_Integer(value));
            });
        }

        [CEReactions]
        public string src
        {
            get => getAttribute(EAttributeName.Src)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () =>
            {
                switch (type)
                {
                    case EInputType.Image:
                        break;
                    default:
                        {
                            if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                            else return;
                        }
                }
                setAttribute(EAttributeName.Src, AttributeValue.From_String(value));
            });
        }

        [CEReactions]
        public new EInputType type
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#dom-input-type */
            get => getAttribute(EAttributeName.Type).Get_Enum<EInputType>();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Type, AttributeValue.From_Enum(value)));
        }

        [CEReactions]
        public string defaultValue
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#dom-input-defaultvalue */
            get => getAttribute(EAttributeName.Value)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Value, AttributeValue.From_String(value)));
        }

        [CEReactions]
        public uint width
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#dom-input-width */
            get
            {
                switch (type)
                {
                    case EInputType.Image:
                        break;
                    default:
                        return 0;
                }

                /* =====================================================================
                 * XXX: WIDTH & HEIGHT FOR IMAGES IS THE IMAGE BOUNDS, SEE DOCUMENTATION
                 * =====================================================================
                 */
                return 0;
            }
            set
            {
                CEReactions.Wrap_CEReaction(this, () =>
                {
                    switch (type)
                    {
                        case EInputType.Image:
                            break;
                        default:
                            {
                                if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                                else return;
                            }
                    }

                    setAttribute(EAttributeName.Width, AttributeValue.From_Integer(value));
                });
            }
        }

        public override HTMLFormElement form
        {
            get => base.form;
            set
            {
                base.form = value;
                update_radio_button_group();
            }
        }
        #endregion


        #region Input Step
        [CEReactions]
        public string step
        {
            get => getAttribute(EAttributeName.Step)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () =>
            {
                switch (type)
                {
                    case EInputType.Date:
                    case EInputType.Month:
                    case EInputType.Week:
                    case EInputType.Time:
                    case EInputType.Local:
                    case EInputType.Number:
                    case EInputType.Range:
                    case EInputType.Color:
                        break;
                    default:
                        {
                            if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This attribute cannot be specified for an input element of this type({Enum.GetName(typeof(EInputType), type)})");
                            else return;
                        }
                }
                setAttribute(EAttributeName.Step, AttributeValue.From_String(value));
            });
        }

        double default_step
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#concept-input-step */
            get
            {
                switch (type)
                {
                    case EInputType.Date:
                    case EInputType.Month:
                    case EInputType.Week:
                        return 1;
                    case EInputType.Time:
                    case EInputType.Local:
                        return 60;
                    case EInputType.Number:
                    case EInputType.Range:
                        return 1;
                    default:
                        return double.NaN;
                }
            }
        }

        double? default_step_base
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#concept-input-step */
            get
            {
                switch (type)
                {
                    case EInputType.Week:
                        return -259200000.0;
                    default:
                        return null;
                }
            }
        }

        double step_scale_factor
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#concept-input-step */
            get
            {
                switch (type)
                {
                    case EInputType.Date:
                        return 86400000;
                    case EInputType.Month:
                        return 1;
                    case EInputType.Week:
                        return 604800000;
                    case EInputType.Time:
                    case EInputType.Local:
                        return 1000;
                    case EInputType.Number:
                    case EInputType.Range:
                        return 1;
                    default:
                        return double.NaN;
                }
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
                if (!HTMLParserCommon.Parse_FloatingPoint(value.AsMemory(), out double outParsed) || MathExt.Flteq(outParsed, 0.0))
                {
                    return default_step * step_scale_factor;
                }

                /* 4) Otherwise, the allowed value step is the number returned by the rules for parsing floating-point number values when they are applied to the attribute's value, multiplied by the step scale factor. */
                return outParsed * step_scale_factor;
            }
        }


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
                    {
                        if (THROW_FOR_METHOD_INVALID_TYPES) throw new InvalidStateError($"The input-type({Enum.GetName(typeof(EInputType), type)}) does not implement the stepUp() method");
                        else return;
                    }
            }

            if (!allowed_value_step.HasValue)
            {
                throw new InvalidStateError("Input element has no allowed-value-step, step cannot be \"any\"");
            }
            var allowed_step = allowed_value_step.Value;

            /* 3) If the element has a minimum and a maximum and the minimum is greater than the maximum, then return. */
            var min = get_minimum();
            var max = get_maximum();
            if (min.HasValue && max.HasValue && min > max)
            {
                if (min > max)
                    return;

                /* 4) If the element has a minimum and a maximum and there is no value greater than or equal to the element's minimum and less than or equal to the element's maximum that, 
                 * when subtracted from the step base, is an integral multiple of the allowed value step, then return. */
                var mn = step_base - min.Value;
                var mx = step_base - max.Value;
                var minMult = allowed_step / mn;
                var maxMult = allowed_step / mx;
                var delta = maxMult - minMult;
                if ((int)delta == 0) // these two values fall within the same multiple of the allowed-step
                {
                    if (!MathExt.Feq(0, allowed_step % mn))
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

            /* 6) Let valueBeforeStepping be value. */
            var valueBeforeStepping = value;
            /* 7) If value subtracted from the step base is not an integral multiple of the allowed value step, 
             * then set value to the nearest value that, when subtracted from the step base, is an integral multiple of the allowed value step, 
             * and that is less than value if the method invoked was the stepDown() method, and more than value otherwise. */
            double deltaValueStepBase = step_base - value;
            double mult = allowed_step / value;
            if (!MathExt.Feq(0, mult))
            {
                value = step_base + Math.Floor(mult) * allowed_step;
                if (value < valueBeforeStepping)
                {
                    value += allowed_step;
                }
            }
            /* Otherwise (value subtracted from the step base is an integral multiple of the allowed value step): */
            else
            {
                /* 2) Let delta be the allowed value step multiplied by n. */
                var delta = allowed_step * n;
                /* 3) If the method invoked was the stepDown() method, negate delta. */
                /* 4) Let value be the result of adding delta to value. */
                value += delta;
            }

            /* 8) If the element has a minimum, and value is less than that minimum, 
             * then set value to the smallest value that, when subtracted from the step base, 
             * is an integral multiple of the allowed value step, and that is more than or equal to minimum. */
            if (min.HasValue && value < min)
            {
                var diffMin = min.Value - value;
                var diffMult = Math.Ceiling(allowed_step / diffMin);
                value += allowed_step * diffMult;
            }

            /* 9) If the element has a maximum, and value is greater than that maximum, 
             * then set value to the largest value that, when subtracted from the step base, 
             * is an integral multiple of the allowed value step, and that is less than or equal to maximum. */
            if (max.HasValue && value > max)
            {
                var diffMax = max.Value - value;
                var diffMult = Math.Ceiling(allowed_step / diffMax);
                value -= allowed_step * diffMult;
            }

            /* 10) If either the method invoked was the stepDown() method and value is greater than valueBeforeStepping, 
             * or the method invoked was the stepUp() method and value is less than valueBeforeStepping, then return. */
            if (value < valueBeforeStepping)
                return;

            /* 11) Let value as string be the result of running the algorithm to convert a number to a string, as defined for the input element's type attribute's current state, on value. */
            convert_number_to_string(value, out string valueAsString);
            this.value = valueAsString;
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
                    {
                        if (THROW_FOR_METHOD_INVALID_TYPES) throw new InvalidStateError($"The input-type({Enum.GetName(typeof(EInputType), type)}) does not implement the stepUp() method");
                        else return;
                    }
            }

            if (!allowed_value_step.HasValue)
            {
                throw new InvalidStateError("Input element has no allowed-value-step, step cannot be \"any\"");
            }
            var allowed_step = allowed_value_step.Value;

            /* 3) If the element has a minimum and a maximum and the minimum is greater than the maximum, then return. */
            var min = get_minimum();
            var max = get_maximum();
            if (min.HasValue && max.HasValue && min > max)
            {
                if (min > max)
                    return;

                /* 4) If the element has a minimum and a maximum and there is no value greater than or equal to the element's minimum and less than or equal to the element's maximum that, 
                 * when subtracted from the step base, is an integral multiple of the allowed value step, then return. */
                var mn = step_base - min.Value;
                var mx = step_base - max.Value;
                var minMult = allowed_step / mn;
                var maxMult = allowed_step / mx;
                var delta = maxMult - minMult;
                if ((int)delta == 0) // these two values fall within the same multiple of the allowed-step
                {
                    if (!MathExt.Feq(0, allowed_step % mn))
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

            /* 6) Let valueBeforeStepping be value. */
            var valueBeforeStepping = value;

            /* 7) If value subtracted from the step base is not an integral multiple of the allowed value step, 
             * then set value to the nearest value that, when subtracted from the step base, is an integral multiple of the allowed value step, 
             * and that is less than value if the method invoked was the stepDown() method, and more than value otherwise. */
            double deltaValueStepBase = step_base - value;
            double mult = allowed_step / value;
            if (!MathExt.Feq(0, mult))
            {
                value = step_base + Math.Floor(mult) * allowed_step;
                if (value > valueBeforeStepping)
                {
                    value -= allowed_step;
                }
            }
            /* Otherwise (value subtracted from the step base is an integral multiple of the allowed value step): */
            else
            {
                /* 2) Let delta be the allowed value step multiplied by n. */
                var delta = allowed_step * n;
                /* 3) If the method invoked was the stepDown() method, negate delta. */
                /* 4) Let value be the result of adding delta to value. */
                value -= delta;
            }

            /* 8) If the element has a minimum, and value is less than that minimum, 
             * then set value to the smallest value that, when subtracted from the step base, 
             * is an integral multiple of the allowed value step, and that is more than or equal to minimum. */
            if (min.HasValue && value < min)
            {
                var diffMin = min.Value - value;
                var diffMult = Math.Ceiling(allowed_step / diffMin);
                value += allowed_step * diffMult;
            }

            /* 9) If the element has a maximum, and value is greater than that maximum, 
             * then set value to the largest value that, when subtracted from the step base, 
             * is an integral multiple of the allowed value step, and that is less than or equal to maximum. */
            if (max.HasValue && value > max)
            {
                var diffMax = max.Value - value;
                var diffMult = Math.Ceiling(allowed_step / diffMax);
                value -= allowed_step * diffMult;
            }

            /* 10) If either the method invoked was the stepDown() method and value is greater than valueBeforeStepping, 
             * or the method invoked was the stepUp() method and value is less than valueBeforeStepping, then return. */
            if (value > valueBeforeStepping)
                return;

            /* 11) Let value as string be the result of running the algorithm to convert a number to a string, as defined for the input element's type attribute's current state, on value. */
            convert_number_to_string(value, out string valueAsString);
            this.value = valueAsString;
        }
        #endregion


        #region Input Value
        private enum EInputValueMode { Value, Default, Default_ON, Filename };
        private static EInputValueMode Get_Input_Value_Mode(EInputType type)
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

        [CEReactions]
        public new string value
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#dom-input-value */
            get
            {
                /* If the name attribute is present and has a value that is a case-sensitive match for the string "_charset_", then the element's value attribute must be omitted. */
                if (type == EInputType.Hidden && hasAttribute(EAttributeName.Name, out Attr nameAttr))
                {
                    if (nameAttr.Value.Get_String().AsSpan().Equals("_charset_".AsSpan(), StringComparison.OrdinalIgnoreCase))
                    {
                        return string.Empty;
                    }
                }


                switch (Get_Input_Value_Mode(type))
                {
                    case EInputValueMode.Value:
                        {
                            return _value ?? string.Empty;
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
                CEReactions.Wrap_CEReaction(this, () =>
                {
                    switch (Get_Input_Value_Mode(type))
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

                });
            }
        }

        /// <summary>
        /// Returns all of the input elements specified individual values
        /// </summary>
        /// <returns></returns>
        private string[] get_values()
        {
            if (multiple)
            {
                switch (type)
                {
                    case EInputType.File:
                        return selected_files.Select(o => o.name).ToArray();
                    case EInputType.Email:
                        return DOMCommon.Parse_Comma_Seperated_List(value.AsMemory()).Select(o => o.ToString()).ToArray();
                }
            }

            return new string[1] { value };
        }
        #endregion


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

            /* default minimum */
            switch (type)
            {
                case EInputType.Range:
                    return 0;
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

            /* default maximum */
            switch (type)
            {
                case EInputType.Range:
                    return 100;
                default:
                    return null;
            }
        }

        static DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        bool convert_string_to_number(ReadOnlyMemory<char> input, out double outValue)
        {
            switch (type)
            {
                case EInputType.Date:
                    {
                        if (!HTMLParserCommon.Parse_Date_String(input, out DateTime outDate))
                        {
                            outValue = double.NaN;
                            return false;
                        }

                        outValue = (outDate - EPOCH).TotalMilliseconds;
                        return true;
                    }
                case EInputType.Month:
                    {
                        if (!HTMLParserCommon.Parse_Month_String(input, out int year, out int month))
                        {
                            outValue = double.NaN;
                            return false;
                        }

                        var date = new DateTime(year, month, 1);
                        int totalMonths = GetTotalMonths(EPOCH, date);
                        outValue = totalMonths;
                        return true;
                    }
                case EInputType.Week:
                    {
                        if (!HTMLParserCommon.Parse_Week_String(input, out int week, out int year))
                        {
                            outValue = double.NaN;
                            return false;
                        }

                        var days = week * 7;
                        var date = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(days - 1);
                        var delta = date.Subtract(EPOCH);

                        outValue = delta.TotalMilliseconds;
                        return true;
                    }
                case EInputType.Time:
                    {
                        if (!HTMLParserCommon.Parse_Time_String(input, out TimeSpan time))
                        {
                            outValue = double.NaN;
                            return false;
                        }

                        outValue = time.TotalMilliseconds;
                        return true;
                    }
                case EInputType.Local:
                    {
                        if (!HTMLParserCommon.Parse_Local_Date_Time_String(input, out DateTime date))
                        {
                            outValue = double.NaN;
                            return false;
                        }
                        var delta = date.Subtract(EPOCH);

                        outValue = delta.TotalMilliseconds;
                        return true;
                    }
                case EInputType.Number:
                case EInputType.Range:
                    {
                        if (!HTMLParserCommon.Parse_FloatingPoint(input, out double value))
                        {
                            outValue = double.NaN;
                            return false;
                        }

                        outValue = value;
                        return true;
                    }
                default:
                    {
                        outValue = double.NaN;
                        return false;
                    }
            }
        }
        bool convert_number_to_string(double num, out string outValue)
        {
            switch (type)
            {
                case EInputType.Date:
                    {
                        var date = EPOCH.AddMilliseconds(num);
                        outValue = date.ToString(HTMLParserCommon.DATE_TIME_FORMAT);

                        return true;
                    }
                case EInputType.Month:
                    {
                        var date = EPOCH.AddMonths((int)num);
                        outValue = date.ToString(HTMLParserCommon.MONTH_FORMAT);

                        return true;
                    }
                case EInputType.Week:
                    {
                        var delta = TimeSpan.FromMilliseconds(num);
                        var date = EPOCH.Add(delta);
                        var week = date.DayOfYear / 7;
                        outValue = date.ToString($"yyyy-W{week}");

                        return true;
                    }
                case EInputType.Time:
                    {
                        var time = TimeSpan.FromMilliseconds(num);
                        outValue = time.ToString(HTMLParserCommon.TIME_FORMAT);

                        return true;
                    }
                case EInputType.Local:
                    {
                        var delta = TimeSpan.FromMilliseconds(num);
                        var date = EPOCH.Add(delta);

                        outValue = date.ToString(HTMLParserCommon.LOCAL_DATE_TIME_FORMAT);
                        return true;
                    }
                case EInputType.Number:
                case EInputType.Range:
                    {
                        outValue = num.ToString();
                        return true;
                    }
                default:
                    {
                        outValue = null;
                        return false;
                    }
            }
        }
        bool convert_string_to_date(ReadOnlyMemory<char> input, out DateTime outValue)
        {
            switch (type)
            {
                case EInputType.Date:
                    {
                        if (!HTMLParserCommon.Parse_Date_String(input, out DateTime outDate))
                        {
                            outValue = DateTime.MinValue;
                            return false;
                        }

                        outValue = outDate;
                        return true;
                    }
                case EInputType.Month:
                    {
                        if (!HTMLParserCommon.Parse_Month_String(input, out int year, out int month))
                        {
                            outValue = DateTime.MinValue;
                            return false;
                        }

                        outValue = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
                        return true;
                    }
                case EInputType.Week:
                    {
                        if (!HTMLParserCommon.Parse_Week_String(input, out int week, out int year))
                        {
                            outValue = DateTime.MinValue;
                            return false;
                        }

                        var days = week * 7;
                        outValue = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(days - 1);
                        return true;
                    }
                case EInputType.Time:
                    {
                        if (!HTMLParserCommon.Parse_Time_String(input, out TimeSpan time))
                        {
                            outValue = DateTime.MinValue;
                            return false;
                        }
                        outValue = EPOCH.Add(time);

                        return true;
                    }
                default:
                    {
                        outValue = DateTime.MinValue;
                        return false;
                    }
            }
        }
        bool convert_date_to_string(DateTimeOffset date, out string outValue)
        {
            switch (type)
            {
                case EInputType.Date:
                    {
                        outValue = date.ToString(HTMLParserCommon.DATE_TIME_FORMAT);
                        return true;
                    }
                case EInputType.Month:
                    {
                        outValue = date.ToString(HTMLParserCommon.MONTH_FORMAT);
                        return true;
                    }
                case EInputType.Week:
                    {
                        var week = date.DayOfYear / 7;
                        outValue = date.ToString($"yyyy-W{week}");
                        return true;
                    }
                case EInputType.Time:
                    {
                        outValue = date.ToString(HTMLParserCommon.TIME_FORMAT);
                        return true;
                    }
                default:
                    {
                        outValue = null;
                        return false;
                    }
            }
        }
        void run_value_sanitization()
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#value-sanitization-algorithm */
            switch (type)
            {
                case EInputType.Text:
                case EInputType.Search:
                    {/* The value sanitization algorithm is as follows: Strip newlines from the value. */
                        _value = StringCommon.Replace(_value.AsMemory(), FilterCRLF.Instance, string.Empty.AsSpan());
                    }
                    break;
                case EInputType.Telephone:
                    {/* The value sanitization algorithm is as follows: Strip newlines from the value. */
                        _value = StringCommon.Replace(_value.AsMemory(), FilterCRLF.Instance, string.Empty.AsSpan());
                    }
                    break;
                case EInputType.Url:
                    {/* The value sanitization algorithm is as follows: Strip newlines from the value, then strip leading and trailing ASCII whitespace from the value. */
                        _value = StringCommon.Replace(_value.AsMemory(), FilterCRLF.Instance, string.Empty.AsSpan()).Trim();
                    }
                    break;
                case EInputType.Email:
                    {
                        if (!multiple)
                        {/* The value sanitization algorithm is as follows: Strip newlines from the value, then strip leading and trailing ASCII whitespace from the value. */
                            _value = StringCommon.Replace(_value.AsMemory(), FilterCRLF.Instance, string.Empty.AsSpan()).Trim();
                        }
                        else
                        {/* The value sanitization algorithm is as follows: */
                         /* 1) Split on commas the element's value, strip leading and trailing ASCII whitespace from each resulting token, 
                          * if any, and let the element's values be the (possibly empty) resulting list of (possibly empty) tokens, maintaining the original order. */
                            var list = DOMCommon.Parse_Comma_Seperated_List(_value.AsMemory());
                            var newList = new ReadOnlyMemory<char>[list.Count];
                            for (int i = 0; i < list.Count; i++)
                            {
                                newList[i] = StringCommon.Trim(list[i], UnicodeCommon.CHAR_SPACE);
                            }
                            /* 2) Let the element's value be the result of concatenating the element's values, separating each value from the next by a single U+002C COMMA character (,), maintaining the list's order. */
                            _value = DOMCommon.Serialize_Comma_Seperated_list(newList);
                        }
                    }
                    break;
                case EInputType.Password:
                    {/* The value sanitization algorithm is as follows: Strip newlines from the value. */
                        _value = StringCommon.Replace(_value.AsMemory(), FilterCRLF.Instance, string.Empty.AsSpan());
                    }
                    break;
                case EInputType.Date:
                    {
                        if (!HTMLParserCommon.Is_Valid_Date_String(_value.AsMemory()))
                            _value = string.Empty;
                    }
                    break;
                case EInputType.Month:
                    {
                        if (!HTMLParserCommon.Is_Valid_Month_String(_value.AsMemory()))
                            _value = string.Empty;
                    }
                    break;
                case EInputType.Week:
                    {
                        if (!HTMLParserCommon.Is_Valid_Week_String(_value.AsMemory()))
                            _value = string.Empty;
                    }
                    break;
                case EInputType.Time:
                    {
                        if (!HTMLParserCommon.Is_Valid_Time_String(_value.AsMemory()))
                            _value = string.Empty;
                    }
                    break;
                case EInputType.Local:
                    {
                        if (!HTMLParserCommon.Parse_Local_Date_Time_String(_value.AsMemory(), out DateTime dateTime))
                            _value = dateTime.ToString(HTMLParserCommon.LOCAL_DATE_TIME_FORMAT);
                        else
                            _value = string.Empty;
                    }
                    break;
                case EInputType.Number:
                    {
                        if (!HTMLParserCommon.Is_Valid_FloatingPoint(_value.AsMemory()))
                            _value = string.Empty;
                    }
                    break;
                case EInputType.Range:
                    {
                        if (!HTMLParserCommon.Is_Valid_FloatingPoint(_value.AsMemory()))
                        {
                            var min = get_minimum().Value;
                            var max = get_maximum().Value;
                            if (min < max)
                            {
                                _value = (min + (max - min) * 0.5).ToString();
                            }
                            else
                            {
                                _value = min.ToString();
                            }
                        }
                    }
                    break;
                case EInputType.Color:
                    {
                        if (HTMLParserCommon.Is_Valid_Simple_Color(_value.AsMemory()))
                        {
                            _value = StringCommon.Transform(_value.AsMemory(), UnicodeCommon.To_ASCII_Lower_Alpha);
                        }
                        else
                        {
                            _value = "#000000";
                        }
                    }
                    break;
            }
        }
        #endregion


        #region Form-Associated Element Overrides
        internal override EValidityState query_validity()
        {
            EValidityState flags = base.query_validity();
            string cValue = this.value;/* Cache this so we dont keep resolving it */

            /* When a control has no value but has a required attribute (input required, textarea required); or, more complicated rules for select elements and controls in radio button groups, as specified in their sections. */
            switch (type)
            {
                case EInputType.Url:
                    {/* Constraint validation: While the value of the element is neither the empty string nor a valid absolute URL, the element is suffering from a type mismatch. */
                        string val = cValue;
                        if (val.Length > 0 && !Uri.IsWellFormedUriString(val, UriKind.Absolute))
                        {
                            flags |= EValidityState.typeMismatch;
                        }
                    }
                    break;
                case EInputType.Email:
                    {
                        if (!multiple)
                        {/* Constraint validation: While the value of the element is neither the empty string nor a single valid e-mail address, the element is suffering from a type mismatch. */
                            string val = cValue;
                            if (val.Length > 0 && !HTMLCommon.Is_Valid_Email(val))
                            {
                                flags |= EValidityState.typeMismatch;
                            }

                            /* Constraint validation: While the user interface is representing input that the user agent cannot convert to punycode, the control is suffering from bad input. */
                            if (!HTMLCommon.Is_Valid_Punycode(val))
                            {
                                flags |= EValidityState.badInput;
                            }
                        }
                        else
                        {/* Constraint validation: While the value of the element is not a valid e-mail address list, the element is suffering from a type mismatch. */
                            var emailList = DOMCommon.Parse_Comma_Seperated_List(cValue.AsMemory());
                            for (int i = 0; i < emailList.Count; i++)
                            {
                                string emailStr = emailList[i].ToString();
                                /* Constraint validation: While the user interface describes a situation where an individual value contains a U+002C COMMA (,) or is representing input that the user agent cannot convert to punycode, the control is suffering from bad input. */
                                if (StringCommon.Contains(emailList[i].Span, UnicodeCommon.CHAR_COMMA) || !HTMLCommon.Is_Valid_Punycode(emailStr))
                                {
                                    flags |= EValidityState.badInput;
                                }

                                if (!HTMLCommon.Is_Valid_Email(emailStr))
                                {
                                    flags |= EValidityState.typeMismatch;
                                    break;
                                }
                            }
                        }
                    }
                    break;
                case EInputType.Date:
                    {
                        if (!HTMLParserCommon.Is_Valid_Date_String(cValue.AsMemory()))
                        {
                            flags |= EValidityState.badInput;
                        }
                    }
                    break;
                case EInputType.Month:
                    {
                        if (!HTMLParserCommon.Is_Valid_Month_String(cValue.AsMemory()))
                        {
                            flags |= EValidityState.badInput;
                        }
                    }
                    break;
                case EInputType.Week:
                    {
                        if (!HTMLParserCommon.Is_Valid_Week_String(cValue.AsMemory()))
                        {
                            flags |= EValidityState.badInput;
                        }
                    }
                    break;
                case EInputType.Time:
                    {
                        if (!HTMLParserCommon.Is_Valid_Time_String(cValue.AsMemory()))
                        {
                            flags |= EValidityState.badInput;
                        }
                    }
                    break;
                case EInputType.Local:
                    {
                        if (!HTMLParserCommon.Is_Valid_Normalized_Local_Date_Time_String(cValue.AsMemory()))
                        {
                            flags |= EValidityState.badInput;
                        }
                    }
                    break;
                case EInputType.Number:
                    {
                        if (!HTMLParserCommon.Is_Valid_FloatingPoint(cValue.AsMemory()))
                        {
                            flags |= EValidityState.badInput;
                        }
                    }
                    break;
                case EInputType.Range:
                    {
                        if (!HTMLParserCommon.Is_Valid_FloatingPoint(cValue.AsMemory()))
                        {
                            flags |= EValidityState.badInput;
                        }
                    }
                    break;
                case EInputType.Color:
                    {
                        if (!HTMLParserCommon.Is_Valid_Lowercase_Simple_Color(cValue.AsMemory()))
                        {
                            flags |= EValidityState.badInput;
                        }
                    }
                    break;
                case EInputType.Checkbox:
                    {
                        if (required && !checkedness)
                        {
                            flags |= EValidityState.valueMissing;
                        }
                    }
                    break;
                case EInputType.Radio:
                    {/* Constraint validation: If an element in the radio button group is required, and all of the input elements in the radio button group have a checkedness that is false, then the element is suffering from being missing. */
                        if (required && !Checked)
                        {
                            var group = Get_Radio_Button_Group();
                            bool missing = true;
                            foreach (var input in group)
                            {
                                if (input.Checked)
                                {
                                    missing = false;
                                    break;
                                }
                            }

                            if (missing)
                            {
                                flags |= EValidityState.valueMissing;
                            }
                        }
                    }
                    break;
                case EInputType.File:
                    {
                        if (required && selected_files.Count <= 0)
                        {
                            flags |= EValidityState.valueMissing;
                        }
                    }
                    break;
                default:
                    {
                        if (ReferenceEquals(null, cValue) || cValue.Length <= 0)
                        {
                            flags |= EValidityState.valueMissing;
                        }
                    }
                    break;
            }

            /* Constraint validation: If the element is required, and its value IDL attribute applies and is in the mode value, and the element is mutable, and the element's value is the empty string, then the element is suffering from being missing. */
            if (0 == (flags & EValidityState.valueMissing))
            {
                if (Get_Input_Value_Mode(type) == EInputValueMode.Value && isMutable && cValue.Length <= 0)
                {
                    flags |= EValidityState.valueMissing;
                }
            }
            /* When a control has a value that is too long for the form control maxlength attribute (input maxlength, textarea maxlength). */
            if (cValue.Length > maxLength)
            {
                flags |= EValidityState.tooLong;
            }

            /* When a control has a value that is too short for the form control minlength attribute (input minlength, textarea minlength). */
            if (cValue.Length < minLength)
            {
                flags |= EValidityState.tooShort;
            }

            /* Constraint validation: When the element has an allowed value step, 
             * and the result of applying the algorithm to convert a string to a number to the string given by the element's value is a number, 
             * and that number subtracted from the step base is not an integral multiple of the allowed value step, 
             * the element is suffering from a step mismatch. */
            if (allowed_value_step.HasValue)
            {
                if (convert_string_to_number(cValue.AsMemory(), out double outValueNumber))
                {
                    double stepSize = allowed_value_step.Value;
                    double delta = outValueNumber - step_base;
                    double remainder = stepSize % delta;
                    if (remainder > 0)
                    {
                        flags |= EValidityState.stepMismatch;
                        switch (type)
                        {
                            case EInputType.Range:
                                {
                                    /* When the element is suffering from a step mismatch, the user agent must round the element's value to the nearest number for which the element would not suffer from a step mismatch, 
                                     * and which is greater than or equal to the minimum, and, if the maximum is not less than the minimum, 
                                     * which is less than or equal to the maximum, if there is a number that matches these constraints. If two numbers match these constraints, 
                                     * then user agents must use the one nearest to positive infinity. */
                                    double multiple = Math.Floor(stepSize / delta);

                                    if (MathExt.Fgteq(remainder, 0.5d))// step up is closest
                                    {
                                        double max = get_maximum().Value;
                                        double upStep = (multiple + 1) * stepSize;
                                        if (upStep <= max)
                                        {
                                            cValue = upStep.ToString();
                                            break;
                                        }
                                    }

                                    double min = get_maximum().Value;
                                    double noStep = multiple * stepSize;
                                    if (noStep >= min)
                                    {
                                        cValue = noStep.ToString();
                                        break;
                                    }
                                }
                                break;
                        }
                    }
                }
            }


            if (!ReferenceEquals(null, cValue) && cValue.Length > 0)/* Value is not null or empty string */
            {
                if (convert_string_to_number(cValue.AsMemory(), out double outValue))
                {
                    switch (type)
                    {
                        case EInputType.Date:
                        case EInputType.Month:
                        case EInputType.Week:
                        case EInputType.Time:
                        case EInputType.Local:
                        case EInputType.Number:
                            {
                                /* When a control has a value that is not the empty string and is too low for the min attribute. */
                                var min = get_minimum();
                                if (min.HasValue && outValue < min.Value)
                                {
                                    flags |= EValidityState.rangeUnderflow;
                                }

                                /* When a control has a value that is not the empty string and is too high for the max attribute. */
                                var max = get_maximum();
                                if (max.HasValue && outValue > max.Value)
                                {
                                    flags |= EValidityState.rangeOverflow;
                                }
                            }
                            break;
                        case EInputType.Range:
                            {
                                /* When a control has a value that is not the empty string and is too low for the min attribute. */
                                var min = get_minimum();
                                if (min.HasValue && outValue < min.Value)
                                {
                                    flags |= EValidityState.rangeUnderflow;
                                    cValue = min.Value.ToString();
                                }

                                /* When a control has a value that is not the empty string and is too high for the max attribute. */
                                var max = get_maximum();
                                if (max.HasValue && outValue > max.Value)
                                {
                                    flags |= EValidityState.rangeOverflow;
                                    cValue = max.Value.ToString();
                                }
                            }
                            break;
                    }
                }

                if (patternCompiled != null)
                {
                    if (!multiple)
                    {
                        if (!patternCompiled.IsMatch(cValue))
                        {
                            flags |= EValidityState.patternMismatch;
                        }
                    }
                    else
                    {
                        foreach (string item in get_values())
                        {
                            if (!patternCompiled.IsMatch(item))
                            {
                                flags |= EValidityState.patternMismatch;
                                break;
                            }
                        }
                    }
                }
            }

            return flags;
        }
        #endregion


        #region Resettable
        public void Reset()
        {/* Docs: https://html.spec.whatwg.org/multipage/input.html#the-input-element:concept-form-reset-control */
            bDirtyValueFlag = false;
            bDirtyCheckednessFlag = false;
            _value = defaultValue ?? string.Empty;
            checkedness = hasAttribute(EAttributeName.Checked);
            selected_files.Clear();
            run_value_sanitization();
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
                        {
                            if (THROW_FOR_METHOD_INVALID_TYPES) throw new InvalidStateError($"The input element type({Enum.GetName(typeof(EInputType), type)}) does not support this function");
                            else return;
                        }
                }
            }

            if (!StringCommon.Contains(value.AsSpan(), FilterCharSelectable.Instance))
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
                            {
                                if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This property is not valid for input elements of type \"{Lookup.Keyword(inputElement.type)}\"");
                                else return;
                            }
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
                            {
                                if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This property is not valid for input elements of type \"{Lookup.Keyword(inputElement.type)}\"");
                                else return;
                            }
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
                            {
                                if (THROW_FOR_ATTR_INVALID_TYPES) throw new InvalidStateError($"This property is not valid for input elements of type \"{Lookup.Keyword(inputElement.type)}\"");
                                else return;
                            }
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
                        {
                            if (THROW_FOR_METHOD_INVALID_TYPES) throw new InvalidStateError($"The input element type({Enum.GetName(typeof(EInputType), type)}) does not support this function");
                            else return;
                        }
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
                Task.Factory.StartNew(() =>
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
                        {
                            if (THROW_FOR_METHOD_INVALID_TYPES) throw new InvalidStateError($"The input element type({Enum.GetName(typeof(EInputType), type)}) does not support this function");
                            else return;
                        }
                }
            }

            bDirtyValueFlag = true;

            /* 3) If the method has only one argument, then let start and end have the values of the selectionStart attribute and the selectionEnd attribute respectively. */
            int? start = selectionStart;
            int? end = selectionEnd;
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
                relevantValue = relevantValue.Substring(start.Value, (end - start).Value - 1);
            }

            /* 10) Insert the value of the first argument into the text of the relevant value of the text control, immediately before the startth character. */
            relevantValue.Insert(start.Value, replacement);

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
                        {
                            if (THROW_FOR_METHOD_INVALID_TYPES) throw new InvalidStateError($"The input element type({Enum.GetName(typeof(EInputType), type)}) does not support this function");
                            else return;
                        }
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
                relevantValue = relevantValue.Substring(start, end - start - 1);
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


        #region Utility
        public override bool isMutable
        {
            get
            {
                if (disabled) return false;
                if (readOnly) return false;
                if (type == EInputType.Hidden) return false;

                return true;
            }
        }
        private static bool is_type_text_selectable(EInputType type)
        {
            switch (type)
            {
                case EInputType.Text:
                case EInputType.Search:
                case EInputType.Url:
                case EInputType.Telephone:
                case EInputType.Password:
                    return true;
                default:
                    return false;
            }
        }
        internal IReadOnlyCollection<HTMLInputElement> Get_Radio_Button_Group()
        {
            Node root = form;
            if (root == null) root = ownerDocument;
            if (root == null) return new HTMLInputElement[0];

            return DOMCommon.Get_Descendents_OfType<HTMLInputElement>(root, new FilterRadioGroup(this), ENodeFilterMask.SHOW_ELEMENT);
        }
        /// <summary>
        /// If this is a radio button input type and its checkedness is currently true then this function will set the checkedness of all other buttons within its group to false.
        /// </summary>
        private void update_radio_button_group()
        {
            if (_checkedness == true && type == EInputType.Radio)
            {/* ...the checkedness state of all the other elements in the same radio button group must be set to false: */
                var group = Get_Radio_Button_Group();
                foreach (var radioButton in group)
                {
                    radioButton.checkedness = false;
                }
            }
        }
        internal void signal_type_change()
        {
            if (type == EInputType.Radio)
            {
                update_radio_button_group();
            }
        }
        /// <summary>
        /// Returns the total months that occur between two dates. this method does not calculate the number of months using the total elapsed days between the dates, it assumes the two dates both start on the first day of their given month.
        /// </summary>
        private int GetTotalMonths(DateTime From, DateTime Till)
        {
            return (Till.Year - From.Year) * 12 + (Till.Month - From.Month);
        }
        #endregion


        #region Rendering
        /// <summary>
        /// Sets up the required states for rendering this element as a particular type
        /// </summary>
        private void update_rendering_for_type(EInputType type)
        {
            /* XXX: CSS Rendering */
        }
        #endregion


        #region Overrides
        internal override void run_cloning_steps(ref Node copy, Document document, bool clone_children = false)
        {
            base.run_cloning_steps(ref copy, document, clone_children);
            /* The cloning steps for input elements must propagate the value, dirty value flag, checkedness, and dirty checkedness flag from the node being cloned to the copy. */
            if (copy is HTMLInputElement otherInput)// of course it is an input element, we just need a cast and if we cast we might aswell check too
            {
                otherInput._value = _value;
                otherInput.bDirtyValueFlag = bDirtyValueFlag;
                otherInput.checkedness = checkedness;
                otherInput.bDirtyCheckednessFlag = bDirtyCheckednessFlag;
            }
        }

        internal override void run_attribute_change_steps(Element element, AtomicName<EAttributeName> localName, AttributeValue oldValue, AttributeValue newValue, string Namespace)
        {
            base.run_attribute_change_steps(element, localName, oldValue, newValue, Namespace);


            if (!localName.EnumValue.HasValue)
                return;
            switch (localName.EnumValue.Value)
            {
                case EAttributeName.Value:/* Universal logic */
                    {
                        /* When the value content attribute is added, set, or removed, 
                         * if the control's dirty value flag is false, the user agent must set the value of the element to the value of the value content attribute, 
                         * if there is one, or the empty string otherwise, and then run the current value sanitization algorithm, if one is defined. */
                        if (!bDirtyValueFlag)
                        {
                            _value = getAttribute(EAttributeName.Value)?.Get_String() ?? string.Empty;
                            run_value_sanitization();
                        }
                    }
                    break;
                case EAttributeName.Type:/* Universal logic */
                    {/* Docs: https://html.spec.whatwg.org/multipage/input.html#input-type-change */
                     /* 1) If the previous state of the element's type attribute put the value IDL attribute in the value mode, 
                      * and the element's value is not the empty string, and the new state of the element's type attribute puts the value IDL attribute in either the default mode or the default/on mode, 
                      * then set the element's value content attribute to the element's value. */
                        EInputType oldType = oldValue.Get_Enum<EInputType>();
                        EInputType newType = newValue.Get_Enum<EInputType>();
                        var oldMode = Get_Input_Value_Mode(oldType);
                        var newMode = Get_Input_Value_Mode(newType);
                        if (oldMode == EInputValueMode.Value && !string.IsNullOrEmpty(value) && (newMode == EInputValueMode.Default || newMode == EInputValueMode.Default_ON))
                        {
                            setAttribute(EAttributeName.Value, AttributeValue.From_String(_value));
                        }
                        /* 2) Otherwise, if the previous state of the element's type attribute put the value IDL attribute in any mode other than the value mode, 
                        * and the new state of the element's type attribute puts the value IDL attribute in the value mode, 
                        * then set the value of the element to the value of the value content attribute, 
                        * if there is one, or the empty string otherwise, and then set the control's dirty value flag to false. */
                        else if (oldMode != EInputValueMode.Value && newMode == EInputValueMode.Value)
                        {

                            _value = getAttribute(EAttributeName.Value)?.Get_String() ?? string.Empty;
                            bDirtyValueFlag = false;
                        }
                        /* 3) Otherwise, if the previous state of the element's type attribute put the value IDL attribute in any mode other than the filename mode, 
                         * and the new state of the element's type attribute puts the value IDL attribute in the filename mode, then set the value of the element to the empty string. */
                        else if (oldMode != EInputValueMode.Filename && newMode == EInputValueMode.Filename)
                        {
                            _value = string.Empty;
                        }
                        /* 4) Update the element's rendering and behavior to the new state's. */
                        update_rendering_for_type(newType);
                        /* 5) Signal a type change for the element. (The Radio Button state uses this, in particular.) */
                        signal_type_change();
                        /* 6) Invoke the value sanitization algorithm, if one is defined for the type attribute's new state. */
                        run_value_sanitization();
                        /* 7) Let previouslySelectable be true if setRangeText() previously applied to the element, and false otherwise. */
                        bool previouslySelectable = is_type_text_selectable(oldType);
                        /* 8) Let nowSelectable be true if setRangeText() now applies to the element, and false otherwise. */
                        bool nowSelectable = is_type_text_selectable(newType);
                        /* 9) If previouslySelectable is false and nowSelectable is true, set the element's text entry cursor position to the beginning of the text control, and set its selection direction to "none". */
                        if (!previouslySelectable && nowSelectable)
                        {
                            text_entry_cursor_position = 0;
                            selection.direction = ESelectionDirection.None;
                        }
                    }
                    break;
                case EAttributeName.Name:
                case EAttributeName.Form:
                    {
                        if (type == EInputType.Radio)
                        {
                            update_radio_button_group();
                        }
                    }
                    break;
                case EAttributeName.Multiple:
                    {
                        run_value_sanitization();
                    }
                    break;
            }

        }


        internal override bool has_activation_behaviour => true;
        internal override bool has_legacy_activation_behaviour => true;
        internal override void activation_behaviour(Event @event)
        {
            base.activation_behaviour(@event);

            /* 1) If this element is not mutable, then return. */
            if (!isMutable)
                return;

            /* 2) Run this element's input activation behavior, if any, and do nothing otherwise. */
            switch (type)
            {
                case EInputType.Checkbox:
                case EInputType.Radio:
                    {
                        Task.Factory.StartNew(() =>
                        {
                            dispatchEvent(new Event(EEventName.Input, new EventInit() { bubbles = true }));
                            dispatchEvent(new Event(EEventName.Change, new EventInit() { bubbles = true }));
                        });
                    }
                    break;
                case EInputType.File: /* Docs: https://html.spec.whatwg.org/multipage/input.html#file-upload-state-(type=file):input-activation-behavior */
                    {
                        if (!DOMCommon.Is_Triggered_By_UserActivation(@event))
                            return;

                        /* XXX: implement the logic laid out in the documentation @ the link */
                    }
                    break;
                case EInputType.Submit:
                    {
                        if (form != null && nodeDocument.Is_FullyActive)
                        {
                            FormCommon.Submit_Form(form, this);
                        }
                    }
                    break;
                case EInputType.Image: /* Docs: https://html.spec.whatwg.org/multipage/input.html#image-button-state-(type=image):input-activation-behavior */
                    {
                        /* XXX: implement the logic laid out in the documentation @ the link */
                    }
                    break;
                case EInputType.Reset:
                    {
                        if (form != null && nodeDocument.Is_FullyActive)
                        {
                            FormCommon.Reset_Form(form);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        HTMLInputElement legacyPreActivation_RadioRef = null;
        bool legacyPreActivation_Checkedness = false;
        bool legacyPreActivation_Indeterminate = false;
        internal override void legacy_pre_activation_behaviour()
        {
            base.legacy_pre_activation_behaviour();

            /* 1) If this element is not mutable, then return. */
            if (!isMutable)
                return;

            /* 2) If this element's type attribute is in the Checkbox state, 
             * then set this element's checkedness to its opposite value (i.e. true if it is false, false if it is true) and set this element's indeterminate IDL attribute to false. */
            if (type == EInputType.Checkbox)
            {
                legacyPreActivation_Checkedness = checkedness;
                legacyPreActivation_Indeterminate = indeterminate;

                checkedness = !checkedness;
                indeterminate = false;
            }
            /* 3) If this element's type attribute is in the Radio Button state, 
             * then get a reference to the element in this element's radio button group that has its checkedness set to true, if any, and then set this element's checkedness to true. */
            else if (type == EInputType.Radio)
            {
                var checkedElement = Get_Radio_Button_Group().FirstOrDefault(e => e.Checked);
                legacyPreActivation_RadioRef = checkedElement;
            }
        }

        internal override void legacy_canceled_pre_activation_behaviour()
        {
            base.legacy_canceled_pre_activation_behaviour();

            /* 1) If this element is not mutable, then return. */
            if (!isMutable)
                return;

            /* 2) If the element's type attribute is in the Checkbox state, 
             * then set the element's checkedness and the element's indeterminate IDL attribute back to the values they had before the legacy-pre-activation behavior was run. */
            if (type == EInputType.Checkbox)
            {
                checkedness = legacyPreActivation_Checkedness;
                indeterminate = legacyPreActivation_Indeterminate;
            }
            /* 3) If this element's type attribute is in the Radio Button state, 
             * then if the element to which a reference was obtained in the legacy-pre-activation behavior, 
             * if any, is still in what is now this element's radio button group, if it still has one, and if so, setting that element's checkedness to true; 
             * or else, if there was no such element, or that element is no longer in this element's radio button group, 
             * or if this element no longer has a radio button group, setting this element's checkedness to false. */
            else if (type == EInputType.Radio)
            {
                if (!ReferenceEquals(null, legacyPreActivation_RadioRef))
                {
                    var group = Get_Radio_Button_Group();
                    if (group.FirstOrDefault(o => ReferenceEquals(o, legacyPreActivation_RadioRef)) != null)
                    {
                        legacyPreActivation_RadioRef.checkedness = true;
                        return;
                    }
                }

                checkedness = false;
            }
        }
        #endregion
    }
}
