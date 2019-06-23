using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CssUI.CSS;

namespace CssUI
{
    public enum EElementStyleState
    {
        Normal,
        Hover,
        Focus,
    }
    /// <summary>
    /// Holds all of the resolved styling values for an element
    /// Manages cascading and accessing all of the different properties for each of an elements style states
    /// </summary>
    public class ElementPropertySystem
    {
        #region State Names
        public static AtomicString STATE_DEFAULT = new AtomicString("Implicit");
        public static AtomicString STATE_USER = new AtomicString("Default");
        public static AtomicString STATE_FOCUS = new AtomicString("Focus");
        public static AtomicString STATE_HOVER = new AtomicString("Hover");
        #endregion

        #region Properties
        /// <summary>
        /// Whether or not this style has changes and needs to update
        /// </summary>
        public bool NeedsResolving { get; private set; } = true;
        /// <summary>
        /// Flags this style as dirty, meaning it's property values need to be re-resolved
        /// </summary>
        public void Flag() { NeedsResolving = true; }

        private readonly uiElement Owner;
        #endregion

        #region Current Values
        /// <summary>
        /// DO NOT MODIFY THE PROPERTIES OF THIS INSTANCE, TREAT THEM AS READONLY!!!
        /// </summary>
        public readonly StyleRuleData Final;
        #endregion

        #region States
        /// <summary>
        /// Default values set/preferred by the element itself
        /// (DO NOT SET THESE PROPERTIES FROM CODE OUTSIDE THE UI ELEMENT CLASS THAT OWNS THIS <see cref="ElementPropertySystem"/> INSTANCE!)
        /// </summary>
        internal readonly StyleRuleData Default;
        /// <summary>
        /// Values set by code external to the element's class definition.
        /// EG: A user stylesheet, or whatever UI element instantiates and uses the element.
        /// </summary>
        public readonly StyleRuleData User;
        /// <summary>
        /// Values that take precedence when the mouse is overtop the element
        /// </summary>
        public readonly StyleRuleData Hover;
        /// <summary>
        /// Values that take precedence when the element is currently targeted by the keyboard or activated by the mouse
        /// </summary>
        public readonly StyleRuleData Focus;
        /// <summary>
        /// Values that take precedence under custom conditions defined by certain element types
        /// </summary>
        private readonly Dictionary<AtomicString, StyleRuleData> Custom = new Dictionary<AtomicString, StyleRuleData>(0);
        #endregion

        #region Values
        public EDisplayMode Display { get { return Final.Display.Computed.AsEnum<EDisplayMode>(); } }
        public EBoxSizingMode BoxSizing { get { return Final.BoxSizing.Computed.AsEnum<EBoxSizingMode>(); } }
        public EPositioning Positioning { get { return Final.Positioning.Computed.AsEnum<EPositioning>(); } }

        /// <summary>
        /// Returns the positioning 'scheme', which defines whether the element follows the normal flow logic.
        /// </summary>
        public EPositioningScheme PositioningScheme
        {
            get
            {
                switch (Positioning)
                {
                    case EPositioning.Absolute:
                    case EPositioning.Fixed:
                        return EPositioningScheme.Absolute;
                    default:
                        return EPositioningScheme.Normal;
                }
            }
        }

        public EOverflowMode Overflow_X { get { return Final.Overflow_X.Computed.AsEnum<EOverflowMode>(); } }
        public EOverflowMode Overflow_Y { get { return Final.Overflow_Y.Computed.AsEnum<EOverflowMode>(); } }
        public ETextAlign TextAlign { get { return Final.TextAlign.Computed.AsEnum<ETextAlign>(); } }

        public EObjectFit ObjectFit { get { return Final.ObjectFit.Computed.AsEnum<EObjectFit>(); } }
        public int ObjectPosition_X { get; private set; } = 0;
        public int ObjectPosition_Y { get; private set; } = 0;

        public int? Intrinsic_Width { get; private set; } = null;
        public int? Intrinsic_Height { get; private set; } = null;
        /// <summary>The intrinsic ratio of  Height/Width </summary>
        public double? Intrinsic_Ratio { get; private set; } = null;

        public eMatrix TransformMatrix { get; private set; } = null;

        /// <summary>
        /// X-Axis position of the owning element relative to it's container
        /// </summary>
        public int X { get; private set; } = 0;
        /// <summary>
        /// Y-Axis position of the owning element relative to it's container
        /// </summary>
        public int Y { get; private set; } = 0;

        public int Layout_Pos_X { get; private set; } = 0;
        public int Layout_Pos_Y { get; private set; } = 0;

        public int Top { get; private set; } = -1;
        public int Right { get; private set; } = -1;
        public int Bottom { get; private set; } = -1;
        public int Left { get; private set; } = -1;

        public int Width { get; private set; } = -1;
        public int Height { get; private set; } = -1;

        public int? Content_Width { get; private set; } = null;
        public int? Content_Height { get; private set; } = null;

        public int Min_Width { get; private set; } = -1;
        public int Min_Height { get; private set; } = -1;

        public int? Max_Width { get; private set; } = null;
        public int? Max_Height { get; private set; } = null;


        //public eBlockOffset Margin { get; private set; } = new eBlockOffset(0, 0, 0, 0);
        public int Margin_Vertical { get { return (Margin_Top + Margin_Bottom); } }
        public int Margin_Horizontal { get { return (Margin_Left + Margin_Right); } }
        public int Margin_Top { get; private set; } = 0;
        public int Margin_Right { get; private set; } = 0;
        public int Margin_Bottom { get; private set; } = 0;
        public int Margin_Left { get; private set; } = 0;


        //public eBlockOffset Padding { get; private set; } = new eBlockOffset(0, 0, 0, 0);
        public int Padding_Vertical { get { return (Padding_Top + Padding_Bottom); } }
        public int Padding_Horizontal { get { return (Padding_Left + Padding_Right); } }
        public int Padding_Top { get; private set; } = 0;
        public int Padding_Right { get; private set; } = 0;
        public int Padding_Bottom { get; private set; } = 0;
        public int Padding_Left { get; private set; } = 0;

        public EBorderStyle Border_Top_Style { get { return Final.Border_Top_Style.Computed.AsEnum<EBorderStyle>(); } }
        public EBorderStyle Border_Right_Style { get { return Final.Border_Right_Style.Computed.AsEnum<EBorderStyle>(); } }
        public EBorderStyle Border_Bottom_Style { get { return Final.Border_Bottom_Style.Computed.AsEnum<EBorderStyle>(); } }
        public EBorderStyle Border_Left_Style { get { return Final.Border_Left_Style.Computed.AsEnum<EBorderStyle>(); } }

        public int Border_Top_Width { get; private set; } = 0;
        public int Border_Right_Width { get; private set; } = 0;
        public int Border_Bottom_Width { get; private set; } = 0;
        public int Border_Left_Width { get; private set; } = 0;

        public int FontWeight { get; private set; }
        public EFontStyle FontStyle { get; private set; }
        public double FontSize { get; private set; }
        public string FontFamily { get; private set; }

        public int LineHeight { get; private set; }
        public double Opacity { get; private set; } = 1.0f;
        public uiColor Blend_Color { get; private set; } = null;
        #endregion

        #region Getters
        /// <summary>
        /// Returns whether the 'positioning' property is set to Fixed OR Absolute
        /// </summary>
        public bool IsAbsolutelyPositioned { get { return (Positioning == EPositioning.Absolute || Positioning == EPositioning.Fixed); } }

        public ePos Get_Offset()
        {
            return new ePos(X, Y);
        }

        public eBlockOffset Get_Margin_NoAuto(int autoValue)
        {
            eBlockOffset retVal = new eBlockOffset();

            if (Final.Margin_Top.Computed == CSSValue.Auto) retVal.Top = autoValue;
            if (Final.Margin_Right.Computed == CSSValue.Auto) retVal.Right = autoValue;
            if (Final.Margin_Bottom.Computed == CSSValue.Auto) retVal.Bottom = autoValue;
            if (Final.Margin_Left.Computed == CSSValue.Auto) retVal.Left = autoValue;

            return retVal;
        }

        public eBlockOffset Get_Padding_OnlyAbsolute(int autoValue)
        {
            eBlockOffset retVal = new eBlockOffset();
            retVal.Top = Final.Padding_Top.Computed.Resolve_Or_Default(autoValue, (o => o.Has_Flags(StyleValueFlags.Absolute)));
            retVal.Right = Final.Padding_Right.Computed.Resolve_Or_Default(autoValue, (o => o.Has_Flags(StyleValueFlags.Absolute)));
            retVal.Bottom = Final.Padding_Bottom.Computed.Resolve_Or_Default(autoValue, (o => o.Has_Flags(StyleValueFlags.Absolute)));
            retVal.Left = Final.Padding_Left.Computed.Resolve_Or_Default(autoValue, (o => o.Has_Flags(StyleValueFlags.Absolute)));

            return retVal;
        }

        public eBlockOffset Get_Margin_OnlyAbsolute(int autoValue)
        {
            eBlockOffset retVal = new eBlockOffset();
            retVal.Top = Final.Margin_Top.Computed.Resolve_Or_Default(autoValue, (o => o.Has_Flags(StyleValueFlags.Absolute)));
            retVal.Right = Final.Margin_Right.Computed.Resolve_Or_Default(autoValue, (o => o.Has_Flags(StyleValueFlags.Absolute)));
            retVal.Bottom = Final.Margin_Bottom.Computed.Resolve_Or_Default(autoValue, (o => o.Has_Flags(StyleValueFlags.Absolute)));
            retVal.Left = Final.Margin_Left.Computed.Resolve_Or_Default(autoValue, (o => o.Has_Flags(StyleValueFlags.Absolute)));

            return retVal;
        }
        #endregion

        #region Setters
        internal void Set_Layout_Pos(int X, int Y)
        {
            Layout_Pos_X = X;
            Layout_Pos_Y = Y;
            StackTrace Stack = null;
#if DEBUG
            Stack = new StackTrace(1);
#endif
            // We need to be resolved again because the 'Layout_Pos_' values directly determine our finalized X/Y property values.
            NeedsResolving = true;
            // These 'Layout_Pos_' vars have the same effect as any other styling property in that WHENEVER they change it will effect how the owning uiElement's BLOCK placement.
            //Property_Changed?.Invoke(null, EPropertyFlags.Block | EPropertyFlags.Flow, Stack);// Replaced with the lines below on 06-19-2017
            Owner.Flag_Block_Dirty(EUIInvalidationReason.Layout_Pos_Changed);
            // TODO: SHOULD we be invalidating Flow(layout) whenever an element's layout pos changes?
            Owner.Invalidate_Layout(EUIInvalidationReason.Layout_Pos_Changed);

        }

        internal void Set_Content_Width(int Width)
        {
            switch (BoxSizing)
            {
                case EBoxSizingMode.BORDER:
                    int Height = 0;
                    Owner.Block_Resize_Content_To_Border(ref Width, ref Height);
                    break;
                case EBoxSizingMode.CONTENT:
                    break;
            }

            User.Content_Width.Set(Width);
        }
        
