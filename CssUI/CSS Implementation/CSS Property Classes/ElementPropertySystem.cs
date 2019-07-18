using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.Enums;
using CssUI.Fonts;
using CssUI.Internal;
using SixLabors.Fonts;

namespace CssUI
{
    // DOCS: https://www.w3.org/TR/css-cascade-3/#value-stages

    /// <summary>
    /// Styling values for an element
    /// Holds resolved values for every possible defined styling property an element can have
    /// Manages cascading and accessing all of the different properties for each of an elements style states
    /// </summary>
    public class ElementPropertySystem
    {
        #region State Names
        public static AtomicString STATE_IMPLICIT = new AtomicString("Implicit");
        public static AtomicString STATE_USER = new AtomicString("Default");
        public static AtomicString STATE_FOCUS = new AtomicString("Focus");
        public static AtomicString STATE_HOVER = new AtomicString("Hover");
        #endregion

        private readonly cssElement Owner;

        #region Dirty Flags
        public EPropertySystemDirtFlags Dirt { get; private set; } = EPropertySystemDirtFlags.Cascade;
        /// <summary>
        /// Adds a flag to the dirty bit
        /// </summary>
        public void Flag(EPropertySystemDirtFlags flag) { Dirt |= flag; }
        /// <summary>
        /// Removes a flag for the dirty bit
        /// </summary>
        public void Unflag(EPropertySystemDirtFlags flag) { Dirt &= ~flag; }
        #endregion

        #region Cascaded Values
        /// <summary>
        /// The cascaded property values for this element
        /// DO NOT MODIFY THE PROPERTIES OF THIS INSTANCE, TREAT THEM AS READONLY!!!
        /// </summary>
        public readonly CssPropertySet Cascaded;
        #endregion

        #region States
        /// <summary>
        /// Default values set/preferred by the element itself
        /// (DO NOT SET THESE PROPERTIES FROM CODE OUTSIDE THE UI ELEMENT CLASS THAT OWNS THIS <see cref="ElementPropertySystem"/> INSTANCE!)
        /// </summary>
        internal CssPropertySet ImplicitRules { get => CssRules[STATE_IMPLICIT]; }
        /// <summary>
        /// Values set by code external to the element's class definition.
        /// EG: A user stylesheet, or whatever UI element instantiates and uses the element.
        /// </summary>
        public CssPropertySet UserRules { get => CssRules[STATE_USER]; }
        /// <summary>
        /// Values that take precedence when the mouse is overtop the element
        /// </summary>
        public CssPropertySet HoverRules { get => CssRules[STATE_HOVER]; }
        /// <summary>
        /// Values that take precedence when the element is currently targeted by the keyboard or activated by the mouse
        /// </summary>
        public CssPropertySet FocusRules { get => CssRules[STATE_FOCUS]; }

        // XXX: The only place we need property data that can calculate specified/computed values is in our post-cascade state, meaning we should find a way to store these property values in something other then CssProperty instances.
        /// <summary>
        /// Contains all <see cref="CssPropertySet"/>s that apply to the element
        /// </summary>
        private readonly ConcurrentDictionary<AtomicString, CssPropertySet> CssRules = new ConcurrentDictionary<AtomicString, CssPropertySet>();
        #endregion

        #region Values
        public EDirection Direction { get => Cascaded.Direction.Actual; }
        public EWritingMode WritingMode { get => Cascaded.WritingMode.Actual; }

        public EDisplayMode Display { get => Cascaded.Display.Actual; }
        public EBoxSizingMode BoxSizing { get => Cascaded.BoxSizing.Actual; }
        public EPositioning Positioning { get => Cascaded.Positioning.Actual; }

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

        public EOverflowMode Overflow_X { get => Cascaded.Overflow_X.Actual; }
        public EOverflowMode Overflow_Y { get => Cascaded.Overflow_Y.Actual; }
        public ETextAlign TextAlign { get => Cascaded.TextAlign.Actual; }

        public EObjectFit ObjectFit { get => Cascaded.ObjectFit.Actual; }

        public Matrix4 TransformMatrix { get; private set; } = null;

        public double DpiX { get => Cascaded.DpiX.Actual; }
        public double DpiY { get => Cascaded.DpiY.Actual; }

