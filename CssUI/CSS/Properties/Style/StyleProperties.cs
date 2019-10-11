using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CssUI.CSS.BoxTree;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM;
using CssUI.DOM.Nodes;
using CssUI.Rendering;
#if DISABLE_FONT_SYSTEM
#else
using CssUI.Fonts;
using SixLabors.Fonts;
#endif

namespace CssUI.CSS
{
    // DOCS: https://www.w3.org/TR/css-cascade-3/#value-stages

    /// <summary>
    /// Styling values for an element
    /// Holds resolved values for every possible defined styling property an element can have
    /// Manages cascading and accessing all of the different properties for each of an elements style states
    /// </summary>
    public partial class StyleProperties
    {
        #region State Names
        public static AtomicString STATE_IMPLICIT = new AtomicString("Implicit");
        public static AtomicString STATE_USER = new AtomicString("Default");
        public static AtomicString STATE_FOCUS = new AtomicString("Focus");
        public static AtomicString STATE_HOVER = new AtomicString("Hover");
        #endregion

        private readonly ICssElement owningElement;
        public EPropertySystemDirtFlags StyleFlags { get; private set; } = EPropertySystemDirtFlags.NeedsToCascade;

        #region Flags
        public bool GetFlag(EPropertySystemDirtFlags Flags) { return (StyleFlags & Flags) != 0; }
        public void SetFlag(EPropertySystemDirtFlags Flags) { StyleFlags |= Flags; }
        public void SetFlag(EPropertySystemDirtFlags Flags, bool State) { StyleFlags = (StyleFlags & ~Flags) | ((EPropertySystemDirtFlags)(-(long)(State ? 1 : 0)) & Flags); }
        public void ClearFlag(EPropertySystemDirtFlags Flags) { StyleFlags &= ~Flags; }
        #endregion


        #region Cascaded Values
        /// <summary>
        /// The cascaded property values for this element
        /// DO NOT MODIFY THE PROPERTIES OF THIS INSTANCE, TREAT THEM AS READONLY!!!
        /// </summary>
        public readonly CssComputedStyle Cascaded;
        #endregion

        #region States
        /// <summary>
        /// Default values set/preferred by the element or CSS itself
        /// </summary>
        internal CssComputedStyle ImplicitRules => CssRules[STATE_IMPLICIT];
        /// <summary>
        /// Values set by code external to the element's class definition.
        /// EG: A user stylesheet, or whatever UI element instantiates and uses the element.
        /// </summary>
        public CssComputedStyle UserRules => CssRules[STATE_USER];
        /// <summary>
        /// Values that take precedence when the mouse is overtop the element
        /// </summary>
        public CssComputedStyle HoverRules => CssRules[STATE_HOVER];
        /// <summary>
        /// Values that take precedence when the element is currently targeted by the keyboard or activated by the mouse
        /// </summary>
        public CssComputedStyle FocusRules => CssRules[STATE_FOCUS];

        // XXX: The only place we need property data that can calculate specified/computed values is in our post-cascade state, meaning we should find a way to store these property values in something other then CssProperty instances.
        /// <summary>
        /// Contains all <see cref="CssComputedStyle"/>s that apply to the element
        /// </summary>
        private readonly ConcurrentDictionary<AtomicString, CssComputedStyle> CssRules = new ConcurrentDictionary<AtomicString, CssComputedStyle>();
        #endregion

        #region Values
        public EDirection Direction => Cascaded.Direction.Actual;
        public EWritingMode WritingMode => Cascaded.WritingMode.Actual;

        public EDisplayMode Display => Cascaded.Display.Actual;
        public EBoxSizingMode BoxSizing => Cascaded.BoxSizing.Actual;
        public EBoxPositioning Positioning => Cascaded.Positioning.Actual;

        /// <summary>
        /// Returns the positioning 'scheme', which defines whether the element follows the normal flow logic.
        /// </summary>
        public EPositioningScheme PositioningScheme
        {
            get
            {
                switch (Positioning)
                {
                    case EBoxPositioning.Absolute:
                    case EBoxPositioning.Fixed:
                        return EPositioningScheme.Absolute;
                    default:
                        return EPositioningScheme.Normal;
                }
            }
        }