        internal void Set_Content_Height(int Height)
        {
            switch (BoxSizing)
            {
                case EBoxSizingMode.BORDER:
                    int Width = 0;
                    Owner.Block_Resize_Content_To_Border(ref Width, ref Height);
                    break;
                case EBoxSizingMode.CONTENT:
                    break;
            }

            User.Content_Height.Set(Height);
        }
        #endregion

        #region Events
        /// <summary>
        /// A property which affects the elements block changed
        /// </summary>
        public event Action<IStyleProperty, EPropertyFlags, StackTrace> Property_Changed;
        #endregion

        #region Constructors
        public ElementPropertySystem(uiElement Owner)
        {
            this.Owner = Owner;
            Default = NewProperties("Default", "", Owner, false);// Holds the values assigned to properties by default for the owning element...
            User = NewProperties("User", "", Owner, true);
            Hover = NewProperties("Hover", ":hover", Owner, true);
            Focus = NewProperties("Focus", ":focus", Owner, true);

            Final = new StyleRuleData(null, null, Owner, true);
            Final.Property_Changed += Current_Property_Changed;
            // Blending
            Final.Opacity.onChanged += Update_Blend_Color;
            // Transformations
            Final.Transform.onChanged += Transform_onChanged;
        }
        #endregion

        private StyleRuleData NewProperties(string Name, string Selector, uiElement Owner, bool Unset)
        {
            var retVal = new StyleRuleData(Name, new CssSelector(false, Selector), Owner, false, Unset);
            // Capture all update events.
            retVal.Property_Changed += Property_Change;
            return retVal;
        }


        #region Property Change Handlers

        /// <summary>
        /// A state-specific property changed
        /// </summary>
        private void Property_Change(IStyleProperty Property, EPropertyFlags Flags, StackTrace Origin)
        {
            AtomicString SourceState = STATE_DEFAULT;
            IStyleProperty Value = Default.Get(Property.FieldName);
            // Override implicitly set values with default ones when set
            IStyleProperty dv = User.Get(Property.FieldName);
            if (dv.HasValue)
            {
                Value = dv;
                SourceState = STATE_USER;
            }

            if (ActiveStates.Contains(STATE_FOCUS))
            {
                SourceState = STATE_FOCUS;
                var v = Focus.Get(Property.FieldName);
                if (v.HasValue) Value = v;
            }

            if (ActiveStates.Contains(STATE_HOVER))
            {
                SourceState = STATE_HOVER;
                var v = Hover.Get(Property.FieldName);
                if (v.HasValue) Value = v;
            }

            foreach (KeyValuePair<AtomicString, StyleRuleData> kv in Custom)
            {
                if (ActiveStates.Contains(kv.Key))
                {
                    SourceState = kv.Key;
                    var v = kv.Value.Get(Property.FieldName);
                    if (v.HasValue) Value = v;
                }
            }

            Final.Set(Property, Value, SourceState);
        }

        private void Current_Property_Changed(IStyleProperty Prop, EPropertyFlags Flags, StackTrace Stack)
        {
            Update_Depends_Flag();// A property changed, update our depends flag

            bool IsFlow = ((Flags & EPropertyFlags.Flow) != 0);// Layout
            bool IsBlock = ((Flags & EPropertyFlags.Block) != 0);
            bool IsVisual = ((Flags & EPropertyFlags.Visual) != 0);
            bool IsFont = ((Flags & EPropertyFlags.Font) != 0);

            if (IsBlock || IsFlow || IsVisual) NeedsResolving = true;

            if (IsFont) Update_Font(Prop, Stack);
            
            //Logging.Log.Info("[PropertyChanged]: {0}", Prop.FieldName);
            Property_Changed?.Invoke(Prop, Flags, Stack);
        }
        #endregion

        #region Compiling
        /// <summary>
        /// Re-Cascades all active style states with the default one to produce a set of 'Final' style properties to be used.
        /// </summary>
        private void Recompile()
        {
            StyleRuleData Compiled = new StyleRuleData(null, null, Owner);
            Compiled.Cascade(Default);
            Compiled.Cascade(User);

            if (ActiveStates.Contains(STATE_FOCUS))
                Compiled.Cascade(Focus);
            if (ActiveStates.Contains(STATE_HOVER))
                Compiled.Cascade(Hover);

            foreach (KeyValuePair<AtomicString, StyleRuleData> kv in Custom)
            {
                if (ActiveStates.Contains(kv.Key))
                {
                    Compiled.Cascade(kv.Value);
                }
            }

            // Go through each property within "Current" and if it doesnt equal the compiled property OR it's source doesnt match then update it...
            Final.Overwrite(Compiled);
        }
        #endregion

        #region Custom States
        public StyleRuleData this[AtomicString State]
        {
            get
            {
                StyleRuleData prop;
                if (!Custom.TryGetValue(State, out prop))
                {
                    throw new Exception(string.Format("The styling state \"{0}\" is invalid!", State));
                }
                return prop;
            }
        }
        /// <summary>
        /// Allows elements to register custom styling states
        /// </summary>
        /// <param name="State"></param>
        internal void RegisterState(AtomicString State)
        {
            if (!Custom.ContainsKey(State))
            {
                Custom.Add(State, NewProperties(State.ToString(), "", Owner, true));
            }
        }
        #endregion

        #region State Setting
        private HashSet<AtomicString> ActiveStates = new HashSet<AtomicString>();

        public void Set_State(AtomicString StateName, bool Status)
        {
            bool changes = false;
            if (Status && !ActiveStates.Contains(StateName))
            {
                ActiveStates.Add(StateName);
                changes = NeedsResolving = true;
            }
            else if (!Status && ActiveStates.Contains(StateName))
            {
                ActiveStates.Remove(StateName);
                changes = NeedsResolving = true;
            }

            if (changes)
                Recompile();
        }
        #endregion
        
        #region Unit Change Handler
        /// <summary>
        /// Notifys all dimension-properties which use the specified unit that its scale has changed and they need to update
        /// </summary>
        /// <param name="Unit"></param>
        public void Notify_Unit_Scale_Change(EStyleUnit Unit)
        {
            if (Unit == EStyleUnit.None) return;
            foreach(IStyleProperty Property in this.Final.Get_Set_Properties())
            {
                Property.Notify_Unit_Change(Unit);
            }
        }
        #endregion

        #region Property Inheritance
        /// <summary>
        /// One of our parents properties changed and they are letting us know so we can update any of our inherited values...
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="Target"></param>
        internal void Update_Inherited(uiElement Sender, IStyleProperty Target)
        {
            IStyleProperty prop = Final.Get(Target);
            if (prop.IsInherited)
            {
            }
            else
            {// Unregister the property from the sender, it is no longer an inherited one
            }
        }
        #endregion
        
        #region IsDependent_On_ContainingBlock
        /// <summary>
        /// Returns whether this style CURRENTLY has properties which depend on it's containing block
        /// </summary>
        public bool Depends_On_ContainingBlock { get; private set; } = false;
        void Update_Depends_Flag()
        {
            Depends_On_ContainingBlock = false;
            // Width / Height
            if (Final.Width.IsPercentageOrAuto || Final.Height.IsPercentageOrAuto) Depends_On_ContainingBlock = true;
            // Size-Max
            else if (Final.Max_Width.IsPercentageOrAuto || Final.Max_Height.IsPercentageOrAuto) Depends_On_ContainingBlock = true;
            // Size-Min
            else if (Final.Min_Width.IsPercentageOrAuto || Final.Min_Height.IsPercentageOrAuto) Depends_On_ContainingBlock = true;
            // Margin
            else if (Final.Margin_Top.IsPercentageOrAuto) Depends_On_ContainingBlock = true;
            else if (Final.Margin_Right.IsPercentageOrAuto) Depends_On_ContainingBlock = true;
            else if (Final.Margin_Bottom.IsPercentageOrAuto) Depends_On_ContainingBlock = true;
            else if (Final.Margin_Left.IsPercentageOrAuto) Depends_On_ContainingBlock = true;
            // Padding
            else if (Final.Padding_Top.IsPercentageOrAuto) Depends_On_ContainingBlock = true;
            else if (Final.Padding_Right.IsPercentageOrAuto) Depends_On_ContainingBlock = true;
            else if (Final.Padding_Bottom.IsPercentageOrAuto) Depends_On_ContainingBlock = true;
            else if (Final.Padding_Left.IsPercentageOrAuto) Depends_On_ContainingBlock = true;
            // Border
            else if (Final.Border_Top_Width.IsPercentageOrAuto) Depends_On_ContainingBlock = true;
            else if (Final.Border_Right_Width.IsPercentageOrAuto) Depends_On_ContainingBlock = true;
            else if (Final.Border_Bottom_Width.IsPercentageOrAuto) Depends_On_ContainingBlock = true;
            else if (Final.Border_Left_Width.IsPercentageOrAuto) Depends_On_ContainingBlock = true;
            // Positioning
            else if (Final.Top.IsPercentageOrAuto) Depends_On_ContainingBlock = true;
            else if (Final.Right.IsPercentageOrAuto) Depends_On_ContainingBlock = true;
            else if (Final.Bottom.IsPercentageOrAuto) Depends_On_ContainingBlock = true;
            else if (Final.Left.IsPercentageOrAuto) Depends_On_ContainingBlock = true;
        }
        #endregion


