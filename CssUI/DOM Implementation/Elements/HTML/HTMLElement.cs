﻿using CssUI.DOM.Events;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Internal;
using CssUI.DOM.Serialization;

namespace CssUI.DOM
{
    /// <summary>
    /// The basic interface, from which all the HTML elements' interfaces inherit, and which must be used by elements that have no additional requirements, is the HTMLElement interface.
    /// </summary>
    public class HTMLElement : Element, IGlobalEventCallbacks, IDocumentAndElementEventCallbacks
    {
        # region Metadata Attributes
        public string title;
        public string lang;
        public bool translate;
        public string dir;
        public readonly DOMStringMap dataset;
        public string nonce;
        #endregion

        #region Constructors
        public HTMLElement(Document document, string localName, string prefix, string Namespace) : base(document, localName, prefix, Namespace)
        {
            dataset = new DOMStringMap(this);
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
                        return this.Disabled;
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
                switch (localName)
                {
                    case "input":
                        {
                            string inputType = getAttribute(EAttributeName.Type);
                            if (inputType.Equals("submit") || inputType.Equals("image") || inputType.Equals("reset") || inputType.Equals("button"))
                            {
                                return !Disabled && is_in_formal_activation_state;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    case "button":
                        {
                            return !Disabled && is_in_formal_activation_state;
                        }
                    case "a":
                    case "area":
                    case "link":
                        return hasAttribute(EAttributeName.HREF);
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

        #region User Interaction
        /// <summary>
        /// Specifies the tab index of this element, that is its selection order when a user cycles through selecting elements by pressing tab
        /// </summary>
        public int TabIndex
        {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#attr-tabindex */
            get
            {
                tabindex_focus_flag = false;
                string attrValue = getAttribute(EAttributeName.ContentEditable);
                int? parsed = null;
                if (!string.IsNullOrEmpty(attrValue))
                    parsed = DOMParser.Parse_Integer(attrValue);

                if (!parsed.HasValue)
                {
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
                else if (parsed.Value < 0)
                {
                    /* The user agent must set the element's tabindex focus flag, but should omit the element from the sequential focus navigation order. */
                    tabindex_focus_flag = true;
                }
                else if (parsed.Value == 0)
                {
                    /* The user agent must set the element's tabindex focus flag, should allow the element and any focusable areas that have the element as their DOM anchor to be reached using sequential focus navigation, following platform conventions to determine the element's relative position in the sequential focus navigation order. */
                    tabindex_focus_flag = true;
                }
                else if (parsed.Value > 0)
                {
                    /*  */
                    tabindex_focus_flag = true;
                }

                return parsed.Value;
            }
            set
            {
                setAttribute(EAttributeName.TabIndex, value.ToString());
            }
        }

        /// <summary>
        /// Determines if this element can be interacted with
        /// </summary>
        public bool Disabled
        {
            get => hasAttribute(EAttributeName.Disabled);
            set => toggleAttribute(EAttributeName.Disabled, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Hidden
        {
            get => hasAttribute(EAttributeName.Hidden);
            set => toggleAttribute(EAttributeName.Hidden, value);
        }

        protected bool click_in_progress = false;
        /// <summary>
        /// Acts as if the element was clicked.
        /// </summary>
        public void click()
        {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#dom-click */
            if (Disabled)
                return;

            if (click_in_progress)
                return;

            click_in_progress = true;
            EventCommon.fire_synthetic_mouse_event(new Events.EventName(Events.EEventName.Click), this, true);
            click_in_progress = false;
        }


        private bool locked_for_focus = false;
        public void Focus(FocusOptions options)
        {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#dom-focus */
            if (locked_for_focus) return;
            locked_for_focus = true;

            if (!DOMCommon.Is_Focusable(this))
            {

            }
        }

        // public void blur(); /* "User agents are encouraged to ignore calls to this blur() method entirely." - https://html.spec.whatwg.org/multipage/interaction.html#dom-window-blur */

        public string accessKey;
        public string accessKeyLabel { get; private set; }

        public bool Draggable
        {/* Docs: https://html.spec.whatwg.org/multipage/dnd.html#the-draggable-attribute */
            get
            {
                if (!hasAttribute(EAttributeName.Draggable, out Attr attr))
                {
                    return false;
                }

                string attrValue = attr.Value;
                if (attrValue == "true")
                {
                    return true;
                }
                else if (attrValue == "auto")
                {
                    return parentElement is HTMLElement element && element.isContentEditable;
                }

                return false;
            }
            set
            {
                setAttribute(EAttributeName.Draggable, value ? "true" : "false");
            }
        }

        public bool spellcheck;
        public string autocapitalize;

        public string innerText;
        #endregion

        #region Editable Content

        /// <summary>
        /// Returns "true", "false", or "inherit", based on the state of the contenteditable attribute.
        /// Can be set, to change that state.
        /// Throws a "SyntaxError" DOMException if the new value isn't one of those strings.
        /// </summary>
        public string ContentEditable
        {
            /* Docs: https://html.spec.whatwg.org/multipage/interaction.html#attr-contenteditable */
            get => getAttribute(EAttributeName.ContentEditable);
            set
            {
                if (value == null)
                    value = string.Empty;

                if (value!="true" && value!="false" && value!="inherit")
                    throw new DomSyntaxError("This attribute only accepts values of \"true\", \"false\", or \"inherit\"");

                setAttribute(EAttributeName.ContentEditable, value);
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

                string attrValue = attr.Value;
                if (attrValue == "true")
                {
                    return true;
                }
                else if (attrValue == "inherit")
                {
                    return parentElement is HTMLElement element && element.isContentEditable;
                }

                return false;
            }
        }

        public string enterKeyHint
        {
            get;
            set;
        }

        public string inputMode
        {
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