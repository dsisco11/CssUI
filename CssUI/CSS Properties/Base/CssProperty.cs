using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CssUI.CSS;

namespace CssUI
{
    /// <summary>
    /// Represents a CSS property value which holds Assigned, Specified, and Computed values.
    /// </summary>
    public class CssProperty : ICssProperty
    {// DOCS: https://www.w3.org/TR/CSS22/cascade.html#usedValue
        #region Properties
        CssValue _initial = CssValue.Null;// The CSS-defined default(initial) value
        CssValue _value = CssValue.Initial;// A properties assigned value starts out as it's CSS-defined default.
        CssValue _specified = null;
        CssValue _computed = null;

        /// <summary>
        /// The UI element which contains this property
        /// </summary>
        public cssElement Owner { get; protected set; } = null;
        /// <summary>
        /// The propertys field-name in whatever class is holding it.
        /// <para>If FullName were "Margins.Left" then this would be "Left"</para>
        /// </summary>
        public AtomicString FieldName { get; set; } = null;
        /// <summary>
        /// The propertys identifier token in stylesheets.
        /// <para>EG; "box-sizing", "margin-left", "margin-top", etc </para>
        /// </summary>
        public AtomicString CssName { get; protected set; } = null;
        /// <summary>
        /// Callback for when the computed value of this property changes
        /// </summary>
        public event PropertyChangeDelegate onChanged;
        /// <summary>
        /// Tracks which styling rule block this property came from
        /// </summary>
        public WeakReference<CssPropertySet> SourcePtr { get; set; } = null;
        /// <summary>
        /// Tracks which styling rule block this property came from
        /// </summary>
        public CssSelector Selector { get; set; } = null;

        /// <summary>
        /// If true then this propertys values cannot be set externally
        /// </summary>
        public readonly bool Locked = false;


        /// <summary>
        /// Tracks the previous value for <see cref="Assigned"/> so we can detect when changes occur
        /// </summary>
        CssValue oldValue = null;

        /// <summary>
        /// All flags which are present for all currently computed <see cref="CssValue"/>'s
        /// </summary>
        public StyleValueFlags Flags { get { return Specified.Flags; } }
        /// <summary>
        /// Value that is used if our Assigned value computes to <see cref="CssValue.Initial"/>
        /// Meaning the property's default value according to the CSS documentation.
        /// </summary>
        public CssValue Initial
        {
            get
            {
                if (_initial.IsNullOrUnset())
                {
                    CssPropertyDefinition Def = Definition;
                    if (Def != null)
                    {
                        if (Def.Initial == null) throw new Exception("Property definition has no initial value defined!");
                        return Def?.Initial;
                    }

                    return CssValue.Null;// If the property has NO CSS-defined default value then return 'CSSValue.Unset'
                }

                return _initial;
            }
            private set
            {
                _initial = value;
            }
        }
        /// <summary>
        /// Options which dictate how this property acts and what values it can accept
        /// </summary>
        protected readonly CssPropertyOptions Options = new CssPropertyOptions();
        #endregion

        #region Accessors

        public bool HasValue { get { return !Assigned.IsNullOrUnset(); } }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// </summary>
        public virtual bool IsAuto { get { return Assigned.Type == EStyleDataType.AUTO; } }
        /// <summary>
        /// Returns TRUE if the assigned value is <see cref="CssValue.Inherit"/>
        /// </summary>
        public virtual bool IsInherited { get { return Assigned.Type == EStyleDataType.INHERIT; } }
        /// <summary>
        /// Returns TRUE if the assigned value has the <see cref="StyleValueFlags.Depends"/> flag
        /// </summary>
        public virtual bool IsDependent { get { return Assigned.Has_Flags(StyleValueFlags.Depends); } }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// Returns TRUE if the assigned value has the <see cref="StyleValueFlags.Depends"/> flag
        /// </summary>
        public virtual bool IsDependentOrAuto { get { return (Assigned.Type == EStyleDataType.AUTO || Assigned.Has_Flags(StyleValueFlags.Depends)); } }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// Returns TRUE if the assigned value type is a percentage
        /// </summary>
        public virtual bool IsPercentageOrAuto { get { return (Assigned.Type == EStyleDataType.AUTO || Assigned.Type == EStyleDataType.PERCENT); } }

