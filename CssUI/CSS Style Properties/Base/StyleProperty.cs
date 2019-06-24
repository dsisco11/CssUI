using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CssUI.CSS;

namespace CssUI
{
    /// <summary>
    /// Represents a CSS style property value which holds Assigned, Specified, and Computed values.
    /// </summary>
    public class StyleProperty : IStyleProperty
    {
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
        public AtomicString Source { get; set; } = null;
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
                    CssPropertyDefinition Def = Get_Definition();
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
        protected readonly PropertyOptions Options = new PropertyOptions();
        #endregion

        #region Accessors

        public bool HasValue { get { return !Assigned.IsNullOrUnset(); } }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// </summary>
        public virtual bool IsAuto { get { return Specified.Type == EStyleDataType.AUTO; } }
        /// <summary>
        /// Returns TRUE if the assigned value is <see cref="CssValue.Inherit"/>
        /// </summary>
        public virtual bool IsInherited { get { return Specified.Type == EStyleDataType.INHERIT; } }
        /// <summary>
        /// Returns TRUE if the assigned value has the <see cref="StyleValueFlags.Depends"/> flag
        /// </summary>
        public virtual bool IsDependent { get { return Specified.Has_Flags(StyleValueFlags.Depends); } }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// Returns TRUE if the assigned value has the <see cref="StyleValueFlags.Depends"/> flag
        /// </summary>
        public virtual bool IsDependentOrAuto { get { return (Specified.Type == EStyleDataType.AUTO || Specified.Has_Flags(StyleValueFlags.Depends)); } }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// Returns TRUE if the assigned value type is a percentage
        /// </summary>
        public virtual bool IsPercentageOrAuto { get { return (Specified.Type == EStyleDataType.AUTO || Specified.Type == EStyleDataType.PERCENT); } }
        
        /// <summary>
        /// Raw value assigned to the property from code.
        /// </summary>
        public CssValue Assigned
        {
            get { return _value; }
            set
            {
                if (Locked) throw new Exception("Cannot modify value of a locked property!");
                Options.CheckAndThrow(this, value);
                // Translate a value of NULL to CSSValue.Unset
                _value = (value == null ? CssValue.Null : value);
                Update();
            }
        }
        
        /// <summary>
        /// Value we USE for the property, which can differ from assigned value.
        /// Eg: If no value is Assigned then the propertys defined initial value will be used.
        /// </summary>
        public CssValue Specified
        {// SEE:  https://www.w3.org/TR/CSS2/cascade.html#specified-value
            get
            {
                if (_specified == null)
                {// Calculate the specified value
                    CssValue value = Assigned;
                    
                    CssPropertyDefinition Def = Get_Definition();
                    // If our Assigned value is NULL then we take that to mean 'use the CSS-defined default value'
                    // Values specifically set to StyleValue.Inherit get handled during the 'Computed' value resolution stage, NOT HERE!
                    // We are just resolving values that are 'Inherited' 
                    if ((Def != null && Def.Inherited && Assigned.IsNull()))
                    {
                        // First try and resolve the value with our assigned resolver function
                        // value = Inheritance_Resolver?.Invoke(this);
                        //if (value == null)
                        //{// Fallback on more direct methods if needed
                        if (FieldName != null)
                        {
                            IStyleProperty prop = Owner?.Parent?.Style.Final.Get(FieldName);
                            if (prop != null) value = (prop as StyleProperty).Computed;
                        }
                        //}
                        
                        // If we get NULL back then either the owning element has no parent(it is the root element) OR the element has no owner
                        // Either way we will use the property's initial value at this point
                    }

                    if (value.IsNull() || value == CssValue.Initial)
                    {
                        value = new CssValue(Initial);
                    }

                    _specified = value;
                }

                return _specified;
            }
        }