        // Scrolling Behaviour
        public EScrollBehavior ScrollBehavior => Cascaded.ScrollBehavior.Actual;
        public EOverflowMode Overflow_X => Cascaded.Overflow_X.Actual;
        public EOverflowMode Overflow_Y => Cascaded.Overflow_Y.Actual;


        public ETextAlign TextAlign => Cascaded.TextAlign.Actual;

        public EObjectFit ObjectFit => Cascaded.ObjectFit.Actual;

        public Matrix4 TransformMatrix { get; private set; } = null;

        public double DpiX => Cascaded.DpiX.Actual;
        public double DpiY => Cascaded.DpiY.Actual;

        public int FontWeight => Cascaded.FontWeight.Actual;
        public EFontStyle FontStyle => Cascaded.FontStyle.Actual;
        public double FontSize => Cascaded.FontSize.Actual;
        public IEnumerable<string> FontFamily => Cascaded.FontFamily.Actual;

#if DISABLE_FONT_SYSTEM
#else
        public Font Font { get; private set; }
#endif

        public double LineHeight => Cascaded.LineHeight.Actual;
        public double Opacity => Cascaded.Opacity.Actual;
        public Color Blend_Color { get; private set; } = null;
        #endregion

        #region Block Values

        public Point2f ObjectPosition => Cascaded.ObjectPosition.Actual;
        // public int ObjectPosition_X => Cascaded.ObjectPosition_X.Actual;
        // public int ObjectPosition_Y => Cascaded.ObjectPosition_Y.Actual;

        public int Top => Cascaded.Top.Actual;
        public int Right => Cascaded.Right.Actual;
        public int Bottom => Cascaded.Bottom.Actual;
        public int Left => Cascaded.Left.Actual;

        public int Width => Cascaded.Width.Actual;
        public int Height => Cascaded.Height.Actual;

        public int Min_Width => Cascaded.Min_Width.Actual;
        public int Min_Height => Cascaded.Min_Height.Actual;

        public int? Max_Width => Cascaded.Max_Width.Actual;
        public int? Max_Height => Cascaded.Max_Height.Actual;


        public int Padding_Vertical => Padding_Top + Padding_Bottom;
        public int Padding_Horizontal => Padding_Left + Padding_Right;

        public int Padding_Top => Cascaded.Padding_Top.Actual;
        public int Padding_Right => Cascaded.Padding_Right.Actual;
        public int Padding_Bottom => Cascaded.Padding_Bottom.Actual;
        public int Padding_Left => Cascaded.Padding_Left.Actual;

        public ReadOnlyRect4f Get_Padding_Size()
        {
            return new ReadOnlyRect4f(Cascaded.Padding_Top.Actual,
                                      Cascaded.Padding_Right.Actual,
                                      Cascaded.Padding_Bottom.Actual,
                                      Cascaded.Padding_Left.Actual);
        }

        public EBorderStyle Border_Top_Style => Cascaded.Border_Top_Style.Actual;
        public EBorderStyle Border_Right_Style => Cascaded.Border_Right_Style.Actual;
        public EBorderStyle Border_Bottom_Style => Cascaded.Border_Bottom_Style.Actual;
        public EBorderStyle Border_Left_Style => Cascaded.Border_Left_Style.Actual;

        public int Border_Top_Width => Cascaded.Border_Top_Width.Actual;
        public int Border_Right_Width => Cascaded.Border_Right_Width.Actual;
        public int Border_Bottom_Width => Cascaded.Border_Bottom_Width.Actual;
        public int Border_Left_Width => Cascaded.Border_Left_Width.Actual;

        public ReadOnlyRect4f Get_Border_Size()
        {
            return new ReadOnlyRect4f(Cascaded.Border_Top_Width.Actual,
                                      Cascaded.Border_Right_Width.Actual,
                                      Cascaded.Border_Bottom_Width.Actual,
                                      Cascaded.Border_Left_Width.Actual);
        }

