using CssUI.DOM.Exceptions;
using CssUI.DOM.Nodes;
using System.Collections.Generic;

namespace CssUI.DOM.Mutation
{
    /// <summary>
    /// A MutationObserver object can be used to observe mutations to the tree of nodes.
    /// </summary>
    public class MutationObserver
    {
        #region Properties
        internal List<Node> Nodes = new List<Node>();
        private Queue<MutationRecord> RecordQueue = new Queue<MutationRecord>();
        public MutationCallback callback { get; private set; } = null;
        #endregion


        #region Constructor
        public MutationObserver(Window window, MutationCallback callback)
        {
            /* The MutationObserver(callback) constructor, when invoked, must run these steps: */
            /* 1) Let mo be a new MutationObserver object whose callback is callback. */
            this.callback = callback;
            /* 2) Append mo to mo’s relevant agent’s mutation observers. */
            window.Observers.Add(this);
            /* 3) Return mo. */
        }
        #endregion

        public void Observe(Node target, MutationObserverInit options = null)
        {
            if (options != null)
            {
                /* 1) If either options’s attributeOldValue or attributeFilter is present and options’s attributes is omitted, then set options’s attributes to true. */
                if (options.attributeOldValue || options.attributeFilter != null)
                    options.attributes = true;
                /* 2) If options’s characterDataOldValue is present and options’s characterData is omitted, then set options’s characterData to true. */
                if (options.characterDataOldValue)
                    options.characterData = true;
                /* 3) If none of options’s childList, attributes, and characterData is true, then throw a TypeError. */
                if (!options.childList && !options.attributes && !options.characterData)
                    throw new TypeError("Mutation observer not set to observe anything!");
                /* 4) If options’s attributeOldValue is true and options’s attributes is false, then throw a TypeError. */
                if (options.attributeOldValue && !options.attributes)
                    throw new TypeError("Mutation observer set to observe attributeOldValue but not attributes!");
                /* 5) If options’s attributeFilter is present and options’s attributes is false, then throw a TypeError */
                if (options.attributeFilter != null && !options.attributes)
                    throw new TypeError("Mutation observer has attributeFilter but is not set to observe attributes!");
                /* 6) If options’s characterDataOldValue is true and options’s characterData is false, then throw a TypeError. */
                if (options.characterDataOldValue && !options.characterData)
                    throw new TypeError("Mutation observer set to observe characterDataOldValue but not characterData!");

                /* 7) For each registered of target’s registered observer list, if registered’s observer is the context object: */
                foreach (RegisteredObserver registered in target.RegisteredObservers)
                {
                    if (ReferenceEquals(registered.observer, this))
                    {
                        /* 1) For each node of the context object’s node list, remove all transient registered observers whose source is registered from node’s registered observer list. */
                        foreach(Node n in this.Nodes)
                        {
                            n.RegisteredObservers.RemoveAll(R => R is TransientRegisteredObserver T && ReferenceEquals(T.source, registered));
                        }
                        /* Set registered’s options to options. */
                        registered.options = options;
                        return;
                    }
                }
                /* 8) Otherwise */
                /* 1) Append a new registered observer whose observer is the context object and options is options to target’s registered observer list. */
                target.RegisteredObservers.Add(new RegisteredObserver(this, options));
                /* 2) Append target to the context object’s node list. */
                Nodes.Add(target);
            }

        }

        public void Disconnect()
        {
            /* 1) For each node of the context object’s node list, remove any registered observer from node’s registered observer list for which the context object is the observer. */
            foreach(Node N in Nodes)
            {
                N.RegisteredObservers.RemoveAll(R => ReferenceEquals(this, R.observer));
            }
            /* 2) Empty the context object’s record queue. */
            RecordQueue.Clear();
        }

        public void Enqueue(MutationRecord Record)
        {
            this.RecordQueue.Enqueue(Record);
        }

        /// <summary>
        /// Returns a clone of the record queue, then emptys the queue
        /// </summary>
        /// <returns></returns>
        public ICollection<MutationRecord> TakeRecords()
        {
            /* 1) Let records be a clone of the context object’s record queue. */
            var records = RecordQueue.ToArray();
            /* 2) Empty the context object’s record queue. */
            RecordQueue.Clear();
            /* 3) Return records. */
            return records;
        }
    }
}