        public int FontWeight { get => Cascaded.FontWeight.Actual; }
        public EFontStyle FontStyle { get => Cascaded.FontStyle.Actual; }
        public double FontSize { get => Cascaded.FontSize.Actual; }
        public IEnumerable<string> FontFamily { get => Cascaded.FontFamily.Actual; }
        public Font Font { get; private set; }

        public double LineHeight { get => Cascaded.LineHeight.Actual; }
        public double Opacity { get => Cascaded.Opacity.Actual; }
        public cssColor Blend_Color { get; private set; } = null;
        #endregion
        
        #region Block Values

        public int ObjectPosition_X { get => Cascaded.ObjectPosition_X.Actual; }
        public int ObjectPosition_Y { get => Cascaded.ObjectPosition_Y.Actual; }

        public int Top { get => Cascaded.Top.Actual; }
        public int Right { get => Cascaded.Right.Actual; }
        public int Bottom { get => Cascaded.Bottom.Actual; }
        public int Left { get => Cascaded.Left.Actual; }

        public int Width { get => Cascaded.Width.Actual; }
        public int Height { get => Cascaded.Height.Actual; }

        public int Min_Width { get => Cascaded.Min_Width.Actual; }
        public int Min_Height { get => Cascaded.Min_Height.Actual; }

        public int? Max_Width { get => Cascaded.Max_Width.Actual; }
        public int? Max_Height { get => Cascaded.Max_Height.Actual; }


        public int Padding_Vertical { get => (Padding_Top + Padding_Bottom); }
        public int Padding_Horizontal { get => (Padding_Left + Padding_Right); }

        public int Padding_Top { get => Cascaded.Padding_Top.Actual; }
        public int Padding_Right { get => Cascaded.Padding_Right.Actual; }
        public int Padding_Bottom { get => Cascaded.Padding_Bottom.Actual; }
        public int Padding_Left { get => Cascaded.Padding_Left.Actual; }

        /*public EBorderStyle Border_Top_Style { get => Cascaded.Border_Top_Style.Computed.AsEnum<EBorderStyle>(); }
        public EBorderStyle Border_Right_Style { get => Cascaded.Border_Right_Style.Computed.AsEnum<EBorderStyle>(); }
        public EBorderStyle Border_Bottom_Style { get => Cascaded.Border_Bottom_Style.Computed.AsEnum<EBorderStyle>(); }
        public EBorderStyle Border_Left_Style { get => Cascaded.Border_Left_Style.Computed.AsEnum<EBorderStyle>(); }*/

        public EBorderStyle Border_Top_Style { get => Cascaded.Border_Top_Style.Actual; }
        public EBorderStyle Border_Right_Style { get => Cascaded.Border_Right_Style.Actual; }
        public EBorderStyle Border_Bottom_Style { get => Cascaded.Border_Bottom_Style.Actual; }
        public EBorderStyle Border_Left_Style { get => Cascaded.Border_Left_Style.Actual; }

        public int Border_Top_Width { get => Cascaded.Border_Top_Width.Actual; }
        public int Border_Right_Width { get => Cascaded.Border_Right_Width.Actual; }
        public int Border_Bottom_Width { get => Cascaded.Border_Bottom_Width.Actual; }
        public int Border_Left_Width { get => Cascaded.Border_Left_Width.Actual; }

        public int Margin_Vertical { get => (Margin_Top + Margin_Bottom); }
        public int Margin_Horizontal { get => (Margin_Left + Margin_Right); }
        public int Margin_Top { get => Cascaded.Margin_Top.Actual; }
        public int Margin_Right { get => Cascaded.Margin_Right.Actual; }
        public int Margin_Bottom { get => Cascaded.Margin_Bottom.Actual; }
        public int Margin_Left { get => Cascaded.Margin_Bottom.Actual; }
        #endregion

        #region Getters
        /// <summary>
        /// Returns whether the 'positioning' property is set to Fixed OR Absolute
        /// </summary>
        public bool IsAbsolutelyPositioned { get { return (Positioning == EPositioning.Absolute || Positioning == EPositioning.Fixed); } }


