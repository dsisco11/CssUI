using CssUI.DOM;
using CssUI.DOM.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.HTML
{
    /// <summary>
    /// The slot element defines a slot. It is typically used in a shadow tree. A slot element represents its assigned nodes, if any, and its contents otherwise.
    /// </summary>
    [MetaElement("slot")]
    public class HTMLSlotElement : HTMLElement, ISlot
    {/* Docs: https://html.spec.whatwg.org/multipage/scripting.html#htmlslotelement */

        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow | EContentCategories.Phrasing;
        #endregion

        #region Properties
        /// <summary>
        /// List of associated slottables
        /// </summary>
        public List<ISlottable> Assigned { get; set; } = new List<ISlottable>();
        #endregion

        #region Constructors
        public HTMLSlotElement(Document document) : this(document, "slot")
        {
        }

        public HTMLSlotElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Content Attributes
        /// <summary>
        /// The name attribute is used to assign slots to other elements: a slot element with a name attribute creates a named slot to which any element is assigned if that element has a slot attribute whose value matches that name attribute's value, and the slot element is a child of the shadow tree whose root's host has that corresponding slot attribute value.
        /// </summary>
        [CEReactions] public string Name
        {
            get => getAttribute(EAttributeName.Name).AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Name, AttributeValue.From(value)));
        }
        #endregion

        /// <summary>
        /// Returns slot's assigned nodes, if any, and slot's children otherwise, and does the same for any slot elements encountered therein, recursively, until there are no slot elements left.
        /// </summary>
        public IReadOnlyCollection<ISlottable> assignedNodes(AssignedNodesOptions options = null)
        {/* Docs: https://html.spec.whatwg.org/multipage/scripting.html#dom-slot-assignednodes */
            if (!options?.flatten ?? false)
            {
                return Assigned;
            }

            return DOMCommon.Find_Flattened_Slotables(this);
        }

        /// <summary>
        /// Returns slot's assigned nodes, limited to elements.
        /// </summary>
        public IReadOnlyCollection<Element> assignedElements(AssignedNodesOptions options = null)
        {/* Docs: https://html.spec.whatwg.org/multipage/scripting.html#dom-slot-assignedelements */
            if (!options?.flatten ?? false)
            {
                return Assigned.Where(c => c is Element).Cast<Element>().ToArray();
            }

            return DOMCommon.Find_Flattened_Slotables(this).Where(c => c is Element).Cast<Element>().ToArray();
        }


        public void Signal_Slot_Change()
        {/* Docs: https://dom.spec.whatwg.org/#signal-a-slot-change */

            Window window = ownerDocument?.defaultView;
            if (window != null)
            {
                window.SignalSlots.Add(this);
                window.QueueObserverMicroTask();
            }
        }
    }
}
