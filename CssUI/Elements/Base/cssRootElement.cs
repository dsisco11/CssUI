using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyPic.Benchmarking;
using CssUI.CSS;
using KeyPic.CSSUI.Enums;

namespace CssUI
{
    /// <summary>
    /// Represents a Root DOM element which does all the rendering and updating of elements within it.
    /// As well as passing input events to UI elements.
    /// </summary>
    public abstract class cssRootElement : cssContainerElement
    {

        #region Viewport
        public cssViewport Get_Viewport() { return this.Viewport; }
        public void Set_Viewport(int X, int Y, int Width, int Height)
        {
            Viewport.Set(X, Y, Width, Height);
        }
        #endregion
        /// <summary>
        /// The containing block of this control
        /// <para>If the control has an ancestor this will be said ancestors content-area block</para>
        /// <para>Otherwise, if the control is a root element, this should have the dimensions of the viewport</para>
        /// </summary>
        public override eBlock Block_Containing { get { return Viewport.Block; } }

        #region Properties

        public readonly MouseManager Mouse = new MouseManager();
        /// <summary>
        /// Rendering engine for drawing the UI
        /// </summary>
        public readonly IRenderEngine Engine;
        private ulong Frame = 0;
        internal ulong FrameNo { get { return Frame; } }
        internal readonly Logging.LogModule Logs;
        public long MouseEnterTime;
        ScheduledFunction MouseHover_TMR;
        #endregion

        #region Input Focus
        /// <summary>
        /// UI Element that currently has input focus.
        /// </summary>
        public cssElement FocusedElement { get; private set; } = null;

        /// <summary>
        /// Sets the input focus to a specified element
        /// </summary>
        /// <param name="E"></param>
        public void SetFocus(cssElement E)
        {
            if (object.ReferenceEquals(E, FocusedElement)) return;
            if (E.HasFlags(EElementFlags.Focusable))
            {
                Unfocus(FocusedElement);
                FocusedElement = E;
            }
        }
        /// <summary>
        /// Removes input focus from a specified element
        /// </summary>
        /// <param name="E"></param>
        public void Unfocus(cssElement E)
        {
            if (object.ReferenceEquals(FocusedElement, E))
            {
                FocusedElement?.Handle_InputFocusLose();// Notify this element that it nolonger has active focus
                FocusedElement = null;
            }
        }
        #endregion

        #region Constructors
        public cssRootElement(IRenderEngine Engine) : base("#Root")
        {
            this.Logs = new Logging.LogModule(() => { return Frame.ToString(); });
            this.Engine = Engine;

            MouseHover_TMR = new ScheduledFunction(TimeSpan.FromMilliseconds(UI_CONSTANTS.HOVER_TIME), () =>
            {
                Handle_MouseHover(this);
            });

            Viewport = new cssViewport();
            Viewport.Resized += Viewport_Resized;
            Viewport.Moved += Viewport_Moved;

            FocusScope = new FocusScope();

            Style.Default.BoxSizing.Set(EBoxSizingMode.BORDER);
            Style.Default.Display.Set(EDisplayMode.BLOCK);
            Style.Default.Width.Assigned = CSSValue.From_Percent(100.0);// Always match the viewport size
            Style.Default.Height.Assigned = CSSValue.From_Percent(100.0);// Always match the viewport size
            Set_Root(this);
            
            Style.Default.Set_Padding_Implicit(2, 2);
            onControl_Removed += RootElement_onControl_Removed;

            // Setup the default font.
            Style.Default.FontSize.Set(14);
            Style.Default.FontFamily.Set("Arial");
            Style.Default.FontWeight.Set(700);
            
        }

        #region Event Handlers (Viewport)
        private void Viewport_Moved(ePos oldPos, ePos newPos)
        {
            Flag_Layout(ELayoutBit.Dirty);
            Flag_Containing_Block_Dirty();
        }

        private void Viewport_Resized(eSize oldSize, eSize newSize)
        {
            Flag_Layout(ELayoutBit.Dirty);
            Flag_Containing_Block_Dirty();
        }
        #endregion

        private void RootElement_onControl_Removed(cssElement Sender, cssElement E)
        {
            if (EnteredElements.ContainsKey(E.UID)) EnteredElements.Remove(E.UID);
        }
        #endregion

