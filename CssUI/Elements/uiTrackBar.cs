using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CssUI.CSS;
using CssUI.DOM;
using CssUI.Enums;

namespace CssUI
{
    /// <summary>
    /// Represents a UI element consisting of a track and slideable thumb element which can be dragged along the track.
    /// </summary>
    public class uiTrackBar : cssScrollableElement
    {
        public static readonly new string CssTagName = "TrackBar";

        #region Events
        public event Action<uiTrackBar> ValueChanged;
        #endregion

        ESliderDirection Direction = ESliderDirection.Vertical;
        
        #region Component Declerations
        public readonly cssBox Thumb;
        #endregion

        #region Values
        int oldValue = 0;
        int oldValueMax = 0;

        int value = 0;
        int value_max = 0;

        bool Dirty_Thumb = false;

        /// <summary>
        /// Current scroll-value for this slider
        /// </summary>
        public int Value
        {
            get { return value; }
            set
            {
                this.value = MathExt.Clamp(value, 0, value_max);
                if (this.value != oldValue)
                {
                    oldValue = this.value;
                    Dirty_Thumb = true;
                    ValueChanged?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Maximum value for this slider
        /// </summary>
        public int ValueMax
        {
            get { return value_max; }
            set
            {
                value_max = value;
                if (value_max != oldValueMax)
                {
                    oldValueMax = value_max;
                    Dirty_Thumb = true;
                    if (value_max < this.value) Value = value_max;
                }
            }
        }

        /// <summary>
        /// How much of a single value each pixel on the trackbar represents
        /// </summary>
        public float ValuePerPixel { get { return ((float)ValueMax / (float)(Get_Major_Dimension(Box.Content.Get_Dimensions() - Thumb.Box.Get_Dimensions()))); } }
        #endregion

        #region Constructors
        public uiTrackBar(Document document, IParentElement Parent, ESliderDirection Dir, string className = null, string ID = null) : base(document, Parent, className, ID)
        {
            Flags_Remove(EElementFlags.DoubleClickable);// Trackbars cannot fire double-click events.
            this.Direction = Dir;
            // So we don't get scrollbars
            Style.ImplicitRules.Overflow_X.Set(EOverflowMode.Visible);
            Style.ImplicitRules.Overflow_Y.Set(EOverflowMode.Visible);

            Thumb = new cssBox(this.ownerDocument, this);
            Thumb.Flags_Add(EElementFlags.Draggable);
            Thumb.Color = new cssColor(0.4f, 0.4f, 0.4f, 1.0f);
            Thumb.DraggingStart += Thumb_DraggingStart;
            Thumb.DraggingUpdate += Thumb_DraggingUpdate;
            Thumb.DraggingEnd += Thumb_DraggingEnd;
            // Do not allow (left) mouse click events to tunnel past the thumb element to the trackbar, or else it loses dragging functionality
            Thumb.PreviewMouseClick += (cssElement e, DomPreviewMouseButtonEventArgs args) =>
            {
                if (args.Button.HasFlag(EMouseButton.Left))
                {
                    args.Handled = true;
                }
            };

            //Thumb.Debug.Draw_Bounds = true;
                        
            switch (Dir)
            {
                case ESliderDirection.Vertical:
                    {
                        Thumb.Style.ImplicitRules.Width.Set(CssValue.Pct_OneHundred);
                    }
                    break;
                case ESliderDirection.Horizontal:
                    {
                        Thumb.Style.ImplicitRules.Height.Set(CssValue.Pct_OneHundred);
                    }
                    break;
            }
        }
        
        #endregion

        #region Helpers

        /// <summary>
        /// Transforms a point on the trackbar to an int for use as the slider's current Value
        /// </summary>
        int Track_Point_To_Value(Vec2i Point)
        {
            int retVal = Value;
            int cp = Get_Major_Dimension(Point);
            int ctr = Get_Major_Dimension(Box.Get_Position());
            int cth = Get_Major_Dimension(Thumb.Box.Get_Dimensions());
            int rel = (cp - ctr);// point value relative to the trackbar
            int rv = (int)(ValuePerPixel * (float)rel);
            return rv;
        }

        /// <summary>
        /// Returns the value we are most concerned with from the given item, as indicated by our ESliderDirection value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Get_Major_Dimension(Size2D Size)
        {
            switch (Direction)
            {
                case ESliderDirection.Vertical:
                    return Size.Height;
                case ESliderDirection.Horizontal:
                    return Size.Width;
                default:
                    throw new ArgumentException("Unhandled ESliderDirection value!");
            }
        }

        /// <summary>
        /// Returns the value we are most concerned with from the given item, as indicated by our ESliderDirection value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Get_Major_Dimension(Vec2i Pos)
        {
            switch (Direction)
            {
                case ESliderDirection.Vertical:
                    return Pos.Y;
                case ESliderDirection.Horizontal:
                    return Pos.X;
                default:
                    throw new ArgumentException("Unhandled ESliderDirection value!");
            }
        }
        /*
                /// <summary>
                /// Returns the value we are most concerned with from the given item, as indicated by our ESliderDirection value
                /// </summary>
                public int Get_Major_Dimension(System.Drawing.Point Pos)
                {
                    switch (Direction)
                    {
                        case ESliderDirection.Vertical:
                            return Pos.Y;
                        case ESliderDirection.Horizontal:
                            return Pos.X;
                        default:
                            throw new ArgumentException("Unhandled ESliderDirection value!");
                    }
                }*/

        /// <summary>
        /// Returns the value we are least concerned with from the given item, as indicated by our ESliderDirection value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Get_Minor_Dimension(Size2D Size)
        {
            switch (Direction)
            {
                case ESliderDirection.Vertical:
                    return Size.Width;
                case ESliderDirection.Horizontal:
                    return Size.Height;
                default:
                    throw new ArgumentException("Unhandled ESliderDirection value!");
            }
        }

        /// <summary>
        /// Returns the value we are least concerned with from the given item, as indicated by our ESliderDirection value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Get_Minor_Dimension(Vec2i Pos)
        {
            switch (Direction)
            {
                case ESliderDirection.Vertical:
                    return Pos.X;
                case ESliderDirection.Horizontal:
                    return Pos.Y;
                default:
                    throw new ArgumentException("Unhandled ESliderDirection value!");
            }
        }

        /*/// <summary>
        /// Returns the value we are least concerned with from the given item, as indicated by our ESliderDirection value
        /// </summary>
        public int Get_Minor_Dimension(System.Drawing.Point Pos)
        {
            switch (Direction)
            {
                case ESliderDirection.Vertical:
                    return Pos.X;
                case ESliderDirection.Horizontal:
                    return Pos.Y;
                default:
                    throw new ArgumentException("Unhandled ESliderDirection value!");
            }
        }*/
        #endregion

        #region Events

        /// <summary>
        /// Offset from the thumb's block origin where the mouse initially started dragging it from.
        /// </summary>
        Vec2i Thumb_Pos_At_DragStart = null;
        private void Thumb_DraggingStart(cssElement Sender, DomItemDragEventArgs Args)
        {
            Thumb_Pos_At_DragStart = Thumb.Box.Get_Position();
        }
        private void Thumb_DraggingEnd(cssElement Sender, DomItemDragEventArgs Args)
        {
            Thumb_Pos_At_DragStart = null;
        }

        private void Thumb_DraggingUpdate(cssElement Sender, DomItemDragEventArgs Args)
        {
            // okay we want to move the thumb such that the point the user clicked on to initiate the drag coincides with the current mouse location
            Vec2i Delta = new Vec2i(Args.XDelta, Args.YDelta);
            if (Math.Abs(Get_Minor_Dimension(Delta)) < UI_CONSTANTS.TRACKBAR_DRAG_THRESHOLD)
            {
                var nPos = (Delta + Thumb_Pos_At_DragStart);// this is where our thumb should be now
                Value = Calculate_Value_For_Thumb_Pos(nPos);
            }
            else// if the users mouse gets too far away from the control then we kind of cancel the action by returning to our previous position
            {
                Value = Calculate_Value_For_Thumb_Pos(Thumb_Pos_At_DragStart);
            }
            Update_Thumb();
        }
        #endregion

        #region Drawing
        public override bool Update()
        {
            bool retVal = false;
            if (Dirty_Thumb)
            {
                if (Update_Thumb())
                {
                    retVal = true;
                }
            }
            if (base.Update()) retVal = true;

            return retVal;
        }
        #endregion

        protected override void Update_ScrollArea()
        {
            base.Update_ScrollArea();
            Dirty_Thumb = true;
            Update_Thumb();
        }

        #region Thumb Updating

        int Calculate_Thumb_Pos(float scalar)
        {
            float pct = ((float)value / (float)value_max);
            return (int)(pct * scalar);
        }

        int Calculate_Value_For_Thumb_Pos(Vec2i pos)
        {
            int tr = Get_Major_Dimension(Box.Content.Get_Dimensions());
            int th = Get_Major_Dimension(Thumb.Box.Get_Dimensions());
            int pv = Get_Major_Dimension(pos);

            float vpp = ((float)ValueMax / (float)(tr - th));
            return (int)(vpp * pv);
        }

        float Calculate_Thumb_Size_Ratio(float Value_Max, float Avail_Size)
        {
            float mn = Math.Min(Value_Max, Avail_Size);
            float mx = Math.Max(Value_Max, Avail_Size);
            float Ratio = Math.Min(1.0f, (mn / mx));
            return Ratio;
        }

        /// <summary>
        /// Updates the thumb's position and size
        /// </summary>
        protected bool Update_Thumb()
        {
            if (Viewport == null) return false;

            switch (Direction)
            {
                case ESliderDirection.Vertical:
                    {
                        float ratio = Calculate_Thumb_Size_Ratio(value_max, Box.Content.Height);
                        int size = (int)(ratio * (float)Box.Content.Height);
                        int pos = Calculate_Thumb_Pos(Box.Content.Height - Thumb.Box.Height);
                        
                        Thumb.Style.ImplicitRules.X.Set(0);
                        Thumb.Style.ImplicitRules.Y.Set(pos);
                        Thumb.Style.ImplicitRules.Height.Set(size);
                    }
                    break;
                case ESliderDirection.Horizontal:
                    {
                        float ratio = Calculate_Thumb_Size_Ratio(value_max, Box.Content.Width);
                        int size = (int)(ratio * (float)Box.Content.Width);
                        int pos = Calculate_Thumb_Pos(Box.Content.Width - Thumb.Box.Width);
                        
                        Thumb.Style.ImplicitRules.X.Set(pos);
                        Thumb.Style.ImplicitRules.Y.Set(0);
                        Thumb.Style.ImplicitRules.Width.Set(size);
                    }
                    break;
            }
            Dirty_Thumb = false;
            return true;
        }
        #endregion

    }
}
