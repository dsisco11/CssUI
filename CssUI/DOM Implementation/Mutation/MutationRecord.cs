using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using System.Collections.Generic;

namespace CssUI.DOM.Mutation
{
    public class MutationRecord
    {
        #region Properties
        /// <summary>
        /// Returns "attributes" if it was an attribute mutation. "characterData" if it was a mutation to a CharacterData node. And "childList" if it was a mutation to the tree of nodes.
        /// </summary>
        public EMutationType type { get; private set; }
        public readonly Node target = null;
        public readonly IEnumerable<Node> addedNodes = null;
        public readonly IEnumerable<Node> removedNodes = null;

        /// <summary>
        /// Return the previous sibling of the added or removed nodes, and null otherwise.
        /// </summary>
        public readonly Node previousSibling = null;
        /// <summary>
        /// Return the next sibling of the added or removed nodes, and null otherwise.
        /// </summary>
        public readonly Node nextSibling = null;

        /// <summary>
        /// Returns the local name of the changed attribute, and null otherwise.
        /// </summary>
        public readonly string attributeName = null;
        // public readonly string attributeNamespace;
        /// <summary>
        /// The return value depends on type. For "attributes", it is the value of the changed attribute before the change. For "characterData", it is the data of the changed node before the change. For "childList", it is null.
        /// </summary>
        public readonly string oldValue = null;
        #endregion

        #region Constructor
        public MutationRecord(EMutationType type, Node target)
        {
            this.type = type;
            this.target = target;
        }

        public MutationRecord(EMutationType type, Node target, string attributeName, string oldValue) : this(type, target)
        {
            this.attributeName = attributeName;
            this.oldValue = oldValue;

        }

        public MutationRecord(EMutationType type, Node target, string attributeName, string oldValue, IEnumerable<Node> addedNodes = null, IEnumerable<Node> removedNodes = null, Node previousSibling = null, Node nextSibling = null)
            : this(type, target, attributeName, oldValue)
        {
            this.addedNodes = addedNodes;
            this.removedNodes = removedNodes;
            this.nextSibling = nextSibling;
            this.previousSibling = previousSibling;
        }
        #endregion

        #region Internal Utility
        internal static void Queue_Text_Mutation_Record(Node target, string oldData)
        {
            MutationRecord record = new MutationRecord(EMutationType.CharacterData, target, null, oldData, null, null, null, null);
            MutationRecord.QueueRecord(record);
        }

        internal static void Queue_Attribute_Mutation_Record(Node target, string name, string oldValue)
        {
            MutationRecord record = new MutationRecord(EMutationType.Attributes, target, name, oldValue, null, null, null, null);
            MutationRecord.QueueRecord(record);
        }

        internal static void Queue_Tree_Mutation_Record(Node target, IEnumerable<Node> addedNodes, IEnumerable<Node> removedNodes, Node previousSibling, Node nextSibling)
        {
            MutationRecord record = new MutationRecord(EMutationType.ChildList, target, null, null, addedNodes, removedNodes, previousSibling, nextSibling);
            MutationRecord.QueueRecord(record);
        }

        internal static void QueueRecord(MutationRecord Record)
        {/* Docs: https://dom.spec.whatwg.org/#queueing-a-mutation-record */

            /* 1) Let interestedObservers be an empty map. */
            Dictionary<MutationObserver, string> interestedObservers = new Dictionary<MutationObserver, string>();
            /* 2) Let nodes be the inclusive ancestors of target. */
            List<Node> Nodes = new List<Node>();
            TreeWalker tree = new TreeWalker(Record.target, ENodeFilterMask.SHOW_ALL);
            Node ancestor = Record.target;
            while (ancestor != null)
            {
                ancestor = tree.parentNode();
                Nodes.Add(ancestor);
            }
            /* 3) For each node in nodes, and then for each registered of node’s registered observer list: */
            foreach (Node node in Nodes)
            {
                foreach (var registered in node.RegisteredObservers)
                {
                    /* 1) Let options be registered’s options. */
                    var options = registered.options;
                    /* 2) If none of the following are true */
                    if (!(!ReferenceEquals(node, Record.target) && !options.subtree)
                    && !(Record.type == EMutationType.Attributes && !options.attributes)
                    && !(Record.type == EMutationType.Attributes && (options.attributeFilter==null || (!options.attributeFilter.Contains("name") && options.attributeFilter.Contains("namespace"))))
                    && !(Record.type == EMutationType.CharacterData && !options.characterData)
                    && !(Record.type == EMutationType.ChildList && !options.childList))
                    {
                        /* 1) Let mo be registered’s observer. */
                        var mo = registered.observer;
                        /* 2) If interestedObservers[mo] does not exist, then set interestedObservers[mo] to null. */
                        if (!interestedObservers.ContainsKey(mo))
                            interestedObservers.Add(mo, null);
                        /* 3) If either type is "attributes" and options’s attributeOldValue is true, or type is "characterData" and options’s characterDataOldValue is true, then set interestedObservers[mo] to oldValue. */
                        if ((Record.type == EMutationType.Attributes && options.attributeOldValue) || (Record.type == EMutationType.CharacterData && options.characterDataOldValue))
                            interestedObservers[mo] = Record.oldValue;
                    }
                }
            }
            /* 4) For each observer → mappedOldValue of interestedObservers: */
            foreach (KeyValuePair<MutationObserver, string> kv in interestedObservers)
            {
                /* 1) Let record be a new MutationRecord object with its type set to type, target set to target, attributeName set to name, attributeNamespace set to namespace, oldValue set to mappedOldValue, addedNodes set to addedNodes, removedNodes set to removedNodes, previousSibling set to previousSibling, and nextSibling set to nextSibling. */
                var record = new MutationRecord(Record.type, Record.target, Record.attributeName, kv.Value, Record.addedNodes, Record.removedNodes, Record.previousSibling, Record.nextSibling);
                /* 2) Enqueue record to observer’s record queue. */
                kv.Key.Enqueue(record);
            }
            /* 5) Queue a mutation observer microtask. */
            Record.target.ownerDocument.defaultView.QueueObserverMicroTask();
        }

        #endregion
    }
}
