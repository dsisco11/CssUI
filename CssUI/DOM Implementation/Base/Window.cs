using CssUI.CSS;
using CssUI.CSS.Internal;
using CssUI.Devices;
using CssUI.DOM.CustomElements;
using CssUI.DOM.Events;
using CssUI.DOM.Geometry;
using CssUI.DOM.Internal;
using CssUI.DOM.Media;
using CssUI.DOM.Mutation;
using CssUI.DOM.Nodes;
using CssUI.HTML;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CssUI.DOM
{
    /// <summary>
    /// Among other things, the window funnels events from the operating system to the <see cref="Document"/> object
    /// </summary>
    public abstract class Window : BrowsingContext, IGlobalEventCallbacks, IWindowEventCallbacks
    {/* Docs: https://html.spec.whatwg.org/multipage/window-object.html#window */
        
        #region Internal Properties
        internal List<IEventTarget> SignalSlots = new List<IEventTarget>();
        /// <summary>
        /// This is the window itsself
        /// </summary>
        internal BrowsingContext BrowsingContext => document?.BrowsingContext;
        internal override Window WindowProxy { get => this; }

        internal readonly ElementReactionStack Reactions;
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; } = "CssUI v0.0.1";/* XXX: add something here to embed the actual version number upon compiling */
        public Document document { get; private set; }

        /// <summary>
        /// Determines the area of the document being rendered
        /// </summary>
        public readonly VisualViewport visualViewport;
        public readonly Screen screen;
        public readonly CustomElementRegistry customElements;
        #endregion

        #region Devices
        public MouseDevice Mouse = null;
        public KeyboardDevice Keyboard = null;

        private ConcurrentDictionary<KeyCombination, Action> KeyCommands = new ConcurrentDictionary<KeyCombination, Action>();
        private ConcurrentHashSet<KeyCombination> ProtectedKeyCommands = new ConcurrentHashSet<KeyCombination>();
        #endregion

        #region Constructors
        private Window() : base()
        {
            visualViewport = new VisualViewport(this);
            Reactions = new ElementReactionStack(this);
            customElements = new CustomElementRegistry(this);
        }

        public Window(Screen screen, MouseDevice mouseDevice, KeyboardDevice keyboardDevice) : this()
        {
            this.screen = screen;
            Mouse = mouseDevice;
            Keyboard = keyboardDevice;

            var dom = new DOMImplementation();
            this.document = dom.createDocument(DOMCommon.HTMLNamespace, "CssUI", new DocumentType("cssui"));
            this.document.BrowsingContext = this;
        }
        #endregion

        #region CSS Object Model Extensions
        /* Docs: https://www.w3.org/TR/cssom-view-1/#extensions-to-the-window-interface */
        public MediaQueryList matchMedia(string query)
        {
            return new CSS.Serialization.CssParser(query).Parse_Media_Query_List(this.document);
        }

        // browsing context
        public void moveTo(long x, long y) => throw new NotImplementedException();
        public void moveBy(long x, long y) => throw new NotImplementedException();
        public void resizeTo(long x, long y) => throw new NotImplementedException();
        public void resizeBy(long x, long y) => throw new NotImplementedException();

        // Viewport
        public long innerWidth => (document.Viewport == null) ? 0 : document.Viewport.Width;
        public long innerHeight => (document.Viewport == null) ? 0 : document.Viewport.Height;

        // viewport scrolling
        public double scrollX
        {
            get
            {/* The scrollX attribute attribute must return the x-coordinate, relative to the initial containing block origin, of the left of the viewport, or zero if there is no viewport. */
                if (document.Viewport == null)
                    return 0;

                /* XXX: Are we doing scrolling wrong or something? why is it measuring the location of the viewport and not its scrollbox? */
                var icb = document.Initial_Containing_Block;
                return document.Viewport.Left - icb.x;
            }
        }
        public double pageXOffset => scrollX;
        public double scrollY
        {
            get
            {/* The scrollY attribute attribute must return the y-coordinate, relative to the initial containing block origin, of the top of the viewport, or zero if there is no viewport. */
                if (document.Viewport == null)
                    return 0;

                /* XXX: Are we doing scrolling wrong or something? why is it measuring the location of the viewport and not its scrollbox? */
                var icb = document.Initial_Containing_Block;
                return document.Viewport.Top - icb.y;
            }
        }
        public double pageYOffset => scrollY;

        internal void scroll_window(double x, double y, EScrollBehavior behavior)
        {
            double viewportWidth = document.Viewport.Width;
            double viewportHeight = document.Viewport.Height;

            var scrollBox = document.Viewport.ScrollBox;
            var scrollArea = scrollBox.ScrollArea;

            if (document.Viewport.ScrollBox.Overflow_Block == CSS.Enums.EOverflowDirection.Rightward)
            {
                x = MathExt.Max(0, MathExt.Min(x, scrollArea.width - viewportWidth));
            }
            else
            {
                x = MathExt.Min(0, MathExt.Max(x, viewportWidth - scrollArea.width));
            }

            if (document.Viewport.ScrollBox.Overflow_Inline == CSS.Enums.EOverflowDirection.Downward)
            {
                y = MathExt.Max(0, MathExt.Min(y, scrollArea.height - viewportHeight));
            }
            else
            {
                y = MathExt.Min(0, MathExt.Max(y, viewportHeight - scrollArea.height));
            }

            /* Let position be the scroll position the viewport would have by aligning the x-coordinate x of the viewport scrolling area with the left of the viewport and aligning the y-coordinate y of the viewport scrolling area with the top of the viewport. */
            double deltaX = x - document.Viewport.Left;
            double deltaY = y - document.Viewport.Top;

            DOMPoint position = new DOMPoint(scrollBox.ScrollX + deltaX, scrollBox.ScrollY + deltaY);
            if (position.Equals(scrollBox.ScrollX, scrollBox.ScrollY) && !scrollBox.IsScrolling)
                return;

            scrollBox.Perform_Scroll(position, document.documentElement, behavior);
        }

        public void Scroll(ScrollToOptions options) => ScrollTo(options);
        public void Scroll(double x, double y) => ScrollTo(x, y);

        public void ScrollTo(ScrollToOptions options)
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-window-scroll */
            if (document.Viewport == null)
                return;

            double x = options.left.HasValue ? options.left.Value : document.Viewport.ScrollBox.ScrollX;
            double y = options.top.HasValue ? options.top.Value : document.Viewport.ScrollBox.ScrollY;

            x = CssCommon.Normalize_Non_Finite(x);
            y = CssCommon.Normalize_Non_Finite(y);

            scroll_window(x, y, options.behavior);
        }
        public void ScrollTo(double x, double y)
        {
            if (document.Viewport == null)
                return;

            x = CssCommon.Normalize_Non_Finite(x);
            y = CssCommon.Normalize_Non_Finite(y);

            scroll_window(x, y, EScrollBehavior.Auto);
        }

        public void ScrollBy(ScrollToOptions options)
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-window-scrollby */
            options.left = !options.left.HasValue ? 0 : CssCommon.Normalize_Non_Finite(options.left.Value);
            options.top = !options.top.HasValue ? 0 : CssCommon.Normalize_Non_Finite(options.top.Value);

            options.left += scrollX;
            options.top += scrollY;

            Scroll(options);
        }
        public void ScrollBy(double x, double y)
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-window-scrollby */
            var options = new ScrollToOptions(EScrollBehavior.Auto, x, y);

            options.left = !options.left.HasValue ? 0 : CssCommon.Normalize_Non_Finite(options.left.Value);
            options.top = !options.top.HasValue ? 0 : CssCommon.Normalize_Non_Finite(options.top.Value);

            options.left += scrollX;
            options.top += scrollY;

            Scroll(options);
        }

        // client

        /// <summary>
        /// The screenX attribute must return the x-coordinate, relative to the origin of the screen of the output device, of the left of the client window as number of pixels, or zero if there is no such thing.
        /// </summary>
        public abstract long screenX { get; }
        /// <summary>
        /// The screenY attribute must return the y-coordinate, relative to the origin of the screen of the output device, of the top of the client window as number of pixels, or zero if there is no such thing.
        /// </summary>
        public abstract long screenY { get; }
        /// <summary>
        /// The outerWidth attribute must return the width of the client window. If there is no client window this attribute must return zero.
        /// </summary>
        public long outerWidth => (screen == null) ? 0 : screen.width;
        /// <summary>
        /// The outerHeight attribute must return the height of the client window. If there is no client window this attribute must return zero.
        /// </summary>
        public long outerHeight => (screen == null) ? 0 : screen.height;
        public double devicePixelRatio { get; }
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
                var candidate = document;
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


        #region Global Events
        public event EventCallback onAbort
        {
            add
            {
                ((IGlobalEventCallbacks)document).onAbort += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onAbort -= value;
            }
        }

        public event EventCallback onAuxClick
        {
            add
            {
                ((IGlobalEventCallbacks)document).onAuxClick += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onAuxClick -= value;
            }
        }

        public event EventCallback onBlur
        {
            add
            {
                ((IGlobalEventCallbacks)document).onBlur += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onBlur -= value;
            }
        }

        public event EventCallback onCancel
        {
            add
            {
                ((IGlobalEventCallbacks)document).onCancel += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onCancel -= value;
            }
        }

        public event EventCallback onCanPlay
        {
            add
            {
                ((IGlobalEventCallbacks)document).onCanPlay += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onCanPlay -= value;
            }
        }

        public event EventCallback onCanPlayThrough
        {
            add
            {
                ((IGlobalEventCallbacks)document).onCanPlayThrough += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onCanPlayThrough -= value;
            }
        }

        public event EventCallback onChange
        {
            add
            {
                ((IGlobalEventCallbacks)document).onChange += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onChange -= value;
            }
        }

        public event EventCallback onClick
        {
            add
            {
                ((IGlobalEventCallbacks)document).onClick += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onClick -= value;
            }
        }

        public event EventCallback onClose
        {
            add
            {
                ((IGlobalEventCallbacks)document).onClose += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onClose -= value;
            }
        }

        public event EventCallback onContextMenu
        {
            add
            {
                ((IGlobalEventCallbacks)document).onContextMenu += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onContextMenu -= value;
            }
        }

        public event EventCallback onCueChange
        {
            add
            {
                ((IGlobalEventCallbacks)document).onCueChange += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onCueChange -= value;
            }
        }

        public event EventCallback onDblClick
        {
            add
            {
                ((IGlobalEventCallbacks)document).onDblClick += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onDblClick -= value;
            }
        }

        public event EventCallback onDrag
        {
            add
            {
                ((IGlobalEventCallbacks)document).onDrag += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onDrag -= value;
            }
        }

        public event EventCallback onDragEnd
        {
            add
            {
                ((IGlobalEventCallbacks)document).onDragEnd += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onDragEnd -= value;
            }
        }

        public event EventCallback onDragEnter
        {
            add
            {
                ((IGlobalEventCallbacks)document).onDragEnter += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onDragEnter -= value;
            }
        }

        public event EventCallback onDragExit
        {
            add
            {
                ((IGlobalEventCallbacks)document).onDragExit += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onDragExit -= value;
            }
        }

        public event EventCallback onDragLeave
        {
            add
            {
                ((IGlobalEventCallbacks)document).onDragLeave += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onDragLeave -= value;
            }
        }

        public event EventCallback onDragOver
        {
            add
            {
                ((IGlobalEventCallbacks)document).onDragOver += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onDragOver -= value;
            }
        }

        public event EventCallback onDragStart
        {
            add
            {
                ((IGlobalEventCallbacks)document).onDragStart += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onDragStart -= value;
            }
        }

        public event EventCallback onDrop
        {
            add
            {
                ((IGlobalEventCallbacks)document).onDrop += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onDrop -= value;
            }
        }

        public event EventCallback onDurationChange
        {
            add
            {
                ((IGlobalEventCallbacks)document).onDurationChange += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onDurationChange -= value;
            }
        }

        public event EventCallback onEmptied
        {
            add
            {
                ((IGlobalEventCallbacks)document).onEmptied += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onEmptied -= value;
            }
        }

        public event EventCallback onEnded
        {
            add
            {
                ((IGlobalEventCallbacks)document).onEnded += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onEnded -= value;
            }
        }

        public event EventCallback onFocus
        {
            add
            {
                ((IGlobalEventCallbacks)document).onFocus += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onFocus -= value;
            }
        }

        public event EventCallback onFormData
        {
            add
            {
                ((IGlobalEventCallbacks)document).onFormData += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onFormData -= value;
            }
        }

        public event EventCallback onInput
        {
            add
            {
                ((IGlobalEventCallbacks)document).onInput += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onInput -= value;
            }
        }

        public event EventCallback onInvalid
        {
            add
            {
                ((IGlobalEventCallbacks)document).onInvalid += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onInvalid -= value;
            }
        }

        public event EventCallback onKeyDown
        {
            add
            {
                ((IGlobalEventCallbacks)document).onKeyDown += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onKeyDown -= value;
            }
        }

        public event EventCallback onKeyPress
        {
            add
            {
                ((IGlobalEventCallbacks)document).onKeyPress += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onKeyPress -= value;
            }
        }

        public event EventCallback onKeyUp
        {
            add
            {
                ((IGlobalEventCallbacks)document).onKeyUp += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onKeyUp -= value;
            }
        }

        public event EventCallback onLoad
        {
            add
            {
                ((IGlobalEventCallbacks)document).onLoad += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onLoad -= value;
            }
        }

        public event EventCallback onLoadedData
        {
            add
            {
                ((IGlobalEventCallbacks)document).onLoadedData += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onLoadedData -= value;
            }
        }

        public event EventCallback onLoadedMetadata
        {
            add
            {
                ((IGlobalEventCallbacks)document).onLoadedMetadata += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onLoadedMetadata -= value;
            }
        }

        public event EventCallback onLoadEnd
        {
            add
            {
                ((IGlobalEventCallbacks)document).onLoadEnd += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onLoadEnd -= value;
            }
        }

        public event EventCallback onLoadStart
        {
            add
            {
                ((IGlobalEventCallbacks)document).onLoadStart += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onLoadStart -= value;
            }
        }

        public event EventCallback onMouseDown
        {
            add
            {
                ((IGlobalEventCallbacks)document).onMouseDown += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onMouseDown -= value;
            }
        }

        public event EventCallback onMouseEnter
        {
            add
            {
                ((IGlobalEventCallbacks)document).onMouseEnter += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onMouseEnter -= value;
            }
        }

        public event EventCallback onMouseLeave
        {
            add
            {
                ((IGlobalEventCallbacks)document).onMouseLeave += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onMouseLeave -= value;
            }
        }

        public event EventCallback onMouseMove
        {
            add
            {
                ((IGlobalEventCallbacks)document).onMouseMove += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onMouseMove -= value;
            }
        }

        public event EventCallback onMouseOut
        {
            add
            {
                ((IGlobalEventCallbacks)document).onMouseOut += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onMouseOut -= value;
            }
        }

        public event EventCallback onMouseOver
        {
            add
            {
                ((IGlobalEventCallbacks)document).onMouseOver += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onMouseOver -= value;
            }
        }

        public event EventCallback onMouseUp
        {
            add
            {
                ((IGlobalEventCallbacks)document).onMouseUp += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onMouseUp -= value;
            }
        }

        public event EventCallback onWheel
        {
            add
            {
                ((IGlobalEventCallbacks)document).onWheel += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onWheel -= value;
            }
        }

        public event EventCallback onPause
        {
            add
            {
                ((IGlobalEventCallbacks)document).onPause += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onPause -= value;
            }
        }

        public event EventCallback onPlay
        {
            add
            {
                ((IGlobalEventCallbacks)document).onPlay += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onPlay -= value;
            }
        }

        public event EventCallback onPlaying
        {
            add
            {
                ((IGlobalEventCallbacks)document).onPlaying += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onPlaying -= value;
            }
        }

        public event EventCallback onProgress
        {
            add
            {
                ((IGlobalEventCallbacks)document).onProgress += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onProgress -= value;
            }
        }

        public event EventCallback onRateChange
        {
            add
            {
                ((IGlobalEventCallbacks)document).onRateChange += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onRateChange -= value;
            }
        }

        public event EventCallback onReset
        {
            add
            {
                ((IGlobalEventCallbacks)document).onReset += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onReset -= value;
            }
        }

        public event EventCallback onResize
        {
            add
            {
                ((IGlobalEventCallbacks)document).onResize += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onResize -= value;
            }
        }

        public event EventCallback onScroll
        {
            add
            {
                ((IGlobalEventCallbacks)document).onScroll += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onScroll -= value;
            }
        }

        public event EventCallback onSecurityPolicyViolation
        {
            add
            {
                ((IGlobalEventCallbacks)document).onSecurityPolicyViolation += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onSecurityPolicyViolation -= value;
            }
        }

        public event EventCallback onSeeked
        {
            add
            {
                ((IGlobalEventCallbacks)document).onSeeked += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onSeeked -= value;
            }
        }

        public event EventCallback onSeeking
        {
            add
            {
                ((IGlobalEventCallbacks)document).onSeeking += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onSeeking -= value;
            }
        }

        public event EventCallback onSelect
        {
            add
            {
                ((IGlobalEventCallbacks)document).onSelect += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onSelect -= value;
            }
        }

        public event EventCallback onStalled
        {
            add
            {
                ((IGlobalEventCallbacks)document).onStalled += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onStalled -= value;
            }
        }

        public event EventCallback onSubmit
        {
            add
            {
                ((IGlobalEventCallbacks)document).onSubmit += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onSubmit -= value;
            }
        }

        public event EventCallback onSuspend
        {
            add
            {
                ((IGlobalEventCallbacks)document).onSuspend += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onSuspend -= value;
            }
        }

        public event EventCallback onTimeUpdate
        {
            add
            {
                ((IGlobalEventCallbacks)document).onTimeUpdate += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onTimeUpdate -= value;
            }
        }

        public event EventCallback onToggle
        {
            add
            {
                ((IGlobalEventCallbacks)document).onToggle += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onToggle -= value;
            }
        }

        public event EventCallback onVolumeChange
        {
            add
            {
                ((IGlobalEventCallbacks)document).onVolumeChange += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onVolumeChange -= value;
            }
        }

        public event EventCallback onWaiting
        {
            add
            {
                ((IGlobalEventCallbacks)document).onWaiting += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onWaiting -= value;
            }
        }

        public event EventCallback onSelectStart
        {
            add
            {
                ((IGlobalEventCallbacks)document).onSelectStart += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onSelectStart -= value;
            }
        }

        public event EventCallback onSelectionChange
        {
            add
            {
                ((IGlobalEventCallbacks)document).onSelectionChange += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onSelectionChange -= value;
            }
        }
        #endregion

        #region Window Events
        public event EventCallback onHashChange;
        public event EventCallback onLanguageChange;
        public event EventCallback onMessage;
        public event EventCallback onMessagEerror;
        public event EventCallback onOffline;
        public event EventCallback onOnline;
        public event EventCallback onPageHide;
        public event EventCallback onPageShow;
        public event EventCallback onPopState;
        public event EventCallback onRejectionHandled;
        public event EventCallback onStorage;
        public event EventCallback onUnhandledRejection;
        #endregion
    }
}