        public void Resolve(uiElement E = null)
        {
            if (E == null)
            {
                E = this.Owner;
                if (E == null) throw new Exception("Cannot resolve style values without an owning element specified!");
            }

            if (!NeedsResolving) return;

            this.Intrinsic_Ratio = Final.Intrinsic_Ratio.Specified.Resolve();
            this.Intrinsic_Width = Resolve_Intrinsic_Width(E, Final.Intrinsic_Width.Specified);
            this.Intrinsic_Height = Resolve_Intrinsic_Height(E, Final.Intrinsic_Height.Specified);

            // Content size values are intended to ALWAYS be given in absolute values
            this.Content_Width = (int?)Final.Content_Width.Specified.Resolve();
            this.Content_Height = (int?)Final.Content_Height.Specified.Resolve();
            Resolve_Line_Height();

            this.Min_Width = (int)Final.Min_Width.Computed.Resolve_Or_Default(0);
            this.Min_Height = (int)Final.Min_Height.Computed.Resolve_Or_Default(0);
            
            this.Max_Width = (int?)Final.Max_Width.Computed.Resolve();
            this.Max_Height = (int?)Final.Max_Height.Computed.Resolve();

            this.Border_Top_Width = (int)Final.Border_Top_Width.Computed.Resolve_Or_Default(0);
            this.Border_Right_Width = (int)Final.Border_Right_Width.Computed.Resolve_Or_Default(0);
            this.Border_Bottom_Width = (int)Final.Border_Bottom_Width.Computed.Resolve_Or_Default(0);
            this.Border_Left_Width = (int)Final.Border_Left_Width.Computed.Resolve_Or_Default(0);

            if ((this.Border_Top_Style & (EBorderStyle.None | EBorderStyle.Hidden)) != 0) this.Border_Top_Width = 0;
            if ((this.Border_Right_Style & (EBorderStyle.None | EBorderStyle.Hidden)) != 0) this.Border_Right_Width = 0;
            if ((this.Border_Bottom_Style & (EBorderStyle.None | EBorderStyle.Hidden)) != 0) this.Border_Bottom_Width = 0;
            if ((this.Border_Left_Style & (EBorderStyle.None | EBorderStyle.Hidden)) != 0) this.Border_Left_Width = 0;

            // Get the tentative values that define our elements blocks
            BlockProperties Block = new BlockProperties(this, Final);
            Get_Tentative_Block(E, Block);

            // Update our used values with the results
            this.Margin_Top = (int)Block.Margin_Top.Resolve_Or_Default(0);
            this.Margin_Right = (int)Block.Margin_Right.Resolve_Or_Default(0);
            this.Margin_Bottom = (int)Block.Margin_Bottom.Resolve_Or_Default(0);
            this.Margin_Left = (int)Block.Margin_Left.Resolve_Or_Default(0);

            this.Padding_Top = (int)Block.Padding_Top.Resolve_Or_Default(0);
            this.Padding_Right = (int)Block.Padding_Right.Resolve_Or_Default(0);
            this.Padding_Bottom = (int)Block.Padding_Bottom.Resolve_Or_Default(0);
            this.Padding_Left = (int)Block.Padding_Left.Resolve_Or_Default(0);


            int Top, Right, Bottom, Left;
            Resolve_Offsets(E, Block.Top, Block.Right, Block.Bottom, Block.Left, out Top, out Right, out Bottom, out Left);
            this.Top = Top;
            this.Right = Right;
            this.Bottom = Bottom;
            this.Left = Left;

            // Resolve the final position X/Y values
            int X, Y;
            Resolve_XY(E, Top, Right, Bottom, Left, out X, out Y);
            this.X = X;
            this.Y = Y;
            
            // Apply constraints to our size
            int tentative_Width = (int)Block.Width.Resolve_Or_Default(0);
            int tentative_Height = (int)Block.Height.Resolve_Or_Default(0);
            int Width, Height;
            Constrain_Size(tentative_Width, tentative_Height, Min_Width, Min_Height, Max_Width, Max_Height, out Width, out Height);
            this.Width = Width;
            this.Height = Height;

            NeedsResolving = false;
        }

        void Resolve_Transform_Matrix()
        {// SEE:  https://www.w3.org/TR/css-transforms-1/#typedef-transform-function
            TransformMatrix = new eMatrix();

            var Transforms = Final.Transform.Get_All();
            foreach (StyleFunction Func in Transforms)
            {
                switch (Func.Name.ToString().ToLower())
                {
                    case "scale":
                        {

                        }
                        break;
                }
            }
        }

        #region Font Updating
        void Update_Font(IStyleProperty Sender, StackTrace Stack)
        {
            // Resolve 'FontStyle'
            if (Final.FontStyle.Computed.Type == EStyleDataType.INTEGER)
            {
                FontStyle = (EFontStyle)Final.FontStyle.Computed.Value;
            }
            // Resolve 'FontFamily'
            if (Final.FontFamily.Computed.Type == EStyleDataType.STRING)
            {
                FontFamily = (string)Final.FontFamily.Computed.Value;
            }
            // Resolve 'FontWeight'
            FontWeight = (int)Final.FontWeight.Computed.Resolve_Or_Default(400);
            // Resolve 'FontSize'
            /*
            double? fsz = Current.FontSize.Computed?.Resolve(Owner.Style.FontSize);
            if (fsz.HasValue) FontSize = fsz.Value;
            */
            double oldFontSize = FontSize;
            FontSize = Final.FontSize.Computed.Resolve_Or_Default(Owner.Style.FontSize, 0.0);
            if (FontSize != oldFontSize)
            {// We ACTUALLY want to be doing these checks by calling UnitResolver(Unit) to get the old value and then calling it again after updating the font instance and checking the first and second returned values
                Notify_Unit_Scale_Change(EStyleUnit.EM);
                Notify_Unit_Scale_Change(EStyleUnit.EX);
                Notify_Unit_Scale_Change(EStyleUnit.CH);
            }

            // Resolve 'LineHeight'
            Resolve_Line_Height();
        }
        #endregion

        #region Update_Blend_Color
        void Update_Blend_Color(IStyleProperty Sender)
        {
            Opacity = Final.Opacity.Computed.Resolve_Or_Default(1.0);

            if (Opacity != 1.0)
            {// We have a useful blend color to set
                Blend_Color = new uiColor(1, 1, 1, Opacity);
            }
            else// We DONT have a useful blend color
            {
                Blend_Color = null;
            }
        }
        #endregion

        #region Transform_Changed

        private void Transform_onChanged(IStyleProperty o)
        {
            Resolve_Transform_Matrix();
        }
        #endregion

        #region Resolving
        void Resolve_Line_Height()
        {
            int? lh = (int?)Final.LineHeight.Computed?.Resolve((float)FontSize);
            if (lh.HasValue) LineHeight = lh.Value;
        }

        public void Resolve_Border_Size(uiElement E, CSSValue Top, CSSValue Right, CSSValue Bottom, CSSValue Left, out int outTop, out int outRight, out int outBottom, out int outLeft)
        {
            outTop = (Final.Border_Top_Style.Computed == CSSValue.None ? 0 : (int)Top.Resolve_Or_Default(0));
            outRight = (Final.Border_Right_Style.Computed == CSSValue.None ? 0 : (int)Right.Resolve_Or_Default(0));
            outBottom = (Final.Border_Bottom_Style.Computed == CSSValue.None ? 0 : (int)Bottom.Resolve_Or_Default(0));
            outLeft = (Final.Border_Left_Style.Computed == CSSValue.None ? 0 : (int)Left.Resolve_Or_Default(0));
        }

        public void Resolve_Object_Position(eSize ObjectArea, eSize ObjectSize)
        {
            int X;
            int Y;
            Resolve_As_Position(Final.ObjectPosition_X.Computed, Final.ObjectPosition_Y.Computed, ObjectArea, ObjectSize, out X, out Y);

            ObjectPosition_X = X;
            ObjectPosition_Y = Y;
        }

        /// <summary>
        /// Resolves the given Margin style values to absolute values for a specified element
        /// </summary>
        /// <param name="E"></param>
        /// <param name="Top"></param>
        /// <param name="Right"></param>
        /// <param name="Bottom"></param>
        /// <param name="Left"></param>
        /// <param name="Margin_Top"></param>
        /// <param name="Margin_Right"></param>
        /// <param name="Margin_Bottom"></param>
        /// <param name="Margin_Left"></param>
        public void Resolve_Margin(uiElement E, CSSValue Top, CSSValue Right, CSSValue Bottom, CSSValue Left, out int Margin_Top, out int Margin_Right, out int Margin_Bottom, out int Margin_Left)
        {
            int? left = 0, top = 0, right = 0, bottom = 0;
            
            left = (int?)Left.Resolve();
            top = (int?)Top.Resolve();
            right = (int?)Right.Resolve();
            bottom = (int?)Bottom.Resolve();

            // Resolve the final values for anything specified 'auto'
            switch (E.Style.Display)
            {
                case EDisplayMode.INLINE:
                case EDisplayMode.INLINE_BLOCK:
                    {
                        if (Top == CSSValue.Auto) top = 0;
                        if (Right == CSSValue.Auto) right = 0;
                        if (Bottom == CSSValue.Auto) bottom = 0;
                        if (Left == CSSValue.Auto) left = 0;
                    }
                    break;
                default:
                    {
                        if (E.Style.Final.Width.Computed != CSSValue.Auto)// Margin left/right auto values are 0 if the elements Width is set to auto
                        {
                            if (Left == CSSValue.Auto || Right == CSSValue.Auto)
                            {
                                /*
                                    The following constraints must hold among the used values of the properties.
                                    ‘margin-left’ + ‘border-left-width’ + ‘padding-left’ + ‘width’ + ‘padding-right’ + ‘border-right-width’ + ‘margin-right’ + scrollbar width (if any) = width of containing block
                                */

                                int W = E.Style.Width, H = 0;
                                // Add everything to this content-block size EXCEPT our margins, because our margins are what we are calculating
                                // IF our size specifies the dimensions of our content-area block then we need to expand that theoretical content-area block
                                // to a border-edge block so we can tell how much free space our margins can auto-consume
                                if (E.Style.BoxSizing == EBoxSizingMode.CONTENT)
                                    E.Block_Resize_Content_To_Border(ref W, ref H);

                                int freeSpace = Math.Max(0, E.Block_Containing.Width - W);

                                if (Left == CSSValue.Auto && Right == CSSValue.Auto)
                                    left = right = (freeSpace / 2);
                                else if (Left == CSSValue.Auto)
                                    left = freeSpace;
                                else if (Right == CSSValue.Auto)
                                    right = freeSpace;
                            }
                        }
                    }
                    break;
            }

            Margin_Left = (left.HasValue ? left.Value : 0);
            Margin_Top = (top.HasValue ? top.Value : 0);
            Margin_Right = (right.HasValue ? right.Value : 0);
            Margin_Bottom = (bottom.HasValue ? bottom.Value : 0);
        }

        /// <summary>
        /// Resolves the given Padding style values to absolute values for a specified element
        /// </summary>
        /// <param name="E"></param>
        /// <param name="Top"></param>
        /// <param name="Right"></param>
        /// <param name="Bottom"></param>
        /// <param name="Left"></param>
        /// <param name="Padding_Top"></param>
        /// <param name="Padding_Right"></param>
        /// <param name="Padding_Bottom"></param>
        /// <param name="Padding_Left"></param>
        public void Resolve_Padding(uiElement E, CSSValue Top, CSSValue Right, CSSValue Bottom, CSSValue Left, out int Padding_Top, out int Padding_Right, out int Padding_Bottom, out int Padding_Left)
        {
            /*
            int? left = 0, top = 0, right = 0, bottom = 0;

            left = (int)Left.Resolve_Or_Default(0);// (E.Block_Containing.Width);
            top = (int)Top.Resolve_Or_Default(0);//Resolve(E.Block_Containing.Width);
            right = (int)Right.Resolve_Or_Default(0);//Resolve(E.Block_Containing.Width);
            bottom = (int)Bottom.Resolve_Or_Default(0);//Resolve(E.Block_Containing.Width);

            if (Left == StyleValue.Auto) left = null;
            if (Top == StyleValue.Auto) top = null;
            if (Right == StyleValue.Auto) right = null;
            if (Bottom == StyleValue.Auto) bottom = null;

            Padding_Top = (top.HasValue ? top.Value : 0);
            Padding_Right = (right.HasValue ? right.Value : 0);
            Padding_Bottom = (bottom.HasValue ? bottom.Value : 0);
            Padding_Left = (left.HasValue ? left.Value : 0);
            */

            Padding_Left = (int)Left.Resolve_Or_Default(0);
            Padding_Top = (int)Top.Resolve_Or_Default(0);
            Padding_Right = (int)Right.Resolve_Or_Default(0);
            Padding_Bottom = (int)Bottom.Resolve_Or_Default(0);
        }