        #region Element ID Map
        /// <summary>
        /// Maps unique element ID strings to the elements which currently own that ID string
        /// </summary>
        protected readonly Dictionary<string, cssElement> ID_MAP = new Dictionary<string, cssElement>();

        /// <summary>
        /// Registers an element and it's unique ID in the Root's ID map
        /// </summary>
        public void Register_Element_ID(string ID, cssElement E)
        {
            if (!ID.StartsWith("#")) ID = string.Concat("#", ID);
            if (ID_MAP.ContainsKey(ID))
            {
                if (!object.ReferenceEquals(ID_MAP[ID], E))
                    throw new Exception("Another element with that ID already exists!");
            }

            ID_MAP.Add(ID, E);
        }

        /// <summary>
        /// Unregisters an element and it's unique ID from the Root's ID map
        /// </summary>
        public void Unregister_Element(cssElement E)
        {
            string ID = E.ID;
            if (string.IsNullOrEmpty(ID)) return;

            if (!ID.StartsWith("#")) ID = string.Concat("#", ID);
            ID_MAP.Remove(ID);
        }

        /// <summary>
        /// Returns the element associated with the given unique ID or NULL if none
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public cssElement Find_ID(string ID)
        {
            if (!ID.StartsWith("#")) ID = string.Concat("#", ID);

            cssElement e = null;
            ID_MAP.TryGetValue(ID, out e);

            return e;
        }

        /// <summary>
        /// Returns the element associated with the given unique ID or NULL if none
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Ty Find_ID<Ty>(string ID) where Ty : cssElement
        {
            return (Ty)Find_ID(ID);
        }
        #endregion

        #region Hit Testing
        /// <summary>
        /// Performs recursive hit tests against a screen-space point until the deepest element intersecting said point is found.
        /// </summary>
        /// <param name="pos">Screen-space point to perform hit testing against</param>
        cssElement Resolve_ScreenSpace_HitTest(ePos pos)
        {
            cssElement last = this;
            do
            {
                cssElement E = last.Get_Hit_Element(pos);
                if (E == last) return E;
                last = E;
            }
            while (last != null);

            return null;
        }
        /// <summary>
        /// Performs recursive hit tests against a screen-space point until the deepest element intersecting said point is found.
        /// </summary>
        /// <param name="pos">Screen-space point to perform hit testing against</param>
        cssElement Resolve_ScreenSpace_HitTest(ePos pos, ref List<cssElement> Path)
        {
            cssElement last = this;
            do
            {
                cssElement E = last.Get_Hit_Element(pos);
                if (E == last) return E;
                Path.Add(last);
                last = E;
            }
            while (last != null);

            return null;
        }
        #endregion

        #region Mouse Handlers
        ePos MouseDragStart = ePos.Zero;
        cssElement MouseDragTarget = null;
        /// <summary>
        /// Tracks the UID of elements for which the MouseEnter event has fired. Used for determining when to fire the MouseLeave event
        /// </summary>
        Dictionary<uint, cssElement> EnteredElements = new Dictionary<uint, cssElement>();
        struct LastClickData { public long Time; public ePos Pos; };
        Dictionary<EMouseButton, LastClickData> LastClick = new Dictionary<EMouseButton, LastClickData>();

        protected void Fire_MouseUp(DomPreviewMouseButtonEventArgs e)
        {
            cssElement Origin = Resolve_ScreenSpace_HitTest(e.Position);
            Halt_ItemDrag_Operation(e.Position);
            // Perform the tunneling event sequence
            var PreviewArgs = new DomPreviewMouseButtonEventArgs(e);
            cssElement pE = this;
            do
            {
                pE.Handle_PreviewMouseUp(pE, PreviewArgs);
                var tE = pE.Get_Hit_Element(PreviewArgs.Position);
                if (object.ReferenceEquals(pE, tE)) break;
                pE = tE;
            }
            while (!PreviewArgs.Handled && pE != null);
            // The Routed event methodology dictates that when a preview event gets intercepted and handled the corrosponding bubbling event is not fired!
            if (PreviewArgs.Handled) return;

            // Perform the bubbling event sequence
            var Args = new DomMouseButtonEventArgs(Origin, PreviewArgs) { Source = PreviewArgs.Handler };
            cssElement E = (cssElement)PreviewArgs.Handler;
            do
            {
                E.Handle_MouseUp(E, Args);
                if (!Args.Handled) E = E.Parent;
            }
            while (E != null && !Args.Handled);
        }

