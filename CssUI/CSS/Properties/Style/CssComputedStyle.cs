using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CssUI.CSS.Internal;
using CssUI.CSS.Enums;
using CssUI.DOM.Nodes;
using System.Diagnostics.Contracts;

namespace CssUI.CSS
{
    // XXX: This class could be MUCH more performant if we didnt have to create an instance of every defined css property during creation, 
    //      if we could just instantiate the properties when they are needed it would save a lot.

    /* XXX: lets move to referencing these by an AtomicName<ECssProperty> and then just instantiate a LUT which maps the AtomicName to an object pointer.
     * Note: will have to change the way these properties are accessed in that case as some will return null rather then the default value they should have
     *          Although we could just return the property definitions default value, or better yet some kind of proxy to it which when it has its value changed will instantiate a LIVE instance for the property and assign it to its slot in the LUT.
     * */

    /// <summary>
    /// Holds an instance of all the defined css propertys that a css element can have
    /// Each different styling state of an element gets it's own instance of this class which 
    /// are then all cascaded together to determine the current value when the elements active state changes.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class CssComputedStyle
    {
        #region Static
        private static int MAX_PROPERTY_ID_INDEX;
        static CssComputedStyle()
        {
            MAX_PROPERTY_ID_INDEX = Enum.GetValues(typeof(ECssPropertyID)).Cast<int>().Max();
        }
        #endregion

        #region Properties
        /// <summary>
        /// If True then none of this instances property values can be altered by external code.
        /// </summary>
        public readonly bool ReadOnly = false;

        /// <summary>
        /// List of Field-Names for all our properties which have a set value
        /// </summary>
        //public HashSet<AtomicName<ECssPropertyID>> SetProperties { get; private set; } = new HashSet<AtomicName<ECssPropertyID>>();
        public readonly FlagCollection<ECssPropertyID> SetProperties = new FlagCollection<ECssPropertyID>(MAX_PROPERTY_ID_INDEX+1);

        private List<ICssProperty> CssProperties = null;
        //private ConcurrentDictionary<AtomicName<ECssPropertyID>, ICssProperty> CssPropertyMap = null;

        /// <summary>
        /// Sequence tracker for <see cref="CssComputedStyle"/>s
        /// </summary>
        private static ulong SEQ = 0;
        /// <summary>
        /// Id number for this <see cref="CssComputedStyle"/>, unique among all <see cref="CssComputedStyle"/>s
        /// </summary>
        public readonly ulong ID = ++SEQ;

        /// <summary>
        /// Name of the rule block, for identification
        /// </summary>
        public string Name { get { return _name ?? defaultName; } set { _name = value; } }

        private string _name = null;
        private string defaultName
        {
            get
            {
                return (string)$"{nameof(CssComputedStyle)} - {ID}".Concat(string.IsNullOrEmpty(Selector.ToString()) ? "" : Selector.ToString());
            }
        }
        /// <summary>
        /// The CSS selector for this rule block
        /// </summary>
        public readonly CssSelector Selector = null;

        public readonly EPropertySetOrigin Origin;
        #endregion

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name)) return Name;
            return base.ToString();
        }

        #region CSS Properties
        #region Display
        public EnumProperty<EDisplayMode> Display => (EnumProperty<EDisplayMode>)Get(ECssPropertyID.Display);

        /// <summary>
        /// Current BoxSizing mode for this element.
        /// (Defaults to <see cref="EBoxSizingMode.BorderBox"/>)
        /// </summary>
        public EnumProperty<EBoxSizingMode> BoxSizing => (EnumProperty<EBoxSizingMode>)Get(ECssPropertyID.BoxSizing);

        public EnumProperty<EBoxPositioning> Positioning => (EnumProperty<EBoxPositioning>)Get(ECssPropertyID.Positioning);
        #endregion

        #region Flow
        public EnumProperty<EDirection> Direction => (EnumProperty<EDirection>)Get(ECssPropertyID.Direction);
        public EnumProperty<EWritingMode> WritingMode => (EnumProperty<EWritingMode>)Get(ECssPropertyID.WritingMode);
        #endregion

        #region Replaced Element Properties
        public EnumProperty<EObjectFit> ObjectFit => (EnumProperty<EObjectFit>)Get(ECssPropertyID.ObjectFit);
        public PositionProperty ObjectPosition => (PositionProperty)Get(ECssPropertyID.ObjectPosition);
        // public IntProperty ObjectPosition_X => (IntProperty)Get(ECssPropertyID.ObjectPositionX);
        // public IntProperty ObjectPosition_Y => (IntProperty)Get(ECssPropertyID.ObjectPositionY);
        #endregion

        #region Scroll Behavior
        public EnumProperty<EScrollBehavior> ScrollBehavior => (EnumProperty<EScrollBehavior>)Get(ECssPropertyID.ScrollBehavior);
        public EnumProperty<EOverflowMode> Overflow_X => (EnumProperty<EOverflowMode>)Get(ECssPropertyID.OverflowX);
        public EnumProperty<EOverflowMode> Overflow_Y => (EnumProperty<EOverflowMode>)Get(ECssPropertyID.OverflowY);
        #endregion

        #region Size
        public IntProperty Width => (IntProperty)Get(ECssPropertyID.Width);
        public IntProperty Height => (IntProperty)Get(ECssPropertyID.Height);
        #endregion

        #region Borders
        public EnumProperty<EBorderStyle> Border_Top_Style => (EnumProperty<EBorderStyle>)Get(ECssPropertyID.BorderTopStyle);
        public EnumProperty<EBorderStyle> Border_Right_Style => (EnumProperty<EBorderStyle>)Get(ECssPropertyID.BorderRightStyle);
        public EnumProperty<EBorderStyle> Border_Bottom_Style => (EnumProperty<EBorderStyle>)Get(ECssPropertyID.BorderBottomStyle);
        public EnumProperty<EBorderStyle> Border_Left_Style => (EnumProperty<EBorderStyle>)Get(ECssPropertyID.BorderLeftStyle);

        public IntProperty Border_Top_Width => (IntProperty)Get(ECssPropertyID.BorderTopWidth);
        public IntProperty Border_Right_Width => (IntProperty)Get(ECssPropertyID.BorderRightWidth);
        public IntProperty Border_Bottom_Width => (IntProperty)Get(ECssPropertyID.BorderBottomWidth);
        public IntProperty Border_Left_Width => (IntProperty)Get(ECssPropertyID.BorderLeftWidth);
        #endregion

        #region Min Size
        /// <summary>
        /// The minimum Width of an elements content-area
        /// </summary>
        public IntProperty Min_Width => (IntProperty)Get(ECssPropertyID.MinWidth);
        /// <summary>
        /// The minimum Height of an elements content-area
        /// </summary>
        public IntProperty Min_Height => (IntProperty)Get(ECssPropertyID.MinHeight);
        #endregion

        #region Max Size
        /// <summary>
        /// The maximum Width of an elements content-area
        /// </summary>
        public NullableIntProperty Max_Width => (NullableIntProperty)Get(ECssPropertyID.MaxWidth);
        /// <summary>
        /// The maximum Height of an elements content-area
        /// </summary>
        public NullableIntProperty Max_Height => (NullableIntProperty)Get(ECssPropertyID.MaxHeight);
        #endregion

        #region Position
        /// <summary>
        /// Points to <see cref="Left"/>
        /// Distance between the elements Left outter edge and the matching edge of its containing block.
        /// </summary>
        public IntProperty X => (IntProperty)Get(ECssPropertyID.Left);
        /// <summary>
        /// Points to <see cref="Top"/>
        /// Distance between the elements Top outter edge and the matching edge of its containing block.
        /// </summary>
        public IntProperty Y => (IntProperty)Get(ECssPropertyID.Top);


        /// <summary>
        /// Distance between the elements Top outter edge and the matching edge of its containing block.
        /// </summary>
        public IntProperty Top => (IntProperty)Get(ECssPropertyID.Top);
        /// <summary>
        /// Distance between the elements Right outter edge and the matching edge of its containing block.
        /// </summary>
        public IntProperty Right => (IntProperty)Get(ECssPropertyID.Right);
        /// <summary>
        /// Distance between the elements Bottom outter edge and the matching edge of its containing block.
        /// </summary>
        public IntProperty Bottom => (IntProperty)Get(ECssPropertyID.Bottom);
        /// <summary>
        /// Distance between the elements Left outter edge and the matching edge of its containing block.
        /// </summary>
        public IntProperty Left => (IntProperty)Get(ECssPropertyID.Left);
        #endregion

        #region Padding
        /// <summary>
        /// Distance between this elements Top border and its content (in pixels)
        /// <para>Clears an area around the content. The padding can be thought of as extending the content area in that the controls background occupys it</para>
        /// </summary>
        public IntProperty Padding_Top => (IntProperty)Get(ECssPropertyID.PaddingTop);
        /// <summary>
        /// Distance between this elements Right border and its content (in pixels)
        /// <para>Clears an area around the content. The padding can be thought of as extending the content area in that the controls background occupys it</para>
        /// </summary>
        public IntProperty Padding_Right => (IntProperty)Get(ECssPropertyID.PaddingRight);
        /// <summary>
        /// Distance between this elements Bottom border and its content (in pixels)
        /// <para>Clears an area around the content. The padding can be thought of as extending the content area in that the controls background occupys it</para>
        /// </summary>
        public IntProperty Padding_Bottom => (IntProperty)Get(ECssPropertyID.PaddingBottom);
        /// <summary>
        /// Distance between this elements Left border and its content (in pixels)
        /// <para>Clears an area around the content. The padding can be thought of as extending the content area in that the controls background occupys it</para>
        /// </summary>
        public IntProperty Padding_Left => (IntProperty)Get(ECssPropertyID.PaddingLeft);
        #endregion

        #region Margins
        /// <summary>
        /// Distance between the elements Top edge and Top border (in pixels)
        /// <para>Clears an area outside the border. The margin is transparent</para>
        /// </summary>
        public IntProperty Margin_Top => (IntProperty)Get(ECssPropertyID.MarginTop);
        /// <summary>
        /// Distance between the elements Right edge and Right border (in pixels)
        /// <para>Clears an area outside the border. The margin is transparent</para>
        /// </summary>
        public IntProperty Margin_Right => (IntProperty)Get(ECssPropertyID.MarginRight);
        /// <summary>
        /// Distance between the elements Bottom edge and Bottom border (in pixels)
        /// <para>Clears an area outside the border. The margin is transparent</para>
        /// </summary>
        public IntProperty Margin_Bottom => (IntProperty)Get(ECssPropertyID.MarginBottom);
        /// <summary>
        /// Distance between the elements Left edge and Left border (in pixels)
        /// <para>Clears an area outside the border. The margin is transparent</para>
        /// </summary>
        public IntProperty Margin_Left => (IntProperty)Get(ECssPropertyID.MarginLeft);
        #endregion

        #region Text
        public EnumProperty<ETextAlign> TextAlign => (EnumProperty<ETextAlign>)Get(ECssPropertyID.TextAlign);
        #endregion

        #region Font
        public NumberProperty DpiX => (NumberProperty)Get(ECssPropertyID.DpiX);
        public NumberProperty DpiY => (NumberProperty)Get(ECssPropertyID.DpiY);

        public IntProperty FontWeight => (IntProperty)Get(ECssPropertyID.FontWeight);
        public EnumProperty<EFontStyle> FontStyle => (EnumProperty<EFontStyle>)Get(ECssPropertyID.FontStyle);
        public NumberProperty FontSize => (NumberProperty)Get(ECssPropertyID.FontSize);
        public MultiStringProperty FontFamily => (MultiStringProperty)Get(ECssPropertyID.FontFamily);

        public ColorProperty Color => (ColorProperty)Get(ECssPropertyID.Color);
        #endregion

        #region Lines
        /// <summary>
        /// 'line-height' specifies the minimal height of line boxes within the element.
        /// </summary>
        public IntProperty LineHeight => (IntProperty)Get(ECssPropertyID.LineHeight);
        #endregion

        #region Opacity
        public NumberProperty Opacity => (NumberProperty)Get(ECssPropertyID.Opacity);
        #endregion

        #region Transforms
        public TransformListProperty Transform => (TransformListProperty)Get(ECssPropertyID.Transform);
        #endregion
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new set of element styling properties
        /// </summary>
        /// <param name="ReadOnly">If TRUE then none of this instances property values may be set directly.</param>
        public CssComputedStyle(string Name, ICssElement Owner, bool ReadOnly = false) : this(Name, null, Owner, ReadOnly)
        {
        }

        /// <summary>
        /// Creates a new set of element styling properties
        /// </summary>
        /// <param name="ReadOnly">If TRUE then none of this instances property values may be set directly.</param>
        public CssComputedStyle(CssSelector Selector, ICssElement Owner, bool ReadOnly = false) : this(null, Selector, Owner, ReadOnly)
        {
        }

        /// <summary>
        /// Creates a new set of element styling properties
        /// </summary>
        /// <param name="ReadOnly">If TRUE then none of this instances property values may be set directly.</param>
        /// <param name="Unset">If TRUE then property values will all be set to <see cref="CssValue.Null"/>.</param>
        public CssComputedStyle(CssSelector Selector, ICssElement Owner, bool ReadOnly = false, bool Unset = false) : this(null, Selector, Owner, ReadOnly, Unset)
        {
        }

        /// <summary>
        /// Creates a new set of element styling properties
        /// </summary>
        /// <param name="ReadOnly">If TRUE then none of this instances property values may be set directly.</param>
        /// <param name="Unset">If TRUE then property values will all be set to <see cref="CssValue.Null"/>.</param>
        public CssComputedStyle(string Name, CssSelector Selector, ICssElement Owner, bool ReadOnly = false, bool Unset = false, EPropertySetOrigin Origin = EPropertySetOrigin.Author)
        {
            this.Name = Name;
            this.Selector = Selector;
            this.Origin = Origin;
            this.ReadOnly = ReadOnly;
            var selfRef = new WeakReference<CssComputedStyle>(this);

            CssProperties = new List<ICssProperty>(MAX_PROPERTY_ID_INDEX+1);

            CssProperties[(int)ECssPropertyID.Top] = new IntProperty(ECssPropertyID.Top, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.Right] = new IntProperty(ECssPropertyID.Right, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.Bottom] = new IntProperty(ECssPropertyID.Bottom, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.Left] = new IntProperty(ECssPropertyID.Left, Owner, selfRef, this.ReadOnly);

            CssProperties[(int)ECssPropertyID.Direction] = new EnumProperty<EDirection>(ECssPropertyID.Direction, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.WritingMode] = new EnumProperty<EWritingMode>(ECssPropertyID.WritingMode, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.Display] = new EnumProperty<EDisplayMode>(ECssPropertyID.Display, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.BoxSizing] = new EnumProperty<EBoxSizingMode>(ECssPropertyID.BoxSizing, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.Positioning] = new EnumProperty<EBoxPositioning>(ECssPropertyID.Positioning, Owner, selfRef, this.ReadOnly);
            
            CssProperties[(int)ECssPropertyID.ScrollBehavior] = new EnumProperty<EScrollBehavior>(ECssPropertyID.ScrollBehavior, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.OverflowX] = new EnumProperty<EOverflowMode>(ECssPropertyID.OverflowX, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.OverflowY] = new EnumProperty<EOverflowMode>(ECssPropertyID.OverflowY, Owner, selfRef, this.ReadOnly);

            CssProperties[(int)ECssPropertyID.ObjectFit] = new EnumProperty<EObjectFit>(ECssPropertyID.ObjectFit, Owner, selfRef, this.ReadOnly);

            CssProperties[(int)ECssPropertyID.ObjectPosition] = new PositionProperty(ECssPropertyID.ObjectPosition, Owner, selfRef, this.ReadOnly);
            // CssProperties[(int)ECssPropertyID.ObjectPositionX] = new IntProperty(ECssPropertyID.ObjectPositionX, Owner, selfRef, this.ReadOnly);
            // CssProperties[(int)ECssPropertyID.ObjectPositionY] = new IntProperty(ECssPropertyID.ObjectPositionY, Owner, selfRef, this.ReadOnly);

            CssProperties[(int)ECssPropertyID.Width] = new IntProperty(ECssPropertyID.Width, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.Height] = new IntProperty(ECssPropertyID.Height, Owner, selfRef, this.ReadOnly);

            CssProperties[(int)ECssPropertyID.MinWidth] = new IntProperty(ECssPropertyID.MinWidth, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.MinHeight] = new IntProperty(ECssPropertyID.MinHeight, Owner, selfRef, this.ReadOnly);

            CssProperties[(int)ECssPropertyID.MaxWidth] = new NullableIntProperty(ECssPropertyID.MaxWidth, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.MaxHeight] = new NullableIntProperty(ECssPropertyID.MaxHeight, Owner, selfRef, this.ReadOnly);

            CssProperties[(int)ECssPropertyID.MarginTop] = new IntProperty(ECssPropertyID.MarginTop, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.MarginRight] = new IntProperty(ECssPropertyID.MarginRight, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.MarginBottom] = new IntProperty(ECssPropertyID.MarginBottom, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.MarginLeft] = new IntProperty(ECssPropertyID.MarginLeft, Owner, selfRef, this.ReadOnly);

            CssProperties[(int)ECssPropertyID.PaddingTop] = new IntProperty(ECssPropertyID.PaddingTop, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.PaddingRight] = new IntProperty(ECssPropertyID.PaddingRight, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.PaddingBottom] = new IntProperty(ECssPropertyID.PaddingBottom, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.PaddingLeft] = new IntProperty(ECssPropertyID.PaddingLeft, Owner, selfRef, this.ReadOnly);
            
            CssProperties[(int)ECssPropertyID.BorderTopStyle] = new EnumProperty<EBorderStyle>(ECssPropertyID.BorderTopStyle, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.BorderRightStyle] = new EnumProperty<EBorderStyle>(ECssPropertyID.BorderRightStyle, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.BorderBottomStyle] = new EnumProperty<EBorderStyle>(ECssPropertyID.BorderBottomStyle, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.BorderLeftStyle] = new EnumProperty<EBorderStyle>(ECssPropertyID.BorderLeftStyle, Owner, selfRef, this.ReadOnly);

            CssProperties[(int)ECssPropertyID.BorderTopWidth] = new IntProperty(ECssPropertyID.BorderTopWidth, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.BorderRightWidth] = new IntProperty(ECssPropertyID.BorderRightWidth, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.BorderBottomWidth] = new IntProperty(ECssPropertyID.BorderBottomWidth, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.BorderLeftWidth] = new IntProperty(ECssPropertyID.BorderLeftWidth, Owner, selfRef, this.ReadOnly);
            
            CssProperties[(int)ECssPropertyID.TextAlign] = new EnumProperty<ETextAlign>(ECssPropertyID.TextAlign, Owner, selfRef, this.ReadOnly);

            CssProperties[(int)ECssPropertyID.DpiX] = new NumberProperty(ECssPropertyID.DpiX, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.DpiY] = new NumberProperty(ECssPropertyID.DpiY, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.FontWeight] = new IntProperty(ECssPropertyID.FontWeight, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.FontStyle] = new EnumProperty<EFontStyle>(ECssPropertyID.FontStyle, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.FontSize] = new NumberProperty(ECssPropertyID.FontSize, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.FontFamily] = new MultiStringProperty(ECssPropertyID.FontFamily, Owner, selfRef, this.ReadOnly);

            CssProperties[(int)ECssPropertyID.Color] = new ColorProperty(ECssPropertyID.Color, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.Opacity] = new NumberProperty(ECssPropertyID.Opacity, Owner, selfRef, this.ReadOnly);

            CssProperties[(int)ECssPropertyID.LineHeight] = new IntProperty(ECssPropertyID.LineHeight, Owner, selfRef, this.ReadOnly);
            CssProperties[(int)ECssPropertyID.Transform] = new TransformListProperty(ECssPropertyID.Transform, Owner, selfRef, this.ReadOnly);
            
            /*CssPropertyMap = new ConcurrentDictionary<AtomicName<ECssPropertyID>, ICssProperty>(3, CssProperties.Count);
            for (int i = 0; i < CssProperties.Count; i++)
            {
                ICssProperty p = CssProperties[i];
                p.Selector = Selector;
                p.onValueChange += Property_onChanged;

                bool success = CssPropertyMap.TryAdd(p.CssName, p);
                if (!success)
                    throw new Exception($"Failed to fully form {nameof(CssPropertyMap)} for {nameof(CssPropertySet)}. Failed on member: '{p.CssName}'");
            }*/

        }
        #endregion

        #region Events
        /// <summary>
        /// A property which affects the elements block changed
        /// </summary>
        public event Action<EPropertyStage, ICssProperty, EPropertyDirtFlags, StackTrace> Property_Changed;
        #endregion

        #region Change Handlers
        const int STACK_FRAME_OFFSET = 3;

        /// <summary>
        /// The value of pre-cascade property has changed
        /// </summary>
        /// <param name="Stage"></param>
        /// <param name="Property"></param>
        private void Property_onChanged(EPropertyStage Stage, ICssProperty Property)
        {
            SetProperties.SetFlag(Property.CssName.Value, Property.HasValue);
            /*if (!Property.HasValue) SetProperties.Remove(Property.CssName);
            else SetProperties.Add(Property.CssName);*/

            if (Property.CssName is null) throw new Exception($"Cannot fire onChange events for unnamed property! (Name: {Property.CssName}");
            StyleDefinition def = CssDefinitions.StyleDefinitions[Property.CssName];
            if (def is null) throw new Exception($"Cannot find a definition for Css property: \"{Property.CssName}\"");

            EPropertyDirtFlags Flags = def.Flags;
            StackTrace Stack = null;
#if DEBUG
            //stack = new StackTrace(STACK_FRAME_OFFSET, true);
#endif
            Property_Changed?.Invoke(Stage, Property, Flags, Stack);
        }

        #endregion

        #region Getters
        public ICssProperty this[AtomicName<ECssPropertyID> CssName] => Get(CssName);

        internal ICssProperty Get_ByIndex(int index) => CssProperties[index];

        /// <summary>
        /// Finds the property within this style set that matches the given property
        /// </summary>
        /// <param name="Property"></param>
        /// <returns></returns>
        internal ICssProperty Get(ICssProperty Property)
        {
            if (Property is null) throw new ArgumentNullException(nameof(Property));

            Contract.EndContractBlock();

            return Get(Property.CssName);
        }

        internal ICssProperty Get(AtomicName<ECssPropertyID> CssName)
        {
            if (CssName is null) throw new ArgumentNullException(nameof(CssName));
            if (CssName.Value < 0) throw new ArgumentOutOfRangeException($"Invalid CSS property ID (negative value): {CssName}");

            Contract.EndContractBlock();

            if (CssName.Value >= CssProperties.Count) return null;

            return CssProperties[CssName.Value];

            /*
             if (CssPropertyMap.TryGetValue(CssName, out ICssProperty prop))
                return prop;

            throw new KeyNotFoundException($"Unable find style property: {CssName}");
            */
        }


        /// <summary>
        /// Returns all of the properties
        /// </summary>
        internal IEnumerable<ICssProperty> GetAll() => CssProperties.AsEnumerable();

        /// <summary>
        /// Returns all of the properties matching a given predicate
        /// </summary>
        internal IEnumerable<ICssProperty> GetAll(Func<ICssProperty, bool> Predicate) => CssProperties.Where(Predicate).AsEnumerable();

        /// <summary>
        /// Returns all of the Css properties that have a value assigned to them
        /// </summary>
        /// <returns></returns>
        internal IList<ICssProperty> Get_Set_Properties()
        {
            if (SetProperties.IsEmpty()) return Array.Empty<ICssProperty>();
            List<ICssProperty> List = new List<ICssProperty>();
            /* XXX: NEED AN ENUMERATOR FOR FlagCollection */
            foreach (var name in SetProperties)
            {
                List.Add(Get(name));
            }

            return List;
        }
        #endregion

        #region Setters
        internal void Set(ICssProperty Property, ICssProperty Value)
        {
            if (Property is null) throw new ArgumentNullException(nameof(Property));

            Contract.EndContractBlock();

            Set(Property.CssName, Value);
        }

        internal void Set(AtomicName<ECssPropertyID> CssName, ICssProperty Value)
        {
            if (CssName is null) throw new ArgumentNullException(nameof(CssName));
            if (CssName.Value < 0) throw new ArgumentOutOfRangeException($"Invalid CSS property ID (negative value): {CssName}");

            Contract.EndContractBlock();

            if (CssName.Value >= CssProperties.Count)
            {// Make more room
                int diff = CssName.Value - CssProperties.Count;
                for (int i = 0; i < diff; i++)
                {
                    CssProperties.Add(null);
                }
            }

            CssProperties[CssName.Value] = Value;
            return;

            /*
            if (CssPropertyMap.TryGetValue(CssName, out ICssProperty Property))
            {
                Property.Overwrite(Value);
                return;
            }

            throw new KeyNotFoundException($"Unable find style property: {CssName}");
            */
        }
        #endregion
        
        #region Padding Helpers
        public void Set_Padding(int? horizontal, int? vertical)
        {
            /*Padding_Top.Set(vertical);
            Padding_Right.Set(horizontal);
            Padding_Bottom.Set(vertical);
            Padding_Left.Set(horizontal);*/

            Padding_Top.Set(!vertical.HasValue ? null : CssValue.From(vertical.Value, ECssUnit.PX));
            Padding_Right.Set(!horizontal.HasValue ? null : CssValue.From(horizontal.Value, ECssUnit.PX));
            Padding_Bottom.Set(!vertical.HasValue ? null : CssValue.From(vertical.Value, ECssUnit.PX));
            Padding_Left.Set(!horizontal.HasValue ? null : CssValue.From(horizontal.Value, ECssUnit.PX));
        }
        public void Set_Padding(CssValue horizontal, CssValue vertical)
        {
            Padding_Top.Set(vertical);
            Padding_Right.Set(horizontal);
            Padding_Bottom.Set(vertical);
            Padding_Left.Set(horizontal);
        }
        public void Set_Padding(int? top, int? right, int? bottom, int? left)
        {
            /*Padding_Top.Set(top);
            Padding_Right.Set(right);
            Padding_Bottom.Set(bottom);
            Padding_Left.Set(left);*/

            Padding_Top.Set(!top.HasValue ? null : CssValue.From(top.Value, ECssUnit.PX));
            Padding_Right.Set(!right.HasValue ? null : CssValue.From(right.Value, ECssUnit.PX));
            Padding_Bottom.Set(!bottom.HasValue ? null : CssValue.From(bottom.Value, ECssUnit.PX));
            Padding_Left.Set(!left.HasValue ? null : CssValue.From(left.Value, ECssUnit.PX));
        }
        public void Set_Padding(CssValue top, CssValue right, CssValue bottom, CssValue left)
        {
            Padding_Top.Set(top);
            Padding_Right.Set(right);
            Padding_Bottom.Set(bottom);
            Padding_Left.Set(left);
        }
        #endregion


        #region Margin Helpers
        public void Set_Margin(int? horizontal, int? vertical)
        {
            /*Margin_Top.Set(vertical);
            Margin_Right.Set(horizontal);
            Margin_Bottom.Set(vertical);
            Margin_Left.Set(horizontal);*/

            Margin_Top.Set(!vertical.HasValue ? null : CssValue.From(vertical.Value, ECssUnit.PX));
            Margin_Right.Set(!horizontal.HasValue ? null : CssValue.From(horizontal.Value, ECssUnit.PX));
            Margin_Bottom.Set(!vertical.HasValue ? null : CssValue.From(vertical.Value, ECssUnit.PX));
            Margin_Left.Set(!horizontal.HasValue ? null : CssValue.From(horizontal.Value, ECssUnit.PX));
        }
        public void Set_Margin(CssValue horizontal, CssValue vertical)
        {
            Margin_Top.Set(vertical);
            Margin_Right.Set(horizontal);
            Margin_Bottom.Set(vertical);
            Margin_Left.Set(horizontal);
        }
        public void Set_Margin(int? top, int? right, int? bottom, int? left)
        {
            /*Margin_Top.Set(top);
            Margin_Right.Set(right);
            Margin_Bottom.Set(bottom);
            Margin_Left.Set(left);*/

            Margin_Top.Set(!top.HasValue ? null : CssValue.From(top.Value, ECssUnit.PX));
            Margin_Right.Set(!right.HasValue ? null : CssValue.From(right.Value, ECssUnit.PX));
            Margin_Bottom.Set(!bottom.HasValue ? null : CssValue.From(bottom.Value, ECssUnit.PX));
            Margin_Left.Set(!left.HasValue ? null : CssValue.From(left.Value, ECssUnit.PX));
        }
        public void Set_Margin(CssValue top, CssValue right, CssValue bottom, CssValue left)
        {
            Margin_Top.Set(top);
            Margin_Right.Set(right);
            Margin_Bottom.Set(bottom);
            Margin_Left.Set(left);
        }
        public void Set_Margin_Implicit(int? horizontal, int? vertical)
        {
            /*Margin_Top.Set(vertical);
            Margin_Right.Set(horizontal);
            Margin_Bottom.Set(vertical);
            Margin_Left.Set(horizontal);*/

            Margin_Top.Set(!vertical.HasValue ? null : CssValue.From(vertical.Value, ECssUnit.PX));
            Margin_Right.Set(!horizontal.HasValue ? null : CssValue.From(horizontal.Value, ECssUnit.PX));
            Margin_Bottom.Set(!vertical.HasValue ? null : CssValue.From(vertical.Value, ECssUnit.PX));
            Margin_Left.Set(!horizontal.HasValue ? null : CssValue.From(horizontal.Value, ECssUnit.PX));
        }
        #endregion


        #region Position Helpers
        public void Set_Position(int? X, int? Y)
        {
            /*Left.Set(X);
            Top.Set(Y);*/

            Left.Set(!X.HasValue ? null : CssValue.From(X.Value, ECssUnit.PX));
            Top.Set(!Y.HasValue ? null : CssValue.From(Y.Value, ECssUnit.PX));
        }
        public void Set_Position(CssValue x, CssValue y)
        {
            Left.Set(x);
            Top.Set(y);
        }
        internal void Set_Position_Implicit(CssValue x, CssValue y)
        {
            Left.Set(x);
            Top.Set(y);
        }
        #endregion


        #region Size Helpers
        public void Set_Size(int? Width, int? Height)
        {
            /*this.Width.Set(Width);
            this.Height.Set(Height);*/

            this.Width.Set(!Width.HasValue ? null : CssValue.From(Width.Value, ECssUnit.PX));
            this.Height.Set(!Height.HasValue ? null : CssValue.From(Height.Value, ECssUnit.PX));
        }
        public void Set_Size(CssValue width, CssValue height)
        {
            Width.Set(width);
            Height.Set(height);
        }
        #endregion


        #region SizeMin Helpers
        public void Set_SizeMin(int? Width, int? Height)
        {
            /*Min_Width.Set(width);
            Min_Height.Set(height);*/
            Min_Width.Set(!Width.HasValue ? null : CssValue.From(Width.Value, ECssUnit.PX));
            Min_Height.Set(!Height.HasValue ? null : CssValue.From(Height.Value, ECssUnit.PX));
        }
        public void Set_SizeMin(CssValue width, CssValue height)
        {
            Min_Width.Set(width);
            Min_Height.Set(height);
        }
        #endregion


        #region SizeMax Helpers
        public void Set_SizeMax(int? Width, int? Height)
        {
            /*Max_Width.Set(width);
            Max_Height.Set(height);*/
            Min_Width.Set(!Width.HasValue ? null : CssValue.From(Width.Value, ECssUnit.PX));
            Min_Height.Set(!Height.HasValue ? null : CssValue.From(Height.Value, ECssUnit.PX));
        }
        public void Set_SizeMax(CssValue width, CssValue height)
        {
            Max_Width.Set(width);
            Max_Height.Set(height);
        }
        #endregion

    }
}