        /// <summary>
        /// Resolves the given Min-Width and Min-Height style values to absolute values for a specified element
        /// </summary>
        /// <param name="E"></param>
        /// <param name="minWidth"></param>
        /// <param name="minHeight"></param>
        /// <param name="Min_Width"></param>
        /// <param name="Min_Height"></param>
        public void Resolve_MinSize(uiElement E, CSSValue minWidth, CSSValue minHeight, out int Min_Width, out int Min_Height)
        {
            int? width = 0, height = 0;

            height = (int?)minHeight.Resolve(E.Block_Containing.Height);
            width = (int?)minWidth.Resolve(E.Block_Containing.Width);

            // min-width & min-height can ONLY have non-negative absolute values, 'auto' and 'none' are invalid!
            int W = 0, H = 0;

            if (width.HasValue) W = Math.Max(0, width.Value);
            if (height.HasValue) H = Math.Max(0, height.Value);

            Min_Width = W;
            Min_Height = H;
        }

        /// <summary>
        /// Resolves the given Min-Width style value to an absolute value for the specified element
        /// </summary>
        /// <param name="E"></param>
        /// <param name="minWidth"></param>
        public int Resolve_MinSize_Width(uiElement E, CSSValue minWidth)
        {
            int? width = 0;
            width = (int?)minWidth.Resolve(E.Block_Containing.Width);
            // min-width & min-height can ONLY have non-negative absolute values, 'auto' and 'none' are invalid!
            return (width.HasValue ? Math.Max(0, width.Value) : 0);
        }

        /// <summary>
        /// Resolves the given Min-Height style value to an absolute value for the specified element
        /// </summary>
        /// <param name="E"></param>
        /// <param name="minHeight"></param>
        public int Resolve_MinSize_Height(uiElement E, CSSValue minHeight)
        {
            int? height = 0;

            height = (int?)minHeight.Resolve(E.Block_Containing.Height);
            // min-width & min-height can ONLY have non-negative absolute values, 'auto' and 'none' are invalid!
            return (height.HasValue ? Math.Max(0, height.Value) : 0);
        }

        /// <summary>
        /// Resolves the given Max-Width and Max-Height style values to absolute values for a specified element
        /// </summary>
        /// <param name="E"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <param name="Max_Width"></param>
        /// <param name="Max_Height"></param>
        public void Resolve_MaxSize(uiElement E, CSSValue maxWidth, CSSValue maxHeight, out int? Max_Width, out int? Max_Height)
        {
            int? width = 0, height = 0;

            width = (int?)maxWidth.Resolve(E.Block_Containing.Width);
            height = (int?)maxHeight.Resolve(E.Block_Containing.Height);

            if (width.HasValue) width = Math.Max(0, width.Value);
            else if (maxWidth == CSSValue.None) width = null;
            else width = 0;

            if (height.HasValue) height = Math.Max(0, height.Value);
            else if (maxHeight == CSSValue.None) height = null;
            else height = 0;

            // Resolve AUTO
            // (This is NOT defined by the W3C standards, which specify no logic for resolving AUTO values for Size-Max so this logic is only relevant for the purposes of THIS ui system)
            // For AUTO values on Max-Width/Height we:
            // Act like our computed value is 100% ONLY IF our parent's size isn't affected by us.
            if (E.Parent != null)
            {
                if (maxWidth == CSSValue.Auto)
                {
                    if (!E.Parent.Style.Final.Width.Assigned.IsNullOrUnset())
                        width = E.Block_Containing.Width;
                }

                if (maxHeight == CSSValue.Auto)
                {
                    if (!E.Parent.Style.Final.Height.Assigned.IsNullOrUnset())
                        height = E.Block_Containing.Height;
                }
            }

            Max_Width = width;
            Max_Height = height;
        }

        /// <summary>
        /// Resolves the given Max-Width style value to an absolute value for the specified element
        /// </summary>
        /// <param name="E"></param>
        /// <param name="maxWidth"></param>
        public int? Resolve_MaxSize_Width(uiElement E, CSSValue maxWidth)
        {
            int? width = 0;

            width = (int?)maxWidth.Resolve(E.Block_Containing.Width);

            if (width.HasValue) width = Math.Max(0, width.Value);
            else if (maxWidth == CSSValue.None) width = null;
            else width = 0;

            // Resolve AUTO
            // (This is NOT defined by the W3C standards, which specify no logic for resolving AUTO values for Size-Max so this logic is only relevant for the purposes of THIS ui system)
            // For AUTO values on Max-Width/Height we
            // Act like our computed value is 100% ONLY IF our parent has an explicit value set!
            if (E.Parent != null)
            {
                if (maxWidth == CSSValue.Auto)
                {
                    if (!E.Parent.Style.Final.Width.Assigned.IsNullOrUnset())
                        width = E.Block_Containing.Width;
                }
            }

            return width;
        }

        /// <summary>
        /// Resolves the given Max-Height style value to an absolute value for the specified element
        /// </summary>
        /// <param name="E"></param>
        /// <param name="maxWidth"></param>
        public int? Resolve_MaxSize_Height(uiElement E, CSSValue maxHeight)
        {
            int? height = 0;
            
            height = (int?)maxHeight.Resolve(E.Block_Containing.Height);
            
            if (height.HasValue) height = Math.Max(0, height.Value);
            else if (maxHeight == CSSValue.None) height = null;
            else height = 0;

            // Resolve AUTO
            // (This is NOT defined by the W3C standards, which specify no logic for resolving AUTO values for Size-Max so this logic is only relevant for the purposes of THIS ui system)
            // For AUTO values on Max-Width/Height we
            // Act like our computed value is 100% ONLY IF our parent has an explicit value set!
            if (E.Parent != null)
            {
                if (maxHeight == CSSValue.Auto)
                {
                    if (!E.Parent.Style.Final.Height.Assigned.IsNullOrUnset())
                        height = E.Block_Containing.Height;
                }
            }

            return height;
        }

        /// <summary>
        /// Resolves the given Width and Height style values to absolute values for a specified element
        /// </summary>
        /// <param name="E"></param>
        /// <param name="width"></param>
        /// <param name="weight"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        public void Resolve_Size(uiElement E, CSSValue Width, CSSValue Height, out int outWidth, out int outHeight)
        {// SEE: https://www.w3.org/TR/css3-box/#width
            outWidth = Resolve_Size_Width(E, Width);
            outHeight = Resolve_Size_Height(E, Height);
        }

        /// <summary>
        /// Resolves the given Width style value to an absolute value for a specified element
        /// </summary>
        /// <param name="E">The UI element to resolve for</param>
        public int Resolve_Size_Width(uiElement E, CSSValue Width)
        {// SEE: https://www.w3.org/TR/css3-box/#width
            int? width = Resolve_Size_Width_Nullable(E, Width);
            return (width.HasValue ? Math.Max(0, width.Value) : 0);
        }

        /// <summary>
        /// Resolves the given Height style value to an absolute value for a specified element
        /// </summary>
        /// <param name="E">The UI element to resolve for</param>
        public int Resolve_Size_Height(uiElement E, CSSValue Height)
        {
            int? height = Resolve_Size_Height_Nullable(E, Height);
            return (height.HasValue ? Math.Max(0, height.Value) : 0);
        }

        /// <summary>
        /// Resolves the given (INTRINSIC) Width and Height style values to absolute values for a specified element
        /// </summary>
        /// <param name="E">The UI element to resolve for</param>
        public void Resolve_Intrinsic_Size(uiElement E, CSSValue Width, CSSValue Height, out int? outWidth, out int? outHeight)
        {
            //Percentage intrinsic widths are first evaluated with respect to the containing block's width, 
            // if that width doesn't itself depend on the replaced element's width. 
            // If it does, then a percentage intrinsic width on that element can't be resolved
            // and the element is assumed to have no intrinsic width.
            int? width = (int?)Width.Resolve(E.Block_Containing.Width);
            int? height = (int?)Height.Resolve(E.Block_Containing.Height);

            if (Width.Type == EStyleDataType.PERCENT && E.Parent.Style.Final.Width.Assigned.IsNullOrUnset())
                width = null;// Containing block's width depends on our size, so we cant actually resolve this percentage

            outWidth = width;
            outHeight = height;
        }

        /// <summary>
        /// Resolves the given (INTRINSIC) Width style value to an absolute value for a specified element
        /// </summary>
        /// <param name="E">The UI element to resolve for</param>
        public int? Resolve_Intrinsic_Width(uiElement E, CSSValue Width)
        {
            //Percentage intrinsic widths are first evaluated with respect to the containing block's width, 
            // if that width doesn't itself depend on the replaced element's width. 
            // If it does, then a percentage intrinsic width on that element can't be resolved
            // and the element is assumed to have no intrinsic width.
            int? width = (int?)Width.Resolve(E.Block_Containing.Width);

            if (Width.Type == EStyleDataType.PERCENT && E.Parent.Style.Final.Width.Assigned.IsNullOrUnset())
                width = null;// Containing block's width depends on our size, so we cant actually resolve this percentage

            return width;
        }
        /// <summary>
        /// Resolves the given (INTRINSIC) Height style value to an absolute value for a specified element
        /// </summary>
        /// <param name="E">The UI element to resolve for</param>
        public int? Resolve_Intrinsic_Height(uiElement E, CSSValue Height)
        {
            int? height = (int?)Height.Resolve(E.Block_Containing.Height);
            return height;
        }



        /// <summary>
        /// Resolves the given Width style value to an absolute value for a specified element
        /// </summary>
        /// <param name="E">The UI element to resolve for</param>
        public int? Resolve_Size_Width_Nullable(uiElement E, CSSValue Width)
        {// SEE: https://www.w3.org/TR/css3-box/#width
            int? width = (int?)Width.Resolve(E.Block_Containing.Width);
            // Resolve all AUTO values
            if (Width == CSSValue.Auto)
            {// If ‘width’ is set to ‘auto’, any other ‘auto’ values become ‘0’ and ‘width’ follows from the resulting equality.
             // ‘margin-left’ + ‘border-left-width’ + ‘padding-left’ + ‘width’ + ‘padding-right’ + ‘border-right-width’ + ‘margin-right’ + scrollbar width (if any) = width of containing block
                int W = 0, H = 0;
                E.Block_Resize_Content_To_Margin_NoAuto(ref W, ref H, 0);
                width = (E.Block_Containing.Width - W);
            }

            return width;
        }
        
        /// <summary>
        /// Resolves the given Height style value to an absolute value for a specified element
        /// </summary>
        /// <param name="E">The UI element to resolve for</param>
        public int? Resolve_Size_Height_Nullable(uiElement E, CSSValue Height)
        {
            return (int?)Height.Resolve(E.Block_Containing.Height);
        }


