using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CssUI.CSS;
using CssUI.Internal;

namespace CssUI
{

    /// <summary>
    /// Represents a CSS property and manages all of its value states: Assigned, Specified, and Computed values.
    /// </summary>
    public class CssProperty : CssPropertyBase, ICssProperty
    {// DOCS: https://www.w3.org/TR/css-cascade-3/#cascaded
        
        #region backing values
        /// <summary>
        /// Backing value for <see cref="Assigned"/>
        /// <para> Docs: https://www.w3.org/TR/css-cascade-3/#cascaded </para>
        /// </summary>
        CssValue _assigned = CssValue.Null;

        /// <summary>
        /// Backing value for <see cref="Specified"/>
        /// <para> Docs: https://www.w3.org/TR/css-cascade-3/#specified </para>
        /// </summary>
        CssValue _specified = null;

        /// <summary>
        /// Backing value for <see cref="Computed"/>
        /// <para> Docs: https://www.w3.org/TR/css-cascade-3/#computed </para>
        /// </summary>
        CssValue _computed = null;

        /// <summary>
        /// Backing value for <see cref="Used"/>
        /// <para> Docs: https://www.w3.org/TR/css-cascade-3/#used </para>
        /// </summary>
        CssValue _used = null;

        /// <summary>
        /// Backing value for <see cref="Actual"/>
        /// <para> Docs: https://www.w3.org/TR/css-cascade-3/#actual </para>
        /// </summary>
        CssValue _actual = null;
        #endregion

        #region value trackers
        /// <summary>
        /// Tracks the previous value for <see cref="Assigned"/> so we can detect when changes occur
        /// </summary>
        CssValueHash oldAssigned = new CssValueHash();
        /// <summary>
        /// Tracks the previous value for <see cref="Specified"/> so we can detect when changes occur
        /// </summary>
        CssValueHash oldSpecified = new CssValueHash();
        /// <summary>
        /// Tracks the previous value for <see cref="Computed"/> so we can detect when changes occur
        /// </summary>
        CssValueHash oldComputed = new CssValueHash();
        /// <summary>
        /// Tracks the previous value for <see cref="Used"/> so we can detect when changes occur
        /// </summary>
        CssValueHash oldUsed = new CssValueHash();
        /// <summary>
        /// Tracks the previous value for <see cref="Actual"/> so we can detect when changes occur
        /// </summary>
        CssValueHash oldActual = new CssValueHash();
        #endregion

        #region Values
        /// <summary>
        /// Raw value assigned to the property from the cascade process.
        /// CSS standards call this the Cascaded value
        /// This is the value that the stylesheet gave us(could be no value at all)
        /// </summary>
        public CssValue Assigned
        {
            get { return _assigned; }
            set
            {
                if (Locked) throw new Exception("Cannot modify the value of a locked css property!");
                Definition.CheckAndThrow(this, value);
                // Translate a value of NULL to CSSValue.Null
                _assigned = (object.ReferenceEquals(value, null) ? CssValue.Null : value);
                //our assigned value has changed, this means our specified and computed valued are now incorrect.
                Update();
            }
        }

