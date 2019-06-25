using CssUI.CSS;

namespace CssUI
{
    /// <summary>
    /// A replaced element is one whose content has intrinsic dimensions and thus needs to be drawn and sized differently.
    /// <para>Intrinsic dimensions here means that the Width/Height is defined by the content itself and not what the element imposes, Eg: images</para>
    /// </summary>
    public abstract class cssReplacedElement : cssElement
    {
        static eSize Default_Object_Size = new eSize(255, 255);
        public readonly EReplacedElementType Kind = EReplacedElementType.NONE;

        #region Accessors
        protected bool Dirty_Texture = false;
        public cssTexture Texture { get; protected set; } = null;
        #endregion
        
        #region Intrinsic Properties
        //public SizeRatio IntrinsicRatio { get; private set; } = null;

        protected void Set_Intrinsic_Size(int? width, int? height)
        {
            //IntrinsicRatio = null;
            Style.ImplicitRules.Intrinsic_Ratio.Set((double?)null);
            Style.ImplicitRules.Intrinsic_Width.Set(CssValue.From_Int(width.Value, CssValue.Null));
            Style.ImplicitRules.Intrinsic_Height.Set(CssValue.From_Int(height.Value, CssValue.Null));
            if (width.HasValue && height.HasValue)
                Style.ImplicitRules.Intrinsic_Ratio.Set((double)height.Value / (double)width.Value);
                //IntrinsicRatio = new SizeRatio(width.Value, height.Value);
            else
                Style.ImplicitRules.Intrinsic_Ratio.Set((double?)null);
        }
        #endregion

        #region Constructors
        public cssReplacedElement(EReplacedElementType Kind, string ID = null) : base(ID)
        {
            this.Kind = Kind;
        }
        #endregion

        #region Drawing
        protected override void Draw()
        {
            Draw_Background();

            Root.Engine.Set_Color(Color);
            //Root.Engine.Set_Texture((Texture == null ? glTexture.Default : Texture));
            if (Texture != null)
            {
                Texture.Update();
                Root.Engine.Set_Texture(Texture);
            }
            Root.Engine.Fill_Rect(Block_ReplacedObjectArea);
            Root.Engine.Set_Texture(null);
        }

        internal override void Draw_Debug_Bounds()
        {
            base.Draw_Debug_Bounds();
            Root.Engine.Set_Color(1f, 0f, 1f, 1f);// Purple
            Root.Engine.Draw_Rect(1, Block_ReplacedObjectArea);
        }
        #endregion

        #region Texture Updating
        protected virtual void Update_Texture()
        {
            Dirty_Texture = false;
        }
        #endregion

        #region Blocks
        /// <summary>
        /// Area where our replaced object (image, text, etc) should be rendered. 
        /// This area specifies the absolute size and position which the object should render at.
        /// </summary>
        protected eBlock Block_ReplacedObjectArea = new eBlock();

        protected override void Update_Cached_Blocks()
        {
            base.Update_Cached_Blocks();

            // TODO: Clipping needs to be done in a way that the clipping area will also be translated properly when the parent element is scrollable
            //Block_Clipping = new eBlock(Block_Content);
            Update_Replaced_Block();
        }

        void Update_Replaced_Block()
        {
            eSize ObjectSize = Get_Replaced_Block_Size(Block_Content.Get_Size());
            this.Style.Resolve_Object_Position(Block_Content.Get_Size(), ObjectSize);
            Block_ReplacedObjectArea = new eBlock(new ePos(Block_Content.X + Style.ObjectPosition_X, Block_Content.Y + Style.ObjectPosition_Y), new eSize(ObjectSize.Width, ObjectSize.Height));

            //ObjectPosition.Value = ObjectPosition.Resolve_As_Background_Position(Block_Content.Get_Size(), ObjectSize);
            //Block_ReplacedObjectArea = new eBlock(new ePos(Block_Content.X + ObjectPosition.Value.X, Block_Content.Y + ObjectPosition.Value.Y), new eSize(ObjectSize.Width, ObjectSize.Height));
        }
        #endregion

