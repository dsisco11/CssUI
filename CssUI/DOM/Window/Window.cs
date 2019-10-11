using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using CssUI.CSS;
using CssUI.CSS.Internal;
using CssUI.Devices;
using CssUI.DOM.Events;
using CssUI.DOM.Geometry;
using CssUI.DOM.Internal;
using CssUI.DOM.Media;
using CssUI.DOM.Mutation;
using CssUI.DOM.Nodes;
using xLog;
using static CssUI.UIWindowBridge;

#if ENABLE_HTML
using CssUI.HTML.CustomElements;
#endif

namespace CssUI.DOM
{
    /// <summary>
    /// Among other things, the window funnels events from the operating system to the <see cref="Document"/> object
    /// </summary>
    public abstract partial class Window : BrowsingContext
    {/* Docs: https://html.spec.whatwg.org/multipage/window-object.html#window */

        #region Internal Properties
        internal List<IEventTarget> SignalSlots = new List<IEventTarget>();
        /// <summary>
        /// This is the window itsself
        /// </summary>
        internal BrowsingContext BrowsingContext => document?.BrowsingContext;
        internal override Window WindowProxy { get => this; }

#if ENABLE_HTML
        internal readonly ElementReactionStack Reactions;
#endif
        #endregion

        #region Properties
        static string CSSUI_VERSION_STRING = $"CssUI v{FileVersionInfo.GetVersionInfo(Assembly.GetCallingAssembly().Location).FileVersion}";
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; } = CSSUI_VERSION_STRING;
        public Document document { get; private set; }

        /// <summary>
        /// Determines the area of the document being rendered
        /// </summary>
        public readonly VisualViewport visualViewport;
        public readonly Screen screen;
#if ENABLE_HTML
        public readonly CustomElementRegistry customElements;
#endif
        #endregion

        #region Devices
        /// <summary>
        /// The primary pointing device
        /// </summary>
        public PointerDevice Mouse => PointerDevice.PrimaryDevice[(int)EPointerType.Mouse];
        /// <summary>
        /// The keyboard device of which there can be only one (the system sees ALL active keyboards as a single device).
        /// </summary>
        public KeyboardDevice Keyboard => KeyboardDevice.PrimaryDevice;

        private ConcurrentDictionary<KeyCombination, Action> KeyCommands = new ConcurrentDictionary<KeyCombination, Action>();
        private ConcurrentHashSet<KeyCombination> ProtectedKeyCommands = new ConcurrentHashSet<KeyCombination>();
        #endregion

        #region Constructors
        private Window() : base()
        {
            visualViewport = new VisualViewport(this);
#if ENABLE_HTML
            Reactions = new ElementReactionStack(this);
            customElements = new CustomElementRegistry(this);
#endif
        }

        public Window(Screen screen, string DocumentName) : this()
        {
            if (screen is null)
                throw new ArgumentNullException(nameof(screen));
            Contract.EndContractBlock();

            this.screen = screen;
            var dom = new DOMImplementation();
            document = dom.createDocument(DOMCommon.HTMLNamespace, DocumentName, new DocumentType("cssui"));
            document.BrowsingContext = this;
        }

        #endregion

        #region Mutation Observers
        internal List<MutationObserver> Observers = new List<MutationObserver>();
        internal int mutation_observer_microtask_queued = 0;
        internal Task observer_task = Task.FromResult(true);

        // XXX: I'm thinking we just throw a new task up instead of bothering to implement the task queue system. Its basically the same concept(~ish).
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
            var signalSet = SignalSlots.ToArray();
            /* 4) Empty the surrounding agent’s signal slots. */
            SignalSlots.Clear();

            /* 5) For each mo of notifySet: */
            foreach (MutationObserver mo in notifySet)
            {
                /* 1) Let records be a clone of mo’s record queue. */
                /* 2) Empty mo’s record queue. */
                var records = mo.TakeRecords();
                /* 3) For each node of mo’s node list, remove all transient registered observers whose observer is mo from node’s registered observer list. */
                foreach (Node node in mo.Nodes)
                {
                    node.RegisteredObservers.RemoveAll(o => o is TransientRegisteredObserver tro && ReferenceEquals(tro, mo));
                }
                /* 4) If records is not empty, then invoke mo’s callback with « records, mo », and mo. If this throws an exception, then report the exception. */
                if (records.Count > 0)
                {
                    mo.callback?.Invoke(records, mo);
                }
            }