        /// <summary>
        /// Resolves the current Top, Right, Bottom, and Left style values to absolute values for a specified element
        /// </summary>
        /// <param name="E"></param>
        /// <param name="Top"></param>
        /// <param name="Right"></param>
        /// <param name="Bottom"></param>
        /// <param name="Left"></param>
        /// <param name="Pos_Top"></param>
        /// <param name="Pos_Right"></param>
        /// <param name="Pos_Bottom"></param>
        /// <param name="Pos_Left"></param>
        public void Resolve_Offsets(uiElement E, CSSValue Top, CSSValue Right, CSSValue Bottom, CSSValue Left, out int Pos_Top, out int Pos_Right, out int Pos_Bottom, out int Pos_Left)
        {
            int top, right, bottom, left;
            switch (Positioning)
            {
                case EPositioning.Relative:// SEE:  https://www.w3.org/TR/CSS2/visuren.html#relative-positioning
                    {
                        // Left & Right
                        if (Left == CSSValue.Auto && Right == CSSValue.Auto)
                        {
                            left = 0;
                            right = 0;
                        }
                        else if (Left == CSSValue.Auto)
                        {
                            right = (int)Right.Resolve();
                            left = -right;
                        }
                        else if (Right == CSSValue.Auto)
                        {
                            left = (int)Left.Resolve();
                            right = -left;
                        }
                        else// Over-constrained
                        {
                            // Since we do not support the 'direction' property we will default to letting 'left' win
                            left = (int)Left.Resolve();
                            right = -left;
                        }

                        // Top & Bottom
                        if (Top == CSSValue.Auto && Bottom == CSSValue.Auto)
                        {
                            top = 0;
                            bottom = 0;
                        }
                        else if (Top == CSSValue.Auto ^ Bottom == CSSValue.Auto)
                        {
                            if (Top == CSSValue.Auto)
                            {
                                bottom = (int)Bottom.Resolve();
                                top = -bottom;
                            }
                            else
                            {
                                top = (int)Top.Resolve();
                                bottom = -top;
                            }
                        }
                        else
                        {
                            top = (int)Top.Resolve();
                            bottom = -top;
                        }
                    }
                    break;
                case EPositioning.Absolute:
                case EPositioning.Fixed:
                    {
                        top = (int)Top.Resolve_Or_Default(0);
                        right = (int)Right.Resolve_Or_Default(0);
                        bottom = (int)Bottom.Resolve_Or_Default(0);
                        left = (int)Left.Resolve_Or_Default(0);
                    }
                    break;
                case EPositioning.Static:// 'Top', 'Right', 'Bottom', and 'Left' properties are IGNORED
                default:
                    {
                        top = 0;
                        right = 0;
                        bottom = 0;
                        left = 0;
                    }
                    break;
            }

            Pos_Top = top;
            Pos_Right = right;
            Pos_Bottom = bottom;
            Pos_Left = left;
        }
        
        /// <summary>
        /// Finalizes the Width and Height style values for a specified element by applying constraints and rules according to our Display value, among others
        /// </summary>
        /// <param name="E"></param>
        /// <param name="tentative_Width"></param>
        /// <param name="tentative_Height"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        internal void Finalize_Element_Size(uiElement E, int tentative_Width, int tentative_Height, out int outWidth, out int outHeight)
        {
            // Resolve our Size to absolute integer values
            int resolvedWidth = 0;
            int resolvedHeight = 0;

            if (Display == EDisplayMode.INLINE)
            {
                // 'auto' values are interpreted differently for a ReplacedElement's size
                if (E is ReplacedElement)
                {
                    if (!E.isChild) throw new Exception("All 'ReplacedElements' MUST be the child of another element!");
                    int? Width = null, Height = null;
                    var rE = (E as ReplacedElement);
                    bool auto_width = (Final.Width.Computed == CSSValue.Auto);
                    bool auto_height = (Final.Height.Computed == CSSValue.Auto);

                    eSize tempRes = null;
                    if (!auto_width || !auto_height)
                    {
                        tempRes = new eSize()
                        {
                            Width = tentative_Width,
                            Height = tentative_Height,
                        };
                        //tempRes = Size.Resolve_As_Size(this);
                    }

                    int? iWidth = Intrinsic_Width;
                    int? iHeight = Intrinsic_Height;
                    //Resolve_Intrinsic_Size(E, Current.Intrinsic_Width.Computed, Current.Intrinsic_Height.Computed, out iWidth, out iHeight);
                    bool has_Intrinsic_Width = iWidth.HasValue;
                    bool has_Intrinsic_Height = iHeight.HasValue;

                    if (!auto_width) Width = tempRes.Width;
                    if (!auto_height) Height = tempRes.Height;

                    if (auto_width && auto_height && has_Intrinsic_Width) Width = iWidth.Value;// iWidth is absolutely supposed to have a valid integer value here! // Width = rE.IntrinsicSize.Resolve_As_Size(E).Width;
                    if (auto_width && auto_height && has_Intrinsic_Height) Height = iHeight.Value;// iHeight is absolutely supposed to have a valid integer value here! // Height = rE.IntrinsicSize.Resolve_As_Size(E).Height;
                    if (Intrinsic_Ratio.HasValue)
                    {
                        // Computing Width using Intrinsic_Ratio
                        if (auto_width && auto_height && !has_Intrinsic_Width && has_Intrinsic_Height) Width = (int)((double)Height.Value / Intrinsic_Ratio.Value);// We can use Height.Value here because if this condition is met then is has definately been given a value
                        if (auto_width && !auto_height) Width = (int)((double)tempRes.Height / Intrinsic_Ratio.Value);
                        // Computing Height using Intrinsic_Ratio
                        if (auto_width && auto_height && !has_Intrinsic_Height && has_Intrinsic_Width) Height = (int)((double)Width.Value * Intrinsic_Ratio.Value);// We can use Width.Value here because if this condition is met then is has definately been given a value
                        if (auto_height && !auto_width) Height = (int)((double)tempRes.Width * Intrinsic_Ratio.Value);

                        if (auto_width && auto_height && !has_Intrinsic_Width && !has_Intrinsic_Height && !E.Parent.Style.Final.Width.Assigned.IsNullOrUnset())
                        {
                            Width = tentative_Width;
                            Height = (int)((double)Width.Value / Intrinsic_Ratio.Value);
                        }
                    }
                    if (auto_width && !Width.HasValue) Width = 300;
                    if (auto_height && !Height.HasValue) Height = Math.Max(150, (int)(0.5f * (float)Width.Value));

                    resolvedWidth = Width.Value;
                    resolvedHeight = Height.Value;
                }
                else// Inline non-replaced element
                {
                    resolvedWidth = (Content_Width.HasValue ? Content_Width.Value : 0);
                    resolvedHeight = (Content_Height.HasValue ? Content_Height.Value : 0);
                }
            }
            else if (Display == EDisplayMode.INLINE_BLOCK)
            {
                int? rw = Resolve_Size_Width_Nullable(E, Final.Width.Computed);
                int? rh = Resolve_Size_Height_Nullable(E, Final.Height.Computed);
                //Resolve_Size(E, Current.Width.Computed, Current.Height.Computed, out resolvedWidth, out resolvedHeight);

                if (rw.HasValue)
                    resolvedWidth = rw.Value;
                else if (Content_Width.HasValue)
                    resolvedWidth = Content_Width.Value;

                if (rh.HasValue)
                    resolvedHeight = rh.Value;
                else if (Content_Height.HasValue)
                    resolvedHeight = Content_Height.Value;
            }
            else if (Display == EDisplayMode.BLOCK)
            {
                // Block elements size to fill up the entire width of their containing-block
                resolvedWidth = E.Block_Containing.Width;

                int? rh = Resolve_Size_Height_Nullable(E, Final.Height.Computed);
                if (rh.HasValue)
                    resolvedHeight = rh.Value;
                else if (Content_Height.HasValue)
                    resolvedHeight = Content_Height.Value;
            }

            // Apply constraints to our size
            Constrain_Size(resolvedWidth, resolvedHeight, Min_Width, Min_Height, Max_Width, Max_Height, out outWidth, out outHeight);
        }

        /// <summary>
        /// Resolves the X and Y offset values for a specified element by accounting for the current 'positioning' value and the assigned layout offset
        /// </summary>
        internal void Resolve_XY(uiElement E, int Top, int Right, int Bottom, int Left, out int X, out int Y)
        {
            int x = 0;
            int y = 0;
            switch (Positioning)
            {
                case EPositioning.Static:// 'Top', 'Right', 'Bottom', and 'Left' properties are IGNORED
                    x = Layout_Pos_X;
                    y = Layout_Pos_Y;
                    break;
                case EPositioning.Relative:// SEE:  https://www.w3.org/TR/CSS2/visuren.html#relative-positioning
                    x = Layout_Pos_X + Left;
                    y = Layout_Pos_Y + Top;
                    break;
                case EPositioning.Absolute:// SEE:  https://www.w3.org/TR/CSS2/visuren.html#absolute-positioning
                case EPositioning.Fixed:
                    x = Left;
                    y = Top;
                    break;
            }

            X = x;
            Y = y;
        }

        /// <summary>
        /// Resolves the given X/Y style values to position offsets using the CSS "Position" value type algorithm.
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="ObjectArea"></param>
        /// <param name="ObjectSize"></param>
        /// <param name="outX"></param>
        /// <param name="outY"></param>
        public void Resolve_As_Position(CSSValue X, CSSValue Y, eSize ObjectArea, eSize ObjectSize, out int outX, out int outY)
        {// SEE: https://www.w3.org/TR/2011/CR-css3-background-20110215/#background-position

            int? x = (int?)X.Resolve(delegate (double pct)
            {
                return (((double)ObjectArea.Width * pct) - ((double)ObjectSize.Width * pct));
            });

            int? y = (int?)Y.Resolve(delegate (double pct)
            {
                return (((double)ObjectArea.Height * pct) - ((double)ObjectSize.Height * pct));
            });

            outX = (x.HasValue ? x.Value : 0);
            outY = (y.HasValue ? y.Value : 0);
        }

        #endregion