        public CssPropertySet Source
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                this.SourcePtr.TryGetTarget(out CssPropertySet src);
                return src;
            }
        }

        public CssPropertyDefinition Definition
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (string.IsNullOrEmpty(this.CssName))
                    return null;
                return CssProperties.Definitions[this.CssName];
            }
        }
        #endregion

        #region Values
        /// <summary>
        /// Raw value assigned to the property from the cascade process.
        /// </summary>
        public CssValue Assigned
        {
            get { return _value; }
            set
            {
                if (Locked) throw new Exception("Cannot modify the value of a locked css property!");
                Options.CheckAndThrow(this, value);
                // Translate a value of NULL to CSSValue.Unset
                _value = (value == null ? CssValue.Null : value);
                //our assigned value has changed, this means our specified and computed valued are now incorrect.
                Update();
            }
        }

        /// <summary>
        /// Value we USE for the property, which can differ from assigned value.
        /// Eg: If no value is Assigned then the properties defined initial value will be used.
        /// </summary>
        public CssValue Specified
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_specified == null)
                {
                    _specified = Calculate_Specified().Result;
                }

                return _specified;
            }
        }

        /// <summary>
        /// The value as used for inheritence.
        /// The Specified value after being resolved to an absolute value, if possible
        /// </summary>
        public CssValue Computed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_computed == null)
                {
                    _computed = Calculate_Computed().Result;
                }

                return _computed;
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task<CssValue> Calculate_Specified()
        {// SEE:  https://www.w3.org/TR/CSS2/cascade.html#specified-value
            CssPropertyDefinition Def = Definition;

            // CSS specs say if the cascade (assigned) resulted in a value, use it.
            if (!Assigned.IsNull())
            {
                if (Assigned == CssValue.Inherit)
                {
                    /*
                     * CSS Specs: 
                     * 1. Each property may also have a cascaded value of 'inherit', which means that, 
                     *    for a given element, the property takes as specified value the computed value of the element's parent.
                     *    If the 'inherit' value is set on the root element, the property is assigned its initial value.
                    */
                    if (Owner is cssRootElement)
                    {// This is the Root element
                        return new CssValue(Def.Initial);
                    }
                    else
                    {// Take our parents computed value
                        ICssProperty prop = Owner.Parent.Style.Cascaded.Get(FieldName);
                        if (prop != null)
                            return new CssValue((prop as CssProperty).Computed);
                        else
                            return null;
                    }
                }

                if (Assigned == CssValue.Initial)
                {// If the Assigned value is the CssValue.Initial literal, then we use our definitions default
                    return new CssValue( Def.Initial );
                }

                return Assigned;
            }
            else// Assigned is null
            {
                /*
                * CSS Specs:
                * 2. if the property is inherited and the element is not the root of the document tree, use the computed value of the parent element.
                */
                if (!(Owner is cssRootElement) && Def != null && Def.Inherited)
                {
                    ICssProperty prop = Owner.Parent.Style.Cascaded.Get(FieldName);
                    if (prop != null)
                        return new CssValue((prop as CssProperty).Computed);
                }
                else
                {
                    /*
                    * CSS Specs:
                    * 3. Otherwise use the property's initial value. The initial value of each property is indicated in the property's definition.
                    */
                    return new CssValue(Def.Initial);
                }
            }

            // this sucks but its all we can do
            throw new Exception($"Failed to resolve a Specified value in {nameof(CssProperty)}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task<CssValue> Calculate_Computed()
        {// SEE:  https://www.w3.org/TR/CSS22/cascade.html#computed-value
            CssPropertyDefinition Def = Definition;

            switch (Specified.Type)
            {
                case EStyleDataType.PERCENT:
                    {
                        if (Def.Percentage_Resolver != null)
                        {
                            double resolved = Specified.Resolve(null, (double Pct) => Def.Percentage_Resolver(Owner, Pct)).Value;
                            return CssValue.From_Number(resolved);
                        }
                        //else throw new ArgumentNullException(string.Concat("CssPropertyDefinition[\'", this.CssName, "\'].Percentage_Resolver"));
                    }
                    break;
                case EStyleDataType.DIMENSION:
                    {
                        double? nv = Specified.Resolve(Get_Unit_Scale);
                        if (nv.HasValue)
                        {
                            return CssValue.From_Number(nv.Value);
                        }
                    }
                    break;
                case EStyleDataType.INHERIT:// SEE:  https://www.w3.org/TR/CSS2/cascade.html#value-def-inherit
                    {// XXX: issue with inherited values atm is that they dont compute immediately once theyre parented
                        if (Owner is cssRootElement)// If 'inherit' is set on the root element the property is assigned it's initial value
                        {
                            return new CssValue( Def.Initial );
                        }
                        else
                        {
                            var prop = Owner.Parent.Style.Cascaded.Get(FieldName);
                            if (prop != null)
                                return new CssValue( (prop as CssProperty).Computed );
                        }
                    }
                    break;
            }

            return new CssValue(Specified);
        }
        #endregion

        #region Unit Resolver
        /// <summary>
        /// Allows external code to notify this property that a certain unit type has changed scale and if we have a value which uses that unit-type we need to fire our Changed event because our Computed value will be different
        /// </summary>
        public void Notify_Unit_Change(EStyleUnit Unit)
        {
            if (Specified.Unit == Unit)
            {
                onChanged.Invoke(this);
            }
        }

        private double Get_Unit_Scale(EStyleUnit Unit)
        {
            return StyleUnitResolver.Get_Scale(Owner, this, Unit);
        }
        #endregion

        #region Cascade
        /// <summary>
        /// Overwrites the values of this instance with any values from another which aren't <see cref="CssValue.Null"/>
        /// </summary>
        /// <returns>Success</returns>
        public async Task<bool> CascadeAsync(ICssProperty prop)
        {// Circumvents locking
            CssProperty o = prop as CssProperty;
            bool changes = false;
            //if (o.Assigned != CssValue.Null)
            if (!o.Assigned.IsNullOrUnset())
            {
                changes = true;
                _value = new CssValue(o.Assigned);

                this.SourcePtr = o.SourcePtr;
                this.Selector = o.Selector;
            }

            if (changes) Update();
            return await Task.FromResult(changes);
        }

        #endregion

        #region Overwrite
        /// <summary>
        /// Overwrites the assigned value of this instance with values from another if they are different
        /// </summary>
        /// <returns>Success</returns>
        public async Task<bool> OverwriteAsync(ICssProperty prop)
        {// Circumvents locking
            CssProperty o = prop as CssProperty;
            bool changes = false;
            if (o.Assigned != Assigned)
            {
                changes = true;
                _value = new CssValue(o.Assigned);

                this.SourcePtr = o.SourcePtr;
                this.Selector = o.Selector;
            }

            if (changes) Update();
            return await Task.FromResult(changes);
        }

        #endregion

        #region Constructors

        public CssProperty(string CssName = null)
        {
            this.CssName = new AtomicString(CssName);
            Update();
        }

        public CssProperty(bool Locked)
        {
            this.Locked = Locked;
            Update();
        }

        public CssProperty(CssPropertyOptions Options)
        {
            this.Options = Options;
            Update();
        }

        public CssProperty(string CssName, CssPropertyOptions Options)
        {
            this.CssName = new AtomicString(CssName);
            this.Options = Options;
            Update();
        }

        public CssProperty(bool Locked, PropertyChangeDelegate onChange)
        {
            this.Locked = Locked;
            this.onChanged += onChange;
            Initial = CssValue.Null;
            Update();
        }

        public CssProperty(bool Locked, CssPropertyOptions Options)
        {
            this.Options = Options;
            this.Locked = Locked;
            Initial = CssValue.Null;
            Update();
        }

        public CssProperty(string CssName, bool Locked, CssPropertyOptions Options)
        {
            this.CssName = new AtomicString(CssName);
            this.Options = Options;
            this.Locked = Locked;
            Update();
        }

        [Obsolete("Please specify the properties Source")]
        public CssProperty(string CssName, bool Locked, bool Unset, cssElement Owner, CssPropertyOptions Options)
        {
            this.CssName = new AtomicString(CssName);
            this.Owner = Owner;
            this.Options = Options;
            this.Locked = Locked;
            if (Unset) Assigned = CssValue.Null;
            Update();
        }

        public CssProperty(string CssName, bool Locked, bool Unset, WeakReference<CssPropertySet> Source, cssElement Owner, CssPropertyOptions Options)
        {
            this.CssName = new AtomicString(CssName);
            this.Owner = Owner;
            this.SourcePtr = Source;
            this.Options = Options;
            this.Locked = Locked;
            if (Unset) Assigned = CssValue.Null;
            Update();
        }

        public CssProperty(string CssName, bool Locked, CssValue Initial, CssPropertyOptions Options)
        {
            this.CssName = new AtomicString(CssName);
            this.Options = Options;
            this.Locked = Locked;
            this.Initial = Initial;
            Update();
        }

        public CssProperty(CssValue initial, PropertyChangeDelegate onChange) : base()
        {
            this.onChanged += onChange;
            Initial = (initial == null ? CssValue.Null : initial);
            Update();
        }

        public CssProperty(CssValue initial, PropertyChangeDelegate onChange, CssPropertyOptions Options) : base()
        {
            this.onChanged += onChange;
            Initial = (initial == null ? CssValue.Null : initial);
            this.Options = Options;
            Update();
        }

        public CssProperty(CssValue initial, PropertyChangeDelegate onChange, bool Locked) : base()
        {
            this.Locked = Locked;
            this.onChanged += onChange;
            Initial = (initial == null ? CssValue.Null : initial);
            Update();
        }

        public CssProperty(CssValue initial, bool Locked, CssPropertyOptions Options) : base()
        {
            this.Options = Options;
            this.Locked = Locked;
            Initial = (initial == null ? CssValue.Null : initial);
            Update();
        }

        public CssProperty(CssValue initial, PropertyChangeDelegate onChange, bool Locked, CssPropertyOptions Options) : base()
        {
            this.Options = Options;
            this.Locked = Locked;
            this.onChanged += onChange;
            Initial = (initial == null ? CssValue.Null : initial);
            Update();
        }

        public CssProperty(CssValue initial, cssElement Owner, bool Locked, CssPropertyOptions Options) : base()
        {
            this.Owner = Owner;
            this.Options = Options;
            this.Locked = Locked;
            Initial = (initial == null ? CssValue.Null : initial);
            Update();
        }

        #endregion

        #region Has Flags
        public bool Has_Flags(StyleValueFlags Flags) { return (this.Flags & Flags) > 0; }
        #endregion

        #region ToString
        public override string ToString() { return Specified.ToString(); }
        #endregion


        /// <summary>
        /// Calculates the 'Assigned' and 'Computed' values
        /// </summary>
        public async Task Update()
        {
            _specified = null;// unset our specified value so it gets updated...
            _computed = null;// it only makes sense to update the computed value aswell

            // XXX: if we move to the new parenting system this check will not be necessary
            if ( Owner.HasFlags(EElementFlags.ReadyForStyle) )
            {
                _specified = await Calculate_Specified();
                _computed = await Calculate_Computed();
            }

            if (oldValue == null || oldValue != Assigned)
            {
                oldValue = new CssValue(Assigned);
                onChanged?.Invoke(this);
            }
        }


        #region Explicit
        /// <summary>
        /// Sets the <see cref="Assigned"/> value for this property
        /// </summary>
        /// <param name="value"></param>
        public void Set(CssValue newValue)
        {
            if (Assigned != newValue)
            {
                Assigned = newValue;
            }
        }
        #endregion

    }
}
