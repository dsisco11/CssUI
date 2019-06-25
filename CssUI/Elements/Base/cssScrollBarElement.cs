using System;
using CssUI.CSS;

namespace CssUI
{
    public abstract class cssScrollBarElement : cssCompoundElement
    {
        #region Events
        public event Action<cssScrollBarElement> ValueChanged;
        #endregion

        #region Components
        /// <summary>
        /// Slider that holds our trackbar and thumb
        /// </summary>
        protected uiTrackBar Track;
        /// <summary>
        /// Button that increments our value
        /// </summary>
        protected cssButtonElement Btn_inc;
        /// <summary>
        /// Button that decrements our value
        /// </summary>
        protected cssButtonElement Btn_dec;
        #endregion

        #region Variables
        ESliderDirection Direction = ESliderDirection.Unset;
        int value_step = 1;
        int oldValueStep = 1;
        bool Dirty_Thumb = false;

        /// <summary>
        /// Current scroll-value for this scrollbar
        /// </summary>
        public int Value
        {
            get { return Track.Value; }
            set { Track.Value = value; }
        }
        /// <summary>
        /// Maximum value for the scrollbar
        /// </summary>
        public int ValueMax
        {
            get { return Track.ValueMax; }
            set { Track.ValueMax = value; }
        }
        /// <summary>
        /// How many units the scroll value changes per increment/decrement
        /// </summary>
        public int StepSize
        {
            get { return value_step; }
            set
            {
                value_step = value;
                if (value_step != oldValueStep)
                {
                    oldValueStep = value_step;
                    Dirty_Thumb = true;
                }
            }
        }

        /// <summary>
        /// How many value steps the currently viewable area encompases
        /// </summary>
        public int PageLength { get; protected set; } = 0;
        #endregion

        #region Thumb Updating
        int Calculate_Thumb_Size(float Content_Size, float Viewport_Size, float Avail_Size)
        {
            Content_Size += Viewport_Size;
            float Ratio = Math.Min(1.0f, (Viewport_Size / Content_Size));
            return (int)(Ratio * Avail_Size);
        }

        /// <summary>
        /// Updates the thumb's position and size
        /// </summary>
        protected void Update_Thumb()
        {
            if (Viewport == null) return;

            switch (Direction)
            {
                case ESliderDirection.Vertical:
                    {
                        int size = Calculate_Thumb_Size(Track.ValueMax, Viewport.Block.Height, Track.Block.Height);
                        //Track.Thumb.Style.User.Width.Set(null);
                        Track.Thumb.Style.UserRules.Height.Set(size);
                        PageLength = (int)(Track.ValuePerPixel * size);
                    }
                    break;
                case ESliderDirection.Horizontal:
                    {
                        int size = Calculate_Thumb_Size(Track.ValueMax, Viewport.Block.Width, Track.Block.Width);
                        Track.Thumb.Style.UserRules.Width.Set(size);
                        //Track.Thumb.Style.User.Height.Set(null);
                        PageLength = (int)(Track.ValuePerPixel * size);
                    }
                    break;
            }
            Dirty_Thumb = false;
        }
        #endregion

        #region Constructors
        public cssScrollBarElement(string ID = null, ESliderDirection Dir = ESliderDirection.Unset) : base(ID)
        {
            Flags_Remove(EElementFlags.DoubleClickable);
            Layout = ELayoutMode.None;
            Style.ImplicitRules.Positioning.Value = EPositioning.Fixed;
            // Just so our scrollbars dont have scrollbars by some chance.
            Style.ImplicitRules.Overflow_X.Value = EOverflowMode.Clip;
            Style.ImplicitRules.Overflow_Y.Value = EOverflowMode.Clip;
            ColorBackground = new cssColor(0.1f, 0.1f, 0.1f, 1.0f);


            Track = new uiTrackBar(Dir, "Track")
            {
                ColorBackground = new cssColor(0f, 0f, 0f, 0.1f)
            };
            Track.ValueChanged += Slider_ValueChanged;
            Track.MouseClick += Track_onMouseClick;

            Btn_dec = new cssButtonElement("Btn_Dec");
            Btn_dec.Style.UserRules.Set_Padding_Implicit(3, 3);
            Btn_dec.Clicked += (cssElement Sender, DomRoutedEventArgs Args) => { Value = (Value - StepSize); };
            Btn_dec.Set_Svg(CssIcons.close);

            Btn_inc = new cssButtonElement("Btn_Inc");
            Btn_inc.Style.UserRules.Set_Padding_Implicit(3, 3);
            Btn_inc.Clicked += (cssElement Sender, DomRoutedEventArgs Args) => { Value = (Value + StepSize); };
            Btn_inc.Set_Svg(CssIcons.close);

            this.Direction = Dir;
            switch (this.Direction)
            {
                case ESliderDirection.Vertical:
                    {
                        var p = Platform.Factory.SystemMetrics.Get_Vertical_Scrollbar_Params();
                        Style.UserRules.Width.Set(p.Size);
                        Track.Style.UserRules.Width.Set(CssValue.Pct_OneHundred);
                        Track.Style.UserRules.Set_Padding(3, 0);

                        Btn_dec.Style.UserRules.Width.Set(p.BtnArrowSize);
                        Btn_dec.Style.UserRules.Height.Set(p.BtnArrowSize);
                        Btn_dec.Set_Svg(CssIcons.arrow_up);

                        Btn_inc.Style.UserRules.Width.Set(p.BtnArrowSize);
                        Btn_inc.Style.UserRules.Height.Set(p.BtnArrowSize);
                        Btn_inc.Set_Svg(CssIcons.arrow_down);
                        Track.Thumb.Style.UserRules.Min_Height.Set(p.ThumbSize);
                    }
                    break;
                case ESliderDirection.Horizontal:
                    {
                        var p = Platform.Factory.SystemMetrics.Get_Horizontal_Scrollbar_Params();
                        Style.UserRules.Height.Set(p.Size);
                        Track.Style.UserRules.Height.Set(CssValue.Pct_OneHundred);
                        Track.Style.UserRules.Set_Padding(0, 3);

                        Btn_dec.Style.UserRules.Width.Set(p.BtnArrowSize);
                        Btn_dec.Style.UserRules.Height.Set(p.BtnArrowSize);
                        Btn_dec.Set_Svg(CssIcons.arrow_left);

                        Btn_inc.Style.UserRules.Width.Set(p.BtnArrowSize);
                        Btn_inc.Style.UserRules.Height.Set(p.BtnArrowSize);
                        Btn_inc.Set_Svg(CssIcons.arrow_right);
                        Track.Thumb.Style.UserRules.Min_Width.Set(p.ThumbSize);
                    }
                    break;
            }
            
            Add(Track);
            Add(Btn_dec);
            Add(Btn_inc);
        }
        #endregion