        public eBlockOffset Get_Margin_NoAuto(int autoValue)
        {
            eBlockOffset retVal = new eBlockOffset();

            if (Cascaded.Margin_Top.Computed == CssValue.Auto) retVal.Top = autoValue;
            if (Cascaded.Margin_Right.Computed == CssValue.Auto) retVal.Right = autoValue;
            if (Cascaded.Margin_Bottom.Computed == CssValue.Auto) retVal.Bottom = autoValue;
            if (Cascaded.Margin_Left.Computed == CssValue.Auto) retVal.Left = autoValue;

            return retVal;
        }

        public eBlockOffset Get_Padding_OnlyAbsolute(int autoValue)
        {
            eBlockOffset retVal = new eBlockOffset();
            retVal.Top = Cascaded.Padding_Top.Computed.Resolve_Or_Default(autoValue, (o => o.Has_Flags(ECssValueFlags.Absolute)));
            retVal.Right = Cascaded.Padding_Right.Computed.Resolve_Or_Default(autoValue, (o => o.Has_Flags(ECssValueFlags.Absolute)));
            retVal.Bottom = Cascaded.Padding_Bottom.Computed.Resolve_Or_Default(autoValue, (o => o.Has_Flags(ECssValueFlags.Absolute)));
            retVal.Left = Cascaded.Padding_Left.Computed.Resolve_Or_Default(autoValue, (o => o.Has_Flags(ECssValueFlags.Absolute)));

            return retVal;
        }

        public eBlockOffset Get_Margin_OnlyAbsolute(int autoValue)
        {
            eBlockOffset retVal = new eBlockOffset();
            retVal.Top = Cascaded.Margin_Top.Computed.Resolve_Or_Default(autoValue, (o => o.Has_Flags(ECssValueFlags.Absolute)));
            retVal.Right = Cascaded.Margin_Right.Computed.Resolve_Or_Default(autoValue, (o => o.Has_Flags(ECssValueFlags.Absolute)));
            retVal.Bottom = Cascaded.Margin_Bottom.Computed.Resolve_Or_Default(autoValue, (o => o.Has_Flags(ECssValueFlags.Absolute)));
            retVal.Left = Cascaded.Margin_Left.Computed.Resolve_Or_Default(autoValue, (o => o.Has_Flags(ECssValueFlags.Absolute)));

            return retVal;
        }
        #endregion

        #region Events
        /// <summary>
        /// The assigned value of a property has changed
        /// </summary>
        public event Action<ICssProperty, EPropertyDirtFlags, StackTrace> onProperty_Change;
        #endregion

        #region Constructors
        public ElementPropertySystem(cssElement Owner)
        {
            this.Owner = Owner;

            // Populate our rules with a few different common states
            CssRules.TryAdd(STATE_IMPLICIT, NewPropertySet(STATE_IMPLICIT, $"#{Owner.id}", Owner, false, EPropertySetOrigin.UserAgent));
            CssRules.TryAdd(STATE_USER, NewPropertySet(STATE_USER, $"#{Owner.id}", Owner, true, EPropertySetOrigin.Author));

            CssRules.TryAdd(STATE_HOVER, NewPropertySet(STATE_HOVER, ":hover", Owner, true, EPropertySetOrigin.Author));
            CssRules.TryAdd(STATE_FOCUS, NewPropertySet(STATE_FOCUS, ":focus", Owner, true, EPropertySetOrigin.Author));

            Cascaded = new CssPropertySet(null, null, Owner, true);
            Cascaded.Property_Changed += Handle_Cascaded_Property_Change;
            // Blending
            Cascaded.Opacity.onValueChange += Handle_Cascaded_Blend_Change;
            // Transformations
            Cascaded.Transform.onValueChange += Handle_Cascaded_Transform_Change;
        }
        #endregion

        #region PropertySets
        /// <summary>
        /// Creates a new property set under the specified name and binds to it to detect when a value changes.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Selector"></param>
        /// <param name="Owner"></param>
        /// <param name="Unset"></param>
        /// <returns></returns>
        private CssPropertySet NewPropertySet(string Name, string Selector, cssElement Owner, bool Unset, EPropertySetOrigin Origin)
        {
            var retVal = new CssPropertySet(Name, new CssSelector(false, Selector), Owner, false, Unset, Origin);
            // Capture all update events.
            retVal.Property_Changed += Handle_Declared_Property_Change;
            return retVal;
        }