        protected void Fire_MouseDown(DomPreviewMouseButtonEventArgs e)
        {
            List<cssElement> Path = new List<cssElement>();
            cssElement Origin = Resolve_ScreenSpace_HitTest(e.Position, ref Path);
            if (Origin == null)
            {
                Halt_ItemDrag_Operation(e.Position, true);
                return;
            }
            // Prepare to handle dragging events
            if (Origin.IsDraggable)
            {
                MouseDragTarget = Origin;
                MouseDragStart = new ePos(e.Position);
            }

            // Perform the tunneling event sequence
            var PreviewArgs = new DomPreviewMouseButtonEventArgs(e);
            cssElement pE = this;
            do
            {
                pE.Handle_PreviewMouseDown(pE, PreviewArgs);
                var tE = pE.Get_Hit_Element(PreviewArgs.Position);
                if (object.ReferenceEquals(pE, tE)) break;
                pE = tE;
            }
            while (!PreviewArgs.Handled && pE != null);
            // The Routed event methodology dictates that when a preview event gets intercepted and handled the corrosponding bubbling event is not fired!
            if (PreviewArgs.Handled) return;

            // Perform the bubbling event sequence
            var Args = new DomMouseButtonEventArgs(Origin, PreviewArgs) { Source = PreviewArgs.Handler };
            cssElement E = (cssElement)PreviewArgs.Handler;
            do
            {
                E.Handle_MouseDown(E, Args);
                if (!Args.Handled) E = E.Parent;
            }
            while (E != null && !Args.Handled);

            // Now we want to handle firing any "Click" type events

            bool isDoubleClick = false;
            if (LastClick.ContainsKey(e.Button))
            {// Double-Click events require the second click to be within a certain distance and timespan of the first
                float deltaTime = ((float)(Environment.TickCount - LastClick[e.Button].Time) / 1000.0f);
                float dX = (e.Position.X - LastClick[e.Button].Pos.X);
                float dY = (e.Position.Y - LastClick[e.Button].Pos.Y);
                // This might be a double click
                // if the time between now and the last click is less than the systems set double click threshold then this is a double click
                if (deltaTime <= Platform.Factory.SystemMetrics.Get_Double_Click_Time())
                {
                    System.Drawing.Point mx = Platform.Factory.SystemMetrics.Get_DoubleClick_Distance_Threshold();
                    if (mx.X > dX && mx.Y > dY)
                    {
                        isDoubleClick = true;
                    }
                }
            }
            // Update our last click tracking data
            LastClick[e.Button] = new LastClickData() { Time = Environment.TickCount, Pos = new ePos(e.X, e.Y) };

            Fire_Click(Origin, Path);// The 'click' event is raised everytime an element is double-clicked also
            if (isDoubleClick)
            {
                Fire_DoubleClick(Origin, Path);
                Fire_MouseDoubleClick(e);
            }
            else
            {
                Fire_MouseClick(e);// The MouseClick event, unlike the Click event, is NOT called everytime a double-click is executed.
            }
        }

