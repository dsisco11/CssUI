using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CssUI.CSS;
using CssUI.Enums;
using System.Numerics;
using CssUI.Types;
using xLog;
using System.Threading;
using CssUI.Rendering;

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

        #region Properties

        public readonly MouseManager Mouse = new MouseManager();
        /// <summary>
        /// Rendering engine for drawing the UI
        /// </summary>
        public readonly IRenderEngine Engine;
        private ulong Frame = 0;
        internal ulong FrameNo { get { return Frame; } }
        internal readonly ILogger Logs;
        public long MouseEnterTime;
        ScheduledFunction MouseHover_TMR;
        #endregion

        #region Multithreaded Updater
        private Task updaterTask;
        CancellationTokenSource updaterCancel;
        ManualResetEvent updaterFree;

        private void Start_Updater()
        {
            // Create a new cancellation token
            updaterCancel = new CancellationTokenSource();
            updaterFree = new ManualResetEvent(false);
            updaterTask = Task.Factory.StartNew(Threaded_Updater, updaterCancel.Token, TaskCreationOptions.LongRunning);
        }
        private void Stop_Updater()
        {
            if (updaterCancel != null)
            {
                updaterCancel.Cancel();
                updaterFree.WaitOne();

                if (updaterTask != null)
                {
                    updaterTask.Wait();
                    updaterTask.Dispose();
                    updaterTask = null;
                }
            }
        }

        private void Threaded_Updater(object state)
        {
            CancellationToken cancelToken = (CancellationToken)state;
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "cssRoodElement Updater";

            while (!cancelToken.IsCancellationRequested)
            {
                cancelToken.ThrowIfCancellationRequested();
                Update();
                xLog.Log.Info("- update - end");
            }

            // DIE
            if (updaterCancel != null)
            {
                updaterCancel.Dispose();
                updaterCancel = null;
            }

            updaterFree.Set();
        }
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
        public cssRootElement(IRenderEngine Engine) : base(null, null, "#Root")
        {
            Start_Updater();
            this.Logs = xLog.LogFactory.GetLogger(() => { return Frame.ToString(); });
            this.Engine = Engine;
            Set_Root(this);

            MouseHover_TMR = new ScheduledFunction(TimeSpan.FromMilliseconds(UI_CONSTANTS.HOVER_TIME), () =>
            {
                Handle_MouseHover(this);
            });

            Viewport = new cssViewport();
            Viewport.Resized += Viewport_Resized;
            Viewport.Moved += Viewport_Moved;

            FocusScope = new FocusScope();

            // The root element should always set and maintain the DPI values for it's style so it's children will all use the correct DPI.
            IntPtr hwnd = Platform.Factory.SystemWindows.Get_Window();
            Vector2 dpi = Platform.Factory.SystemMetrics.Get_DPI(hwnd);
            Style.ImplicitRules.DpiX.Set(CssValue.From_Number(dpi.X));
            Style.ImplicitRules.DpiY.Set(CssValue.From_Number(dpi.Y));


            // Style ourself

            Style.ImplicitRules.BoxSizing.Set(EBoxSizingMode.BorderBox);
            Style.ImplicitRules.Display.Set(EDisplayMode.BLOCK);
            Style.ImplicitRules.Width.Set( CssValue.From_Percent(100.0));// Always match the viewport size
            Style.ImplicitRules.Height.Set( CssValue.From_Percent(100.0));// Always match the viewport size
            
            Style.ImplicitRules.Set_Padding(2, 2);


            //Style.ImplicitRules.FontSize.Set(14);
            //Style.ImplicitRules.FontWeight.Set(700);

        }

        #region Event Handlers (Viewport)
        private void Viewport_Moved(Vec2i oldPos, Vec2i newPos)
        {
            Flag_Layout(ELayoutDirt.Dirty);
            Handle_Containing_Block_Dirty();
        }

        private void Viewport_Resized(Size2D oldSize, Size2D newSize)
        {
            Flag_Layout(ELayoutDirt.Dirty);
            Handle_Containing_Block_Dirty();
        }
        #endregion

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
            string ID = E.id;
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
        public Ty Find_By_ID<Ty>(string ID) where Ty : cssElement
        {
            return (Ty)Find_ID(ID);
        }
        #endregion

        #region Hit Testing
        /// <summary>
        /// Performs recursive hit tests against a screen-space point until the deepest element intersecting said point is found.
        /// </summary>
        /// <param name="pos">Screen-space point to perform hit testing against</param>
        cssElement Resolve_ScreenSpace_HitTest(Vec2i pos)
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
        cssElement Resolve_ScreenSpace_HitTest(Vec2i pos, ref List<cssElement> Path)
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

        #region Drawing
        /// <summary>
        /// Updates and renders all controls managed by this root element
        /// </summary>
        public void Present()
        {
            Frame++;
            // Update();
            Engine.Begin();
            Render();
            Engine.End();
            //Viewport.Scissor_Safety_Check();
        }
        #endregion

    }
}
