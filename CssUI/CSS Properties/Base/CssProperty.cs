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
        CssValue _value = CssValue.Null;// A properties assigned value defaults to not being set
        CssValue _specified = null;
        CssValue _computed = null;

        /// <summary>
        /// The UI element which contains this property
        /// </summary>
        public cssElement Owner { get; protected set; } = null;
        /// <summary>
        /// The propertys identifier token in stylesheets.
        /// <para>EG; "box-sizing", "margin-left", "margin-top", etc </para>
        /// </summary>
        public AtomicString CssName { get; protected set; } = null;
        /// <summary>
        /// Callback for when any value stage of this property changes
        /// </summary>
        public event Action<ECssPropertyStage, ICssProperty> onValueChange;

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
        CssValue oldAssignedValue = null;
        /// <summary>
        /// Tracks the previous value for <see cref="Computed"/> so we can detect when changes occur
        /// </summary>
        CssValue oldComputedValue = null;

        /// <summary>
        /// All flags which are present for all currently computed <see cref="CssValue"/>'s
        /// </summary>
        public StyleValueFlags Flags { get { return Specified.Flags; } }

        /// <summary>
        /// Options which dictate how this property acts and what values it can accept
        /// </summary>
        protected readonly CssPropertyOptions Options = new CssPropertyOptions();
        #endregion

        #region Accessors

        public bool HasValue { get { return !Assigned.IsNull(); } }
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
        /// CSS standards call this the Cascaded value
        /// </summary>
        public CssValue Assigned
        {
            get { return _value; }
            set
            {
                if (Locked) throw new Exception("Cannot modify the value of a locked css property!");
                Options.CheckAndThrow(this, value);
                // Translate a value of NULL to CSSValue.Null
                _value = (object.ReferenceEquals(value, null) ? CssValue.Null : value);
                //our assigned value has changed, this means our specified and computed valued are now incorrect.
                Update();
            }
        }

        /// <summary>
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

                    if (ReferenceEquals(oldComputedValue, null) || oldComputedValue != Assigned)
                    {
                        oldComputedValue = new CssValue(_computed);
                        onValueChange?.Invoke(ECssPropertyStage.Computed, this);
                    }
                }

                return _computed;
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task<CssValue> Calculate_Specified()
        {// SEE:  https://www.w3.org/TR/css-cascade-3/#specified
            CssPropertyDefinition Def = Definition;


#if RELEASE
            try
            {
#endif
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
                            ICssProperty prop = Owner.Parent.Style.Cascaded.Get(CssName);
                            if (prop != null)
                                return new CssValue((prop as CssProperty).Computed);
                            else
                                return null;
                        }
                    }

                    if (Assigned == CssValue.Initial)
                    {// If the Assigned value is the CssValue.Initial literal, then we use our definitions default
                        return new CssValue(Def.Initial);
                    }

                    return Assigned;
                }
                // Assigned is null
                /*
                * CSS Specs:
                * 2. if the property is inherited and the element is not the root of the document tree, use the computed value of the parent element.
                */
                if (!(Owner is cssRootElement) && Def != null && Def.Inherited)
                {
                    ICssProperty prop = Owner.Parent.Style.Cascaded.Get(CssName);
                    if (prop != null)
                        return new CssValue((prop as CssProperty).Computed);
                    else
                        throw new Exception($"CSS property '{CssName}' Cascaded value is null!");
                }
                else
                {
                    /*
                    * CSS Specs:
                    * 3. Otherwise use the property's initial value. The initial value of each property is indicated in the property's definition.
                    */
                    return new CssValue(Def.Initial);
                }
#if RELEASE
            }
            catch(Exception ex)
            {
                xLog.Log.Error(ex);
                throw;
            }