        protected void Fire_MouseMove(DomPreviewMouseMoveEventArgs e)
        {
            Mouse.Location.X = e.X;
            Mouse.Location.Y = e.Y;

            List<cssElement> Path = new List<cssElement>();
            cssElement Origin = Resolve_ScreenSpace_HitTest(e.Position, ref Path);
            // Perform the tunneling event sequence
            var PreviewArgs = new DomPreviewMouseMoveEventArgs(e);
            cssElement pE = this;
            do
            {
                pE.Handle_PreviewMouseMove(pE, PreviewArgs);
                var tE = pE.Get_Hit_Element(PreviewArgs.Position);
                if (object.ReferenceEquals(pE, tE)) break;
                pE = tE;
            }
            while (!PreviewArgs.Handled && pE != null);

            // The Routed event methodology dictates that when a preview event gets intercepted and handled the corrosponding bubbling event is not fired!
            if (PreviewArgs.Handled) return;

            // Perform the bubbling event sequence
            var Args = new DomMouseMoveEventArgs(Origin, PreviewArgs) { Source = PreviewArgs.Handler };
            cssElement E = (cssElement)PreviewArgs.Handler;
            do
            {
                if (!EnteredElements.ContainsKey(E.UID))
                {
                    EnteredElements.Add(E.UID, E);
                    E.Handle_MouseEnter(E);
                }
                E.Handle_MouseMove(E, Args);

                if (!Args.Handled) E = E.Parent;
            }
            while (E != null && !Args.Handled);

            // Perform mouse leave events
            List<uint> Trash = new List<uint>();
            foreach (KeyValuePair<uint, cssElement> kv in EnteredElements)
            {
                cssElement tE = kv.Value;
                if (object.ReferenceEquals(MouseDragTarget, tE)) continue;
                if (tE.IsMouseOver)
                {
                    if (!tE.HitTest_ScreenPos(e.Position))
                    {
                        Trash.Add(tE.UID);
                        tE.Handle_MouseLeave(tE);
                    }
                }
            }

            foreach(uint UID in Trash)
            {
                EnteredElements.Remove(UID);
            }

            if (MouseDragTarget != null)
            {
                if (!MouseDragTarget.IsBeingDragged)
                {
                    var dArgs = new DomItemDragEventArgs(MouseDragStart, new ePos(Args.Position));
                    if (Math.Abs(dArgs.XDelta) > UI_CONSTANTS.DRAG_START_THRESHOLD || Math.Abs(dArgs.YDelta) > UI_CONSTANTS.DRAG_START_THRESHOLD)
                    {
                        Root.Mouse.Start_Dragging(MouseDragTarget, MouseDragTarget, dArgs);
                    }
                }
                else
                {
                    var dArgs = new DomItemDragEventArgs(MouseDragStart, new ePos(Args.Position));
                    MouseDragTarget.Handle_DraggingUpdate(MouseDragTarget, dArgs);
                    if (dArgs.Abort)
                    {
                        Halt_ItemDrag_Operation(dArgs.Location, true);
                    }
                }

            }
        }

        /// <summary>
        /// Fires a dummy MouseMove event, mostly used to update elements when their parent scrolls.
        /// </summary>
        /// <param name="e"></param>
        internal void Fire_Dummy_MouseMove()
        {
            Fire_MouseMove(new DomPreviewMouseMoveEventArgs(Mouse.Location.X, Mouse.Location.Y, 0, 0));
        }

        void Halt_ItemDrag_Operation(ePos MousePos, bool Aborted=false)
        {
            var Args = new DomItemDragEventArgs(MouseDragStart, MousePos) { Abort = Aborted };

            Root.Mouse.Stop_Dragging(MouseDragTarget, Args);
            MouseDragTarget = null;
            MouseDragStart = ePos.Zero;
        }

        /// <summary>
        /// Indicates the mouse has left the entire root element's area, and thus all of it's children aswell
        /// </summary>
        protected void Fire_MouseLeave()
        {
            foreach(KeyValuePair<uint, cssElement> kv in EnteredElements)
            {
                cssElement E = kv.Value;
                if (E.IsMouseOver) E.Handle_MouseLeave(E);
            }
            EnteredElements.Clear();
        }
        
        protected void Fire_MouseWheel(DomPreviewMouseWheelEventArgs e)
        {
            cssElement Origin = Resolve_ScreenSpace_HitTest(e.Position);
            // Perform the tunneling event sequence
            var PreviewArgs = new DomPreviewMouseWheelEventArgs(e);
            cssElement pE = this;
            do
            {
                pE.Handle_PreviewMouseWheel(pE, PreviewArgs);
                var tE = pE.Get_Hit_Element(PreviewArgs.Position);
                if (object.ReferenceEquals(pE, tE)) break;
                pE = tE;
            }
            while (!PreviewArgs.Handled && pE != null);

            // The Routed event methodology dictates that when a preview event gets intercepted and handled the corrosponding bubbling event is not fired!
            if (PreviewArgs.Handled) return;
            // Perform the bubbling event sequence
            var Args = new DomMouseWheelEventArgs(Origin, PreviewArgs) { Source = PreviewArgs.Handler };
            cssElement E = (cssElement)PreviewArgs.Handler;
            do
            {
                E.Handle_MouseWheel(E, Args);
                if (!Args.Handled) E = E.Parent;
            }
            while (E != null && !Args.Handled);
        }
        
