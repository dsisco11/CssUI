﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        #region Properties
        /// <summary>
        /// If True then none of this instances property values can be altered by external code.
        /// </summary>
        public readonly bool Locked = false;

        /// <summary>
        /// List of Field-Names for all our properties which have a set value
        /// </summary>
        public HashSet<AtomicString> SetProperties { get; private set; } = new HashSet<AtomicString>();

        private List<ICssProperty> CssProperties = null;
        private ConcurrentDictionary<AtomicString, ICssProperty> CssPropertyMap = null;

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
        public EnumProperty<EDisplayMode> Display => (EnumProperty<EDisplayMode>)this["display"];

        /// <summary>
        /// Current BoxSizing mode for this element.
        /// (Defaults to <see cref="EBoxSizingMode.BORDER"/>)
        /// </summary>
        public EnumProperty<EBoxSizingMode> BoxSizing => (EnumProperty<EBoxSizingMode>)this["box-sizing"];

        public EnumProperty<EPositioning> Positioning => (EnumProperty<EPositioning>)this["positioning"];
        #endregion

        #region Replaced Element Properties
        public EnumProperty<EObjectFit> ObjectFit => (EnumProperty<EObjectFit>)this["object-fit"];
        public IntProperty ObjectPosition_X => (IntProperty)this["object-position-x"];
        public IntProperty ObjectPosition_Y => (IntProperty)this["object-position-y"];
        public IntProperty Intrinsic_Width => (IntProperty)this["intrinsic-width"];
        public IntProperty Intrinsic_Height => (IntProperty)this["intrinsic-height"];
        public NumberProperty Intrinsic_Ratio => (NumberProperty)this["intrinsic-ratio"];
        #endregion

        #region Overflow
        public EnumProperty<EOverflowMode> Overflow_X => (EnumProperty<EOverflowMode>)this["overflow-x"];
        public EnumProperty<EOverflowMode> Overflow_Y => (EnumProperty<EOverflowMode>)this["overflow-y"];
        #endregion

        #region Size
        public IntProperty Width => (IntProperty)this["width"];
        public IntProperty Height => (IntProperty)this["height"];

        public IntProperty Content_Width => (IntProperty)this["content-width"];
        public IntProperty Content_Height => (IntProperty)this["content-height"];
        #endregion

        #region Borders
        public EnumProperty<EBorderStyle> Border_Top_Style => (EnumProperty<EBorderStyle>)this["border-top-style"];
        public EnumProperty<EBorderStyle> Border_Right_Style => (EnumProperty<EBorderStyle>)this["border-right-style"];
        public EnumProperty<EBorderStyle> Border_Bottom_Style => (EnumProperty<EBorderStyle>)this["border-bottom-style"];
        public EnumProperty<EBorderStyle> Border_Left_Style => (EnumProperty<EBorderStyle>)this["border-left-style"];

        public IntProperty Border_Top_Width => (IntProperty)this["border-top-width"];
        public IntProperty Border_Right_Width => (IntProperty)this["border-right-width"];
        public IntProperty Border_Bottom_Width => (IntProperty)this["border-bottom-width"];
        public IntProperty Border_Left_Width => (IntProperty)this["border-left-width"];
        #endregion

        #region Min Size
        /// <summary>
        /// The minimum Width of an elements content-area
        /// </summary>
        public IntProperty Min_Width => (IntProperty)this["min-width"];
        /// <summary>
        /// The minimum Height of an elements content-area
        /// </summary>
        public IntProperty Min_Height => (IntProperty)this["min-height"];
        #endregion

        #region Max Size
        /// <summary>
        /// The maximum Width of an elements content-area
        /// </summary>
        public IntProperty Max_Width => (IntProperty)this["max-width"];
        /// <summary>
        /// The maximum Height of an elements content-area
        /// </summary>
        public IntProperty Max_Height => (IntProperty)this["max-height"];
        #endregion

        #region Position
        /// <summary>
        /// Distance between the elements Left outter edge and the matching edge of its containing block.
        /// </summary>
        public IntProperty X => (IntProperty)this["left"];
        /// <summary>
        /// Distance between the elements Top outter edge and the matching edge of its containing block.
        /// </summary>
        public IntProperty Y => (IntProperty)this["top"];


        /// <summary>
        /// Distance between the elements Top outter edge and the matching edge of its containing block.
        /// </summary>
        public IntProperty Top => (IntProperty)this["top"];
        /// <summary>
        /// Distance between the elements Right outter edge and the matching edge of its containing block.
        /// </summary>
        public IntProperty Right => (IntProperty)this["right"];
        /// <summary>
        /// Distance between the elements Bottom outter edge and the matching edge of its containing block.
        /// </summary>
        public IntProperty Bottom => (IntProperty)this["bottom"];
        /// <summary>
        /// Distance between the elements Left outter edge and the matching edge of its containing block.
        /// </summary>
        public IntProperty Left => (IntProperty)this["left"];
        #endregion

        #region Margins
        /// <summary>
        /// Distance between the elements Top edge and Top border (in pixels)
        /// <para>Clears an area outside the border. The margin is transparent</para>
        /// </summary>
        public IntProperty Margin_Top => (IntProperty)this["margin-top"];
        /// <summary>
        /// Distance between the elements Right edge and Right border (in pixels)
        /// <para>Clears an area outside the border. The margin is transparent</para>
        /// </summary>
        public IntProperty Margin_Right => (IntProperty)this["margin-right"];
        /// <summary>
        /// Distance between the elements Bottom edge and Bottom border (in pixels)
        /// <para>Clears an area outside the border. The margin is transparent</para>
        /// </summary>
        public IntProperty Margin_Bottom => (IntProperty)this["margin-bottom"];
        /// <summary>
        /// Distance between the elements Left edge and Left border (in pixels)
        /// <para>Clears an area outside the border. The margin is transparent</para>
        /// </summary>
        public IntProperty Margin_Left => (IntProperty)this["margin-left"];
        #endregion

        #region Padding
        /// <summary>
        /// Distance between this elements Top border and its content (in pixels)
        /// <para>Clears an area around the content. The padding can be thought of as extending the content area in that the controls background occupys it</para>
        /// </summary>
        public IntProperty Padding_Top => (IntProperty)this["padding-top"];
        /// <summary>
        /// Distance between this elements Right border and its content (in pixels)
        /// <para>Clears an area around the content. The padding can be thought of as extending the content area in that the controls background occupys it</para>
        /// </summary>
        public IntProperty Padding_Right => (IntProperty)this["padding-right"];
        /// <summary>
        /// Distance between this elements Bottom border and its content (in pixels)
        /// <para>Clears an area around the content. The padding can be thought of as extending the content area in that the controls background occupys it</para>
        /// </summary>
        public IntProperty Padding_Bottom => (IntProperty)this["padding-bottom"];
        /// <summary>
        /// Distance between this elements Left border and its content (in pixels)
        /// <para>Clears an area around the content. The padding can be thought of as extending the content area in that the controls background occupys it</para>
        /// </summary>
        public IntProperty Padding_Left => (IntProperty)this["padding-left"];
        #endregion

        #region Text
        public EnumProperty<ETextAlign> TextAlign => (EnumProperty<ETextAlign>)this["text-align"];
        #endregion

        #region Font
        public NumberProperty DpiX => (NumberProperty)this["dpi-x"];
        public NumberProperty DpiY => (NumberProperty)this["dpi-y"];
        public IntProperty FontWeight => (IntProperty)this["font-weight"];
        public EnumProperty<EFontStyle> FontStyle => (EnumProperty<EFontStyle>)this["font-style"];
        public NumberProperty FontSize => (NumberProperty)this["font-size"];
        public StringProperty FontFamily => (StringProperty)this["font-family"];
        #endregion

        #region Lines
        /// <summary>
        /// 'line-height' specifies the minimal height of line boxes within the element.
        /// </summary>
        public IntProperty LineHeight => (IntProperty)this["line-height"];
        #endregion

        #region Opacity
        public NumberProperty Opacity => (NumberProperty)this["opacity"];
        #endregion

        #region Transforms
        public TransformListProperty Transform => (TransformListProperty)this["transform"];
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

            CssProperties = new List<ICssProperty>()
            {
                new IntProperty("top", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new IntProperty("right", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new IntProperty("bottom", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new IntProperty("left", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),

                new EnumProperty<EDisplayMode>("display", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowInherited = false }),
                new EnumProperty<EBoxSizingMode>("box-sizing", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new EnumProperty<EPositioning>("positioning", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new EnumProperty<EOverflowMode>("overflow-x", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new EnumProperty<EOverflowMode>("overflow-y", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),

                new EnumProperty<EObjectFit>("object-fit", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowInherited = false, AllowAuto = false }),
                new IntProperty("object-position-x", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowInherited = false }),
                new IntProperty("object-position-y", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowInherited = false }),
                new IntProperty("intrinsic-width", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowInherited = false, AllowAuto = false }),
                new IntProperty("intrinsic-height", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowInherited = false, AllowAuto = false }),
                new NumberProperty("intrinsic-ratio", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowInherited = false, AllowAuto = false }),

                new IntProperty("width", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new IntProperty("height", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),

                new IntProperty("content-width", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowAuto = false, AllowPercentage = false, AllowInherited = false }),
                new IntProperty("content-height", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowAuto = false, AllowPercentage = false, AllowInherited = false }),

                new IntProperty("min-width", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new IntProperty("min-height", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),

                new IntProperty("max-width", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new IntProperty("max-height", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),

                new IntProperty("margin-top", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new IntProperty("margin-right", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new IntProperty("margin-bottom", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new IntProperty("margin-left", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),

                new IntProperty("padding-top", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new IntProperty("padding-right", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new IntProperty("padding-bottom", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new IntProperty("padding-left", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),


                new EnumProperty<EBorderStyle>("border-top-style", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new EnumProperty<EBorderStyle>("border-right-style", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new EnumProperty<EBorderStyle>("border-bottom-style", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new EnumProperty<EBorderStyle>("border-left-style", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),

                new IntProperty("border-top-width", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new IntProperty("border-right-width", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new IntProperty("border-bottom-width", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new IntProperty("border-left-width", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),

                new EnumProperty<ETextAlign>("text-align", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),

                new NumberProperty("dpi-x", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new NumberProperty("dpi-y", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new IntProperty("font-weight", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowPercentage = false }),
                new EnumProperty<EFontStyle>("font-style", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new NumberProperty("font-size", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),
                new StringProperty("font-family", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),

                new IntProperty("line-height", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { }),

                new NumberProperty("opacity", Owner, selfRef, this.Locked, Unset, new CssPropertyOptions() { AllowPercentage = false }),

                new TransformListProperty("transform", Owner, selfRef, this.Locked, Unset)
            };

            CssPropertyMap = new ConcurrentDictionary<AtomicString, ICssProperty>(3, CssProperties.Count);

            for (int i = 0; i < CssProperties.Count; i++)
            {
                ICssProperty p = CssProperties[i];
                p.Selector = Selector;
                p.onChanged += Property_onChanged;

                bool success = CssPropertyMap.TryAdd(p.CssName, p);
                if (!success)
                    throw new Exception($"Failed to fully form {nameof(CssPropertyMap)} for {nameof(CssPropertySet)}. Failed on member: '{p.CssName}'");
            }

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
            if (!property.HasValue) SetProperties.Remove(property.CssName);
            else SetProperties.Add(property.CssName);

            if (property.CssName == null) throw new Exception(string.Concat("Cannot fire onChange events for unnamed property! (Name: ", property.CssName, ")"));
            var def = CSS.CssProperties.Definitions[property.CssName];
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
        internal ICssProperty Get_ByIndex(int index) => this.CssProperties[index];

        /// <summary>
        /// Finds the property within this style set that matches the given property
        /// </summary>
        /// <param name="Property"></param>
        /// <returns></returns>
        internal ICssProperty Get(ICssProperty Property)
        {
            if (object.ReferenceEquals(Property, null))
                throw new ArgumentNullException($"{nameof(Property)} cannot be null!");

            return Get(Property.CssName);
        }

        internal ICssProperty Get(AtomicString CssName)
        {
            if (object.ReferenceEquals(CssName, null))
                throw new ArgumentNullException($"{nameof(CssName)} cannot be null!");

            if (this.CssPropertyMap.TryGetValue(CssName, out ICssProperty prop))
                return prop;

            throw new KeyNotFoundException($"Unable find style property: {CssName}");
        }

        public ICssProperty this[AtomicString CssName]
        {
            get
            {
                if (object.ReferenceEquals(CssName, null))
                    throw new ArgumentNullException($"{nameof(CssName)} cannot be null!");

                if (this.CssPropertyMap.TryGetValue(CssName, out ICssProperty prop))
                    return prop;

                throw new KeyNotFoundException($"Unable find style property: {CssName}");
            }
        }

        public ICssProperty this[string CssName]
        {
            get
            {
                if (string.IsNullOrEmpty(CssName))
                    throw new ArgumentNullException($"{nameof(CssName)} cannot be null!");

                if (this.CssPropertyMap.TryGetValue(new AtomicString(CssName), out ICssProperty prop))
                    return prop;

                throw new KeyNotFoundException($"Unable find style property: {CssName}");
            }
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
            List<ICssProperty> List = new List<ICssProperty>();
            foreach (AtomicString name in SetProperties)
            {
                List.Add(Get(name));
            }

            return List;
        }
        #endregion

        #region Setters
        internal async Task Set(ICssProperty Property, ICssProperty Value)
        {
            if (object.ReferenceEquals(Property, null))
                throw new ArgumentNullException($"{nameof(Property)} cannot be null!");

            await Set(Property.CssName, Value);
        }

        internal async Task Set(AtomicString CssName, ICssProperty Value)
        {
            if (object.ReferenceEquals(CssName, null))
                throw new ArgumentNullException($"{nameof(CssName)} cannot be null!");

            if (this.CssPropertyMap.TryGetValue(CssName, out ICssProperty Property))
            {
                await Property.OverwriteAsync(Value);
                return;
            }

            throw new KeyNotFoundException($"Unable find style property: {CssName}");
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
            Parallel.ForEach<AtomicString>(props.SetProperties, async (cssName) =>
            {
                ICssProperty val = props[cssName];
                ICssProperty mv = this[cssName];

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
        /// <param name="Target"></param>
        internal async Task OverwriteAsync(CssPropertySet Target)
        {
            AsyncCountdownEvent ctdn = new AsyncCountdownEvent(CssProperties.Count);
            Parallel.For(0, CssProperties.Count, async (i) =>
            {
                ICssProperty prop = CssProperties[i];
                ICssProperty mv = Target.Get_ByIndex(i);

                await mv.OverwriteAsync(prop);
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