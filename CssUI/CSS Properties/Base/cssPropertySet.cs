using KeyPic.Benchmarking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CssUI.CSS;
using System.Collections.Concurrent;
using CssUI.Enums;

namespace CssUI
{
    // XXX: This class could be MUCH more performant if we didnt have to create an instance of every defined css property during creation, 
    //      if we could just instantiate the properties when they are needed it would save a lot.

    /// <summary>
    /// Holds an instance of all the defined css propertys that a css element can have
    /// Each different styling state of an element gets it's own instance of this class which 
    /// are then all cascaded together to determine the current value when the elements active state changes.
    /// </summary>
    public class CssPropertySet
    {
        #region Static Property List

        /// <summary>
        /// List of FieldInfo for all <see cref="ICssProperty"/> variables
        /// </summary>
        public static readonly List<FieldInfo> PropertyList;
        /// <summary>
        /// Map of all <see cref="ICssProperty"/> variable names to their FieldInfo
        /// </summary>
        public static readonly ConcurrentDictionary<AtomicString, FieldInfo> PropertyMap;

        static CssPropertySet()
        {
            // For each defined cssProperty create an instance of said property and store it in our PropertyList
            PropertyList = typeof(CssPropertySet).GetFields().Where(o => (o.Attributes.HasFlag(FieldAttributes.InitOnly) && typeof(ICssProperty).IsAssignableFrom(o.FieldType))).ToList();

            // To allow for quick lookup of propertys by their name, map each property in our PropertyList
            PropertyMap = new ConcurrentDictionary<AtomicString, FieldInfo>();
            foreach(FieldInfo field in PropertyList)
            {
                PropertyMap.TryAdd(new AtomicString(field.Name), field);
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
        public HashSet<AtomicString> SetProperties { get; private set; }  = new HashSet<AtomicString>();

        /// <summary>
        /// Sequence tracker for <see cref="CssPropertySet"/>s
        /// </summary>
        private static ulong SEQ = 0;
        /// <summary>
        /// Id number for this <see cref="CssPropertySet"/>, unique among all <see cref="CssPropertySet"/>s
        /// </summary>
        public readonly UniqueID ID;

        /// <summary>
        /// Name of the rule block, for identification
        /// </summary>
        public string Name { get { return _name ?? defaultName; } set { _name = value; } }

        private string _name = null;
        private string defaultName {
            get {
                return (string)($"{nameof(CssPropertySet)} - {this.ID.Value}").Concat(string.IsNullOrEmpty(this.Selector.ToString()) ? "" : this.Selector.ToString());
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
        public readonly NumberProperty DpiX;
        public readonly NumberProperty DpiY;
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
        public CssPropertySet(string Name, cssElement Owner, bool Locked = false) : this(Name, null, Owner, Locked)
        {
        }

        /// <summary>
        /// Creates a new set of element styling properties
        /// </summary>
        /// <param name="Locked">If TRUE then none of this instances property values may be set directly.</param>
        public CssPropertySet(CssSelector Selector, cssElement Owner, bool Locked = false) : this(null, Selector, Owner, Locked)
        {
        }

        /// <summary>
        /// Creates a new set of element styling properties
        /// </summary>
        /// <param name="Locked">If TRUE then none of this instances property values may be set directly.</param>
        /// <param name="Unset">If TRUE then property values will all be set to <see cref="CssValue.Null"/>.</param>
        public CssPropertySet(CssSelector Selector, cssElement Owner, bool Locked = false, bool Unset = false) : this(null, Selector, Owner, Locked, Unset)
        {
        }

        /// <summary>
        /// Creates a new set of element styling properties
        /// </summary>
        /// <param name="Locked">If TRUE then none of this instances property values may be set directly.</param>
        /// <param name="Unset">If TRUE then property values will all be set to <see cref="CssValue.Null"/>.</param>
        public CssPropertySet(string Name, CssSelector Selector, cssElement Owner, bool Locked = false, bool Unset = false, EPropertySetOrigin Origin = EPropertySetOrigin.Author)
        {
            this.ID = new UniqueID(++SEQ);
            this.Name = Name;
            this.Selector = Selector;
            this.Origin = Origin;
            var selfRef = new WeakReference<CssPropertySet>(this);

            Top = new IntProperty("top", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Right = new IntProperty("right", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Bottom = new IntProperty("bottom", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Left = new IntProperty("left", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });

            Display = new EnumProperty<EDisplayMode>("display", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowInherited = false });
            BoxSizing = new EnumProperty<EBoxSizingMode>("box-sizing", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Positioning = new EnumProperty<EPositioning>("positioning", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Overflow_X = new EnumProperty<EOverflowMode>("overflow-x", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Overflow_Y = new EnumProperty<EOverflowMode>("overflow-y", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });

            ObjectFit = new EnumProperty<EObjectFit>("object-fit", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowInherited = false, AllowAuto = false });
            ObjectPosition_X = new IntProperty("object-position-x", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowInherited = false });
            ObjectPosition_Y = new IntProperty("object-position-y", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowInherited = false });
            Intrinsic_Width = new IntProperty("intrinsic-width", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowInherited = false, AllowAuto = false });
            Intrinsic_Height = new IntProperty("intrinsic-height", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowInherited = false, AllowAuto = false });
            Intrinsic_Ratio = new NumberProperty("intrinsic-ratio", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowInherited = false, AllowAuto = false });

            Width = new IntProperty("width", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Height = new IntProperty("height", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });

            Content_Width = new IntProperty("content-width", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowAuto = false, AllowPercentage = false, AllowInherited = false });
            Content_Height = new IntProperty("content-height", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowAuto = false, AllowPercentage = false, AllowInherited = false });