        protected void Fire_MouseClick(DomPreviewMouseButtonEventArgs e)
        {
            cssElement Origin = Resolve_ScreenSpace_HitTest(e.Position);
            // Perform the tunneling event sequence
            var PreviewArgs = new DomPreviewMouseButtonEventArgs(e);
            cssElement pE = this;
            do
            {
                if (pE.HasFlags(EElementFlags.Clickable))
                {
                    pE.Handle_PreviewMouseClick(pE, PreviewArgs);
                }
                var tE = pE.Get_Hit_Element(PreviewArgs.Position);
                if (object.ReferenceEquals(pE, tE)) break;// If no elements within the current one being tested are hit by this mouse click then break
                pE = tE;
            }
            while (!PreviewArgs.Handled && pE != null);

            // The Routed event methodology dictates that when a preview event gets intercepted and handled the corrosponding bubbling event is not fired!
            if (PreviewArgs.Handled) return;

            // Perform the bubbling event sequence
            var Args = new DomMouseButtonEventArgs(Origin, PreviewArgs) { Source = PreviewArgs.Handler };
            cssElement E = (cssElement)PreviewArgs.Handler;
            do
            {
                if (E.HasFlags(EElementFlags.Clickable))
                {
                    E.Handle_MouseClick(E, Args);
                }
                if (!Args.Handled) E = E.Parent;
            }
            while (E != null && !Args.Handled);
        }

        protected void Fire_MouseDoubleClick(DomPreviewMouseButtonEventArgs e)
        {
            cssElement Origin = Resolve_ScreenSpace_HitTest(e.Position);
            // Perform the tunneling event sequence
            var PreviewArgs = new DomPreviewMouseButtonEventArgs(e);
            cssElement pE = this;
            do
            {
                if (pE.HasFlags(EElementFlags.Clickable))
                {
                    if (pE.HasFlags(EElementFlags.DoubleClickable))
                    {
                        pE.Handle_PreviewMouseDoubleClick(pE, PreviewArgs);
                    }
                    else
                    {
                        pE.Handle_PreviewMouseClick(pE, PreviewArgs);
                    }
                }
                var tE = pE.Get_Hit_Element(PreviewArgs.Position);
                if (object.ReferenceEquals(pE, tE)) break;
                pE = tE;
            }
            while (!PreviewArgs.Handled && pE != null);
            // The Routed event methodology dictates that when a preview event gets intercepted and handled the corrosponding bubbling event is not fired!
            if (PreviewArgs.Handled) return;

            // Perform the bubbling event sequence
            var Args = new DomMouseButtonEventArgs(Origin, PreviewArgs) { Source = PreviewArgs.Handler };
            cssElement E = (cssElement)PreviewArgs.Handler;
            do
            {
                if (E.HasFlags(EElementFlags.Clickable))
                {
                    if (E.HasFlags(EElementFlags.DoubleClickable))
                    {
                        E.Handle_MouseDoubleClick(E, Args);
                    }
                    else
                    {
                        E.Handle_MouseClick(E, Args);
                    }
                }
                if (!Args.Handled) E = E.Parent;
            }
            while (E != null && !Args.Handled);
        }
        protected void Fire_Click(cssElement Origin, List<cssElement> Path)
        {
            Stack<cssElement> eStack = new Stack<cssElement>(Path.Count);
            // Perform the tunneling event sequence
            var PreviewArgs = new DomPreviewEventArgs();
            foreach(cssElement E in Path)
            {
                eStack.Push(E);
                if (E.HasFlags(EElementFlags.Clickable))
                {
                    E.Handle_PreviewClick(E, PreviewArgs);
                }
                if (PreviewArgs.Handled) break;
            }
            // The Routed event methodology dictates that when a preview event gets intercepted and handled the corrosponding bubbling event is not fired!
            if (PreviewArgs.Handled) return;

            // Perform the bubbling event sequence
            var Args = new DomRoutedEventArgs(Origin) { Source = PreviewArgs.Handler };
            while (eStack.Count > 0 && !Args.Handled)
            {
                var E = eStack.Pop();
                if (E.HasFlags(EElementFlags.Clickable))
                {
                    E.Handle_Click(E, Args);
                }
            }
        }
        protected void Fire_DoubleClick(cssElement Origin, List<cssElement> Path)
        {
            Stack<cssElement> eStack = new Stack<cssElement>(Path.Count);
            // Perform the tunneling event sequence
            var PreviewArgs = new DomPreviewEventArgs();
            foreach (cssElement E in Path)
            {
                eStack.Push(E);
                if (E.HasFlags(EElementFlags.Clickable) && E.HasFlags(EElementFlags.DoubleClickable))
                {
                    E.Handle_PreviewDoubleClick(E, PreviewArgs);
                }
                if (PreviewArgs.Handled) break;
            }
            // The Routed event methodology dictates that when a preview event gets intercepted and handled the corrosponding bubbling event is not fired!
            if (PreviewArgs.Handled) return;

            // Perform the bubbling event sequence
            var Args = new DomRoutedEventArgs(Origin) { Source = PreviewArgs.Handler };
            while (eStack.Count > 0 && !Args.Handled)
            {
                var E = eStack.Pop();
                if (E.HasFlags(EElementFlags.Clickable) && E.HasFlags(EElementFlags.DoubleClickable))
                {
                    E.Handle_DoubleClick(E, Args);
                }
            }
        }