        #region Tentative Block Calculation Methods
        internal static void Get_Tentative_Block(uiElement E, BlockProperties Block)
        {
            switch(E.Style.Positioning)
            {
                case EPositioning.Fixed:
                case EPositioning.Absolute:
                    {
                        if (E is ReplacedElement)
                        {
                            Get_Tentative_Abs_Replaced(E, Block);
                        }
                        else
                        {
                            Get_Tentative_Abs_NonReplaced(E, Block);
                        }
                    }
                    break;
                case EPositioning.Static:
                case EPositioning.Relative:
                default:
                    {
                        switch (E.Style.Display)
                        {
                            case EDisplayMode.INLINE:
                                {
                                    if (E is ReplacedElement)
                                    {
                                        Get_Tentative_Inline_Or_InlineBlock_Or_Floating_Replaced(E, Block);
                                    }
                                    else
                                    {
                                        Get_Tentative_Inline_NonReplaced(E, Block);
                                    }
                                }
                                break;
                            case EDisplayMode.INLINE_BLOCK:
                                {
                                    if (E is ReplacedElement)
                                    {
                                        Get_Tentative_Inline_Or_InlineBlock_Or_Floating_Replaced(E, Block);
                                    }
                                    else
                                    {
                                        Get_Tentative_Inline_Block_NonReplaced(E, Block);
                                    }
                                }
                                break;
                            case EDisplayMode.BLOCK:
                                {
                                    if (E is ReplacedElement)
                                    {
                                        Get_Tentative_Block_Level_Replaced(E, Block);
                                    }
                                    else
                                    {
                                        Get_Tentative_Block_Level_NonReplaced(E, Block);
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        internal static int Solve_Block_Level_Width_Equality(uiElement E, BlockProperties Block, CSSValue Width, CSSValue Left_Margin, CSSValue Right_Margin)
        {
            return (int)(Left_Margin.Resolve_Or_Default(0) + Block.Border_Left + Block.Padding_Left.Resolve_Or_Default(0) + Width.Resolve_Or_Default(0) + Block.Padding_Right.Resolve_Or_Default(0) + Block.Border_Right + Right_Margin.Resolve_Or_Default(0) + E.Scrollbar_Offset.Horizontal);
        }

        internal static int Solve_Abs_Block_Equation(uiElement E, BlockProperties Block, params string[] Targets)
        {
            // Solving following equation dynamically
            // 'left' + 'margin-left' + 'border-left-width' + 'padding-left' + 'width' + 'padding-right' + 'border-right-width' + 'margin-right' + 'right' = width of containing block
            double eqRight = E.Block_Containing.Width;
            double eqLeft = 0;
            // Add all of the width-related properties to 'eqLeft' except for the ones we are solving for!
            if (!Targets.Contains("Left", StringComparer.OrdinalIgnoreCase)) eqLeft += Block.Left.Resolve_Or_Default(0);
            if (!Targets.Contains("Margin_Left", StringComparer.OrdinalIgnoreCase)) eqLeft += Block.Margin_Left.Resolve_Or_Default(0);
            if (!Targets.Contains("Border_Left", StringComparer.OrdinalIgnoreCase)) eqLeft += Block.Border_Left;
            if (!Targets.Contains("Padding_Left", StringComparer.OrdinalIgnoreCase)) eqLeft += Block.Padding_Left.Resolve_Or_Default(0);
            if (!Targets.Contains("Width", StringComparer.OrdinalIgnoreCase)) eqLeft += Block.Width.Resolve_Or_Default(0);
            if (!Targets.Contains("Padding_Right", StringComparer.OrdinalIgnoreCase)) eqLeft += Block.Padding_Right.Resolve_Or_Default(0);
            if (!Targets.Contains("Border_Right", StringComparer.OrdinalIgnoreCase)) eqLeft += Block.Border_Right;
            if (!Targets.Contains("Margin_Right", StringComparer.OrdinalIgnoreCase)) eqLeft += Block.Margin_Right.Resolve_Or_Default(0);
            if (!Targets.Contains("Right", StringComparer.OrdinalIgnoreCase)) eqLeft += Block.Right.Resolve_Or_Default(0);

            return (int)(eqRight - eqLeft);
        }


        /// <summary>
        /// Calculates the used Width and Margin values for an Inline, Non-Replaced element.
        /// </summary>
        internal static void Get_Tentative_Inline_NonReplaced(uiElement E, BlockProperties Block)
        {// SEE:  https://www.w3.org/TR/css3-box/#inline-non-replaced
            
            Block.Width = (Block.Content_Width.HasValue ? CSSValue.From_Int(Block.Content_Width.Value) : CSSValue.Zero);
            Block.Height = (Block.Content_Height.HasValue ? CSSValue.From_Int(Block.Content_Height.Value) : CSSValue.Zero);
            
            if (Block.Top == CSSValue.Auto) Block.Top = CSSValue.Zero;
            if (Block.Right == CSSValue.Auto) Block.Right = CSSValue.Zero;
            if (Block.Bottom == CSSValue.Auto) Block.Bottom = CSSValue.Zero;
            if (Block.Left == CSSValue.Auto) Block.Left = CSSValue.Zero;

            if (Block.Margin_Top == CSSValue.Auto) Block.Margin_Top = CSSValue.Zero;
            if (Block.Margin_Right == CSSValue.Auto) Block.Margin_Right = CSSValue.Zero;
            if (Block.Margin_Bottom == CSSValue.Auto) Block.Margin_Bottom = CSSValue.Zero;
            if (Block.Margin_Left == CSSValue.Auto) Block.Margin_Left = CSSValue.Zero;
        }
        /// <summary>
        /// Calculates the used Width and Margin values for an Inline, Replaced element.
        /// </summary>
        internal static void Get_Tentative_Inline_Or_InlineBlock_Or_Floating_Replaced(uiElement E, BlockProperties Block)
        {// SEE:  https://www.w3.org/TR/css3-box/#inline-replaced

            if (Block.Margin_Left == CSSValue.Auto) Block.Margin_Left = CSSValue.Zero;
            if (Block.Margin_Right == CSSValue.Auto) Block.Margin_Right = CSSValue.Zero;

            int? width = null;
            int? height = null;

            bool auto_width = (Block.Width == CSSValue.Auto);
            bool auto_height = (Block.Height == CSSValue.Auto);

            if (auto_width && auto_height && Block.Intrinsic_Width.HasValue)
                width = Block.Intrinsic_Width.Value;
            else if (auto_width && auto_height && !Block.Intrinsic_Width.HasValue && Block.Intrinsic_Height.HasValue && Block.Intrinsic_Ratio.HasValue)
                width = (int)(Block.Intrinsic_Ratio.Value * Block.Intrinsic_Height.Value);
            else if (auto_width && !auto_height && Block.Intrinsic_Ratio.HasValue)
                width = (int)(Block.Intrinsic_Ratio.Value * Block.Height.Resolve());
            if (auto_width && auto_height && Block.Intrinsic_Ratio.HasValue && !Block.Intrinsic_Height.HasValue && !Block.Intrinsic_Width.HasValue && !E.Parent.Style.Final.Width.Assigned.IsNullOrUnset())
            {
                BlockProperties tmp = new BlockProperties(Block);
                Get_Tentative_Block_Level_NonReplaced(E, tmp);

                width = (int)tmp.Width.Resolve_Or_Default(0);
                height = (int)((double)width.Value / Block.Intrinsic_Ratio.Value);
            }

            if (auto_width && auto_height && Block.Intrinsic_Height.HasValue)
                height = Block.Intrinsic_Height.Value;
            else if (auto_width && auto_height && !Block.Intrinsic_Height.HasValue && Block.Intrinsic_Width.HasValue && Block.Intrinsic_Ratio.HasValue)
                height = (int)((double)Block.Intrinsic_Width.Value / Block.Intrinsic_Ratio.Value);
            else if (!auto_width && auto_height && Block.Intrinsic_Ratio.HasValue)
                height = (int)((double)Block.Width.Resolve() / Block.Intrinsic_Ratio.Value);

            if (auto_width && !width.HasValue) width = 300;
            if (auto_height && !height.HasValue) height = Math.Max(150, (int)(0.5 * (double)width.Value));


            if (width.HasValue) Block.Width = CSSValue.From_Int(width.Value);
            if (height.HasValue) Block.Height = CSSValue.From_Int(height.Value);
        }
        /// <summary>
        /// Calculates the used Width and Margin values for a Block-level, Non-Replaced element.
        /// </summary>
        internal static void Get_Tentative_Block_Level_NonReplaced(uiElement E, BlockProperties Block)
        {// SEE:  https://www.w3.org/TR/css3-box/#blockwidth

            /*
                The following constraints must hold among the used values of the properties.
                ‘margin-left’ + ‘border-left-width’ + ‘padding-left’ + ‘width’ + ‘padding-right’ + ‘border-right-width’ + ‘margin-right’ + scrollbar width (if any) = width of containing block
            */

            if (Block.Margin_Left != CSSValue.Auto && Block.Width != CSSValue.Auto && Block.Margin_Right != CSSValue.Auto)
            {// Over-constrained, ignore right margin
                Block.Margin_Right = CSSValue.From_Int(0);
            }

            if (Block.Width == CSSValue.Auto)
            {// If width is 'auto' then all other 'auto' values are treated as 0
                if (Block.Margin_Left == CSSValue.Auto) Block.Margin_Left = CSSValue.From_Int(0);
                if (Block.Margin_Right == CSSValue.Auto) Block.Margin_Right = CSSValue.From_Int(0);
                int outter = Solve_Block_Level_Width_Equality(E, Block, CSSValue.Null, Block.Margin_Left, Block.Margin_Right);
                Block.Width = CSSValue.From_Int(Math.Max(0, E.Block_Containing.Width - outter));
            }
            else
            {
                if (Block.Margin_Left == CSSValue.Auto || Block.Margin_Right == CSSValue.Auto)
                {
                    int W = (int)Block.Width.Resolve_Or_Default(0);
                    // Add everything to this content-block size EXCEPT our margins, because our margins are what we are calculating
                    // IF our size specifies the dimensions of our content-area block then we need to expand that theoretical content-area block
                    // to a border-edge block so we can tell how much free space our margins can auto-consume
                    if (E.Style.BoxSizing == EBoxSizingMode.CONTENT)
                    {
                        W = Solve_Block_Level_Width_Equality(E, Block, Block.Width, CSSValue.Zero, CSSValue.Zero);
                        //W += (Style.Padding.Horizontal + Scrollbar_Offset.Horizontal + (Border.Left.Size + Border.Right.Size));
                    }

                    int freeSpace = Math.Max(0, E.Block_Containing.Width - W);

                    if (Block.Margin_Left == CSSValue.Auto && Block.Margin_Right == CSSValue.Auto)
                    {
                        int x = (freeSpace / 2);
                        Block.Margin_Left = CSSValue.From_Int(x);
                        Block.Margin_Right = CSSValue.From_Int(x);
                    }
                    else if (Block.Margin_Left == CSSValue.Auto)
                    {
                        Block.Margin_Left = CSSValue.From_Int(freeSpace);
                    }
                    else if (Block.Margin_Right == CSSValue.Auto)
                    {
                        Block.Margin_Right = CSSValue.From_Int(freeSpace);
                    }
                }
            }

            if (Block.Margin_Top == CSSValue.Auto) Block.Margin_Top = CSSValue.Zero;
            if (Block.Margin_Bottom == CSSValue.Auto) Block.Margin_Bottom = CSSValue.Zero;
            if (Block.Height == CSSValue.Auto) Block.Height = CSSValue.From_Int((Block.Content_Height.HasValue ? Block.Content_Height.Value : 0));
        }
        /// <summary>
        /// Calculates the used Width and Margin values for a Block-level, Replaced element.
        /// </summary>
        internal static void Get_Tentative_Block_Level_Replaced(uiElement E, BlockProperties Block)
        {// SEE:  https://www.w3.org/TR/css3-box/#block-level1

            BlockProperties orig = new BlockProperties(Block);
            Get_Tentative_Inline_Or_InlineBlock_Or_Floating_Replaced(E, Block);
            // We ignore the margin values the previous function came back with, so restore them to what they originally were.
            Block.Margin_Left = orig.Margin_Left;
            Block.Margin_Right = orig.Margin_Right;

            Get_Tentative_Block_Level_NonReplaced(E, Block);
        }
        /// <summary>
        /// Calculates the used Width and Margin values for a floating, Non-Replaced element.
        /// </summary>
        internal static void Get_Tentative_Floating_NonReplaced(uiElement E, BlockProperties Block)
        {// SEE:  https://www.w3.org/TR/css3-box/#floating

            if (Block.Width == CSSValue.Auto) Block.Width = CSSValue.From_Int(Find_Shrink_To_Fit_Width(E, Block));
            if (Block.Height == CSSValue.Auto) Block.Height = CSSValue.From_Int((Block.Content_Height.HasValue ? Block.Content_Height.Value : 0));
        }
        /// <summary>
        /// Calculates the used Width and Margin values for an Absolutely positioned, Non-Replaced element.
        /// </summary>
        internal static void Get_Tentative_Abs_NonReplaced(uiElement E, BlockProperties Block)
        {// SEE:  https://www.w3.org/TR/css3-box/#abs-non-replaced-width

            if (Block.Left == CSSValue.Auto && Block.Width == CSSValue.Auto && Block.Right == CSSValue.Auto)
            {
                if (Block.Margin_Left == CSSValue.Auto) Block.Margin_Left = CSSValue.Zero;
                if (Block.Margin_Right == CSSValue.Auto) Block.Margin_Right = CSSValue.Zero;

                // Set Left to the static position | SEE:  https://www.w3.org/TR/CSS2/visudet.html#static-position
                Block.Left = CSSValue.From_Number(E.Block_Containing.Left);
                // Apply rule 3
                if (Block.Width == CSSValue.Auto && Block.Right == CSSValue.Auto && Block.Left != CSSValue.Auto)
                {
                    // To find the shrink-to-fit width we need to use 0 for 'left' during it's calculation
                    CSSValue oleft = Block.Left;
                    Block.Left = CSSValue.Zero;
                    Block.Width = CSSValue.From_Int(Find_Shrink_To_Fit_Width(E, Block));
                    Block.Right = CSSValue.From_Int(Solve_Abs_Block_Equation(E, Block, "Right"));
                }
            }
            else if (Block.Left != CSSValue.Auto && Block.Width != CSSValue.Auto && Block.Right != CSSValue.Auto)
            {// None of the three are Auto
                if (Block.Margin_Left == CSSValue.Auto && Block.Margin_Right == CSSValue.Auto)
                {
                    int margin = Solve_Abs_Block_Equation(E, Block, "margin_left", "margin_right");
                    if (margin > 0)
                    {
                        int mval = (margin / 2);
                        Block.Margin_Left = CSSValue.From_Int(mval);
                        Block.Margin_Right = CSSValue.From_Int(mval);
                    }
                    else// equal margins would be zero, solve for just one
                    {
                        Block.Margin_Left = CSSValue.Zero;
                        Block.Margin_Right = CSSValue.From_Int(Solve_Abs_Block_Equation(E, Block));
                    }
                }
                else if (Block.Margin_Left == CSSValue.Auto)
                {
                    Block.Margin_Left = CSSValue.From_Int(Solve_Abs_Block_Equation(E, Block));
                }
                else if (Block.Margin_Right == CSSValue.Auto)
                {
                    Block.Margin_Right = CSSValue.From_Int(Solve_Abs_Block_Equation(E, Block));
                }
                else// Overconstrained (all values that affect block width are set to an absolute value)
                {// We need to ignore one of the set property values that affect block width and act like it's 'Auto', eg solve for it.
                    // Only support LTR direction, so ignore the 'Right' value
                    Block.Right = CSSValue.From_Int(Solve_Abs_Block_Equation(E, Block, "right"));
                }
            }
            else
            {// If all else fails we set margin left/right to 0 and do some rule matching
                Block.Margin_Left = CSSValue.Zero;
                Block.Margin_Right = CSSValue.Zero;
                
                if (Block.Right != CSSValue.Auto && Block.Left == CSSValue.Auto && Block.Width == CSSValue.Auto)
                {// Left and Width are 'Auto'
                    Block.Left = CSSValue.Zero;
                    Block.Width = CSSValue.From_Int(Find_Shrink_To_Fit_Width(E, Block));
                    Block.Left = CSSValue.From_Int(Solve_Abs_Block_Equation(E, Block, "left"));
                }
                else if (Block.Width != CSSValue.Auto && Block.Left == CSSValue.Auto && Block.Right == CSSValue.Auto)
                {// Left and Right are 'Auto', we can only solve for one though
                    // Left gets the static position
                    Block.Left = CSSValue.From_Number(E.Block_Containing.Left);
                    Block.Right = CSSValue.From_Int(Solve_Abs_Block_Equation(E, Block));
                }
                else if (Block.Left != CSSValue.Auto && Block.Width == CSSValue.Auto && Block.Right == CSSValue.Auto)
                {// Width and Right are 'Auto'
                    Block.Width = CSSValue.From_Int(Find_Shrink_To_Fit_Width(E, Block));
                    Block.Right = CSSValue.From_Int(Solve_Abs_Block_Equation(E, Block));
                }
                else if (Block.Left == CSSValue.Auto && Block.Width != CSSValue.Auto && Block.Right != CSSValue.Auto)
                {// Solve for Left
                    Block.Left = CSSValue.From_Int(Solve_Abs_Block_Equation(E, Block));
                }
                else if (Block.Width == CSSValue.Auto && Block.Left != CSSValue.Auto && Block.Right != CSSValue.Auto)
                {// Solve for Width
                    Block.Width = CSSValue.From_Int(Solve_Abs_Block_Equation(E, Block));
                }
                else if (Block.Right == CSSValue.Auto && Block.Left != CSSValue.Auto && Block.Width != CSSValue.Auto)
                {// Solve for Right
                    Block.Right = CSSValue.From_Int(Solve_Abs_Block_Equation(E, Block));
                }
            }
        }
        /// <summary>
        /// Calculates the used Width and Margin values for an Absolutely positioned, Replaced element.
        /// </summary>
        internal static void Get_Tentative_Abs_Replaced(uiElement E, BlockProperties Block)
        {
            // Width determined using same method as for inline-replaced elements
            BlockProperties tmp = new BlockProperties(Block);
            Get_Tentative_Inline_Or_InlineBlock_Or_Floating_Replaced(E, tmp);
            Block.Width = CSSValue.From_Number(tmp.Width.Resolve_Or_Default(0));

            if (Block.Margin_Left == CSSValue.Auto || Block.Margin_Right == CSSValue.Auto)
            {
                if (Block.Left == CSSValue.Auto && Block.Right == CSSValue.Auto)
                {
                    Block.Left = CSSValue.From_Int(E.Block_Containing.Left);
                }

                if (Block.Left == CSSValue.Auto || Block.Right == CSSValue.Auto)
                {
                    if (Block.Margin_Left == CSSValue.Auto) Block.Margin_Left = CSSValue.Zero;
                    if (Block.Margin_Right == CSSValue.Auto) Block.Margin_Right = CSSValue.Zero;
                }

                if (Block.Margin_Left == CSSValue.Auto && Block.Margin_Right == CSSValue.Auto)
                {
                    int margin = Solve_Abs_Block_Equation(E, Block, "margin_left", "margin_right");
                    if (margin > 0)
                    {
                        int mval = (margin / 2);
                        Block.Margin_Left = CSSValue.From_Int(mval);
                        Block.Margin_Right = CSSValue.From_Int(mval);
                    }
                    else
                    {
                        Block.Margin_Left = CSSValue.Zero;
                        Block.Margin_Right = CSSValue.From_Number(Solve_Abs_Block_Equation(E, Block));
                    }
                }

                // If at this point there is an 'Auto' value left, solve for it
                // Width was already solved, margin-left/right HAVE to be solved now, padding cannot have AUTO values, nor can border-widths. so we just have to check on Left and Right
                if (Block.Left == CSSValue.Auto)
                {
                    Block.Left = CSSValue.From_Number(Solve_Abs_Block_Equation(E, Block));
                }
                if (Block.Right == CSSValue.Auto)
                {
                    Block.Right = CSSValue.From_Number(Solve_Abs_Block_Equation(E, Block));
                }

                if (Block.Left != CSSValue.Auto && Block.Margin_Left != CSSValue.Auto && Block.Padding_Left != CSSValue.Auto && Block.Width != CSSValue.Auto && Block.Padding_Right != CSSValue.Auto && Block.Margin_Right != CSSValue.Auto && Block.Right != CSSValue.Auto)
                {// Over-constrained
                    Block.Right = CSSValue.From_Number(Solve_Abs_Block_Equation(E, Block, "right"));
                }
            }
        }
        /// <summary>
        /// Calculates the used Width and Margin values for an Inline-Block, Non-Replaced element.
        /// </summary>
        internal static void Get_Tentative_Inline_Block_NonReplaced(uiElement E, BlockProperties Block)
        {// SEE:  https://www.w3.org/TR/css3-box/#lsquo

            if (Block.Width == CSSValue.Auto) Block.Width = CSSValue.From_Int(Find_Shrink_To_Fit_Width(E, Block));
            if (Block.Height == CSSValue.Auto) Block.Height = CSSValue.From_Int((Block.Content_Height.HasValue ? Block.Content_Height.Value : 0));
            
            if (Block.Margin_Top == CSSValue.Auto) Block.Margin_Top = CSSValue.Zero;
            if (Block.Margin_Right == CSSValue.Auto) Block.Margin_Right = CSSValue.Zero;
            if (Block.Margin_Bottom == CSSValue.Auto) Block.Margin_Bottom = CSSValue.Zero;
            if (Block.Margin_Left == CSSValue.Auto) Block.Margin_Left = CSSValue.Zero;
        }
        #endregion

        #region Standard Sizing Constraints


        internal static int Find_Shrink_To_Fit_Width(uiElement E, BlockProperties Block)
        {// SEE:  https://www.w3.org/TR/css3-box/#shrink-to-fit

            int width_preferred_min = 0;
            int width_available = E.Block_Containing.Width - (int)(Block.Margin_Left.Resolve_Or_Default(0) + Block.Border_Left + Block.Padding_Left.Resolve_Or_Default(0) + Block.Padding_Right.Resolve_Or_Default(0) + Block.Border_Right + Block.Margin_Right.Resolve_Or_Default(0) + E.Scrollbar_Offset.Horizontal);
            int width_preferred = 0;
            //Roughly: calculate the preferred width by formatting the content without breaking lines other than where explicit line breaks occur, and also calculate the preferred minimum width, e.g., by trying all possible line breaks.
            int? pref = (int?)E.Style.Final.Content_Width.Specified.Resolve();
            switch(pref.HasValue)
            {
                case true:
                    width_preferred = pref.Value;
                    break;
                case false:
                    width_preferred = (int)E.Style.Default.Width.Computed.Resolve_Or_Default(0);
                    break;
            }

            return Math.Min(Math.Max(width_preferred_min, width_available), width_preferred);
        }

        /// <summary>
        /// Returns the given size constrained so it fits within the specified Min and Max bounds
        /// </summary>
        void Constrain_Size(int Width, int Height, int minWidth, int minHeight, int? maxWidth, int? maxHeight, out int outWidth, out int outHeight)
        {
            // Implementation of the W3C box model widths, heights, and margins calculation algorithm specs
            // https://www.w3.org/TR/css3-box/#the-lsquo0
            
            // First lets get our tentative width
            Constrain_Width(ref Width, null, null, null, minHeight, maxHeight);
            if (maxWidth.HasValue && Width > maxWidth.Value)
                Width = maxWidth.Value;
            if (Width < minWidth)
                Width = minWidth;

            // Now lets get our tentative height
            Constrain_Height(null, ref Height, minWidth, maxWidth, null, null);
            if (maxHeight.HasValue && Height > maxHeight.Value)
                Height = maxHeight.Value;
            if (Height < minHeight)
                Height = minHeight;

            outWidth = Width;
            outHeight = Height;
        }
        
        /// <summary>
        /// Returns the given size constrained so it fits within the specified Min and Max bounds
        /// </summary>
        internal protected void Constrain_Size_Preserve_AspectRatio(int width, int height, int minWidth, int minHeight, int? maxWidth, int? maxHeight, out int outWidth, out int outHeight)
        {
            // Implementation of the W3C box model widths, heights, and margins calculation algorithm specs
            // https://www.w3.org/TR/css3-box/#the-lsquo0

            // First lets get our tentative width
            int Width = width;
            int Height = height;
            Constrain_Width(ref Width, Height, null, null, minHeight, maxHeight);
            if (maxWidth.HasValue && Width > maxWidth.Value)
            {
                Width = maxWidth.Value;
                Constrain_Width(ref Width, Height, null, null, minHeight, maxHeight);
            }
            if (Width < minWidth)
            {
                Width = minWidth;
                Constrain_Width(ref Width, Height, null, null, minHeight, maxHeight);
            }

            // Now lets get our tentative height
            Height = height;// Reset the height value to what it is normally
            Constrain_Height(Width, ref Height, minWidth, maxWidth, null, null);
            if (maxHeight.HasValue && Height > maxHeight.Value)
            {
                Height = maxHeight.Value;
                Constrain_Height(Width, ref Height, minWidth, maxWidth, null, null);
            }
            if (Height < minHeight)
            {
                Height = minHeight;
                Constrain_Height(Width, ref Height, minWidth, maxWidth, null, null);
            }

            outWidth = Width;
            outHeight = Height;
        }

        /// <summary>
        /// Applies W3C standard width/height constraints
        /// </summary>
        internal void Constrain_Width_Height_Preserve_AspectRatio(ref int W, ref int H, int? min_width, int? max_width, int? min_height, int? max_height)
        {
            // Implementation of the W3C box model widths, heights, and margins calculation algorithm specs
            // https://www.w3.org/TR/css3-box/#the-lsquo0

            if (max_width.HasValue && W > max_width.Value)
            {
                H = (int)((float)max_width.Value * ((float)H / (float)W));
                if (min_height.HasValue) H = Math.Max(H, min_height.Value);
                W = max_width.Value;
            }
            else if (min_width.HasValue && W < min_width.Value)
            {
                H = (int)((float)min_width.Value * ((float)H / (float)W));
                if (max_height.HasValue) H = Math.Min(H, max_height.Value);
                W = min_width.Value;
            }
            else if (max_height.HasValue && H > max_height.Value)
            {
                W = (int)((float)max_height.Value * ((float)W / (float)H));
                if (min_width.HasValue) W = Math.Max(W, min_width.Value);
                H = max_height.Value;
            }
            else if (min_height.HasValue && H < min_height.Value)
            {
                W = (int)((float)min_height.Value * ((float)W / (float)H));
                if (max_width.HasValue) W = Math.Min(W, max_width.Value);
                H = min_height.Value;
            }
            else if ((max_width.HasValue && max_height.HasValue) && (W > max_width.Value && H > max_height.Value) && (((float)max_width.Value / (float)W) <= ((float)max_height.Value / (float)H)))
            {
                H = (int)((float)max_width.Value * ((float)H / (float)W));
                if (min_height.HasValue) H = Math.Max(min_height.Value, H);
                W = max_width.Value;
            }
            else if ((max_width.HasValue && max_height.HasValue) && (W > max_width.Value && H > max_height.Value) && (((float)max_width.Value / (float)W) > ((float)max_height.Value / (float)H)))
            {
                W = (int)((float)max_height.Value * ((float)W / (float)H));
                if (min_width.HasValue) W = Math.Max(min_width.Value, W);
                H = max_height.Value;
            }
            else if ((min_width.HasValue && min_height.HasValue) && (W < min_width.Value && H < min_height.Value) && (((float)min_width / (float)W) <= ((float)min_height.Value / (float)H)))
            {
                W = (int)((float)min_height.Value * ((float)W / (float)H));
                if (max_width.HasValue) W = Math.Min(max_width.Value, W);
                H = min_height.Value;
            }
            else if ((min_width.HasValue && min_height.HasValue) && (W < min_width.Value && H < min_height.Value) && (((float)min_width / (float)W) > ((float)min_height.Value / (float)H)))
            {
                H = (int)((float)min_width.Value * ((float)H / (float)W));
                if (max_height.HasValue) H = Math.Min(max_height.Value, H);
                W = min_width.Value;
            }
            else if ((min_width.HasValue && W < min_width.Value) && (max_height.HasValue && H > max_height.Value))
            {
                W = min_width.Value;
                H = max_height.Value;
            }
            else if ((max_width.HasValue && W > max_width.Value) && (min_height.HasValue && H < min_height.Value))
            {
                W = max_width.Value;
                H = min_height.Value;
            }
        }

        /// <summary>
        /// Applies W3C standard width/height constraints
        /// </summary>
        internal void Constrain_Width(ref int W, int? H, int? min_width, int? max_width, int? min_height, int? max_height)
        {
            // Implementation of the W3C box model widths, heights, and margins calculation algorithm specs
            // https://www.w3.org/TR/css3-box/#the-lsquo0

            if (max_width.HasValue && W > max_width.Value)
            {
                W = max_width.Value;
            }
            else if (min_width.HasValue && W < min_width.Value)
            {
                W = min_width.Value;
            }
            else if (H.HasValue)
            {
                if (max_height.HasValue && H.Value > max_height.Value)
                {
                    W = (int)((float)max_height.Value * ((float)W / (float)H.Value));
                    if (min_width.HasValue) W = Math.Max(W, min_width.Value);
                }
                else if (min_height.HasValue && H.Value < min_height.Value)
                {
                    W = (int)((float)min_height.Value * ((float)W / (float)H.Value));
                    if (max_width.HasValue) W = Math.Min(W, max_width.Value);
                }
                else if ((max_width.HasValue && max_height.HasValue) && (W > max_width.Value && H.Value > max_height.Value) && (((float)max_width.Value / (float)W) <= ((float)max_height.Value / (float)H.Value)))
                {
                    W = max_width.Value;
                }
                else if ((max_width.HasValue && max_height.HasValue) && (W > max_width.Value && H.Value > max_height.Value) && (((float)max_width.Value / (float)W) > ((float)max_height.Value / (float)H.Value)))
                {
                    W = (int)((float)max_height.Value * ((float)W / (float)H.Value));
                    if (min_width.HasValue) W = Math.Max(min_width.Value, W);
                }
                else if ((min_width.HasValue && min_height.HasValue) && (W < min_width.Value && H.Value < min_height.Value) && (((float)min_width / (float)W) <= ((float)min_height.Value / (float)H.Value)))
                {
                    W = (int)((float)min_height.Value * ((float)W / (float)H.Value));
                    if (max_width.HasValue) W = Math.Min(max_width.Value, W);
                }
                else if ((min_width.HasValue && min_height.HasValue) && (W < min_width.Value && H.Value < min_height.Value) && (((float)min_width / (float)W) > ((float)min_height.Value / (float)H.Value)))
                {
                    W = min_width.Value;
                }
                else if ((min_width.HasValue && W < min_width.Value) && (max_height.HasValue && H.Value > max_height.Value))
                {
                    W = min_width.Value;
                }
                else if ((max_width.HasValue && W > max_width.Value) && (min_height.HasValue && H.Value < min_height.Value))
                {
                    W = max_width.Value;
                }
            }
        }

        /// <summary>
        /// Applies W3C standard width/height constraints
        /// </summary>
        internal void Constrain_Height(int? W, ref int H, int? min_width, int? max_width, int? min_height, int? max_height)
        {
            // Implementation of the W3C box model widths, heights, and margins calculation algorithm specs
            // https://www.w3.org/TR/css3-box/#the-lsquo0

            if (max_height.HasValue && H > max_height.Value)
            {
                H = max_height.Value;
            }
            else if (min_height.HasValue && H < min_height.Value)
            {
                H = min_height.Value;
            }
            else if (W.HasValue)
            {
                if (max_width.HasValue && W.Value > max_width.Value)
                {
                    H = (int)((float)max_width.Value * ((float)H / (float)W.Value));
                    if (min_height.HasValue) H = Math.Max(H, min_height.Value);
                }
                else if (min_width.HasValue && W.Value < min_width.Value)
                {
                    H = (int)((float)min_width.Value * ((float)H / (float)W.Value));
                    if (max_height.HasValue) H = Math.Min(H, max_height.Value);
                }
                else if ((max_width.HasValue && max_height.HasValue) && (W.Value > max_width.Value && H > max_height.Value) && (((float)max_width.Value / (float)W.Value) <= ((float)max_height.Value / (float)H)))
                {
                    H = (int)((float)max_width.Value * ((float)H / (float)W.Value));
                    if (min_height.HasValue) H = Math.Max(min_height.Value, H);
                }
                else if ((max_width.HasValue && max_height.HasValue) && (W.Value > max_width.Value && H > max_height.Value) && (((float)max_width.Value / (float)W.Value) > ((float)max_height.Value / (float)H)))
                {
                    H = max_height.Value;
                }
                else if ((min_width.HasValue && min_height.HasValue) && (W.Value < min_width.Value && H < min_height.Value) && (((float)min_width / (float)W.Value) <= ((float)min_height.Value / (float)H)))
                {
                    H = min_height.Value;
                }
                else if ((min_width.HasValue && min_height.HasValue) && (W.Value < min_width.Value && H < min_height.Value) && (((float)min_width / (float)W.Value) > ((float)min_height.Value / (float)H)))
                {
                    H = (int)((float)min_width.Value * ((float)H / (float)W.Value));
                    if (max_height.HasValue) H = Math.Min(max_height.Value, H);
                }
                else if ((min_width.HasValue && W.Value < min_width.Value) && (max_height.HasValue && H > max_height.Value))
                {
                    H = max_height.Value;
                }
                else if ((max_width.HasValue && W.Value > max_width.Value) && (min_height.HasValue && H < min_height.Value))
                {
                    H = min_height.Value;
                }
            }
        }
        #endregion


    }
}