        #region Sizing
        /// <summary>
        /// Uses the appropriate sizing rules to find the replaced block size
        /// </summary>
        /// <returns></returns>
        eSize Get_Replaced_Block_Size(eSize Size)
        {
            eSize size = new eSize();
            // https://www.w3.org/TR/css3-images/#default-sizing
            switch (Style.ObjectFit)
            {
                case EObjectFit.None:
                    return Apply_Default_Sizing_Algorithm(new StyleSize(CssValue.Null, CssValue.Null), Size);
                case EObjectFit.Fill:
                    return Size;// Apply_Default_Sizing_Algorithm(new StyleSize(StyleValue.Unset, StyleValue.Unset), );
                case EObjectFit.Contain:
                    return Apply_Contain_Constraint(Size);
                case EObjectFit.Cover:
                    return Apply_Cover_Constraint(Size);
                case EObjectFit.Scale_Down:
                    {
                        var sz = Apply_Default_Sizing_Algorithm(new StyleSize(CssValue.Null, CssValue.Null), Size);
                        if (sz.Width < Size.Width || sz.Height < Size.Height) return Size;
                        return sz;
                    }
            }
            
            return size;
        }

        eSize Apply_Default_Sizing_Algorithm(StyleSize Size_Specified, eSize Size_Default)
        {
            int rWidth;
            int rHeight;
            Style.Resolve_Size(this, Size_Specified.Width, Size_Specified.Height, out rWidth, out rHeight);
            eSize ResolvedSize = new eSize(rWidth, rHeight);

            if (!Size_Specified.Width.IsNullOrUnset() && !Size_Specified.Height.IsNullOrUnset())
            {// Both of the dimensions are specified, so this is the size we will use
                return ResolvedSize;
            }
            
            if (Size_Specified.Width.IsNullOrUnset() ^ Size_Specified.Height.IsNullOrUnset())
            {// Only one of the dimensions isn't specified, so we use our intrinsic aspect ratio
                if (Size_Specified.Width.IsNullOrUnset())
                {// Height was specified
                    int Width = 0;
                    if (Style.Intrinsic_Ratio.HasValue) Width = (int)(Style.Intrinsic_Ratio.Value * (double)ResolvedSize.Height);
                    else if (!Style.Intrinsic_Width.HasValue)
                    {
                        //Width = ElementStyle.Resolve_Size_Width(this, IntrinsicSize.Width.Computed);
                        int? ival = Style.Intrinsic_Width;// ElementStyle.Resolve_Intrinsic_Width(this, Style.Current.Intrinsic_Width.Computed);
                        Width = (ival.HasValue ? MathExt.Max(0, ival.Value) : 0);// Negative values are illegal
                    }
                    else Width = Size_Default.Width;

                    return new eSize(Width, ResolvedSize.Height);
                }
                else
                {// Width was specified
                    int Height = 0;
                    //if (Style.Intrinsic_Ratio.HasValue) Height = (int)(IntrinsicRatio.WH * (double)ResolvedSize.Width);
                    if (Style.Intrinsic_Ratio.HasValue) Height = (int)((double)ResolvedSize.Width / Style.Intrinsic_Ratio.Value);
                    else if (!Style.Intrinsic_Height.HasValue)
                    {
                        //Height = ElementStyle.Resolve_Size_Height(this, IntrinsicSize.Height.Computed);
                        int? ival = Style.Intrinsic_Height;// ElementStyle.Resolve_Intrinsic_Height(this, IntrinsicSize.Height.Computed);
                        Height = (ival.HasValue ? MathExt.Max(0, ival.Value) : 0);// Negative values are illegal
                    }
                    else Height = Size_Default.Height;

                    return new eSize(ResolvedSize.Width, Height);
                }
            }
            
            // Neither of the dimensions are specified
            if (!Style.Intrinsic_Width.HasValue || !Style.Intrinsic_Height.HasValue) return Apply_Default_Sizing_Algorithm(new StyleSize(Style.Specified.Intrinsic_Width.Computed, Style.Specified.Intrinsic_Height.Computed), Size_Default);

            return Apply_Contain_Constraint(Default_Object_Size);
        }