#endif

            // this sucks but its all we can do
            throw new Exception($"Failed to resolve the Specified value in {nameof(CssProperty)}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task<CssValue> Calculate_Computed()
        {// SEE:  https://www.w3.org/TR/css-cascade-3/#computed
            CssPropertyDefinition Def = Definition;

#if RELEASE
            try
            {
#endif
                switch (Specified.Type)
                {
                    case EStyleDataType.PERCENT:
                        {
                            if (Def.Percentage_Resolver != null)
                            {
                                double resolved = Specified.Resolve(null, (double Pct) => Def.Percentage_Resolver(Owner, Pct)).Value;
                                return CssValue.From_Number(resolved);
                            }
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
                        {
                            if (Owner is cssRootElement)// If 'inherit' is set on the root element the property is assigned it's initial value
                            {
                                return new CssValue( Def.Initial );
                            }
                            else
                            {
                                var prop = Owner.Parent.Style.Cascaded.Get(CssName);
                                if (prop != null)
                                    return new CssValue( (prop as CssProperty).Computed );
                            }
                        }
                        break;
                    case EStyleDataType.UNSET:// SEE:  https://www.w3.org/TR/css-cascade-4/#valdef-all-unset
                        {
                            /*
                            * CSS Specs:
                            * If the cascaded value of a property is the unset keyword, 
                            * then if it is an inherited property, this is treated as inherit, and if it is not, this is treated as initial. 
                            * This keyword effectively erases all declared values occurring earlier in the cascade, 
                            * correctly inheriting or not as appropriate for the property (or all longhands of a shorthand).
                            */
                            if (!(Owner is cssRootElement) && Def != null && Def.Inherited)
                            {
                                ICssProperty prop = Owner.Parent.Style.Cascaded.Get(CssName);
                                if (prop != null)
                                    return new CssValue((prop as CssProperty).Computed);
                            }
                            else
                            {
                                return new CssValue(Def.Initial);
                            }
                        }
                        break;
                }

                return new CssValue(Specified);
#if RELEASE
            }
            catch (Exception ex)
            {
                xLog.Log.Error(ex);
                throw;
            }
#endif

            throw new Exception($"Failed to resolve the Computed value in {nameof(CssProperty)}");
        }
#endregion

#region Unit Resolver
        /// <summary>
        /// Allows external code to notify this property that a certain unit type has changed scale and if we have a value which uses that unit-type we need to fire our Changed event because our Computed value will be different
        /// </summary>
        public void Handle_Unit_Change(EStyleUnit Unit)
        {
            if (Specified.Unit == Unit)
            {
                // This unit change will affect our computed value
                onValueChange.Invoke(ECssPropertyStage.Computed, this);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<bool> CascadeAsync(ICssProperty prop)
        {// Circumvents locking
            CssProperty o = prop as CssProperty;
            bool changes = false;
            if (o.Assigned.HasValue())
            {
                changes = true;
                // Don't make a copy of the value, they are readonly anyhow
                //_value = new CssValue(o.Assigned);
                _value = o.Assigned;

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<bool> OverwriteAsync(ICssProperty prop)
        {
            CssProperty o = prop as CssProperty;
            bool changes = false;
            if (o.Assigned != Assigned)
            {
                changes = true;
                //_value = new CssValue(o.Assigned);
                // Don't make a copy of the value, they are readonly anyhow
                _value = o.Assigned;

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

        /*public CssProperty(bool Locked, PropertyChangeDelegate onChange)
        {
            this.Locked = Locked;
            this.onValueChange += onChange;
            Update();
        }*/

        public CssProperty(bool Locked, CssPropertyOptions Options)
        {
            this.Options = Options;
            this.Locked = Locked;
            Update();
        }

        public CssProperty(string CssName, bool Locked, CssPropertyOptions Options)
        {
            this.CssName = new AtomicString(CssName);
            this.Options = Options;
            this.Locked = Locked;
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
        
#endregion

#region Has Flags
        public bool Has_Flags(StyleValueFlags Flags) { return (this.Flags & Flags) > 0; }
#endregion

#region ToString
        public override string ToString() { return $"{CssName}: {Assigned.ToString()}"; }
        #endregion


        #region Updating
        /// <summary>
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public async Task Update(bool ComputeNow = false)
        {
            _specified = null;// unset our specified value so it gets updated...
            _computed = null;// it only makes sense to update the computed value aswell

            if (ComputeNow)
            {
                _specified = await Calculate_Specified();
                _computed = await Calculate_Computed();
            }


            if (ReferenceEquals(oldAssignedValue, null) || oldAssignedValue != Assigned)
            {
                oldAssignedValue = new CssValue(Assigned);
                onValueChange?.Invoke(ECssPropertyStage.Assigned, this);
            }
        }

        /// <summary>
        /// If the Assigned value is one that depends on another value for its final value then
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public async Task UpdateDependent(bool ComputeNow = false)
        {
            if (this.IsDependent)
                await Update(ComputeNow);
        }

        /// <summary>
        /// If the Assigned value is one that depends on another value for its final value OR is <see cref="CssValue.Auto"/> then
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public async Task UpdateDependentOrAuto(bool ComputeNow = false)
        {
            if (this.IsDependentOrAuto)
                await Update(ComputeNow);
        }

        /// <summary>
        /// If the Assigned value is a percentage OR is <see cref="CssValue.Auto"/> then
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public async Task UpdatePercentageOrAuto(bool ComputeNow = false)
        {
            if (this.IsPercentageOrAuto)
                await Update(ComputeNow);
        }
        #endregion

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
