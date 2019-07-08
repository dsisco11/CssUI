using CssUI.DOM.Nodes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CssUI.DOM
{
    /// <summary>
    /// Represents the global 'Window' object
    /// </summary>
    public class Window : IGlobalEventHandlers, IWindowEventHandlers
    {/* Docs: https://html.spec.whatwg.org/multipage/window-object.html#window */

        public Document document { get; private set; }

        #region Mutation Observers
        internal List<MutationObserver> Observers = new List<MutationObserver>();
        internal int mutation_observer_microtask_queued = 0;
        internal Task observer_task = Task.FromResult(true);
        // XXX: I'm thinking we jsut throw a new task up instead of bothering to implement the task queue system. We arent a browser afterall.

        internal void QueueObserverMicroTask()
        {
            /* To queue a mutation observer microtask, run these steps: */
            /* 1) If the surrounding agent’s mutation observer microtask queued is true, then return. */
            /* 2) Set the surrounding agent’s mutation observer microtask queued to true. */
            if (0 == Interlocked.CompareExchange(ref mutation_observer_microtask_queued, 1, 0)) return;
            /* 3) Queue a microtask to notify mutation observers. */
            observer_task = Task.Factory.StartNew(Task_Notify_Mutation_Observers);
        }

        private void Task_Notify_Mutation_Observers()
        {
            /* To notify mutation observers, run these steps: */
            /* 1) Set the surrounding agent’s mutation observer microtask queued to false. */
            /* 2) Let notifySet be a clone of the surrounding agent’s mutation observers. */
            MutationObserver[] notifySet = Observers.ToArray();
            Interlocked.Exchange(ref mutation_observer_microtask_queued, 0);
            /* 3) Let signalSet be a clone of the surrounding agent’s signal slots. */
            /* 4) Empty the surrounding agent’s signal slots. */
            // We dont implement slottables

            /* 5) For each mo of notifySet: */
            foreach(MutationObserver mo in notifySet)
            {
                /* 1) Let records be a clone of mo’s record queue. */
                /* 2) Empty mo’s record queue. */
                var records = mo.TakeRecords();
                /* 3) For each node of mo’s node list, remove all transient registered observers whose observer is mo from node’s registered observer list. */
                foreach(Node node in mo.Nodes)
                {
                    node.RegisteredObservers.RemoveAll(o => o is TransientRegisteredObserver tro && ReferenceEquals(tro, mo));
                }
                /* 4) If records is not empty, then invoke mo’s callback with « records, mo », and mo. If this throws an exception, then report the exception. */
                if (records.Count > 0)
                {
                    mo.callback?.Invoke(records, mo);
                }
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; } = "CssUI v0.0.1";/* XXX: add something here to embed the actual version number upon compiling */

        public string status;
        /* Docs: https://html.spec.whatwg.org/multipage/interaction.html#dom-window-focus */
        public void focus();
        /* Docs: https://html.spec.whatwg.org/multipage/interaction.html#dom-window-blur */
        public void blur();

        public static DOMHighResTimeStamp getTimestamp()
        {
            return new DOMHighResTimeStamp() { Timestmap = DateTime.UtcNow.Millisecond };
        }
    }
}