            /* 6) For each slot of signalSet, fire an event named slotchange, with its bubbles attribute set to true, at slot. */
            foreach (var slot in signalSet)
            {
                slot.dispatchEvent(new Event(EEventName.SlotChange, new EventInit() { bubbles = true }));
            }
        }
        #endregion


        #region Selection
        public Selection getSelection()
        {/* Docs: https://www.w3.org/TR/selection-api/#extensions-to-window-interface */
            return document?.getSelection();
        }
        #endregion

        #region Keyboard Commands
        /* XXX: KeyCommand processing on keyboard input */

        /// <summary>
        /// Returns true if the given <paramref name="command"/> was able to be registered to <paramref name="combo"/>
        /// </summary>
        /// <param name="combo">The key combination to register</param>
        /// <returns></returns>
        public bool Register_Key_Command(KeyCombination combo, Action command)
        {
            if (ProtectedKeyCommands.Contains(combo)) return false;

            return KeyCommands.TryAdd(combo, command);
        }

        /// <summary>
        /// Returns true if the given key-combination <paramref name="combo"/> was able to be unegistered
        /// </summary>
        /// <param name="combo">The key combination to register</param>
        /// <returns></returns>
        public bool Unregister_Key_Command(KeyCombination combo)
        {
            if (ProtectedKeyCommands.Contains(combo)) return false;

            return KeyCommands.TryRemove(combo, out _);
        }

        /// <summary>
        /// Attempts to register the given <paramref name="command"/> as a protected key command
        /// <para>Protected commands cannot be unregistered or replaced</para>
        /// </summary>
        /// <param name="combo">The key combination to register</param>
        /// <returns></returns>
        public bool Register_Protected_Key_Command(KeyCombination combo, Action command)
        {
            if (ProtectedKeyCommands.Contains(combo)) return false;

            if (KeyCommands.ContainsKey(combo)) KeyCommands.TryRemove(combo, out _);
            if (KeyCommands.TryAdd(combo, command))
            {
                return ProtectedKeyCommands.Add(combo);
            }

            return false;
        }
        #endregion

        public static DOMHighResTimeStamp getTimestamp() => new DOMHighResTimeStamp() { Timestmap = DateTime.UtcNow.Ticks };


        /// <summary>
        /// The term focusable area is used to refer to regions of the interface that can become the target of keyboard input. Focusable areas can be elements, parts of elements, or other regions managed by the user agent.
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/interaction.html#currently-focused-area-of-a-top-level-browsing-context
        internal FocusableArea FocusedArea
        {
            get
            {
                /* 1) Let candidate be the Document of the top-level browsing context. */
                var candidate = this.Get_Top_Level_Browsing_Context().activeDocument;
                while (true)
                {
                    /* 2) If the designated focused area of the document is a browsing context container with a non-null nested browsing context, 
                     * then let candidate be the active document of that browsing context container's nested browsing context, and redo this step. */
                    if (candidate.focusedArea is IBrowsingContextContainer container && container.Nested_Browsing_Context != null)
                    {
                        candidate = container.Nested_Browsing_Context.activeDocument;
                    }
                    else
                    {
                        break;
                    }
                }

                /* 3) If candidate has a focused area, set candidate to candidate's focused area. */
                if (candidate.focusedArea != null)
                {
                    return candidate.focusedArea;
                }

                /* 4) Return candidate. */
                return candidate;
            }
        }
        // public string status;
        public void focus()
        {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#dom-window-focus */
            DOMCommon.Run_Focusing_Steps(document);
        }


        #region Node Management
        public Node importNode(Node node, bool deep = false)
        {
            return this.document?.importNode(node, deep);
        }

        public Node adoptNode(Node node)
        {
            return this.document?.adoptNode(node);
        }
        #endregion


        /// <summary>
        /// Performs main-loop processing
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Run_Event_Loop()
        {
            if (document is object)
            {
                document.Run_Event_Loop();
            }
        }
    }
}