        eSize Apply_Contain_Constraint(eSize Size_Specified)
        {
            if (!Style.Intrinsic_Ratio.HasValue) return Size_Specified;
            int Width = Size_Specified.Width;
            int Height = Size_Specified.Height;
            
            if (Size_Specified.Width >= Size_Specified.Height)// Our constraint size's largest dimension is it's Width
            {
                //if (IntrinsicRatio.HW < IntrinsicRatio.WH)// Our largest instrinsic dimension is ALSO the Width
                if (Style.Intrinsic_Ratio.Value < 1.0)// Our largest instrinsic dimension is ALSO the Width
                {// So use it's Width and find the constrained Height
                    //Height = (int)(IntrinsicRatio.HW * (float)Size_Specified.Width);
                    Height = (int)(Style.Intrinsic_Ratio.Value * (double)Size_Specified.Width);
                }
                else// Our largest instrinsic dimension is the Height
                {// So use it's Height and find the constrained Width
                    //Width = (int)(IntrinsicRatio.WH * (float)Size_Specified.Height);
                    Width = (int)((double)Size_Specified.Height / Style.Intrinsic_Ratio.Value);
                }
            }
            else// Our constraint size's largest dimension is it's Height 
            {
                //if (IntrinsicRatio.HW > IntrinsicRatio.WH)// Our largest instrinsic dimension is ALSO the Height
                if (Style.Intrinsic_Ratio.Value > 1.0)
                {// So use it's Height and find the constrained Width
                    //Width = (int)(IntrinsicRatio.WH * (float)Size_Specified.Height);
                    Width = (int)((double)Size_Specified.Height / Style.Intrinsic_Ratio.Value);
                }
                else// Our largest instrinsic dimension is the Width
                {// So use it's Width and find the constrained Height
                    //Height = (int)(IntrinsicRatio.HW * (float)Size_Specified.Width);
                    Height = (int)(Style.Intrinsic_Ratio.Value * (double)Size_Specified.Width);
                }
            }

            return new eSize() { Width = Width, Height = Height };
        }

        eSize Apply_Cover_Constraint(eSize Size_Specified)
        {
            if (!Style.Intrinsic_Ratio.HasValue) return Size_Specified;
            int Width = Size_Specified.Width;
            int Height = Size_Specified.Height;

            if (Size_Specified.Width >= Size_Specified.Height)// Our constraint size's largest dimension is it's Width
            {
                //if (IntrinsicRatio.HW < IntrinsicRatio.WH)// Our largest instrinsic dimension is ALSO the Width
                if (Style.Intrinsic_Ratio.Value < 1.0)
                {// So use it's Height and find the constrained Width
                    //Width = (int)(IntrinsicRatio.WH * (float)Size_Specified.Height);
                    Width = (int)((double)Size_Specified.Height / Style.Intrinsic_Ratio.Value);
                }
                else// Our largest instrinsic dimension is the Height
                {// So use it's Width and find the constrained Height
                    //Height = (int)(IntrinsicRatio.HW * (float)Size_Specified.Width);
                    Height = (int)(Style.Intrinsic_Ratio.Value * Size_Specified.Width);
                }
            }
            else// Our constraint size's largest dimension is it's Height
            {
                //if (IntrinsicRatio.HW > IntrinsicRatio.WH)// Our largest instrinsic dimension is ALSO the Height
                if (Style.Intrinsic_Ratio.Value > 1.0)
                {// So use it's Width and find the constrained Height
                    //Height = (int)(IntrinsicRatio.HW * (float)Size_Specified.Width);
                    Height = (int)(Style.Intrinsic_Ratio.Value * Size_Specified.Width);
                }
                else// Our largest instrinsic dimension is the Width
                {// So use it's Height and find the constrained Width
                    //Width = (int)(IntrinsicRatio.WH * (float)Size_Specified.Height);
                    Width = (int)((double)Size_Specified.Height / Style.Intrinsic_Ratio.Value);
                }
            }

            return new eSize() { Width = Width, Height = Height };
        }
        #endregion

    }
}
