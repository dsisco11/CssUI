using CssUI.DOM.Nodes;
using System.Collections.Generic;

namespace CssUI.DOM
{
    public class HTMLSlotElement : HTMLElement, ISlot
    {/* Docs: https://html.spec.whatwg.org/multipage/scripting.html#htmlslotelement */

        #region Properties
        /// <summary>
        /// The name attribute is used to assign slots to other elements: a slot element with a name attribute creates a named slot to which any element is assigned if that element has a slot attribute whose value matches that name attribute's value, and the slot element is a child of the shadow tree whose root's host has that corresponding slot attribute value.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// List of associated slottables
        /// </summary>
        public List<ISlottable> Assigned { get; set; } = new List<ISlottable>();
        #endregion

        public HTMLSlotElement(string Name, Document document, string localName) : base(document, localName)
        {
            this.Name = Name;
            tagName = "slot";
        }

        /// <summary>
        /// Returns slot's assigned nodes, if any, and slot's children otherwise, and does the same for any slot elements encountered therein, recursively, until there are no slot elements left.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IEnumerable<ISlottable> assignedNodes(AssignedNodesOptions options = null)
        {/* Docs: https://html.spec.whatwg.org/multipage/scripting.html#dom-slot-assignednodes */
            if (!options?.flatten ?? false)
            {
                return this.Assigned;
            }

            return DOMCommon.find_flattened_slotables(this);
        }

        /// <summary>
        /// Returns slot's assigned nodes, limited to elements.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IEnumerable<Element> assignedElements(AssignedNodesOptions options = null);
    }
}