        /// <summary>
        /// Adds a new <see cref="CssPropertySet"/> to the list of styling rules and binds to it to detect when a value changes.
        /// </summary>
        /// <param name="Selector"></param>
        /// <param name="Owner"></param>
        /// <param name="Unset"></param>
        /// <returns>Success</returns>
        internal bool Add_PropertySet(CssPropertySet prop)
        {
            var retVal = this.CssRules.TryAdd(new AtomicString(prop.Name), prop);
            // Capture all update events.
            prop.Property_Changed += Handle_Declared_Property_Change;
            // We just took on another group of proerties, we should recascade
            this.Flag(EPropertySystemDirtFlags.Cascade);

            return retVal;
        }
        #endregion

        #region Property Change Handlers

        /// <summary>
        /// A state-specific property changed, we need to resolve this single property
        /// </summary>
        private void Handle_Declared_Property_Change(ECssPropertyStage Stage, ICssProperty Property, EPropertyDirtFlags Flags, StackTrace Origin)
        {
            /* XXX:
             * To be honest cascading here doesnt make sense
             * if a declared property changes that wont always change the value of our cascaded property.
             * We should check if this property IS the cascaded property and if so then just update that single property!
             */
            CascadeProperty(Property);
        }

        /// <summary>
        /// A post-cascade property has changed assigned values
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="Flags"></param>
        /// <param name="Stack"></param>
        private void Handle_Cascaded_Property_Change(ECssPropertyStage Stage, ICssProperty Property, EPropertyDirtFlags Flags, StackTrace Stack)
        {
            bool IsFlow = ((Flags & EPropertyDirtFlags.Flow) != 0);// Layout
            bool IsBlock = ((Flags & EPropertyDirtFlags.Box) != 0);
            bool IsVisual = ((Flags & EPropertyDirtFlags.Visual) != 0);
            bool IsFont = ((Flags & EPropertyDirtFlags.Text) != 0);

            // If the value that changed was a specified one and it affects the block then we need to update our block
            if (IsBlock && Stage >= ECssPropertyStage.Specified)
            {
                // Flag us dirty so we can resolve next time its called
                this.Flag(EPropertySystemDirtFlags.Block);
                // Notify our parent by flagging them aswell
                this.Owner.Flag_Box_Dirty(EBoxInvalidationReason.Property_Changed);
                Owner.Box.Flag(EBoxInvalidationReason.Property_Changed);
                Owner.Box.FlagProperty(Flags);
            }

            // Update our dirt flags appropriately
            if (IsFlow || IsVisual) this.Flag(EPropertySystemDirtFlags.Block);
            if (IsFont) this.Flag(EPropertySystemDirtFlags.Font);
            
            //Logging.Log.Info("[Property Changed]: {0}", Prop.FieldName);
            onProperty_Change?.Invoke(Property, Flags, Stack);
        }

        /// <summary>
        /// An inheritable property of an element within our hierarchy has changed.
        /// </summary>
        /// <param name="Sender">The element whose property changed</param>
        /// <param name="Property"></param>
        public async Task Handle_Inherited_Property_Change_In_Hierarchy(cssElement Sender, ICssProperty Property)
        {
            // check if we have this property set to inherit
            ICssProperty prop = Cascaded.Get(Property);
            if (prop.IsInherited)
            {
                // we dont have to recascade this value, we just need to update its interpreted values
                prop.Update();
            }
        }
        #endregion


        #region Cascading

