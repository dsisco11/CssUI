using KeyPic.Benchmarking;
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
    /// <summary>
    /// Holds all of the styling propertys that a UI element uses
    /// Each different styling state of an element gets it's own instance of this class which 
    /// are then all cascaded together to determine the current value when the elements active state changes.
    /// </summary>
    public class StyleRuleData
    {
        #region Property List
        /// <summary>
        /// List of FieldInfo for all <see cref="IStyleProperty"/> variables
        /// </summary>
        public static readonly List<FieldInfo> PropertyList;
        /// <summary>
        /// Map of all <see cref="IStyleProperty"/> variable names to their FieldInfo
        /// </summary>
        public static readonly Dictionary<AtomicString, FieldInfo> PropertyMap;

        static StyleRuleData()
        {
            PropertyList = typeof(StyleRuleData).GetFields().Where(o => (o.Attributes.HasFlag(FieldAttributes.InitOnly) && typeof(IStyleProperty).IsAssignableFrom(o.FieldType))).ToList();

            PropertyMap = new Dictionary<AtomicString, FieldInfo>();
            foreach(FieldInfo field in PropertyList)
            {
                PropertyMap.Add(new AtomicString(field.Name), field);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// If True then none of this instances property values can be altered by external code.
        /// </summary>
        public readonly bool Locked = false;
        
        /// <summary>
        /// List of Field-Names for all our properties which have a set value
        /// </summary>
        private HashSet<AtomicString> SetProperties = new HashSet<AtomicString>();

        /// <summary>
        /// Optional name of the rule block, for debug visualization
        /// </summary>
        private readonly string Name = null;
        /// <summary>
        /// The CSS selector for this rule block
        /// </summary>
        public readonly CssSelector Selector = null;
        #endregion

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name)) return Name;
            return base.ToString();
        }

        #region CSS Properties
        #region Display
        public readonly EnumProperty<EDisplayMode> Display;

        /// <summary>
        /// Current BoxSizing mode for this element.
        /// (Defaults to <see cref="EBoxSizingMode.BORDER"/>)
        /// </summary>
        public readonly EnumProperty<EBoxSizingMode> BoxSizing;

        public readonly EnumProperty<EPositioning> Positioning;
        #endregion

        #region Replaced Element Properties
        public readonly EnumProperty<EObjectFit> ObjectFit;
        public readonly IntProperty ObjectPosition_X;
        public readonly IntProperty ObjectPosition_Y;
        public readonly IntProperty Intrinsic_Width;
        public readonly IntProperty Intrinsic_Height;
        public readonly NumberProperty Intrinsic_Ratio;
        #endregion

        #region Overflow
        public readonly EnumProperty<EOverflowMode> Overflow_X;
        public readonly EnumProperty<EOverflowMode> Overflow_Y;
        #endregion

        #region Size
        public readonly IntProperty Width;
        public readonly IntProperty Height;

        public readonly IntProperty Content_Width;
        public readonly IntProperty Content_Height;
        #endregion

        #region Borders
        public readonly EnumProperty<EBorderStyle> Border_Top_Style;
        public readonly EnumProperty<EBorderStyle> Border_Right_Style;
        public readonly EnumProperty<EBorderStyle> Border_Bottom_Style;
        public readonly EnumProperty<EBorderStyle> Border_Left_Style;

        public readonly IntProperty Border_Top_Width;
        public readonly IntProperty Border_Right_Width;
        public readonly IntProperty Border_Bottom_Width;
        public readonly IntProperty Border_Left_Width;
        #endregion

        #region Min Size
        /// <summary>
        /// The minimum Width of an elements content-area
        /// </summary>
        public readonly IntProperty Min_Width;
        /// <summary>
        /// The minimum Height of an elements content-area
        /// </summary>
        public readonly IntProperty Min_Height;
        #endregion

        #region Max Size
        /// <summary>
        /// The maximum Width of an elements content-area
        /// </summary>
        public readonly IntProperty Max_Width;
        /// <summary>
        /// The maximum Height of an elements content-area
        /// </summary>
        public readonly IntProperty Max_Height;
        #endregion

        #region Position
        /// <summary>
        /// Distance between the elements Left outter edge and the matching edge of its containing block.
        /// </summary>
        public IntProperty X { get { return this.Left; } }
        /// <summary>
        /// Distance between the elements Top outter edge and the matching edge of its containing block.
        /// </summary>
        public IntProperty Y { get { return this.Top; } }


        /// <summary>
        /// Distance between the elements Top outter edge and the matching edge of its containing block.
        /// </summary>
        public readonly IntProperty Top;
        /// <summary>
        /// Distance between the elements Right outter edge and the matching edge of its containing block.
        /// </summary>
        public readonly IntProperty Right;
        /// <summary>
        /// Distance between the elements Bottom outter edge and the matching edge of its containing block.
        /// </summary>
        public readonly IntProperty Bottom;
        /// <summary>
        /// Distance between the elements Left outter edge and the matching edge of its containing block.
        /// </summary>
        public readonly IntProperty Left;
        #endregion

        #region Margins
        /// <summary>
        /// Distance between the elements Top edge and Top border (in pixels)
        /// <para>Clears an area outside the border. The margin is transparent</para>
        /// </summary>
        public readonly IntProperty Margin_Top;
        /// <summary>
        /// Distance between the elements Right edge and Right border (in pixels)
        /// <para>Clears an area outside the border. The margin is transparent</para>
        /// </summary>
        public readonly IntProperty Margin_Right;
        /// <summary>
        /// Distance between the elements Bottom edge and Bottom border (in pixels)
        /// <para>Clears an area outside the border. The margin is transparent</para>
        /// </summary>
        public readonly IntProperty Margin_Bottom;
        /// <summary>
        /// Distance between the elements Left edge and Left border (in pixels)
        /// <para>Clears an area outside the border. The margin is transparent</para>
        /// </summary>
        public readonly IntProperty Margin_Left;
        #endregion

        #region Padding
        /// <summary>
        /// Distance between this elements Top border and its content (in pixels)
        /// <para>Clears an area around the content. The padding can be thought of as extending the content area in that the controls background occupys it</para>
        /// </summary>
        public readonly IntProperty Padding_Top;
        /// <summary>
        /// Distance between this elements Right border and its content (in pixels)
        /// <para>Clears an area around the content. The padding can be thought of as extending the content area in that the controls background occupys it</para>
        /// </summary>
        public readonly IntProperty Padding_Right;
        /// <summary>
        /// Distance between this elements Bottom border and its content (in pixels)
        /// <para>Clears an area around the content. The padding can be thought of as extending the content area in that the controls background occupys it</para>
        /// </summary>
        public readonly IntProperty Padding_Bottom;
        /// <summary>
        /// Distance between this elements Left border and its content (in pixels)
        /// <para>Clears an area around the content. The padding can be thought of as extending the content area in that the controls background occupys it</para>
        /// </summary>
        public readonly IntProperty Padding_Left;
        #endregion

        #region Text
        public readonly EnumProperty<ETextAlign> TextAlign;
        #endregion

        #region Font
        public readonly IntProperty FontWeight;
        public readonly EnumProperty<EFontStyle> FontStyle;
        public readonly NumberProperty FontSize;
        public readonly StringProperty FontFamily;
        #endregion

        #region Lines
        /// <summary>
        /// 'line-height' specifies the minimal height of line boxes within the element.
        /// </summary>
        public IntProperty LineHeight;
        #endregion

        #region Opacity
        public readonly NumberProperty Opacity;
        #endregion

        #region Transforms
        public readonly TransformListProperty Transform;
        #endregion
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new set of element styling properties
        /// </summary>
        /// <param name="Locked">If TRUE then none of this instances property values may be set directly.</param>
        public StyleRuleData(string Name, uiElement Owner, bool Locked = false) : this(Name, null, Owner, Locked)
        {
        }

        /// <summary>
        /// Creates a new set of element styling properties
        /// </summary>
        /// <param name="Locked">If TRUE then none of this instances property values may be set directly.</param>
        public StyleRuleData(CssSelector Selector, uiElement Owner, bool Locked = false) : this(null, Selector, Owner, Locked)
        {
        }

        /// <summary>
        /// Creates a new set of element styling properties
        /// </summary>
        /// <param name="Locked">If TRUE then none of this instances property values may be set directly.</param>
        /// <param name="Unset">If TRUE then property values will all be set to <see cref="CSSValue.Null"/>.</param>
        public StyleRuleData(CssSelector Selector, uiElement Owner, bool Locked = false, bool Unset = false) : this(null, Selector, Owner, Locked, Unset)
        {
        }

        /// <summary>
        /// Creates a new set of element styling properties
        /// </summary>
        /// <param name="Locked">If TRUE then none of this instances property values may be set directly.</param>
        /// <param name="Unset">If TRUE then property values will all be set to <see cref="CSSValue.Null"/>.</param>
        public StyleRuleData(string Name, CssSelector Selector, uiElement Owner, bool Locked = false, bool Unset = false)
        {
            this.Name = Name;
            this.Selector = Selector;

            Top = new IntProperty("top", Owner, this.Locked, Unset, new PropertyOptions() { });
            Right = new IntProperty("right", Owner, this.Locked, Unset, new PropertyOptions() { });
            Bottom = new IntProperty("bottom", Owner, this.Locked, Unset, new PropertyOptions() { });
            Left = new IntProperty("left", Owner, this.Locked, Unset, new PropertyOptions() { });

            Display = new EnumProperty<EDisplayMode>("display", Owner, this.Locked, Unset, new PropertyOptions() { AllowInherited = false });
            BoxSizing = new EnumProperty<EBoxSizingMode>("box-sizing", Owner, this.Locked, Unset, new PropertyOptions() { });
            Positioning = new EnumProperty<EPositioning>("positioning", Owner, this.Locked, Unset, new PropertyOptions() { });
            Overflow_X = new EnumProperty<EOverflowMode>("overflow-x", Owner, this.Locked, Unset, new PropertyOptions() { });
            Overflow_Y = new EnumProperty<EOverflowMode>("overflow-y", Owner, this.Locked, Unset, new PropertyOptions() { });

            ObjectFit = new EnumProperty<EObjectFit>("object-fit", Owner, this.Locked, Unset, new PropertyOptions() { AllowInherited = false, AllowAuto = false });
            ObjectPosition_X = new IntProperty("object-position-x", Owner, this.Locked, Unset, new PropertyOptions() { AllowInherited = false });
            ObjectPosition_Y = new IntProperty("object-position-y", Owner, this.Locked, Unset, new PropertyOptions() { AllowInherited = false });
            Intrinsic_Width = new IntProperty("intrinsic-width", Owner, this.Locked, Unset, new PropertyOptions() { AllowInherited = false, AllowAuto = false });
            Intrinsic_Height = new IntProperty("intrinsic-height", Owner, this.Locked, Unset, new PropertyOptions() { AllowInherited = false, AllowAuto = false });
            Intrinsic_Ratio = new NumberProperty("intrinsic-ratio", Owner, this.Locked, Unset, new PropertyOptions() { AllowInherited = false, AllowAuto = false });

            Width = new IntProperty("width", Owner, this.Locked, Unset, new PropertyOptions() { });
            Height = new IntProperty("height", Owner, this.Locked, Unset, new PropertyOptions() { });

            Content_Width = new IntProperty("content-width", Owner, this.Locked, Unset, new PropertyOptions() { AllowAuto = false, AllowPercentage = false, AllowInherited = false });
            Content_Height = new IntProperty("content-height", Owner, this.Locked, Unset, new PropertyOptions() { AllowAuto = false, AllowPercentage = false, AllowInherited = false });

            Min_Width = new IntProperty("min-width", Owner, this.Locked, Unset, new PropertyOptions() { });
            Min_Height = new IntProperty("min-height", Owner, this.Locked, Unset, new PropertyOptions() { });

            Max_Width = new IntProperty("max-width", Owner, this.Locked, Unset, new PropertyOptions() { });
            Max_Height = new IntProperty("max-height", Owner, this.Locked, Unset, new PropertyOptions() { });

            Margin_Top = new IntProperty("margin-top", Owner, this.Locked, Unset, new PropertyOptions() { });
            Margin_Right = new IntProperty("margin-right", Owner, this.Locked, Unset, new PropertyOptions() { });
            Margin_Bottom = new IntProperty("margin-bottom", Owner, this.Locked, Unset, new PropertyOptions() { });
            Margin_Left = new IntProperty("margin-left", Owner, this.Locked, Unset, new PropertyOptions() { });

            Padding_Top = new IntProperty("padding-top", Owner, this.Locked, Unset, new PropertyOptions() { });
            Padding_Right = new IntProperty("padding-right", Owner, this.Locked, Unset, new PropertyOptions() { });
            Padding_Bottom = new IntProperty("padding-bottom", Owner, this.Locked, Unset, new PropertyOptions() { });
            Padding_Left = new IntProperty("padding-left", Owner, this.Locked, Unset, new PropertyOptions() { });
            

            Border_Top_Style = new EnumProperty<EBorderStyle>("border-top-style", Owner, this.Locked, Unset, new PropertyOptions() { });
            Border_Right_Style = new EnumProperty<EBorderStyle>("border-right-style", Owner, this.Locked, Unset, new PropertyOptions() { });
            Border_Bottom_Style = new EnumProperty<EBorderStyle>("border-bottom-style", Owner, this.Locked, Unset, new PropertyOptions() { });
            Border_Left_Style = new EnumProperty<EBorderStyle>("border-left-style", Owner, this.Locked, Unset, new PropertyOptions() { });

            Border_Top_Width = new IntProperty("border-top-width", Owner, this.Locked, Unset, new PropertyOptions() { });
            Border_Right_Width = new IntProperty("border-right-width", Owner, this.Locked, Unset, new PropertyOptions() { });
            Border_Bottom_Width = new IntProperty("border-bottom-width", Owner, this.Locked, Unset, new PropertyOptions() { });
            Border_Left_Width = new IntProperty("border-left-width", Owner, this.Locked, Unset, new PropertyOptions() { });

            TextAlign = new EnumProperty<ETextAlign>("text-align", Owner, this.Locked, Unset, new PropertyOptions() { });

            FontWeight = new IntProperty("font-weight", Owner, this.Locked, Unset, new PropertyOptions() { AllowPercentage = false });
            FontStyle = new EnumProperty<EFontStyle>("font-style", Owner, this.Locked, Unset, new PropertyOptions() { });
            FontSize = new NumberProperty("font-size", Owner, this.Locked, Unset, new PropertyOptions() { });
            FontFamily = new StringProperty("font-family", Owner, this.Locked, Unset, new PropertyOptions() { });

            LineHeight = new IntProperty("line-height", Owner, this.Locked, Unset, new PropertyOptions() { });

            Opacity = new NumberProperty("opacity", Owner, this.Locked, Unset, new PropertyOptions() { AllowPercentage = false });

            Transform = new TransformListProperty("transform", Owner, this.Locked, Unset);

            // Name each of our properties
            foreach(FieldInfo Field in PropertyList)
            {
                var property = (IStyleProperty)Field.GetValue(this);
                property.FieldName = new AtomicString(Field.Name);
                property.Selector = Selector;
                property.onChanged += Property_onChanged;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// A property which affects the elements block changed
        /// </summary>
        public event Action<IStyleProperty, EPropertyFlags, StackTrace> Property_Changed;
        #endregion

        #region Change Handlers
        const int STACK_FRAME_OFFSET = 3;

        private void Property_onChanged(IStyleProperty property)
        {
            if (!property.HasValue) SetProperties.Remove(property.FieldName);
            else SetProperties.Add(property.FieldName);

            if (property.CssName == null) throw new Exception(string.Concat("Cannot fire onChange events for unnamed property! (FieldName: ", property.FieldName, ")"));
            var def = CssProperties.Definitions[property.CssName];
            if (def == null) throw new Exception(string.Concat("Cannot find a definition for Css property: \"", property.CssName, "\""));

            EPropertyFlags Flags = def.Flags;
            StackTrace stack = null;
#if DEBUG
            //stack = new StackTrace(STACK_FRAME_OFFSET, true);
#endif
            Property_Changed?.Invoke(property, Flags, stack);
        }
        
        #endregion

        #region Getters
        internal IStyleProperty Get(AtomicString FieldName)
        {
            FieldInfo Field = PropertyMap[FieldName];
            if (!typeof(IStyleProperty).IsAssignableFrom(Field.FieldType)) throw new Exception(string.Format("Unable find style property with the field name: {0}", FieldName));
            return (IStyleProperty)Field.GetValue(this);
        }

        internal IStyleProperty Get(IStyleProperty Property)
        {
            AtomicString FieldName = Property.FieldName;
            FieldInfo Field = PropertyMap[FieldName];
            if (!typeof(IStyleProperty).IsAssignableFrom(Field.FieldType)) throw new Exception(string.Format("Unable find style property with the field name: {0}", FieldName));
            return (IStyleProperty)Field.GetValue(this);
        }

        /// <summary>
        /// Returns all of the properties matching a given predicate
        /// </summary>
        internal IStyleProperty[] GetAll(Func<IStyleProperty, bool> Predicate)
        {
            List<IStyleProperty> Ret = new List<IStyleProperty>();
            for (int i = 0; i < PropertyList.Count; i++)
            {
                FieldInfo Field = PropertyList[i];
                IStyleProperty val = (IStyleProperty)Field.GetValue(this);
                if (Predicate(val))
                {
                    Ret.Add(val);
                }
            }

            return Ret.ToArray();
        }

        internal List<IStyleProperty> Get_Set_Properties()
        {
            List<IStyleProperty> List = new List<IStyleProperty>();
            foreach (AtomicString fieldName in SetProperties)
            {
                List.Add(Get(fieldName));
            }

            return List;
        }
        #endregion

        #region Setters
        internal void Set(IStyleProperty Property, IStyleProperty Value, AtomicString SourceState)
        {
            AtomicString FieldName = Property.FieldName;
            FieldInfo Field = typeof(StyleRuleData).GetField(FieldName.ToString());
            if (!typeof(IStyleProperty).IsAssignableFrom(Field.FieldType)) throw new Exception(string.Format("Unable find style property with the field name: {0}", FieldName));
            
            ((IStyleProperty)Field.GetValue(this)).Overwrite(Value);
        }
        #endregion

        #region Value Management
        // TODO: Don't do cascading/overwriting on ALL properties, just a list of the ones that we know are set...

        /// <summary>
        /// Overwrites the property values of this instance with those of any set property values from another instance.
        /// </summary>
        /// <param name="props"></param>
        internal void Cascade(StyleRuleData props)
        {
            //var tid = Timing.Start("Cascade");
            foreach(var name in props.SetProperties)
            {
                FieldInfo Field = PropertyMap[name];
                var val = (IStyleProperty)Field.GetValue(props);
                var mv = (IStyleProperty)Field.GetValue(this);

                mv.Cascade(val);
            }
            //Timing.Stop(tid);
        }

        /// <summary>
        /// Overwrites any differing property values
        /// </summary>
        /// <param name="props"></param>
        internal void Overwrite(StyleRuleData props)
        {
            for (int i = 0; i < PropertyList.Count; i++)
            {
                FieldInfo Field = PropertyList[i];
                var val = (IStyleProperty)Field.GetValue(props);
                var mv = (IStyleProperty)Field.GetValue(this);
                
                mv.Overwrite(val);
            }
        }
        #endregion


        #region Padding Helpers
        public void Set_Padding(int? horizontal, int? vertical)
        {
            Padding_Top.Set(vertical);
            Padding_Right.Set(horizontal);
            Padding_Bottom.Set(vertical);
            Padding_Left.Set(horizontal);
        }
        public void Set_Padding(CSSValue horizontal, CSSValue vertical)
        {
            Padding_Top.Set(vertical);
            Padding_Right.Set(horizontal);
            Padding_Bottom.Set(vertical);
            Padding_Left.Set(horizontal);
        }
        public void Set_Padding(int? top, int? right, int? bottom, int? left)
        {
            Padding_Top.Set(top);
            Padding_Right.Set(right);
            Padding_Bottom.Set(bottom);
            Padding_Left.Set(left);
        }
        public void Set_Padding(CSSValue top, CSSValue right, CSSValue bottom, CSSValue left)
        {
            Padding_Top.Set(top);
            Padding_Right.Set(right);
            Padding_Bottom.Set(bottom);
            Padding_Left.Set(left);
        }
        public void Set_Padding_Implicit(int? horizontal, int? vertical)
        {
            Padding_Top.Set(vertical);
            Padding_Right.Set(horizontal);
            Padding_Bottom.Set(vertical);
            Padding_Left.Set(horizontal);
        }
        public void Set_Padding_Implicit(CSSValue horizontal, CSSValue vertical)
        {
            Padding_Top.Set(vertical);
            Padding_Right.Set(horizontal);
            Padding_Bottom.Set(vertical);
            Padding_Left.Set(horizontal);
        }
        public void Set_Padding_Implicit(int? top, int? right, int? bottom, int? left)
        {
            Padding_Top.Set(top);
            Padding_Right.Set(right);
            Padding_Bottom.Set(bottom);
            Padding_Left.Set(left);
        }
        public void Set_Padding_Implicit(CSSValue top, CSSValue right, CSSValue bottom, CSSValue left)
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
            Margin_Top.Set(vertical);
            Margin_Right.Set(horizontal);
            Margin_Bottom.Set(vertical);
            Margin_Left.Set(horizontal);
        }
        public void Set_Margin(CSSValue horizontal, CSSValue vertical)
        {
            Margin_Top.Set(vertical);
            Margin_Right.Set(horizontal);
            Margin_Bottom.Set(vertical);
            Margin_Left.Set(horizontal);
        }
        public void Set_Margin(int? top, int? right, int? bottom, int? left)
        {
            Margin_Top.Set(top);
            Margin_Right.Set(right);
            Margin_Bottom.Set(bottom);
            Margin_Left.Set(left);
        }
        public void Set_Margin(CSSValue top, CSSValue right, CSSValue bottom, CSSValue left)
        {
            Margin_Top.Set(top);
            Margin_Right.Set(right);
            Margin_Bottom.Set(bottom);
            Margin_Left.Set(left);
        }
        public void Set_Margin_Implicit(int? horizontal, int? vertical)
        {
            Margin_Top.Set(vertical);
            Margin_Right.Set(horizontal);
            Margin_Bottom.Set(vertical);
            Margin_Left.Set(horizontal);
        }
        public void Set_Margin_Implicit(CSSValue horizontal, CSSValue vertical)
        {
            Margin_Top.Set(vertical);
            Margin_Right.Set(horizontal);
            Margin_Bottom.Set(vertical);
            Margin_Left.Set(horizontal);
        }
        public void Set_Margin_Implicit(int? top, int? right, int? bottom, int? left)
        {
            Margin_Top.Set(top);
            Margin_Right.Set(right);
            Margin_Bottom.Set(bottom);
            Margin_Left.Set(left);
        }
        public void Set_Margin_Implicit(CSSValue top, CSSValue right, CSSValue bottom, CSSValue left)
        {
            Margin_Top.Set(top);
            Margin_Right.Set(right);
            Margin_Bottom.Set(bottom);
            Margin_Left.Set(left);
        }
        #endregion


        #region Position Helpers
        public void Set_Position(int? x, int? y)
        {
            Left.Set(x);
            Top.Set(y);
        }
        public void Set_Position(CSSValue x, CSSValue y)
        {
            Left.Set(x);
            Top.Set(y);
        }
        internal void Set_Position_Implicit(int? x, int? y)
        {
            Left.Set(x);
            Top.Set(y);
        }
        internal void Set_Position_Implicit(CSSValue x, CSSValue y)
        {
            Left.Set(x);
            Top.Set(y);
        }
        #endregion


        #region Size Helpers
        public void Set_Size(int? width, int? height)
        {
            Width.Set(width);
            Height.Set(height);
        }
        public void Set_Size(CSSValue width, CSSValue height)
        {
            Width.Set(width);
            Height.Set(height);
        }
        public void Set_Size_Implicit(int? width, int? height)
        {
            Width.Set(width);
            Height.Set(height);
        }
        public void Set_Size_Implicit(CSSValue width, CSSValue height)
        {
            Width.Set(width);
            Height.Set(height);
        }
        #endregion


        #region SizeMin Helpers
        public void Set_SizeMin(int? width, int? height)
        {
            Min_Width.Set(width);
            Min_Height.Set(height);
        }
        public void Set_SizeMin(CSSValue width, CSSValue height)
        {
            Min_Width.Set(width);
            Min_Height.Set(height);
        }
        public void Set_SizeMin_Implicit(int? width, int? height)
        {
            Min_Width.Set(width);
            Min_Height.Set(height);
        }
        public void Set_SizeMin_Implicit(CSSValue width, CSSValue height)
        {
            Min_Width.Set(width);
            Min_Height.Set(height);
        }
        #endregion


        #region SizeMax Helpers
        public void Set_SizeMax(int? width, int? height)
        {
            Max_Width.Set(width);
            Max_Height.Set(height);
        }
        public void Set_SizeMax(CSSValue width, CSSValue height)
        {
            Max_Width.Set(width);
            Max_Height.Set(height);
        }
        public void Set_SizeMax_Implicit(int? width, int? height)
        {
            Max_Width.Set(width);
            Max_Height.Set(height);
        }
        public void Set_SizeMax_Implicit(CSSValue width, CSSValue height)
        {
            Max_Width.Set(width);
            Max_Height.Set(height);
        }
        #endregion

    }
}