        /// <summary>
        /// The value we interpreted from <see cref="Assigned"/>
        /// </summary>
        public CssValue Specified
        {
            get
            {
                if (_specified == null)
                {
                    Reinterpret_Specified();
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
            get
            {
                if (_computed == null)
                {
                    Reinterpret_Computed();
                }

                return _computed;
            }
        }

        /// <summary>
        /// The final calculated value after applying a property specific resolution method to it
        /// </summary>
        public CssValue Used
        {
            get
            {
                if (_used == null)
                {
                    Reinterpret_Used();
                }

                return _used;
            }
        }

        /// <summary>
        /// The value that will be used by the element
        /// <para>This is the <see cref="Used"/> value with system restrictions placed on it</para>
        /// </summary>
        public CssValue Actual
        {
            get
            {
                if (_actual == null)
                {
                    Reinterpret_Actual();
                }

                return _actual;
            }
        }
        #endregion

        #region Accessors

        public override bool HasValue { get => !Assigned.IsNull(); }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// </summary>
        public override bool IsAuto { get => Assigned.Type == EStyleDataType.AUTO; }
        /// <summary>
        /// Returns TRUE if the assigned value is <see cref="CssValue.Inherit"/>
        /// </summary>
        public override bool IsInherited { get => Assigned.Type == EStyleDataType.INHERIT; }
        /// <summary>
        /// Returns TRUE if the assigned value has the <see cref="StyleValueFlags.Depends"/> flag
        /// </summary>
        public override bool IsDependent { get => Assigned.Has_Flags(StyleValueFlags.Depends); }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// Returns TRUE if the assigned value has the <see cref="StyleValueFlags.Depends"/> flag
        /// </summary>
        public override bool IsDependentOrAuto { get => (Assigned.Type == EStyleDataType.AUTO || Assigned.Has_Flags(StyleValueFlags.Depends)); }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// Returns TRUE if the assigned value type is a percentage
        /// </summary>
        public override bool IsPercentageOrAuto { get => (Assigned.Type == EStyleDataType.AUTO || Assigned.Type == EStyleDataType.PERCENT); }

        /// <summary>
        /// All flags which are present for all currently computed <see cref="CssValue"/>'s
        /// </summary>
        public override StyleValueFlags Flags { get { return Specified.Flags; } }
        #endregion

        #region Constructor
        public CssProperty(string CssName, bool Locked, WeakReference<CssPropertySet> Source, cssElement Owner) 
            : base(CssName, Locked, Source, Owner)
        {
        }
        #endregion

        #region Interpreting

        /// <summary>
        /// Reinterprets our <see cref="Specified"/> value
        /// </summary>
        private void Reinterpret_Specified()
        {
            _specified = Assigned.Derive_SpecifiedValue(this);
            // detect changes, fire events
            if (ReferenceEquals(oldSpecified, null) || oldSpecified != _specified)
            {// the computed value changed
                oldSpecified.Set(_specified);
                // FireValueChangeEvent(ECssPropertyStage.Specified);

                // update the Computed value
                Reinterpret_Computed();
            }
        }

        private void Reinterpret_Computed()
        {
            _computed = Specified.Derive_ComputedValue(this);
            // detect changes, fire events
            if (ReferenceEquals(oldComputed, null) || oldComputed != _computed)
            {
                oldComputed.Set(_computed);
                // FireValueChangeEvent(ECssPropertyStage.Computed);

                // Update the Used value
                Reinterpret_Used();
            }
        }

        private void Reinterpret_Used()
        {
            _used = Computed.Derive_UsedValue(this);
            // detect changes, fire events
            if (ReferenceEquals(oldUsed, null) || oldUsed != _used)
            {
                oldUsed.Set(_used);
                // FireValueChangeEvent(ECssPropertyStage.Used);

                // update the Actual value
                Reinterpret_Actual();
            }
        }

        private void Reinterpret_Actual()
        {
            _actual = Used.Derive_ActualValue(this);
            // detect changes, fire events
            if (ReferenceEquals(oldActual, null) || oldActual != _actual)
            {
                oldActual.Set(_actual);
                FireValueChangeEvent(ECssPropertyStage.Actual);
            }
        }
        #endregion

        /*#region Inherited Value
        /// <summary>
        /// Returns the inherited value from the properties owners parent element
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override CssValue Find_Inherited_Value()
        {
            if (Owner is cssRootElement)
            {// Root elements cannot inherit, they use the INITIAL value
                return new CssValue(Definition.Initial);
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
        #endregion*/

        #region Unit Resolver
        /// <summary>
        /// Allows external code to notify this property that a certain unit type has changed scale and if we have a value which uses that unit-type we need to fire our Changed event because our Computed value will be different
        /// </summary>
        public override void Handle_Unit_Change(EStyleUnit Unit)
        {
            if (Specified.Unit == Unit)
            {// We are using this unit and its change will affect our computed value
                FireValueChangeEvent(ECssPropertyStage.Computed);
            }
        }
        #endregion

        #region Cascade
        /// <summary>
        /// Overwrites the values of this instance with any values from another which aren't <see cref="CssValue.Null"/>
        /// </summary>
        /// <returns>Success</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override async Task<bool> CascadeAsync(ICssProperty prop)
        {// Circumvents locking
            CssProperty o = prop as CssProperty;
            bool changes = false;
            if (o.Assigned.HasValue())
            {
                changes = true;
                // Don't make a copy of the value, they are readonly anyhow
                //_value = new CssValue(o.Assigned);
                _assigned = o.Assigned;

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
        public override async Task<bool> OverwriteAsync(ICssProperty prop)
        {
            CssProperty o = prop as CssProperty;
            bool changes = false;
            if (o.Assigned != Assigned)
            {
                changes = true;
                //_value = new CssValue(o.Assigned);
                // Don't make a copy of the value, they are readonly anyhow
                _assigned = o.Assigned;

                this.SourcePtr = o.SourcePtr;
                this.Selector = o.Selector;
            }

            if (changes) Update();
            return await Task.FromResult(changes);
        }
        #endregion

        #region Has Flags
        public bool Has_Flags(StyleValueFlags Flags) { return (this.Flags & Flags) != 0; }
        #endregion

        #region ToString
        public override string ToString() { return $"{CssName}: {Assigned.ToString()}"; }
        #endregion

        #region Serialization
        public override string Serialize() { return $"{CssName}: {Assigned.ToString()}"; }
        #endregion

        #region Updating
        /// <summary>
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public override void Update(bool ComputeNow = false)
        {
            // unset our backing values so they can be updated...
            _specified = null;
            _computed = null;
            _used = null;
            _actual = null;

            if (ReferenceEquals(oldAssigned, null) || oldAssigned != Assigned)
            {
                oldAssigned.Set(Assigned);
                FireValueChangeEvent(ECssPropertyStage.Assigned);
            }

            if (ComputeNow)
            {
                Reinterpret_Specified();
                // check if we should reinterpreted Computed aswell
                if (ReferenceEquals(_computed, null))
                    Reinterpret_Computed();

                // check if we should reinterpreted Used aswell
                if (ReferenceEquals(_used, null))
                    Reinterpret_Used();

                // check if we should reinterpreted Actual aswell
                if (ReferenceEquals(_actual, null))
                    Reinterpret_Actual();
            }
        }

        /// <summary>
        /// If the Assigned value is one that depends on another value for its final value then
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public override void UpdateDependent(bool ComputeNow = false)
        {
            if (this.IsDependent)
                Update(ComputeNow);
        }

        /// <summary>
        /// If the Assigned value is one that depends on another value for its final value OR is <see cref="CssValue.Auto"/> then
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public override void UpdateDependentOrAuto(bool ComputeNow = false)
        {
            if (this.IsDependentOrAuto)
                Update(ComputeNow);
        }

        /// <summary>
        /// If the Assigned value is a percentage OR is <see cref="CssValue.Auto"/> then
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public override void UpdatePercentageOrAuto(bool ComputeNow = false)
        {
            if (this.IsPercentageOrAuto)
                Update(ComputeNow);
        }
        #endregion

        #region Explicit
        /// <summary>
        /// Sets the <see cref="Assigned"/> value for this property
        /// </summary>
        /// <param name="value"></param>
        public void Set(CssValue DeclaredValue)
        {
            if (Assigned != DeclaredValue)
            {
                Assigned = DeclaredValue;
            }
        }
        #endregion

    }
}
