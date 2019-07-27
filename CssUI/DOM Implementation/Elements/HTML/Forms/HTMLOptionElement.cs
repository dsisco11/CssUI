using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CssUI.DOM.CustomElements;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    /// <summary>
    /// The option element represents an option in a select element or as part of a list of suggestions in a datalist element.
    /// </summary>
    public class HTMLOptionElement : FormAssociatedElement
    {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#the-option-element */
        #region Properties
        public bool dirtiness { get; protected set; } = false;

        /* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#concept-option-selectedness */
        public bool selectedness { get; protected set; } = false;
        #endregion


        #region Constructors
        /// <summary>
        /// Returns a new option element.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="text">The text argument sets the contents of the element.</param>
        /// <param name="value">The value argument sets the value attribute.</param>
        /// <param name="defaultSelected">The defaultSelected argument sets the selected attribute.</param>
        /// <param name="selected">The selected argument sets whether or not the element is selected.If it is omitted, even if the defaultSelected argument is true, the element is not selected.</param>
        public HTMLOptionElement(Document document, string text = null, string value = null, bool defaultSelected = false, bool selected = false) : base(document, "option")
        {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#dom-option */

            /* 3) If text is not the empty string, then append to option a new Text node whose data is text. */
            if (text.Length > 0)
            {
                var textNode = new Text(document, text);
                appendChild(textNode);
            }

            /* 4) If value is given, then set an attribute value for option using "value" and value. */
            if (!ReferenceEquals(null, value))
            {
                setAttribute(EAttributeName.Value, AttributeValue.From_String(value));
            }

            /* 5) If defaultSelected is true, then set an attribute value for option using "selected" and the empty string. */
            if (defaultSelected)
            {
                setAttribute(EAttributeName.Selected, AttributeValue.From_String(""));
            }

            /* 6) If selected is true, then set option's selectedness to true; otherwise set its selectedness to false (even if defaultSelected is true). */
            this.selected = selected;
        }

        public HTMLOptionElement(Document document, string localName, string text = null, string value = null, bool defaultSelected = false, bool selected = false) : this(document, text, value, defaultSelected, selected)
        {
        }
        #endregion


        #region Utility
        private HTMLSelectElement get_select()
        {
            if (parentElement is HTMLSelectElement parentSelect)
            {
                return parentSelect;
            }

            if (parentElement is HTMLOptGroupElement && parentElement.parentElement is HTMLSelectElement parentParentSelect)
            {
                return parentParentSelect;
            }

            return null;
        }
        #endregion


        #region Accessors
        public override HTMLFormElement form
        {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#dom-option-form */
            get
            {
                HTMLSelectElement select = get_select();
                if (select != null)
                {
                    return select.form;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns true if the element is selected, and false otherwise.
        /// </summary>
        public bool selected
        {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#dom-option-selected */
            get => selectedness;
            set
            {
                selectedness = value;
                dirtiness = true;
                get_select()?.ask_for_reset(this);
            }
        }

        /// <summary>
        /// Returns the index of the element in its select element's options list.
        /// </summary>
        public new int index
        {
            get
            {
                HTMLSelectElement select = get_select();
                if (select == null)
                {
                    return 0;
                }

                var optionsList = select.options;
                for (int i = 0; i < optionsList.Count; i++)
                {
                    if (ReferenceEquals(this, optionsList.ElementAt(i)))
                    {
                        return i;
                    }
                }

                throw new Exception("Unable to find current option within parent <select>s options list!");
            }
        }

        [CEReactions]
        public override bool disabled
        {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#attr-option-disabled */
            get
            {
                if (hasAttribute(EAttributeName.Disabled))
                    return true;

                if (parentElement is HTMLOptGroupElement optgroup && optgroup.disabled)
                    return true;

                return false;
            }
            set => CEReactions.Wrap_CEReaction(this, () => toggleAttribute(EAttributeName.Disabled, value));
        }

        [CEReactions]
        public bool defaultSelected
        {
            get => hasAttribute(EAttributeName.Selected);
            set => CEReactions.Wrap_CEReaction(this, () => toggleAttribute(EAttributeName.Selected, value));
        }

        [CEReactions]
        public string label
        {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#attr-option-label */
            get
            {
                string labelStr = getAttribute(EAttributeName.Label).Get_String();
                if (!ReferenceEquals(null, labelStr) && labelStr.Length > 0)
                {
                    return labelStr;
                }

                return text;
            }
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Label, AttributeValue.From_String(value)));
        }

        [CEReactions]
        public override string value
        {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#dom-option-value */
            get
            {
                string valueStr = getAttribute(EAttributeName.Value)?.Get_String();
                if (!ReferenceEquals(null, valueStr) || valueStr.Length > 0)
                {
                    return valueStr;
                }

                return text;
            }
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Value, AttributeValue.From_String(value)));
        }

        [CEReactions]
        public string text
        {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#dom-option-text */
            get
            {
                /* The text IDL attribute, on getting, must return the result of stripping and collapsing ASCII whitespace from the concatenation of data of all the Text node descendants of the option element, 
                 * in tree order, excluding any that are descendants of descendants of the option element that are themselves script or SVG script elements. */
                StringBuilder sb = new StringBuilder();
                LinkedList<Text> textList = new LinkedList<Text>();
                var tree = new TreeWalker(this, Enums.ENodeFilterMask.SHOW_ALL);

                Node descendant = tree.nextNode();
                while (descendant != null)
                {
                    if (descendant is HTMLScriptElement)
                    {
                        /* Skip this element, move to its sibling and dont traverse its descendants */
                        descendant = tree.nextSibling();
                        continue;
                    }
                    else
                    {
                        if (descendant is Text descendantText)
                        {
                            sb.Append(descendantText.data);
                            /* textList.AddLast(descendantText); */
                        }

                        descendant = tree.nextNode();
                    }
                }

                string textStr = sb.ToString();
                return StringCommon.Strip_And_Collapse_Whitespace(textStr.AsMemory());
            }

            set
            {
                CEReactions.Wrap_CEReaction(this, () =>
                {
                    if (!ReferenceEquals(null, value) && value.Length > 0)
                    {
                        Text newText = new Text(nodeDocument, value);
                        _replace_all_within_node(newText, this);
                    }
                    else
                    {
                        _replace_all_within_node(null, this);
                    }
                });
            }
        }

        #endregion


        internal override EValidityState query_validity()
        {
            EValidityState flags = 0x0;




        }
    }
}
