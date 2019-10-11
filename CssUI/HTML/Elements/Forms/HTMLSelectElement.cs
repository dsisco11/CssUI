using CssUI.DOM;
using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.HTML
{
    /// <summary>
    /// The select element represents a control for selecting amongst a set of options.
    /// </summary>
    [MetaElement("select")]
    public class HTMLSelectElement : FormAssociatedElement, IListedElement, ILableableElement, ISubmittableElement, IResettableElement, IAutoCapitalizeInheritingElement
    {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#htmlselectelement */

        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow | EContentCategories.Phrasing | EContentCategories.Interactive | EContentCategories.Palpable;
        #endregion

        #region Properties
        public readonly HTMLOptionsCollection options;
        #endregion

        #region Constructor
        public HTMLSelectElement(Document document) : this(document, "select")
        {
        }

        public HTMLSelectElement(Document document, string localName) : base(document, localName)
        {
            options = new HTMLOptionsCollection(this);
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
        public bool multiple
        {
            get => hasAttribute(EAttributeName.Multiple);
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => toggleAttribute(EAttributeName.Multiple, value));
        }

        [CEReactions]
        public string name
        {
            get => getAttribute(EAttributeName.Name).AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Name, AttributeValue.From(value)));
        }

        [CEReactions]
        public bool required
        {
            get => hasAttribute(EAttributeName.Required);
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => toggleAttribute(EAttributeName.Required, value));
        }

        [CEReactions]
        public int size
        {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#dom-select-size */
            get => getAttribute(EAttributeName.Size).AsInt();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Size, AttributeValue.From(value)));
        }
        #endregion

        #region Overrides
        public override string type => hasAttribute(EAttributeName.Multiple) ? "select-multiple" : "select-one";

        public override string value
        {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#dom-select-value */
            get
            {
                var selected = selectedOptions;
                if (selected.Count <= 0)
                {
                    return string.Empty;
                }

                return selected.First()?.value;
            }

            set
            {
                /* On setting, the value attribute must set the selectedness of all the option elements in the list of options to false, 
                 * and then the first option element in the list of options, in tree order, whose value is equal to the given new value, 
                 * if any, must have its selectedness set to true and its dirtiness set to true. */

                var optionList = options;
                HTMLOptionElement first = null;
                foreach (HTMLOptionElement option in optionList)
                {
                    option.selectedness = false;
                    if (first != null) continue;

                    if (StringCommon.StrEq(option.value, value))
                        first = option;

                }

                if (first != null)
                {
                    first.selectedness = true;
                    first.dirtiness = true;
                }
            }
        }

        internal override void Run_node_insertion_steps(Node node)
        {
            base.Run_node_insertion_steps(node);

            /* If the multiple attribute is absent, ... whenever an option element with its selectedness set to true is added to the select element's list of options, the user agent must set the selectedness of all the other option elements in its list of options to false */
            if (!multiple)
            {
                if (node is HTMLOptionElement optionElement)
                {
                    if (optionElement.selected)
                    {
                        var optionList = options;
                        foreach (HTMLOptionElement option in optionList)
                        {
                            if (ReferenceEquals(optionElement, option)) continue;
                            option.selectedness = false;
                        }
                    }

                    /* If nodes are inserted or nodes are removed causing the list of options to gain or lose one or more option elements,
                     * ...then, if the select element's multiple attribute is absent, ...ask for a reset*/
                    request_reset();
                }
            }
        }

        internal override void Run_node_removing_steps(Node node)
        {
            base.Run_node_removing_steps(node);

            if (!multiple)
            {
                if (node is HTMLOptionElement)
                {
                    /* If nodes are inserted or nodes are removed causing the list of options to gain or lose one or more option elements,
                     * ...then, if the select element's multiple attribute is absent, ...ask for a reset*/
                    request_reset();
                }
            }
        }
        #endregion

        #region Accessors
        [CEReactions]
        public int length
        {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#dom-select-length */
            get => options.length;
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => options.length = value);
        }

        public IReadOnlyCollection<HTMLOptionElement> selectedOptions
        {
            get => (IReadOnlyCollection<HTMLOptionElement>)DOMCommon.Get_Descendents(this, FilterOptionElementSelected.Instance, ENodeFilterMask.SHOW_ELEMENT);
        }

        public int selectedIndex
        {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#dom-select-selectedindex */
            get
            {
                var element = DOMCommon.Get_Nth_Descendant(this, 1, FilterOptionElementSelected.Instance, ENodeFilterMask.SHOW_ELEMENT);
                return element.index;
            }
            set
            {
                if (value < 0 || value >= options.length)
                {
                    throw new IndexOutOfRangeException();
                }
                /* On setting, the selectedIndex attribute must set the selectedness of all the option elements in the list of options to false, 
                 * and then the option element in the list of options whose index is the given new value, 
                 * if any, must have its selectedness set to true and its dirtiness set to true. */
                for (int i=0; i<options.length; i++)
                {
                    options[i].selectedness = false;
                }

                options[value].selected = true;
            }
        }

        public IReadOnlyCollection<HTMLLabelElement> labels
        {
            get => (IReadOnlyCollection<HTMLLabelElement>)DOMCommon.Get_Descendents(form, new FilterLabelFor(this), ENodeFilterMask.SHOW_ELEMENT);
        }
        #endregion

        #region Indexers
        [CEReactions]
        public HTMLOptionElement this[int index]
        {
            get => options[index];
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => options[index] = value);
        }

        public HTMLOptionElement this[string name] => options[name];
        #endregion

        #region Utility
        int display_size
        {
            get
            {
                /* The display size of a select element is the result of applying the rules for parsing non-negative integers to the value of element's size attribute, if it has one and parsing it is successful. 
                 * If applying those rules to the attribute's value is not successful, 
                 * or if the size attribute is absent, then the element's display size is 4 if the element's multiple content attribute is present, and 1 otherwise. */
                if (hasAttribute(EAttributeName.Size, out Attr outSizeAttr))
                {
                    if (outSizeAttr.IsDefined)
                    {
                        return outSizeAttr.Value.AsInt();
                    }
                }

                return multiple ? 4 : 1;
            }
        }

        private HTMLOptionElement get_placeholder_label_option()
        {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#placeholder-label-option */
            if (!required)
            {
                return null;
            }

            if (multiple)
            {
                return null;
            }

            if (display_size != 1)
            {
                return null;
            }

            var optionList = options;
            if (optionList.length > 0 && string.IsNullOrEmpty(optionList[0].value) && !(optionList[0].parentNode is HTMLOptGroupElement))
            {
                return optionList[0];
            }

            return null;
        }
        #endregion

        #region Input Validation
        internal override EValidityState query_validity()
        {
            var flags = base.query_validity();
            /* If the element has its required attribute specified, and either none of the option elements in the select element's list of options have their selectedness set to true, 
             * or the only option element in the select element's list of options with its selectedness set to true is the placeholder label option, 
             * then the element is suffering from being missing. */
            if (required)
            {
                /* Reset the valueMissing flag first */
                flags &= ~EValidityState.valueMissing;
                /* Calculate the valueMissing constraint using this specific elements logic */
                var selected = selectedOptions;
                if (selected.Count == 0)
                {
                    flags |= EValidityState.valueMissing;
                }
                else if (selected.Count == 1 && selected.First().selected)
                {
                    if (ReferenceEquals(get_placeholder_label_option(), selected.First()))
                    {
                        flags |= EValidityState.valueMissing;
                    }
                }
            }

            return flags;
        }
        #endregion

        #region Form-Associated Element Overrides
        /// <summary>
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/form-elements.html#the-select-element:concept-fe-mutable
        public override bool isMutable => !disabled;
        #endregion

        #region IResettable
        public void Reset()
        {/* Docs:  */
            /* The reset algorithm for select elements is to go through all the option elements in the element's list of options, 
             * set their selectedness to true if the option element has a selected attribute, 
             * and false otherwise, set their dirtiness to false, and then have the option elements ask for a reset. */

            foreach (var option in options)
            {
                if (option.defaultSelected)
                {
                    option.selectedness = true;
                    option.dirtiness = false;
                }
            }

            request_reset();
        }


        public void request_reset()
        {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#ask-for-a-reset */

            if (!multiple)
            {
                var selected = selectedOptions;
                /* If the select element's display size is 1, and no option elements in the select element's list of options have their selectedness set to true */
                if (display_size == 1 && selected.Count == 0)
                {
                    /* Set the selectedness of the first option element in the list of options in tree order that is not disabled, if any, to true. */
                    var first = DOMCommon.Get_Nth_Descendant<HTMLOptionElement>(this, 1, FilterNotDisabled.Instance, ENodeFilterMask.SHOW_ELEMENT);
                    if (first != null)
                    {
                        first.selectedness = true;
                    }

                }
                /* If two or more option elements in the select element's list of options have their selectedness set to true */
                else if (selected.Count >= 2)
                {
                    /* Set the selectedness of all but the last option element with its selectedness set to true in the list of options in tree order to false. */
                    var last = selected.Last();
                    foreach (HTMLOptionElement option in selected)
                    {
                        if (ReferenceEquals(last, option))
                        {
                            continue;
                        }

                        option.selectedness = false;
                    }
                }
            }
        }
        #endregion

        [CEReactions]
        public void add(HTMLOptionElement element, long before)
        {
            CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => options.add(element, before));
        }

        [CEReactions]
        public void add(HTMLOptionElement element, HTMLElement before = null)
        {
            CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => options.add(element, before));
        }

        [CEReactions]
        public void add(HTMLOptGroupElement element, long before)
        {
            CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => options.add(element, before));
        }

        [CEReactions]
        public void add(HTMLOptGroupElement element, HTMLElement before = null)
        {
            CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => options.add(element, before));
        }

        [CEReactions]
        public void remove(long index)
        {
            CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => options.remove(index));
        }
    }

}
