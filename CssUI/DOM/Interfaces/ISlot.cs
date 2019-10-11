using CssUI.DOM.Events;
using System.Collections.Generic;

namespace CssUI.DOM.Nodes
{
    public interface ISlot : INode
    {
        string Name { get; set; }
        /// <summary>
        /// List of associated slottables
        /// </summary>
        List<ISlottable> Assigned { get; set; }

        /// <summary>
        /// Returns slot's assigned nodes, if any, and slot's children otherwise, and does the same for any slot elements encountered therein, recursively, until there are no slot elements left.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        IReadOnlyCollection<ISlottable> assignedNodes(AssignedNodesOptions options = null);

        /// <summary>
        /// Returns slot's assigned nodes, limited to elements.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        IReadOnlyCollection<Element> assignedElements(AssignedNodesOptions options = null);


        void Signal_Slot_Change();
    }
}
