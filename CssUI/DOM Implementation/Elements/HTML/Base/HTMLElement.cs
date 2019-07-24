using CssUI.DOM.CustomElements;
using CssUI.DOM.Events;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Internal;
using CssUI.DOM.Nodes;
using CssUI.DOM.Serialization;

namespace CssUI.DOM
{
    /// <summary>
    /// The basic interface, from which all the HTML elements' interfaces inherit, and which must be used by elements that have no additional requirements, is the HTMLElement interface.
    /// </summary>
    public class HTMLElement : Element, IGlobalEventCallbacks, IDocumentAndElementEventCallbacks
    {/* Docs: https://html.spec.whatwg.org/multipage/dom.html#htmlelement */
        # region Metadata Attributes

        /// <summary>
        /// The lang attribute (in no namespace) specifies the primary language for the element's contents and for any of the element's attributes that contain text. 
        /// Its value must be a valid BCP 47 language tag, or the empty string. Setting the attribute to the empty string indicates that the primary language is unknown.
        /// </summary>
        [CEReactions]
        public string Title
        {
            get => getAttribute(EAttributeName.Title).Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Title, AttributeValue.From_String(value)));
        }

        [CEReactions]
        public string Lang
        {
            get => getAttribute(EAttributeName.Lang).Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Lang, AttributeValue.From_String(value)));
        }

        [CEReactions]
        public bool Translate
        {
            get => hasAttribute(EAttributeName.Translate);
            set => CEReactions.Wrap_CEReaction(this, () => toggleAttribute(EAttributeName.Translate, value));
        }

        /// <summary>
        /// The dir attribute specifies the element's text directionality.
        /// </summary>
        [CEReactions]
        public EDir Dir
        {
            get => getAttribute(EAttributeName.Dir).Get_Enum<EDir>();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Dir, AttributeValue.From_Enum(value)));
        }

        /// <summary>
        /// Resolves the directionality of the element
        /// </summary>
        public CSS.EDirection directionality
        {/* Docs: https://html.spec.whatwg.org/multipage/dom.html#the-directionality */
            get
            {
                /* If the element's dir attribute is in the ltr state
                 * If the element is a document element and the dir attribute is not in a defined state (i.e. it is not present or has an invalid value)
                 * If the element is an input element whose type attribute is in the Telephone state, and the dir attribute is not in a defined state (i.e. it is not present or has an invalid value)
                 */
                Attr attr = getAttributeNode(EAttributeName.Dir);
                EDir dirValue = Dir;
                if (dirValue == EDir.Ltr)
                {
                    return CSS.EDirection.LTR;
                }
                else if (ReferenceEquals(this, ownerDocument.documentElement) && !attr.Is_Defined)
                {
                    return CSS.EDirection.LTR;
                }
                else if (this is HTMLInputElement inputElement && inputElement.type == EInputType.Telephone)
                {
                    return CSS.EDirection.LTR;
                }

                if (dirValue == EDir.Rtl)
                {
                    return CSS.EDirection.RTL;
                }
                /* XXX: Finish this */
                /*
                if (dirValue == EDir.Auto && (this is HTMLTextAreaElement || (this is HTMLInputElement e && (e.type == EInputType.Text || e.type == EInputType.Search || e.type == EInputType.Telephone || e.type == EInputType.Url || e.type == EInputType.Email)))
                {
                    *//* If the element's value contains a character of bidirectional character type AL or R, and there is no character of bidirectional character type L anywhere before it in the element's value, then the directionality of the element is 'rtl'. [BIDI]
                     * Otherwise, if the element's value is not the empty string, or if the element is a document element, the directionality of the element is 'ltr'.
                     * Otherwise, the directionality of the element is the same as the element's parent element's directionality. 
                     *//*

                }
                */

                /* XXX: Finish this */
                if (dirValue == EDir.Auto || (!attr.Is_Defined && this is HTMLBdiElement))
                {
                    /* Find the first character in tree order that matches the following criteria:
                     * The character is from a Text node that is a descendant of the element whose directionality is being determined.
                     * The character is of bidirectional character type L, AL, or R. [BIDI]
                     * The character is not in a Text node that has an ancestor element that is a descendant of the element whose directionality is being determined and that is either:
                     * -  A bdi element.
                     * -  A script element.
                     * -  A style element.
                     * -  A textarea element.
                     * An element with a dir attribute in a defined state.
                     * If such a character is found and it is of bidirectional character type AL or R, the directionality of the element is 'rtl'.
                     * If such a character is found and it is of bidirectional character type L, the directionality of the element is 'ltr'.
                     * Otherwise, if the element is a document element, the directionality of the element is 'ltr'.
                     * Otherwise, the directionality of the element is the same as the element's parent element's directionality.
                     */
                }

                if (!attr.Is_Defined && !ReferenceEquals(null, parentElement))
                {
                    return (parentElement as HTMLElement).directionality;
                }

                return CSS.EDirection.LTR;
            }
        }

        /// <summary>
        /// Returns the value of the element's [[CryptographicNonce]] internal slot.
        /// Can be set, to update that slot's value.
        /// </summary>
        public string nonce// Intentionally no [CEReactions]
        {/* Docs: https://html.spec.whatwg.org/multipage/urls-and-fetching.html#dom-noncedelement-nonce */
            get => getAttribute(EAttributeName.Nonce).Get_String();
            set => setAttribute(EAttributeName.Nonce, AttributeValue.From_String(value));
        }

        public readonly DOMStringMap dataset;
        #endregion

        #region Constructors
        /// <summary>
        /// Instantiates a new HTML element
        /// </summary>
        /// <param name="document">The document this element resides in</param>
        /// <param name="localName">Also know as the HTML tag</param>
        public HTMLElement(Document document, string localName) : base(document, localName, "html", DOMCommon.HTMLNamespace)
        {
            dataset = new DOMStringMap(this);
        }
        #endregion


        #region Custom Element
        public bool attached_internals { get; private set; } = false;

        public ElementInternals attachInternals()
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#dom-attachinternals */
            if (!ReferenceEquals(null, is_value))
            {
                throw new NotSupportedError("Cannot attach internals to element that still has it's 'is' value");
            }

            var definition = nodeDocument.defaultView.customElements.Lookup(nodeDocument, NamespaceURI, localName, null);
            if (ReferenceEquals(null, definition))
            {
                throw new NotSupportedError("Element internals may only be attached to custom elements");
            }

            if (definition.bDisableInternals)
            {
                throw new NotSupportedError("Cannot attach internals to custom elements whose definition has internals disabled");
            }

            if (attached_internals)
            {
                throw new NotSupportedError("Cannot attach internals to element which already has internals attached");
            }

            attached_internals = true;

            internals = new ElementInternals(this);
            return internals;
        }
        #endregion

        #region Internal
        internal bool tabindex_focus_flag = true;

        /// <summary>
        /// This definition is used to determine what elements can be focused and which elements match the :enabled and :disabled pseudo-classes.
        /// </summary>
        internal bool is_actually_disabled
        {
            get
            {
                switch (tagName)
                {
                    case "button":
                    case "input":
                    case "select":
                    case "textarea":
                    case "optgroup":
                    case "option":
                    case "fieldset":
                        return disabled;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Does this element match the ':active' pseudoclass
        /// </summary>
        internal bool is_being_activated
        {/* Docs: https://html.spec.whatwg.org/multipage/semantics-other.html#concept-selector-active */
            get
            {
                if (this is HTMLInputElement inputElement)
                {
                    if (inputElement.type == EInputType.Submit || inputElement.type == EInputType.Image || inputElement.type == EInputType.reset || inputElement.type == EInputType.button)
                    {
                        return !disabled && is_in_formal_activation_state;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (this is HTMLButtonElement)
                {
                    return !disabled && is_in_formal_activation_state;
                }
                else
                {
                    switch (localName)
                    {
                        case "a":
                        case "area":
                        case "link":
                            return hasAttribute(EAttributeName.HREF);
                    }
                }
                /*
                 * If the element has its tabindex focus flag set
                 * The element is being activated if it is in a formal activation state. 
                 */
                if (tabindex_focus_flag)
                {
                    return is_in_formal_activation_state;
                }

                /* - If the element has a descendant that is currently matching the :active pseudo-class */
                var tree = new TreeWalker(this, Enums.ENodeFilterMask.SHOW_ELEMENT, FilterIsActive.Instance);
                if (!ReferenceEquals(null, tree.nextNode()))
                    return true;

                /* If the element is being actively pointed at */
                return is_actively_pointed_at;
            }

        }
        #endregion

        #region CSS Object Model
        public Element offsetParent
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-htmlelement-offsetparent */
            get
            {
                if (ReferenceEquals(null, Box) || is_root || ReferenceEquals(this, ownerDocument.body) || Style.Positioning == CSS.EPositioning.Fixed)
                    return null;

                /* 2) Return the nearest ancestor element of the element for which at least one of the following is true and terminate this algorithm if such an ancestor is found: */
                var tree = new TreeWalker(this, Enums.ENodeFilterMask.SHOW_ELEMENT);
                Element ancestor = tree.parentNode() as Element;
                while (!ReferenceEquals(null, ancestor))
                {
                    if (ancestor.Style.Positioning != CSS.EPositioning.Static)
                    {
                        return ancestor;
                    }
                    else if (ReferenceEquals(ancestor, ownerDocument.body))
                    {
                        return ancestor;
                    }
                    else if (ancestor.Style.Positioning == CSS.EPositioning.Static && (ancestor is HTMLTableRowElement || ancestor is HTMLTableHeadElement || ancestor is HTMLTableElement))
                    {
                        return ancestor;
                    }

                    ancestor = tree.parentNode() as Element;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the position of the top edge of this element relative to its container
        /// </summary>
        public long offsetTop
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-htmlelement-offsettop */
            get
            {
                if (ReferenceEquals(null, Box) || ReferenceEquals(this, ownerDocument.body))
                    return 0;

                var offsetParent = this.offsetParent;
                if (ReferenceEquals(null, offsetParent))
                {
                    return (long)(Box.Border.Top - ownerDocument.Initial_Containing_Block.top);
                }
                /* 3) Return the result of subtracting the y-coordinate of the top padding edge of the first CSS layout box associated with the offsetParent 
                 * of the element from the y-coordinate of the top border edge of the first CSS layout box associated with the element, 
                 * relative to the initial containing block origin, ignoring any transforms that apply to the element and its ancestors. */
                return (long)(Box.Border.Top - offsetParent.Box.Padding.Top - ownerDocument.Initial_Containing_Block.top);
            }
        }
        /// <summary>
        /// Returns the position of the left edge of this element relative to its container
        /// </summary>
        public long offsetLeft
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-htmlelement-offsetleft */
            get
            {
                if (ReferenceEquals(null, Box) || ReferenceEquals(this, ownerDocument.body))
                    return 0;

                var offsetParent = this.offsetParent;
                if (ReferenceEquals(null, offsetParent))
                {
                    return (long)(Box.Border.Left - ownerDocument.Initial_Containing_Block.left);
                }
                /* 3) Return the result of subtracting the x-coordinate of the left padding edge of the first CSS layout box associated with the offsetParent 
                 * of the element from the x-coordinate of the left border edge of the first CSS layout box associated with the element, 
                 * relative to the initial containing block origin, ignoring any transforms that apply to the element and its ancestors. */
                return (long)(Box.Border.Left - offsetParent.Box.Padding.Left - ownerDocument.Initial_Containing_Block.left);
            }
        }
        /// <summary>
        /// Returns the border width of this element
        /// </summary>
        public long offsetWidth
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-htmlelement-offsetwidth */
            get
            {
                if (ReferenceEquals(null, Box) || ReferenceEquals(this, ownerDocument.body))
                    return 0;

                return Box.Border.Width;
            }
        }
        /// <summary>
        /// Returns the border height of this element
        /// </summary>
        public long offsetHeight
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-htmlelement-offsetheight */
            get
            {
                if (ReferenceEquals(null, Box) || ReferenceEquals(this, ownerDocument.body))
                    return 0;

                return Box.Border.Height;
            }
        }
        #endregion

        #region User Interaction
        /// <summary>
        /// Specifies the tab index of this element, that is its selection order when a user cycles through selecting elements by pressing tab
        /// </summary>
        [CEReactions]
        public int TabIndex
        {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#attr-tabindex */
            get
            {
                tabindex_focus_flag = false;
                Attr Attrib = getAttributeNode(EAttributeName.ContentEditable);
                /* Get the attributes integer value and default to -1 if it cannot be resolved */
                int attrValue = Attrib?.Value?.Get_Int() ?? -1;

                /* If the attribute is omitted or parsing the value returns an error */
                if (ReferenceEquals(null, Attrib.Value) || Attrib.Is_MissingValue || Attrib.Is_InvalidValue)
                {
                    /* The tabIndex IDL attribute must reflect the value of the tabindex content attribute. 
                     * The default value is 0 if the element is an a, area, button, iframe, input, select, or textarea element, or is a summary element that is a summary for its parent details. 
                     * The default value is −1 otherwise. */
                    if (Attrib.Is_MissingValue)
                    {
                        /* XXX: Implement checks for these special element classes once they exist */
                    }

                    /*
                     * Modulo platform conventions, it is suggested that for the following elements, the tabindex focus flag be set:
                     * 
                     * - a elements that have an href attribute
                     * - link elements that have an href attribute
                     * - button elements
                     * - input elements whose type attribute are not in the Hidden state
                     * - select elements
                     * - textarea elements
                     * - summary elements that are the first summary element child of a details element
                     * - Elements with a draggable attribute set, if that would enable the user agent to allow the user to begin a drag operations for those elements without the use of a pointing device
                     * 
                     */
                    if (this.Draggable)
                        tabindex_focus_flag = true;
                }
                else
                {
                    if (attrValue < 0)
                    {
                        /* The user agent must set the element's tabindex focus flag, but should omit the element from the sequential focus navigation order. */
                        tabindex_focus_flag = true;
                    }
                    else if (attrValue == 0)
                    {
                        /* The user agent must set the element's tabindex focus flag, should allow the element and any focusable areas that have the element as their DOM anchor to be reached using sequential focus navigation, following platform conventions to determine the element's relative position in the sequential focus navigation order. */
                        tabindex_focus_flag = true;
                    }
                    else if (attrValue > 0)
                    {
                        /*  */
                        tabindex_focus_flag = true;
                    }
                }

                return attrValue;
            }
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.TabIndex, AttributeValue.From_Integer(value)));
        }

        /// <summary>
        /// Determines if this element can be interacted with
        /// </summary>
        [CEReactions]
        public virtual bool disabled
        {
            get => hasAttribute(EAttributeName.Disabled);
            set => CEReactions.Wrap_CEReaction(this, () => toggleAttribute(EAttributeName.Disabled, value));
        }

        /// <summary>
        /// 
        /// </summary>
        [CEReactions]
        public bool Hidden
        {
            get => hasAttribute(EAttributeName.Hidden);
            set => CEReactions.Wrap_CEReaction(this, () => toggleAttribute(EAttributeName.Hidden, value));
        }

        protected bool click_in_progress = false;
        /// <summary>
        /// Acts as if the element was clicked.
        /// </summary>
        public void click()
        {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#dom-click */
            if (disabled)
                return;

            if (click_in_progress)
                return;

            click_in_progress = true;
            EventCommon.fire_synthetic_mouse_event(new EventName(EEventName.Click), this, true);
            click_in_progress = false;
        }


        private bool locked_for_focus = false;
        public void Focus(FocusOptions options)
        {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#dom-focus */
            if (locked_for_focus) return;
            locked_for_focus = true;

            if (!DOMCommon.Is_Focusable(this))
            {
                /* XXX: */
            }
        }

        // public void blur(); /* "User agents are encouraged to ignore calls to this blur() method entirely." - https://html.spec.whatwg.org/multipage/interaction.html#dom-window-blur */

        /* XXX: finish access key logic */
        [CEReactions]
        public string AccessKey
        {
            get => getAttribute(EAttributeName.AccessKey).Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.AccessKey, AttributeValue.Parse(EAttributeName.AccessKey, value)));
        }

        public string AccessKeyLabel { get; private set; }

        /// <summary>
        /// Returns true if the element is draggable; otherwise, returns false.
        /// Can be set, to override the default and set the draggable content attribute.
        /// </summary>
        [CEReactions]
        public bool Draggable
        {/* Docs: https://html.spec.whatwg.org/multipage/dnd.html#the-draggable-attribute */
            get
            {
                if (!hasAttribute(EAttributeName.Draggable, out Attr attr))
                {
                    return false;
                }

                EDraggable attrValue = attr.Value.Get_Enum<EDraggable>();
                if (attrValue == EDraggable.True)
                {
                    return true;
                }
                else if (attrValue == EDraggable.Auto)
                {
                    return parentElement is HTMLElement element && element.isContentEditable;
                }

                return false;
            }
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Draggable, AttributeValue.From_Enum(value ? EDraggable.True : EDraggable.False)));
        }

        /* XXX: implement these */
        /// <summary>
        /// Determines the default behavior of spellchecking for this element
        /// </summary>
        protected ESpellcheckBehavior Spellcheck_Default = ESpellcheckBehavior.False_By_Default;
        /// <summary>
        /// Returns true if the element is to have its spelling and grammar checked; otherwise, returns false.
        /// Can be set, to override the default and set the spellcheck content attribute.
        /// </summary>
        [CEReactions]
        public bool Spellcheck
        {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#spelling-and-grammar-checking */
            get
            {
                /* The spellcheck IDL attribute, on getting, must return true if the element's spellcheck content attribute is in the true state, 
                 * or if the element's spellcheck content attribute is in the default state and the element's default behavior is true-by-default, 
                 * or if the element's spellcheck content attribute is in the default state and the element's default behavior is inherit-by-default and the element's parent element's spellcheck IDL attribute would return true; 
                 * otherwise, if none of those conditions applies, then the attribute must instead return false. */
                ESpellcheck enumValue = getAttribute(EAttributeName.Spellcheck).Get_Enum<ESpellcheck>();

                if (enumValue == ESpellcheck.True)
                {
                    return true;
                }
                else if (enumValue == ESpellcheck.Default)
                {
                    switch (Spellcheck_Default)
                    {
                        case ESpellcheckBehavior.True_By_Default:
                            return true;

                        case ESpellcheckBehavior.Inherit_By_Default:
                            {
                                if (!ReferenceEquals(null, parentElement) && parentElement is HTMLElement parentHTML)
                                {
                                    if (parentHTML.Spellcheck)
                                    {
                                        return true;
                                    }
                                }
                            }
                            break;
                    }
                }

                /* otherwise, if none of those conditions applies, then the attribute must instead return false. */
                return false;
            }
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Spellcheck, AttributeValue.From_Enum(value ? ESpellcheck.True : ESpellcheck.False)));
        }

        /// <summary>
        /// Is this element an autocapitalize-inheriting element?
        /// </summary>
        protected bool Autocapitalize_Inherited = false;
        /// <summary>
        /// Returns the current autocapitalization state for the element, or an empty string if it hasn't been set. Note that for input and textarea elements that inherit their state from a form element, 
        /// this will return the autocapitalization state of the form element, but for an element in an editable region, 
        /// this will not return the autocapitalization state of the editing host (unless this element is, in fact, the editing host).
        /// Can be set, to set the autocapitalize content attribute(and thereby change the autocapitalization behavior for the element).
        /// </summary>
        [CEReactions]
        public EAutoCapitalizationHint Autocapitalize
        {
            get
            {
                Attr attr = getAttributeNode(EAttributeName.AutoCapitalize);
                if (!ReferenceEquals(null, attr))
                {
                    var enumValue = attr.Value.Get_Enum<EAutoCapitalizationHint>();
                    if (enumValue != EAutoCapitalizationHint.Default)
                    {
                        return enumValue;
                    }
                    else/* Default */
                    {
                        if (Autocapitalize_Inherited)
                        {
                            if (this is IFormAssociatedElement formElement)
                            {
                                if (!ReferenceEquals(null, formElement.form))
                                {
                                    return formElement.form.Autocapitalize;
                                }
                            }
                        }
                    }
                }

                return EAutoCapitalizationHint.Default;
            }
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.AutoCapitalize, AttributeValue.From_Enum(value)));
        }

        [CEReactions]
        public string innerText;
        #endregion

        #region Editable Content

        /// <summary>
        /// Returns "true", "false", or "inherit", based on the state of the contenteditable attribute.
        /// Can be set, to change that state.
        /// Throws a "SyntaxError" DOMException if the new value isn't one of those strings.
        /// </summary>
        [CEReactions]
        public EContentEditable ContentEditable
        {
            /* Docs: https://html.spec.whatwg.org/multipage/interaction.html#attr-contenteditable */
            get => getAttribute(EAttributeName.ContentEditable).Get_Enum<EContentEditable>();
            set
            {
                CEReactions.Wrap_CEReaction(this, () =>
                {
                    setAttribute(EAttributeName.ContentEditable, AttributeValue.From_Enum(value));
                });
            }
        }

        /// <summary>
        /// Returns true if the element is editable; otherwise, returns false.
        /// </summary>
        public bool isContentEditable
        {
            get
            {
                if (!hasAttribute(EAttributeName.ContentEditable, out Attr attr))
                {
                    return false;
                }

                EContentEditable attrValue = attr.Value.Get_Enum<EContentEditable>();
                if (attrValue == EContentEditable.True)
                {
                    return true;
                }
                else if (attrValue == EContentEditable.Inherit)
                {
                    return parentElement is HTMLElement element && element.isContentEditable;
                }

                return false;
            }
        }

        /* XXX: Implement this */
        [CEReactions]
        public string EnterKeyHint
        {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#attr-enterkeyhint */
            get;
            set;
        }

        /* XXX: Implement this */
        [CEReactions]
        public string InputMode
        {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#attr-inputmode */
            get;
            set;
        }
        #endregion

        #region Document Events
        public event EventCallback onCopy
        {
            add => handlerMap.Add(EEventName.Copy, value);
            remove => handlerMap.Remove(EEventName.Copy, value);
        }

        public event EventCallback onCut
        {
            add => handlerMap.Add(EEventName.Cut, value);
            remove => handlerMap.Remove(EEventName.Cut, value);
        }

        public event EventCallback onPaste
        {
            add => handlerMap.Add(EEventName.Paste, value);
            remove => handlerMap.Remove(EEventName.Paste, value);
        }
        #endregion

        #region Window Events
        public event EventCallback onAbort
        {
            add => handlerMap.Add(EEventName.Abort, value);
            remove => handlerMap.Remove(EEventName.Abort, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onAuxClick
        {
            add => handlerMap.Add(EEventName.AuxClick, value);
            remove => handlerMap.Remove(EEventName.AuxClick, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onBlur
        {
            add => handlerMap.Add(EEventName.Blur, value);
            remove => handlerMap.Remove(EEventName.Blur, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onCancel
        {
            add => handlerMap.Add(EEventName.Cancel, value);
            remove => handlerMap.Remove(EEventName.Cancel, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onCanPlay
        {
            add => handlerMap.Add(EEventName.CanPlay, value);
            remove => handlerMap.Remove(EEventName.CanPlay, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onCanPlayThrough
        {
            add => handlerMap.Add(EEventName.CanPlayThrough, value);
            remove => handlerMap.Remove(EEventName.CanPlayThrough, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onChange
        {
            add => handlerMap.Add(EEventName.Change, value);
            remove => handlerMap.Remove(EEventName.Change, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onClick
        {
            add => handlerMap.Add(EEventName.Click, value);
            remove => handlerMap.Remove(EEventName.Click, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onClose
        {
            add => handlerMap.Add(EEventName.Close, value);
            remove => handlerMap.Remove(EEventName.Close, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onContextMenu
        {
            add => handlerMap.Add(EEventName.ContextMenu, value);
            remove => handlerMap.Remove(EEventName.ContextMenu, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onCueChange
        {
            add => handlerMap.Add(EEventName.CueChange, value);
            remove => handlerMap.Remove(EEventName.CueChange, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onDblClick
        {
            add => handlerMap.Add(EEventName.DoubleClick, value);
            remove => handlerMap.Remove(EEventName.DoubleClick, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onDrag
        {
            add => handlerMap.Add(EEventName.Drag, value);
            remove => handlerMap.Remove(EEventName.Drag, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onDragEnd
        {
            add => handlerMap.Add(EEventName.DragEnd, value);
            remove => handlerMap.Remove(EEventName.DragEnd, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onDragEnter
        {
            add => handlerMap.Add(EEventName.DragEnter, value);
            remove => handlerMap.Remove(EEventName.DragEnter, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onDragExit
        {
            add => handlerMap.Add(EEventName.DragExit, value);
            remove => handlerMap.Remove(EEventName.DragExit, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onDragLeave
        {
            add => handlerMap.Add(EEventName.DragLeave, value);
            remove => handlerMap.Remove(EEventName.DragLeave, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onDragOver
        {
            add => handlerMap.Add(EEventName.DragOver, value);
            remove => handlerMap.Remove(EEventName.DragOver, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onDragStart
        {
            add => handlerMap.Add(EEventName.DragStart, value);
            remove => handlerMap.Remove(EEventName.DragStart, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onDrop
        {
            add => handlerMap.Add(EEventName.Drop, value);
            remove => handlerMap.Remove(EEventName.Drop, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onDurationChange
        {
            add => handlerMap.Add(EEventName.DurationChange, value);
            remove => handlerMap.Remove(EEventName.DurationChange, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onEmptied
        {
            add => handlerMap.Add(EEventName.Emptied, value);
            remove => handlerMap.Remove(EEventName.Emptied, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onEnded
        {
            add => handlerMap.Add(EEventName.Ended, value);
            remove => handlerMap.Remove(EEventName.Ended, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onFocus
        {
            add => handlerMap.Add(EEventName.Focus, value);
            remove => handlerMap.Remove(EEventName.Focus, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onFormData
        {
            add => handlerMap.Add(EEventName.FormData, value);
            remove => handlerMap.Remove(EEventName.FormData, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onInput
        {
            add => handlerMap.Add(EEventName.Input, value);
            remove => handlerMap.Remove(EEventName.Input, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onInvalid
        {
            add => handlerMap.Add(EEventName.Invalid, value);
            remove => handlerMap.Remove(EEventName.Invalid, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onKeyDown
        {
            add => handlerMap.Add(EEventName.KeyDown, value);
            remove => handlerMap.Remove(EEventName.KeyDown, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onKeyPress
        {
            add => handlerMap.Add(EEventName.KeyPress, value);
            remove => handlerMap.Remove(EEventName.KeyPress, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onKeyUp
        {
            add => handlerMap.Add(EEventName.KeyUp, value);
            remove => handlerMap.Remove(EEventName.KeyUp, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onLoad
        {
            add => handlerMap.Add(EEventName.Load, value);
            remove => handlerMap.Remove(EEventName.Load, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onLoadedData
        {
            add => handlerMap.Add(EEventName.LoadedData, value);
            remove => handlerMap.Remove(EEventName.LoadedData, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onLoadedMetadata
        {
            add => handlerMap.Add(EEventName.LoadedMetadata, value);
            remove => handlerMap.Remove(EEventName.LoadedMetadata, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onLoadEnd
        {
            add => handlerMap.Add(EEventName.LoadEnd, value);
            remove => handlerMap.Remove(EEventName.LoadEnd, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onLoadStart
        {
            add => handlerMap.Add(EEventName.LoadStart, value);
            remove => handlerMap.Remove(EEventName.LoadStart, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onMouseDown
        {
            add => handlerMap.Add(EEventName.MouseDown, value);
            remove => handlerMap.Remove(EEventName.MouseDown, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onMouseEnter
        {
            add => handlerMap.Add(EEventName.MouseEnter, value);
            remove => handlerMap.Remove(EEventName.MouseEnter, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onMouseLeave
        {
            add => handlerMap.Add(EEventName.MouseLeave, value);
            remove => handlerMap.Remove(EEventName.MouseLeave, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onMouseMove
        {
            add => handlerMap.Add(EEventName.MouseMove, value);
            remove => handlerMap.Remove(EEventName.MouseMove, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onMouseOut
        {
            add => handlerMap.Add(EEventName.MouseOut, value);
            remove => handlerMap.Remove(EEventName.MouseOut, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onMouseOver
        {
            add => handlerMap.Add(EEventName.MouseOver, value);
            remove => handlerMap.Remove(EEventName.MouseOver, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onMouseUp
        {
            add => handlerMap.Add(EEventName.MouseUp, value);
            remove => handlerMap.Remove(EEventName.MouseUp, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onWheel
        {
            add => handlerMap.Add(EEventName.Wheel, value);
            remove => handlerMap.Remove(EEventName.Wheel, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onPause
        {
            add => handlerMap.Add(EEventName.Pause, value);
            remove => handlerMap.Remove(EEventName.Pause, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onPlay
        {
            add => handlerMap.Add(EEventName.Play, value);
            remove => handlerMap.Remove(EEventName.Play, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onPlaying
        {
            add => handlerMap.Add(EEventName.Playing, value);
            remove => handlerMap.Remove(EEventName.Playing, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onProgress
        {
            add => handlerMap.Add(EEventName.Progress, value);
            remove => handlerMap.Remove(EEventName.Progress, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onRateChange
        {
            add => handlerMap.Add(EEventName.RateChange, value);
            remove => handlerMap.Remove(EEventName.RateChange, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onReset
        {
            add => handlerMap.Add(EEventName.Reset, value);
            remove => handlerMap.Remove(EEventName.Reset, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onResize
        {
            add => handlerMap.Add(EEventName.Resize, value);
            remove => handlerMap.Remove(EEventName.Resize, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onScroll
        {
            add => handlerMap.Add(EEventName.Scroll, value);
            remove => handlerMap.Remove(EEventName.Scroll, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onSecurityPolicyViolation
        {
            add => handlerMap.Add(EEventName.SecurityPolicyViolation, value);
            remove => handlerMap.Remove(EEventName.SecurityPolicyViolation, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onSeeked
        {
            add => handlerMap.Add(EEventName.Seeked, value);
            remove => handlerMap.Remove(EEventName.Seeked, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onSeeking
        {
            add => handlerMap.Add(EEventName.Seeking, value);
            remove => handlerMap.Remove(EEventName.Seeking, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onSelect
        {
            add => handlerMap.Add(EEventName.Select, value);
            remove => handlerMap.Remove(EEventName.Select, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onStalled
        {
            add => handlerMap.Add(EEventName.Stalled, value);
            remove => handlerMap.Remove(EEventName.Stalled, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onSubmit
        {
            add => handlerMap.Add(EEventName.Submit, value);
            remove => handlerMap.Remove(EEventName.Submit, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onSuspend
        {
            add => handlerMap.Add(EEventName.Suspend, value);
            remove => handlerMap.Remove(EEventName.Suspend, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onTimeUpdate
        {
            add => handlerMap.Add(EEventName.TimeUpdate, value);
            remove => handlerMap.Remove(EEventName.TimeUpdate, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onToggle
        {
            add => handlerMap.Add(EEventName.Toggle, value);
            remove => handlerMap.Remove(EEventName.Toggle, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onVolumeChange
        {
            add => handlerMap.Add(EEventName.VolumeChange, value);
            remove => handlerMap.Remove(EEventName.VolumeChange, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onWaiting
        {
            add => handlerMap.Add(EEventName.Waiting, value);
            remove => handlerMap.Remove(EEventName.Waiting, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onSelectStart
        {
            add => handlerMap.Add(EEventName.SelectStart, value);
            remove => handlerMap.Remove(EEventName.SelectStart, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onSelectionChange
        {
            add => handlerMap.Add(EEventName.SelectionChange, value);
            remove => handlerMap.Remove(EEventName.SelectionChange, value);
        }

        #endregion

    }

}