            Min_Width = new IntProperty("min-width", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Min_Height = new IntProperty("min-height", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });

            Max_Width = new IntProperty("max-width", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Max_Height = new IntProperty("max-height", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });

            Margin_Top = new IntProperty("margin-top", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Margin_Right = new IntProperty("margin-right", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Margin_Bottom = new IntProperty("margin-bottom", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Margin_Left = new IntProperty("margin-left", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });

            Padding_Top = new IntProperty("padding-top", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Padding_Right = new IntProperty("padding-right", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Padding_Bottom = new IntProperty("padding-bottom", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Padding_Left = new IntProperty("padding-left", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            

            Border_Top_Style = new EnumProperty<EBorderStyle>("border-top-style", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Border_Right_Style = new EnumProperty<EBorderStyle>("border-right-style", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Border_Bottom_Style = new EnumProperty<EBorderStyle>("border-bottom-style", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Border_Left_Style = new EnumProperty<EBorderStyle>("border-left-style", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });

            Border_Top_Width = new IntProperty("border-top-width", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Border_Right_Width = new IntProperty("border-right-width", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Border_Bottom_Width = new IntProperty("border-bottom-width", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            Border_Left_Width = new IntProperty("border-left-width", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });

            TextAlign = new EnumProperty<ETextAlign>("text-align", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });

            DpiX = new NumberProperty("dpi-x", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            DpiY = new NumberProperty("dpi-y", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            FontWeight = new IntProperty("font-weight", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowPercentage = false });
            FontStyle = new EnumProperty<EFontStyle>("font-style", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            FontSize = new NumberProperty("font-size", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });
            FontFamily = new StringProperty("font-family", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });

            LineHeight = new IntProperty("line-height", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { });

            Opacity = new NumberProperty("opacity", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowPercentage = false });

            Transform = new TransformListProperty("transform", Owner, selfRef, this.Locked, Unset);

            // Name each of our properties
            Parallel.ForEach(PropertyList, (FieldInfo Field) =>
            {
                var property = (ICssProperty)Field.GetValue(this);
                property.FieldName = new AtomicString(Field.Name);
                property.Selector = Selector;
                property.onChanged += Property_onChanged;
            });
        }
        #endregion

        #region Events
        /// <summary>
        /// A property which affects the elements block changed
        /// </summary>
        public event Action<ICssProperty, EPropertyFlags, StackTrace> Property_Changed;
        #endregion

        #region Change Handlers
        const int STACK_FRAME_OFFSET = 3;

        private void Property_onChanged(ICssProperty property)
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
        internal ICssProperty Get(AtomicString FieldName)
        {
            FieldInfo Field = PropertyMap[FieldName];
            if (!typeof(ICssProperty).IsAssignableFrom(Field.FieldType))
                throw new Exception($"Unable find style property with the field name: {FieldName}");
            return (ICssProperty)Field.GetValue(this);
        }

        /// <summary>
        /// Finds the property within this style set that matches the given property
        /// </summary>
        /// <param name="Property"></param>
        /// <returns></returns>
        internal ICssProperty Get(ICssProperty Property)
        {
            AtomicString FieldName = Property.FieldName;
            return Get(FieldName);
        }

        /// <summary>
        /// Returns all of the properties
        /// </summary>
        internal IEnumerable<ICssProperty> GetAll()
        {
            List<ICssProperty> Ret = new List<ICssProperty>();
            for (int i = 0; i < PropertyList.Count; i++)
            {
                FieldInfo Field = PropertyList[i];
                Ret.Add( (ICssProperty)Field.GetValue(this) );
            }

            return Ret.ToArray();
        }

        /// <summary>
        /// Returns all of the properties matching a given predicate
        /// </summary>
        internal IEnumerable<ICssProperty> GetAll(Func<ICssProperty, bool> Predicate)
        {
            List<ICssProperty> Ret = new List<ICssProperty>();
            for (int i = 0; i < PropertyList.Count; i++)
            {
                FieldInfo Field = PropertyList[i];
                ICssProperty val = (ICssProperty)Field.GetValue(this);
                if (Predicate(val))
                {
                    Ret.Add(val);
                }
            }

            return Ret.ToArray();
        }

        internal List<ICssProperty> Get_Set_Properties()
        {
            List<ICssProperty> List = new List<ICssProperty>();
            foreach (AtomicString fieldName in SetProperties)
            {
                List.Add(Get(fieldName));
            }

            return List;
        }
        #endregion

        #region Setters
        internal async Task Set(ICssProperty Property, ICssProperty Value, AtomicString SourceState)
        {
            AtomicString FieldName = Property.FieldName;
            await Set(FieldName, Value, SourceState);
        }

        internal async Task Set(AtomicString FieldName, ICssProperty Value, AtomicString SourceState)
        {
            FieldInfo Field = typeof(CssPropertySet).GetField(FieldName.ToString());
            if (!typeof(ICssProperty).IsAssignableFrom(Field.FieldType)) throw new Exception(string.Format("Unable find style property with the field name: {0}", FieldName));
            
            await ((ICssProperty)Field.GetValue(this)).OverwriteAsync(Value);
        }
        #endregion

        #region Value Management

        /// <summary>
        /// Overwrites the property values of this instance with those of any set property values from another instance.
        /// </summary>
        /// <param name="props"></param>
        internal async Task CascadeAsync(CssPropertySet props)
        {
            //var tid = Timing.Start("Cascade");
            if (props.SetProperties.Count <= 0)
                return;

            AsyncCountdownEvent ctdn = new AsyncCountdownEvent(props.SetProperties.Count);
            Parallel.ForEach<AtomicString>(props.SetProperties, async (name) =>
            {
                FieldInfo Field = PropertyMap[name];
                var val = (ICssProperty)Field.GetValue(props);
                var mv = (ICssProperty)Field.GetValue(this);

                await mv.CascadeAsync(val);
                // Signal our original thread that we are 1 step closer to being done
                ctdn.Signal();
            });

            await ctdn.WaitAsync();
            //Timing.Stop(tid);
        }

        /// <summary>
        /// Overwrites any differing property values
        /// </summary>
        /// <param name="props"></param>
        internal async Task OverwriteAsync(CssPropertySet props)
        {
            AsyncCountdownEvent ctdn = new AsyncCountdownEvent(PropertyList.Count);
            Parallel.For(0, PropertyList.Count, async (i) =>
            {
                FieldInfo Field = PropertyList[i];
                var val = (ICssProperty)Field.GetValue(props);
                var mv = (ICssProperty)Field.GetValue(this);

                await mv.OverwriteAsync(val);
                ctdn.Signal();
            });

            await ctdn.WaitAsync();
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
        public void Set_Padding(CssValue horizontal, CssValue vertical)
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
        public void Set_Padding(CssValue top, CssValue right, CssValue bottom, CssValue left)
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
        public void Set_Padding_Implicit(CssValue horizontal, CssValue vertical)
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
        public void Set_Padding_Implicit(CssValue top, CssValue right, CssValue bottom, CssValue left)
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
        public void Set_Margin(CssValue horizontal, CssValue vertical)
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
        public void Set_Margin(CssValue top, CssValue right, CssValue bottom, CssValue left)
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
        public void Set_Margin_Implicit(CssValue horizontal, CssValue vertical)
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
        public void Set_Margin_Implicit(CssValue top, CssValue right, CssValue bottom, CssValue left)
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
        public void Set_Position(CssValue x, CssValue y)
        {
            Left.Set(x);
            Top.Set(y);
        }
        internal void Set_Position_Implicit(int? x, int? y)
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
        public void Set_Size(int? width, int? height)
        {
            Width.Set(width);
            Height.Set(height);
        }
        public void Set_Size(CssValue width, CssValue height)
        {
            Width.Set(width);
            Height.Set(height);
        }
        public void Set_Size_Implicit(int? width, int? height)
        {
            Width.Set(width);
            Height.Set(height);
        }
        public void Set_Size_Implicit(CssValue width, CssValue height)
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
        public void Set_SizeMin(CssValue width, CssValue height)
        {
            Min_Width.Set(width);
            Min_Height.Set(height);
        }
        public void Set_SizeMin_Implicit(int? width, int? height)
        {
            Min_Width.Set(width);
            Min_Height.Set(height);
        }
        public void Set_SizeMin_Implicit(CssValue width, CssValue height)
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
        public void Set_SizeMax(CssValue width, CssValue height)
        {
            Max_Width.Set(width);
            Max_Height.Set(height);
        }
        public void Set_SizeMax_Implicit(int? width, int? height)
        {
            Max_Width.Set(width);
            Max_Height.Set(height);
        }
        public void Set_SizeMax_Implicit(CssValue width, CssValue height)
        {
            Max_Width.Set(width);
            Max_Height.Set(height);
        }
        #endregion

    }
}