        /// <summary>
        /// The value as used for inheritence.
        /// The Specified value after being resolved to an absolute value, if possible
        /// </summary>
        public CssValue Computed
        {// SEE:  https://www.w3.org/TR/CSS2/cascade.html#computed-value
            get
            {
                if (_computed == null)
                {
                    CssValue value = Specified;
                    CssPropertyDefinition Def = Get_Definition();
                    switch (value.Type)
                    {
                        case EStyleDataType.PERCENT:
                            {
                                if (Def.Percentage_Resolver != null)
                                {
                                    double resolved = value.Resolve(null, (double Pct) => Def.Percentage_Resolver(Owner, Pct)).Value;
                                    value = CssValue.From_Number(resolved);
                                }
                                //else throw new ArgumentNullException(string.Concat("CssPropertyDefinition[\'", this.CssName, "\'].Percentage_Resolver"));
                            }
                            break;
                        case EStyleDataType.DIMENSION:
                            {
                                double? nv = value.Resolve(Get_Unit_Scale);
                                if (nv.HasValue)
                                {
                                    value = CssValue.From_Number(nv.Value);
                                }
                            }
                            break;
                        case EStyleDataType.INHERIT:// SEE:  https://www.w3.org/TR/CSS2/cascade.html#value-def-inherit
                            {// XXX: issue with inherited values atm is that they dont compute immediately once theyre parented
                                if (Owner is cssRootElement)// If 'inherit' is set on the root element the property is assigned it's initial value
                                {
                                    value = Def.Initial;
                                }
                                else
                                {
                                    var prop = Owner?.Parent?.Style.Final.Get(FieldName);
                                    if (prop != null) value = (prop as StyleProperty).Specified;
                                }
                            }
                            break;
                    }

                    return value;
                }

                return _computed;
            }
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
            return StyleUnitResolver.Get_Scale(Owner, Unit);
        }
        #endregion

        #region Cascade
        /// <summary>
        /// Overwrites the values of this instance with any values from another which aren't <see cref="CssValue.Null"/>
        /// </summary>
        /// <returns>Success</returns>
        [Obsolete("Non - asynchronous function are now obsolete, please use CascadeAsync instead.")]
        public bool Cascade(IStyleProperty prop)
        {// Circumvents locking
            StyleProperty o = prop as StyleProperty;
            bool changes = false;
            if (o.Assigned != CssValue.Null)
            {
                changes = true;
                _value = new CssValue(o.Assigned);

                this.Source = o.Source;
                this.Selector = o.Selector;
            }

            if (changes) Update();
            return changes;
        }