        /// <summary>
        /// Resolves all Css properties to their specified values by cascading
        /// </summary>
        public void Cascade()
        {
            var benchmark_id = Benchmark.Start("style-cascade");

            // Get a list of only the properties with an Assigned value
            AsyncCountdownEvent ctdn = null;
            CssPropertyComparator cm = new CssPropertyComparator();
            HashSet<AtomicString> targetFields = new HashSet<AtomicString>();
            List<HashSet<AtomicString>> allFields = CssRules.Values.Select(x => { return x.SetProperties; }).ToList();

            // Remove duplicates
            foreach(HashSet<AtomicString> fields in allFields)
            {
                targetFields.UnionWith(fields);
            }

            if (targetFields.Count > 0)
            {
                // Cascade all those set values
                ctdn = new AsyncCountdownEvent(targetFields.Count);

                // Loop over all target properties
                foreach (AtomicString propName in targetFields)
                //Parallel.ForEach(targetFields, async (AtomicString propName) =>
                {
                    try
                    {
                        // Extract this property from every CssPropertySet that has a value for it
                        var propertyList = CssRules.Values.Select(x => { return x.Get(propName); }).ToList();

                        // Order these properties according to CSS 3.0 specifications
                        propertyList.Sort(cm);

                        /*
                        // Because cascading overwrites an existing value with the one from the next propertyset we need to reverse this list.
                        propertyList.Reverse();*/

                        // Cascade this list and get what CSS calls the 'Specified' value
                        ICssProperty Value = Cascaded.Get(propName);
                        foreach (ICssProperty o in propertyList)
                        {
                            //bool b = await Value.CascadeAsync(o);
                            bool b = Value.Cascade(o);
                            if (b) break;// stop cascading the instant we find a set value
                        }

                        string SourceState = Value.Source.ToString();
                        //await Cascaded.Set(propName, Value);
                        Cascaded.Set(propName, Value);
                    }
                    finally
                    {
                        ctdn.Signal();
                    }

                //});
                }

                ctdn.WaitAsync().Wait();
            }

            // Recalculate ALL properties
            var PropList = Cascaded.GetAll().ToList();

            /*ctdn = new AsyncCountdownEvent(PropList.Count);
            Parallel.For(0, PropList.Count, (int i) =>
            {
                ICssProperty prop = PropList[i];
                // We always want to compute these now to get their values resolved. otherwise any with just assigned values will not interpret and output computed values.
                prop.Update(ComputeNow: true);
                ctdn.Signal();
            });
            ctdn.WaitAsync().Wait();*/


            for (int i = 0; i < PropList.Count; i++)
            {
                ICssProperty prop = PropList[i];
                // We always want to compute these now to get their values resolved. otherwise any with just assigned values will not interpret and output computed values.
                prop.Update(ComputeNow: true);
            }


            // Any values that changed due to this cascade should have thrown a property change event to let the style system know what it needs to update

            // Remove cascade flag
            this.Unflag(EPropertySystemDirtFlags.Cascade);
            Benchmark.Stop(benchmark_id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Property"></param>
        /// <returns></returns>
        private void CascadeProperty(ICssProperty Property)
        {
            // Extract this property from every CssPropertySet that has a value for it
            //var propertyList = CssRules.Values.Select(propSet => { return propSet[Property.CssName]; }).ToList();

            List<ICssProperty> propertyList = new List<ICssProperty>();
            foreach(var propSet in CssRules.Values)
            {
                propertyList.Add(propSet[Property.CssName]);
            }

            // Order these properties according to CSS 3.0 specifications
            propertyList.Sort(new CssPropertyComparator());

            // Cascade this list and get what CSS calls the 'Specified' value
            ICssProperty Value = Property;
            foreach (ICssProperty o in propertyList)
            {
                /*var cascade = Value.CascadeAsync(o);
                Task.WhenAll(cascade).Wait();// we HAVE to wait here
                if (cascade.Result) break;// stop cascading the instant we find a set value*/
                var changed = Value.Cascade(o);
                if (changed) break;// stop cascading the instant we find a set value
            }

            string SourceState = Value.Source.ToString();
            Cascaded.Set(Property.CssName, Value);
        }
        #endregion

        #region Element Change Handlers

        /// <summary>
        /// Our elements block changed so we need to clear any calculated values for Css Properties related to the block
        /// </summary>
        public void Handle_Parent_Block_Change()
        {
            Force_Dependent_Block_Property_Updates();
        }

        /// <summary>
        /// Our box's layout position has changed
        /// </summary>
        public void Handle_Layout_Position_Change()
        {
            if (Owner.Box.IsAbsolutelyPositioned && 0!=(Owner.Box.Flags & EBoxFlags.REPLACED_ELEMENT))
            {
                // Width / Height depend on these
                Cascaded.Width.Update(true);
                Cascaded.Height.Update(true);
            }
        }
        #endregion

        #region Custom States
        public CssPropertySet this[AtomicString State]
        {
            get
            {
                CssPropertySet prop;
                if (!CssRules.TryGetValue(State, out prop))
                {
                    throw new Exception(string.Format("The styling state \"{0}\" is invalid!", State));
                }
                return prop;
            }
        }
        #endregion

        #region State Setting
        private HashSet<AtomicString> ActiveStates = new HashSet<AtomicString>();

        public async Task Try_Update_Style()
        {
            /*
             * Ok so this will get called A LOT, like everytime ANY pseudo-state or anything about the element changes
             * The purpose of this function is to go back over all our known CSSDeclerationBlocks and check if their selectors have changed whether they apply to the element or not.
             * If they have changed we immediately re-cascade
             */

        }

        public async Task Set_State(AtomicString StateName, bool Status)
        {
            bool changes = false;
            if (Status && !ActiveStates.Contains(StateName))
            {
                ActiveStates.Add(StateName);
                changes = true;
            }
            else if (!Status && ActiveStates.Contains(StateName))
            {
                ActiveStates.Remove(StateName);
                changes = true;
            }

            if (changes)
            {
                // The element state has changed, we will need to re-cascade and then resolve the properties
                this.Flag(EPropertySystemDirtFlags.Block);
                Cascade();
            }
        }
        #endregion
        
        #region Unit Change Handler
        /// <summary>
        /// Notifys all dimension-properties which use the specified unit that its scale has changed and they need to update
        /// </summary>
        /// <param name="Unit"></param>
        public void Notify_Unit_Scale_Change(EUnit Unit)
        {
            if (Unit == EUnit.None) return;
            foreach(ICssProperty Property in this.Cascaded.Get_Set_Properties())
            {
                Property.Handle_Unit_Change(Unit);
            }
        }
        #endregion
                

        #region Dependent Block Values
        /// <summary>
        /// Forces any properties which depend on a block value (ours or our parents) to update and recompute
        /// </summary>
        public void Force_Dependent_Block_Property_Updates()
        {
            if (!Owner.Box.DependsOnContainer) return;

            /*
            await this.Cascaded.Border_Top_Width.UpdateDependentOrAuto(true).ConfigureAwait(false);
            await this.Cascaded.Border_Right_Width.UpdateDependentOrAuto(true).ConfigureAwait(false);
            await this.Cascaded.Border_Bottom_Width.UpdateDependentOrAuto(true).ConfigureAwait(false);
            await this.Cascaded.Border_Left_Width.UpdateDependentOrAuto(true).ConfigureAwait(false);

            await this.Cascaded.Margin_Top.UpdateDependentOrAuto(true).ConfigureAwait(false);
            await this.Cascaded.Margin_Right.UpdateDependentOrAuto(true).ConfigureAwait(false);
            await this.Cascaded.Margin_Bottom.UpdateDependentOrAuto(true).ConfigureAwait(false);
            await this.Cascaded.Margin_Left.UpdateDependentOrAuto(true).ConfigureAwait(false);

            await this.Cascaded.Padding_Top.UpdateDependentOrAuto(true).ConfigureAwait(false);
            await this.Cascaded.Padding_Right.UpdateDependentOrAuto(true).ConfigureAwait(false);
            await this.Cascaded.Padding_Bottom.UpdateDependentOrAuto(true).ConfigureAwait(false);
            await this.Cascaded.Padding_Left.UpdateDependentOrAuto(true).ConfigureAwait(false);

            await this.Cascaded.Top.UpdateDependentOrAuto(true).ConfigureAwait(false);
            await this.Cascaded.Right.UpdateDependentOrAuto(true).ConfigureAwait(false);
            await this.Cascaded.Bottom.UpdateDependentOrAuto(true).ConfigureAwait(false);
            await this.Cascaded.Left.UpdateDependentOrAuto(true).ConfigureAwait(false);



            await this.Cascaded.Min_Width.UpdateDependentOrAuto(true).ConfigureAwait(false);
            await this.Cascaded.Min_Height.UpdateDependentOrAuto(true).ConfigureAwait(false);

            await this.Cascaded.Max_Width.UpdateDependentOrAuto(true).ConfigureAwait(false);
            await this.Cascaded.Max_Height.UpdateDependentOrAuto(true).ConfigureAwait(false);

            await this.Cascaded.Width.UpdateDependentOrAuto(true).ConfigureAwait(false);
            await this.Cascaded.Height.UpdateDependentOrAuto(true).ConfigureAwait(false);
            */



            this.Cascaded.Border_Top_Width.UpdateDependentOrAuto(true);
            this.Cascaded.Border_Right_Width.UpdateDependentOrAuto(true);
            this.Cascaded.Border_Bottom_Width.UpdateDependentOrAuto(true);
            this.Cascaded.Border_Left_Width.UpdateDependentOrAuto(true);

            this.Cascaded.Margin_Top.UpdateDependentOrAuto(true);
            this.Cascaded.Margin_Right.UpdateDependentOrAuto(true);
            this.Cascaded.Margin_Bottom.UpdateDependentOrAuto(true);
            this.Cascaded.Margin_Left.UpdateDependentOrAuto(true);

            this.Cascaded.Padding_Top.UpdateDependentOrAuto(true);
            this.Cascaded.Padding_Right.UpdateDependentOrAuto(true);
            this.Cascaded.Padding_Bottom.UpdateDependentOrAuto(true);
            this.Cascaded.Padding_Left.UpdateDependentOrAuto(true);

            this.Cascaded.Top.UpdateDependentOrAuto(true);
            this.Cascaded.Right.UpdateDependentOrAuto(true);
            this.Cascaded.Bottom.UpdateDependentOrAuto(true);
            this.Cascaded.Left.UpdateDependentOrAuto(true);

            this.Cascaded.Min_Width.UpdateDependentOrAuto(true);
            this.Cascaded.Min_Height.UpdateDependentOrAuto(true);

            this.Cascaded.Max_Width.UpdateDependentOrAuto(true);
            this.Cascaded.Max_Height.UpdateDependentOrAuto(true);

            this.Cascaded.Width.UpdateDependentOrAuto(true);
            this.Cascaded.Height.UpdateDependentOrAuto(true);
        }
        
        #endregion


        #region Font Updating
        /// <summary>
        /// Helpes us track changes to the font size of this element, to detect changes to font relative units
        /// </summary>
        double oldFontSize = -1;
        public void Resolve_Font()
        {
            if (0 == (Dirt & EPropertySystemDirtFlags.Font)) return;

            if (!MathExt.floatEq(FontSize, oldFontSize))
            {
                oldFontSize = FontSize;
                Notify_Unit_Scale_Change(EUnit.EM);
                Notify_Unit_Scale_Change(EUnit.EX);
                Notify_Unit_Scale_Change(EUnit.CH);
            }

            // Get font from font factory, which will help cache identical fonts
            FontOptions fontOptions = new FontOptions(FontFamily, FontSize, FontWeight, FontStyle);
            this.Font = FontFactory.Get(fontOptions);

            // Remove font dirt flag
            this.Unflag(EPropertySystemDirtFlags.Font);

            // Flag our elements font dirty flag so it updates whatever is using it
            Owner.Flag_Dirty(EElementDirtyFlags.Font);
        }
        #endregion

        #region Update_Blend_Color
        void Handle_Cascaded_Blend_Change(ECssPropertyStage Stage, ICssProperty Property)
        {
            if (Stage < ECssPropertyStage.Actual)// wait for the Actual value stage
                return;

            if (Opacity != 1.0)
            {// We have a useful blend color to set
                Blend_Color = new cssColor(1, 1, 1, Opacity);
            }
            else// We DONT have a useful blend color
            {
                Blend_Color = null;
            }
        }
        #endregion

        #region Transform_Changed

        private void Handle_Cascaded_Transform_Change(ECssPropertyStage Stage, ICssProperty Property)
        {
            if (Stage < ECssPropertyStage.Actual)// wait for the Actual value stage
                return;

            throw new NotImplementedException("Transforms are not yet implemented");
        }
        #endregion

    }
}