        /*
        private void Process_Routed_MouseEvent<PreviewArgsTy, BubblingArgsTy>(PreviewArgsTy PreviewArgs, Action<uiElement, PreviewArgsTy> PreviewHandler, Action<uiElement, BubblingArgsTy> BubblingHandler) 
            where PreviewArgsTy : PreviewMouseEventArgs
            where BubblingArgsTy : RoutedMouseEventArgs
        {
            uiElement Origin = Resolve_ScreenSpace_HitTest(PreviewArgs.Position);
            // Perform the tunneling event sequence
            uiElement pE = this;// Start at this (root) element
            do
            {
                PreviewHandler(pE, PreviewArgs);

                var tE = pE.Get_Hit_Element(PreviewArgs.Position);
                if (object.ReferenceEquals(pE, tE)) break;// If no elements within the current one being tested are hit by this mouse click then break
                pE = tE;
            }
            while (!PreviewArgs.Handled && pE != null);
            
            // The Routed event methodology dictates that when a preview event gets intercepted and handled the corrosponding bubbling event is not fired!
            if (PreviewArgs.Handled) return;

            // Perform the bubbling event sequence
            var Args = new BubblingArgsTy(Origin, PreviewArgs) { Source = PreviewArgs.Handler };
            uiElement E = (uiElement)PreviewArgs.Handler;
            do
            {
                BubblingHandler(E, Args);
                if (!Args.Handled) E = E.Parent;
            }
            while (E != null && !Args.Handled);
        }
        */


        /// <summary>
        /// Called whenever the mouse first moves overtop the element.
        /// <para>Fires before MouseMove</para>
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public override void Handle_MouseEnter(cssElement Sender)
        {
            MouseEnterTime = Environment.TickCount;
            MouseHover_TMR?.Start();

            base.Handle_MouseEnter(Sender);
        }
        /// <summary>
        /// Called whenever the mouse moves off the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        public override void Handle_MouseLeave(cssElement Sender)
        {
            MouseHover_TMR?.Stop();
            base.Handle_MouseLeave(Sender);
        }

        public override void Handle_MouseMove(cssElement Sender, DomMouseMoveEventArgs Args)
        {
            if (Mouse.Dragging_Target != null)
            {
                Mouse.Dragging_Target.Handle_MouseMove(this, Args);
            }
            else
            {
                base.Handle_MouseMove(Sender, Args);
            }
        }
        #endregion

        #region Keyboard Event Handlers
        public override bool Handle_KeyUp(cssElement Sender, DomKeyboardKeyEventArgs Args)
        {
        #if DEBUG == true
            if (Args.Key == EKey.F11)
            {
                Benchmark.Print_All();
            }
        #endif
            return base.Handle_KeyUp(Sender, Args);
        }
        #endregion

        #region Drawing
        /// <summary>
        /// Updates and renders all controls managed by this root element
        /// </summary>
        public void Present()
        {
            Frame++;
            Update();
            Engine.Begin();

            Render();

            Engine.End();
            //Viewport.Scissor_Safety_Check();
        }
        #endregion

    }
}