        public int Margin_Vertical => Margin_Top + Margin_Bottom;
        public int Margin_Horizontal => Margin_Left + Margin_Right;

        public int Margin_Top => Cascaded.Margin_Top.Actual;
        public int Margin_Right => Cascaded.Margin_Right.Actual;
        public int Margin_Bottom => Cascaded.Margin_Bottom.Actual;
        public int Margin_Left => Cascaded.Margin_Bottom.Actual;

        public ReadOnlyRect4f Get_Margin_Size()
        {
            return new ReadOnlyRect4f(Cascaded.Margin_Top.Actual,
                                      Cascaded.Margin_Right.Actual,
                                      Cascaded.Margin_Bottom.Actual,
                                      Cascaded.Margin_Left.Actual);
        }
        #endregion

        #region Accessors
        public bool isHorizontalWritingMode => (WritingMode != EWritingMode.Horizontal_TB);
        public bool IsReplacedElement => owningElement.GetFlag(ENodeFlags.IsReplaced);
        public CssPrincipalBox Box => owningElement.Box;
        #endregion

        #region Events
        /// <summary>
        /// The assigned value of a property has changed
        /// </summary>
        public event Action<ICssProperty, EPropertyDirtFlags, StackTrace> onProperty_Change;
        #endregion

        #region Constructors
        public StyleProperties(ICssElement owning_element)
        {
            owningElement = owning_element;

            // Populate our rules with a few different common states
            CssRules.TryAdd(STATE_IMPLICIT, NewPropertySet(STATE_IMPLICIT, null, owning_element, false, EPropertySetOrigin.UserAgent));
            CssRules.TryAdd(STATE_USER, NewPropertySet(STATE_USER, null, owning_element, true, EPropertySetOrigin.Author));

            CssRules.TryAdd(STATE_HOVER, NewPropertySet(STATE_HOVER, ":hover", owning_element, true, EPropertySetOrigin.Author));
            CssRules.TryAdd(STATE_FOCUS, NewPropertySet(STATE_FOCUS, ":focus", owning_element, true, EPropertySetOrigin.Author));

            Cascaded = new CssComputedStyle(null, null, owning_element, true);
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
        private CssComputedStyle NewPropertySet(string Name, string Selector, ICssElement Owner, bool Unset, EPropertySetOrigin Origin)
        {
            var retVal = new CssComputedStyle(Name, new CssSelector(false, Selector), Owner, false, Unset, Origin);
            // Capture all update events.
            retVal.Property_Changed += Handle_Declared_Property_Change;
            return retVal;
        }

        /// <summary>
        /// Adds a new <see cref="CssComputedStyle"/> to the list of styling rules and binds to it to detect when a value changes.
        /// </summary>
        /// <param name="Selector"></param>
        /// <param name="Owner"></param>
        /// <param name="Unset"></param>
        /// <returns>Success</returns>
        internal bool Add_PropertySet(CssComputedStyle prop)
        {
            var retVal = CssRules.TryAdd(new AtomicString(prop.Name), prop);
            // Capture all update events.
            prop.Property_Changed += Handle_Declared_Property_Change;
            // We just took on another group of proerties, we should recascade
            SetFlag(EPropertySystemDirtFlags.NeedsToCascade);

            return retVal;
        }
        #endregion

        #region Property Change Handlers

        /// <summary>
        /// A state-specific property changed, we need to resolve this single property
        /// </summary>
        private void Handle_Declared_Property_Change(EPropertyStage Stage, ICssProperty Property, EPropertyDirtFlags Flags, StackTrace Origin)
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
        private void Handle_Cascaded_Property_Change(EPropertyStage Stage, ICssProperty Property, EPropertyDirtFlags Flags, StackTrace Stack)
        {
            bool IsFlow = (Flags & EPropertyDirtFlags.Flow) != 0;// Layout
            bool IsBlock = (Flags & EPropertyDirtFlags.Box) != 0;
            bool IsVisual = (Flags & EPropertyDirtFlags.Visual) != 0;
            bool IsFont = (Flags & EPropertyDirtFlags.Text) != 0;

            // If the value that changed was a specified one and it affects the block then we need to update our block
            if (IsBlock && Stage >= EPropertyStage.Specified)
            {
                // Flag us dirty so we can resolve next time its called
                SetFlag(EPropertySystemDirtFlags.NeedsToResolveBlock);
                // Notify our parent by flagging them aswell
                // Owner.Box.Flag(EBoxInvalidationReason.Property_Changed);
                // Owner.Box.FlagProperty(Flags);
            }

            // Update our dirt flags appropriately
            if (IsFlow || IsVisual) SetFlag(EPropertySystemDirtFlags.NeedsToResolveBlock);
            if (IsFont) SetFlag(EPropertySystemDirtFlags.NeedsToResolveFont);

            //Logging.Log.Info("[Property Changed]: {0}", Prop.FieldName);
            onProperty_Change?.Invoke(Property, Flags, Stack);
        }

        /// <summary>
        /// An inheritable property of an element within our hierarchy has changed.
        /// </summary>
        /// <param name="Sender">The element whose property changed</param>
        /// <param name="Property"></param>
        public async Task Handle_Inherited_Property_Change_In_Hierarchy(Element Sender, ICssProperty Property)
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
            /* XXX: LOTS of room for speed improvement here. We should avoid using LINQ statements in performance critical areas like this. */
            var benchmark_id = Benchmark.Start("style-cascade");

            // Get a list of only the properties with an Assigned value
            AsyncCountdownEvent ctdn = null;
            /*
            HashSet<AtomicName<ECssPropertyID>> targetFields = new HashSet<AtomicName<ECssPropertyID>>();
            List<HashSet<AtomicName<ECssPropertyID>>> allFields = CssRules.Values.Select(x => { return x.SetProperties; }).ToList();
            */
            var targetFields = new FlagCollection<ECssPropertyID>((int)ECssPropertyID.MAX_VALUE);
            List<FlagCollection<ECssPropertyID>> allFields = CssRules.Values.Select(x => { return x.SetProperties; }).ToList();

            // Remove duplicates
            //foreach (HashSet<AtomicName<ECssPropertyID>> fields in allFields)
            foreach (FlagCollection<ECssPropertyID> fields in allFields)
            {
                targetFields.And(fields);
            }

            if (targetFields.ActiveFlags > 0)
            {
                // Cascade all those set values
                ctdn = new AsyncCountdownEvent(targetFields.ActiveFlags);

                // Loop over all target properties
                foreach (int flagIndex in targetFields)
                //Parallel.ForEach(targetFields, async (AtomicString propName) =>
                {
                    try
                    {
                        AtomicName<ECssPropertyID> propName = new AtomicName<ECssPropertyID>(flagIndex);
                        // Extract this property from every CssPropertySet that has a value for it
                        var propertyList = CssRules.Values.Select(x => { return x.Get(propName); }).ToList();

                        // Order these properties according to CSS 3.0 specifications
                        propertyList.Sort(CssPropertyComparator.Instance);

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
            ClearFlag(EPropertySystemDirtFlags.NeedsToCascade);
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

            var propertyList = new List<ICssProperty>(4);
            foreach (var propSet in CssRules.Values)
            {
                propertyList.Add(propSet[Property.CssName]);
            }

            // Order these properties according to CSS 3.0 specifications
            propertyList.Sort(CssPropertyComparator.Instance);

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
            if (owningElement.Box.IsAbsolutelyPositioned && owningElement.Box.IsReplacedElement)
            {
                // Width / Height depend on these
                Cascaded.Width.Update(true);
                Cascaded.Height.Update(true);
            }
        }
        #endregion

        #region Custom States
        public CssComputedStyle this[AtomicString State]
        {
            get
            {
                CssComputedStyle prop;
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
                SetFlag(EPropertySystemDirtFlags.NeedsToResolveBlock);
                Cascade();
            }
        }
        #endregion

        #region Unit Change Handler
        /// <summary>
        /// Notifys all dimension-properties which use the specified unit that its scale has changed and they need to update
        /// </summary>
        /// <param name="Unit"></param>
        public void Notify_Unit_Scale_Change(ECssUnit Unit)
        {
            if (Unit == ECssUnit.None) return;
            foreach (ICssProperty Property in Cascaded.Get_Set_Properties())
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
            if (!owningElement.Box.DependsOnContainer) return;

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



            Cascaded.Border_Top_Width.UpdateDependentOrAuto(true);
            Cascaded.Border_Right_Width.UpdateDependentOrAuto(true);
            Cascaded.Border_Bottom_Width.UpdateDependentOrAuto(true);
            Cascaded.Border_Left_Width.UpdateDependentOrAuto(true);

            Cascaded.Margin_Top.UpdateDependentOrAuto(true);
            Cascaded.Margin_Right.UpdateDependentOrAuto(true);
            Cascaded.Margin_Bottom.UpdateDependentOrAuto(true);
            Cascaded.Margin_Left.UpdateDependentOrAuto(true);

            Cascaded.Padding_Top.UpdateDependentOrAuto(true);
            Cascaded.Padding_Right.UpdateDependentOrAuto(true);
            Cascaded.Padding_Bottom.UpdateDependentOrAuto(true);
            Cascaded.Padding_Left.UpdateDependentOrAuto(true);

            Cascaded.Top.UpdateDependentOrAuto(true);
            Cascaded.Right.UpdateDependentOrAuto(true);
            Cascaded.Bottom.UpdateDependentOrAuto(true);
            Cascaded.Left.UpdateDependentOrAuto(true);

            Cascaded.Min_Width.UpdateDependentOrAuto(true);
            Cascaded.Min_Height.UpdateDependentOrAuto(true);

            Cascaded.Max_Width.UpdateDependentOrAuto(true);
            Cascaded.Max_Height.UpdateDependentOrAuto(true);

            Cascaded.Width.UpdateDependentOrAuto(true);
            Cascaded.Height.UpdateDependentOrAuto(true);
        }

        #endregion


        #region Font Updating
        /// <summary>
        /// Helpes us track changes to the font size of this element, to detect changes to font relative units
        /// </summary>
        double oldFontSize = -1;
        public void Resolve_Font()
        {
            if (0 == (StyleFlags & EPropertySystemDirtFlags.NeedsToResolveFont)) return;

            if (!(FontSize ==  oldFontSize))
            {
                oldFontSize = FontSize;
                Notify_Unit_Scale_Change(ECssUnit.EM);
                Notify_Unit_Scale_Change(ECssUnit.EX);
                Notify_Unit_Scale_Change(ECssUnit.CH);
            }

#if DISABLE_FONT_SYSTEM
#else
            // Get font from font factory, which will help cache identical fonts
            FontOptions fontOptions = new FontOptions(FontFamily, FontSize, FontWeight, FontStyle);
            Font = FontFactory.Get(fontOptions);

            // XXX: Find the new equivalent of the below code once the new rendering system is finished
            // Flag our elements font dirty flag so it updates whatever is using it
            //this.Flag(EElementDirtyFlags.Font);
#endif

            // Remove font dirt flag
            ClearFlag(EPropertySystemDirtFlags.NeedsToResolveFont);
        }
        #endregion

        #region Update_Blend_Color
        void Handle_Cascaded_Blend_Change(EPropertyStage Stage, ICssProperty Property)
        {
            if (Stage < EPropertyStage.Actual)// wait for the Actual value stage
                return;

            if (Opacity != 1.0)
            {// We have a useful blend color to set
                Blend_Color = new Color(new System.Numerics.Vector4(1f, 1f, 1f, (float)Opacity));
            }
            else// We DONT have a useful blend color
            {
                Blend_Color = null;
            }
        }
        #endregion

        #region Transform_Changed

        private void Handle_Cascaded_Transform_Change(EPropertyStage Stage, ICssProperty Property)
        {
            if (Stage < EPropertyStage.Actual)// wait for the Actual value stage
                return;

            throw new NotImplementedException("Transforms are not yet implemented");
        }
        #endregion

    }
}