        /// <summary>
        /// Overwrites the values of this instance with any values from another which aren't <see cref="CssValue.Null"/>
        /// </summary>
        /// <returns>Success</returns>
        public async Task<bool> CascadeAsync(IStyleProperty prop)
        {// Circumvents locking
            StyleProperty o = prop as StyleProperty;
            bool changes = false;
            if (o.Assigned != CssValue.Null)
            {
                changes = true;
                _value = new CssValue(o.Assigned);

                this.Source = o.Source;
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
        [Obsolete("Non - asynchronous function are now obsolete, please use OverwriteAsync instead.")]
        public bool Overwrite(IStyleProperty prop)
        {// Circumvents locking
            StyleProperty o = prop as StyleProperty;
            bool changes = false;
            if (o.Assigned != Assigned)
            {
                changes = true;
                _value = new CssValue(o.Assigned);

                this.Source = o.Source;
                this.Selector = o.Selector;
            }

            if (changes) Update();
            return changes;
        }

        /// <summary>
        /// Overwrites the assigned value of this instance with values from another if they are different
        /// </summary>
        /// <returns>Success</returns>
        public async Task<bool> OverwriteAsync(IStyleProperty prop)
        {// Circumvents locking
            StyleProperty o = prop as StyleProperty;
            bool changes = false;
            if (o.Assigned != Assigned)
            {
                changes = true;
                _value = new CssValue(o.Assigned);

                this.Source = o.Source;
                this.Selector = o.Selector;
            }

            if (changes) Update();
            return await Task.FromResult(changes);
        }

        #endregion

        #region Constructors

        public StyleProperty(string CssName = null)
        {
            this.CssName = new AtomicString(CssName);
            Update();
        }

        public StyleProperty(bool Locked)
        {
            this.Locked = Locked;
            Update();
        }

        public StyleProperty(PropertyOptions Options)
        {
            this.Options = Options;
            Update();
        }

        public StyleProperty(string CssName, PropertyOptions Options)
        {
            this.CssName = new AtomicString(CssName);
            this.Options = Options;
            Update();
        }

        public StyleProperty(bool Locked, PropertyChangeDelegate onChange)
        {
            this.Locked = Locked;
            this.onChanged += onChange;
            Initial = CssValue.Null;
            Update();
        }

        public StyleProperty(bool Locked, PropertyOptions Options)
        {
            this.Options = Options;
            this.Locked = Locked;
            Initial = CssValue.Null;
            Update();
        }

        public StyleProperty(string CssName, bool Locked, PropertyOptions Options)
        {
            this.CssName = new AtomicString(CssName);
            this.Options = Options;
            this.Locked = Locked;
            Update();
        }

        public StyleProperty(string CssName, bool Locked, bool Unset, cssElement Owner, PropertyOptions Options)
        {
            this.CssName = new AtomicString(CssName);
            this.Owner = Owner;
            this.Options = Options;
            this.Locked = Locked;
            if (Unset) Assigned = CssValue.Null;
            Update();
        }

        public StyleProperty(string CssName, bool Locked, CssValue Initial, PropertyOptions Options)
        {
            this.CssName = new AtomicString(CssName);
            this.Options = Options;
            this.Locked = Locked;
            this.Initial = Initial;
            Update();
        }

        public StyleProperty(CssValue initial, PropertyChangeDelegate onChange) : base()
        {
            this.onChanged += onChange;
            Initial = (initial == null ? CssValue.Null : initial);
            Update();
        }

        public StyleProperty(CssValue initial, PropertyChangeDelegate onChange, PropertyOptions Options) : base()
        {
            this.onChanged += onChange;
            Initial = (initial == null ? CssValue.Null : initial);
            this.Options = Options;
            Update();
        }

        public StyleProperty(CssValue initial, PropertyChangeDelegate onChange, bool Locked) : base()
        {
            this.Locked = Locked;
            this.onChanged += onChange;
            Initial = (initial == null ? CssValue.Null : initial);
            Update();
        }

        public StyleProperty(CssValue initial, bool Locked, PropertyOptions Options) : base()
        {
            this.Options = Options;
            this.Locked = Locked;
            Initial = (initial == null ? CssValue.Null : initial);
            Update();
        }

        public StyleProperty(CssValue initial, PropertyChangeDelegate onChange, bool Locked, PropertyOptions Options) : base()
        {
            this.Options = Options;
            this.Locked = Locked;
            this.onChanged += onChange;
            Initial = (initial == null ? CssValue.Null : initial);
            Update();
        }

        public StyleProperty(CssValue initial, cssElement Owner, bool Locked, PropertyOptions Options) : base()
        {
            this.Owner = Owner;
            this.Options = Options;
            this.Locked = Locked;
            Initial = (initial == null ? CssValue.Null : initial);
            Update();
        }

        public StyleProperty(CssValue initial, PropertyChangeDelegate onChange, NamedProperty Parent, string Name)
        {
            this.FieldName = new AtomicString(Name);
            this.onChanged += onChange;
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
        /// Gets the definition for this property
        /// </summary>
        CssPropertyDefinition Get_Definition()
        {
            if (this.CssName == null) return null;
            return CssProperties.Definitions[this.CssName];
        }

        /// <summary>
        /// Calculates the 'Assigned' value
        /// </summary>
        protected void Update()
        {
            _specified = null;// unset our specified value so it gets updated...
            _computed = null;// it only makes sense to update the computed value aswell

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
