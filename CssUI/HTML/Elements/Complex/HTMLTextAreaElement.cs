using CssUI.DOM;
using CssUI.DOM.Enums;
using CssUI.DOM.Events;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CssUI.HTML
{
    /// <summary>
    /// The textarea element represents a multiline plain text edit control for the element's raw value. The contents of the control represent the control's default value.
    /// </summary>
    [MetaElement("textarea")]
    public class HTMLTextAreaElement : FormAssociatedElement, IListedElement, ILableableElement, ISubmittableElement, IResettableElement, IAutoCapitalizeInheritingElement
    {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#the-textarea-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow | EContentCategories.Phrasing | EContentCategories.Interactive | EContentCategories.Palpable;
        #endregion

        #region Backing Values
        private string raw_value = string.Empty;
        #endregion

        #region Properties
        private readonly FilterLabelFor labelFilter;
        /// <summary>
        /// input and textarea elements have a dirty value flag. This is used to track the interaction between the value and default value. If it is false, value mirrors the default value. If it is true, the default value is ignored.
        /// </summary>
        private bool bDirtyValueFlag = false;
        #endregion

        #region Constructors
        public HTMLTextAreaElement(Document document) : this(document, "textarea")
        {
        }

        public HTMLTextAreaElement(Document document, string localName) : base(document, localName)
        {
            labelFilter = new FilterLabelFor(this);
        }
        #endregion

        #region Content Attributes
        [CEReactions]
        public string autocomplete
        {
            get
            {
                HTMLCommon.Resolve_Autofill(this, out _, out _, out _, out string outValue);
                return outValue;
            }
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Autocomplete, AttributeValue.From(value)));
        }

        [CEReactions]
        public bool autofocus
        {
            get => hasAttribute(EAttributeName.Autofocus);
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => toggleAttribute(EAttributeName.Autofocus, value));
        }

        [CEReactions]
        public uint cols
        {
            get => getAttribute(EAttributeName.Cols).AsUInt();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Cols, AttributeValue.From(value)));
        }

        [CEReactions]
        public string dirName
        {
            get => getAttribute(EAttributeName.Dirname).AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Dirname, AttributeValue.From(value)));
        }

        [CEReactions]
        public string name
        {
            get => getAttribute(EAttributeName.Name).AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Name, AttributeValue.From(value)));
        }

        [CEReactions]
        public string placeholder
        {
            get => getAttribute(EAttributeName.Placeholder).AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Placeholder, AttributeValue.From(value)));
        }

        [CEReactions]
        public bool readOnly
        {
            get => hasAttribute(EAttributeName.ReadOnly);
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => toggleAttribute(EAttributeName.ReadOnly, value));
        }

        [CEReactions]
        public bool required
        {
            get => hasAttribute(EAttributeName.Required);
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => toggleAttribute(EAttributeName.Required, value));
        }

        [CEReactions]
        public uint rows
        {
            get => getAttribute(EAttributeName.Rows).AsUInt();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Rows, AttributeValue.From(value)));
        }

        [CEReactions]
        public string wrap
        {
            get => getAttribute(EAttributeName.Wrap).AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Wrap, AttributeValue.From(value)));
        }

        [CEReactions]
        public int maxLength
        {
            get => getAttribute(EAttributeName.MaxLength).AsInt();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.MaxLength, AttributeValue.From(value)));
        }

        [CEReactions]
        public int minLength
        {
            get => getAttribute(EAttributeName.MinLength).AsInt();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.MinLength, AttributeValue.From(value)));
        }
        #endregion

        public override string type => "textarea";

        [CEReactions]
        public string defaultValue
        {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#dom-textarea-defaultvalue */
            get
            {
                var textNodes = DOMCommon.Get_Children<Text>(this);
                var textList = textNodes.Select(txt => txt.data.AsMemory());
                return StringCommon.Concat(textList);
            }
            set
            {
                CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
                {
                    Node node = null;
                    if (!ReferenceEquals(null, value) && value.Length > 0)
                    {
                        node = new Text(nodeDocument, value);
                        Dom_replace_all_within_node(node, this);
                    }
                });
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal string get_api_value()
        {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#the-textarea-element:concept-fe-api-value-2 */
            return StringCommon.Replace(raw_value.AsMemory(), Filters.FilterCRLF.Instance, Filters.FilterCRLF.LF.AsSpan());
        }

        #region Overrides
        public override string value
        {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#dom-textarea-value */
            get
            {
                return get_api_value();
            }
            set
            {
                string oldAPIValue = get_api_value();
                raw_value = value;
                bDirtyValueFlag = true;
                /* 4) If the new API value is different from oldAPIValue, then move the text entry cursor position to the end of the text control, unselecting any selected text and resetting the selection direction to "none". */
                if (!oldAPIValue.AsSpan().Equals(get_api_value().AsSpan(), StringComparison.Ordinal))
                {
                    text_entry_cursor_position = value.Length;
                    selection.Collapse();
                    selection.direction = ESelectionDirection.None;
                }
            }
        }
        #endregion

        public uint textLength => (uint)get_api_value().Length;

        public IReadOnlyCollection<HTMLLabelElement> labels
        {
            get => (IReadOnlyCollection<HTMLLabelElement>)DOMCommon.Get_Descendents(form, labelFilter, ENodeFilterMask.SHOW_ELEMENT);
        }

        #region Form-Associated Element Overrides
        internal override EValidityState query_validity()
        {
            EValidityState flags = base.query_validity();

            var length = value.Length;
            /* When a control has no value but has a required attribute (input required, textarea required); or, more complicated rules for select elements and controls in radio button groups, as specified in their sections. */
            if (hasAttribute(EAttributeName.Required))
            {
                if (ReferenceEquals(null, value) || length <= 0)
                {
                    flags |= EValidityState.valueMissing;
                }
            }

            /* When a control has a value that is too long for the form control maxlength attribute (input maxlength, textarea maxlength). */
            if (length > maxLength)
            {
                flags |= EValidityState.tooLong;
            }

            /* When a control has a value that is too short for the form control minlength attribute (input minlength, textarea minlength). */
            if (length < minLength)
            {
                flags |= EValidityState.tooShort;
            }

            return flags;
        }
        /// <summary>
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/form-elements.html#the-textarea-element:concept-fe-mutable
        public override bool isMutable => (!disabled && !readOnly);
        #endregion

        #region Resettable
        public void Reset()
        {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#the-textarea-element:concept-form-reset-control */
            bDirtyValueFlag = false;
            value = defaultValue;
        }
        #endregion

        #region Overrides
        internal override void Run_child_text_node_change_steps(Node node)
        {
            base.Run_child_text_node_change_steps(node);
            /* The child text content change steps for textarea elements must, if the element's dirty value flag is false, set the element's raw value to its child text content. */
            if (!bDirtyValueFlag)
            {
                value = defaultValue;
            }
        }

        internal override void Run_cloning_steps(ref Node copy, Document document, bool clone_children = false)
        {
            base.Run_cloning_steps(ref copy, document, clone_children);
            /* The cloning steps for textarea elements must propagate the raw value and dirty value flag from the node being cloned to the copy. */
            var other = copy as HTMLTextAreaElement;
            other.value = base.value;
            other.bDirtyValueFlag = bDirtyValueFlag;
        }
        #endregion


        #region Text Selection
        /* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#textFieldSelection:concept-textarea/input-selection */

        private readonly TextSelection selection = new TextSelection();
        private int text_entry_cursor_position = 0;

        public void select()
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#dom-textarea/input-select */
            /* 1) If this element... ...has no selectable text, return. */
            if (!StringCommon.Contains(value.AsSpan(), Filters.FilterCharSelectable.Instance))
            {
                return;
            }

            /* 2) Set the selection range with 0 and infinity. */
            setSelectionRange(0, int.MaxValue);
        }

        public int selectionStart
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#dom-textarea/input-selectionstart */
            get
            {
                /* 2) If there is no selection, return the offset (in logical order) within the relevant value to the character that immediately follows the text entry cursor. */
                if (!selection.HasSelection)
                    return text_entry_cursor_position;

                return selection.start;
            }

            set
            {
                var end = selectionEnd;
                if (end < value)
                    end = value;

                setSelectionRange(value, end, selectionDirection);
            }
        }

        public int selectionEnd
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#dom-textarea/input-selectionend */
            get
            {
                /* 2) If there is no selection, return the offset (in logical order) within the relevant value to the character that immediately follows the text entry cursor. */
                if (!selection.HasSelection)
                    return text_entry_cursor_position;

                return selection.end;
            }

            set
            {
                setSelectionRange(selectionStart, value, selectionDirection);
            }
        }

        public ESelectionDirection selectionDirection
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#dom-textarea/input-selectiondirection */
            get => selection.direction;
            set => setSelectionRange(selectionStart, selectionEnd, value);
        }

        public void setSelectionRange(int? start, int? end, ESelectionDirection? direction = null)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#dom-textarea/input-setselectionrange */
            /* 1) If this element is an input element, and setSelectionRange() does not apply to this element, throw an "InvalidStateError" DOMException. */
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
            /* 1) If this element is an input element, and setRangeText() does not apply to this element, throw an "InvalidStateError" DOMException. */
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
                relevantValue = relevantValue.Substring(start, end - start - 1);
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
            /* 1) If this element is an input element, and setRangeText() does not apply to this element, throw an "InvalidStateError" DOMException. */
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
    }
}