        #region Drawing
        public override bool Update()
        {
            bool retVal = false;
            if (Dirty_Thumb)
            {
                retVal = true;
                Update_Thumb();
            }
            if (base.Update()) retVal = true;

            return retVal;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Scrolls a single page length upwards
        /// </summary>
        /// <returns>Success</returns>
        public bool PageUp()
        {
            if (Value <= 0) return false;
            Value -= PageLength;
            return true;
        }

        /// <summary>
        /// Scrolls a single page length downwards
        /// </summary>
        /// <returns>Success</returns>
        public bool PageDown()
        {
            if (Value >= ValueMax) return false;
            Value += PageLength;
            return true;
        }
        #endregion

        #region Events

        private void Track_onMouseClick(cssElement Sender, DomMouseButtonEventArgs Args)
        {// Whenever the track of the scrollbar is clicked we want to scroll a page-length toward the clicked location on the track
            int rcPos = Track.Get_Major_Dimension(Track.PointToLocal(new ePos(Args.Position)));
            int rtPos = Track.Get_Major_Dimension(Track.PointToLocal(Track.Thumb.Block.Get_Center_Pos()));
            int dir = MathExt.Clamp(rcPos - rtPos, -1, 1);

            if (dir < 0) PageUp();
            else if (dir > 0) PageDown();
        }

        private void Slider_ValueChanged(uiTrackBar obj)
        {
            ValueChanged?.Invoke(this);
        }
        #endregion

        #region Update
        protected override void Update_Cached_Blocks()
        {
            base.Update_Cached_Blocks();
            Dirty_Thumb = true;
            Update_Track();
            Update_Thumb();
        }

        void Update_Track()
        {
            switch (Direction)
            {
                case ESliderDirection.Vertical:
                    {
                        int h = Block_Content.Height - (Btn_dec.Block.Height + Btn_inc.Block.Height);
                        //Track.Size.Set(null, h);
                        Track.Style.UserRules.Height.Set(h);
                    }
                    break;
                case ESliderDirection.Horizontal:
                    {
                        int w = Block_Content.Width - (Btn_dec.Block.Width + Btn_inc.Block.Width);
                        //Track.Size.Set(w, null);
                        Track.Style.UserRules.Width.Set(w);
                    }
                    break;
            }
        }
        #endregion

        #region Layout
        protected override void Handle_Layout()
        {
            base.Handle_Layout();
            switch (Direction)
            {
                case ESliderDirection.Vertical:
                    {
                        Btn_dec.Style.UserRules.Set_Position(0, 0);
                        Track.Style.UserRules.Set_Position(0, Btn_dec.Block.Height);
                        Btn_inc.Style.UserRules.Set_Position(0, Track.Block.Bottom);
                    }
                    break;
                case ESliderDirection.Horizontal:
                    {
                        Btn_dec.Style.UserRules.Set_Position(0, 0);
                        Track.Style.UserRules.Set_Position(Btn_dec.Block.Width, 0);
                        Btn_inc.Style.UserRules.Set_Position(Track.Block.Right, 0);
                    }
                    break;
            }
            Update_Thumb();
        }
        #endregion
    }
}
