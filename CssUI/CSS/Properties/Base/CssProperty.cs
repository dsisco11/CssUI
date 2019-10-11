using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CssUI.CSS.Internal;
using CssUI.CSS.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.CSS
{

    /// <summary>
    /// Represents a CSS property and manages all of its value states: Assigned, Specified, and Computed values.
    /// </summary>
    public class CssProperty : CssPropertyBase, ICssProperty
    {/* Docs: https://www.w3.org/TR/css-cascade-3/#cascaded */

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
        ValueTracker<CssValue> oldAssigned = new ValueTracker<CssValue>();
        /// <summary>
        /// Tracks the previous value for <see cref="Specified"/> so we can detect when changes occur
        /// </summary>
        ValueTracker<CssValue> oldSpecified = new ValueTracker<CssValue>();
        /// <summary>
        /// Tracks the previous value for <see cref="Computed"/> so we can detect when changes occur
        /// </summary>
        ValueTracker<CssValue> oldComputed = new ValueTracker<CssValue>();
        /// <summary>
        /// Tracks the previous value for <see cref="Used"/> so we can detect when changes occur
        /// </summary>
        ValueTracker<CssValue> oldUsed = new ValueTracker<CssValue>();
        /// <summary>
        /// Tracks the previous value for <see cref="Actual"/> so we can detect when changes occur
        /// </summary>
        ValueTracker<CssValue> oldActual = new ValueTracker<CssValue>();
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
                _assigned = ReferenceEquals(value, null) ? CssValue.Null : value;
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
                {// Whomever is asking for this value obviously didnt want the later ones yet, also properties used-value resolution can access early stages of other properties
                    Reinterpret_Specified(false);
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
                {// Whomever is asking for this value obviously didnt want the later ones yet, also properties used-value resolution can access early stages of other properties
                    Reinterpret_Computed(false);
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
                {// Whomever is asking for this value obviously didnt want the later ones yet, also properties used-value resolution can access early stages of other properties
                    Reinterpret_Used(false);
                }

                return _used;
            }
        }

        /// <summary>
        /// The value that will be used by the element
        /// <para>This is the <see cref="Used"/> value with user-agent/system(platform) restrictions placed on it</para>
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

        /// <summary>
        /// Returns TRUE if the <see cref="Assigned"/> value is non-null
        /// </summary>
        public override bool HasValue { get => !Assigned.HasValue; }
        /// <summary>
        /// Returns TRUE if the <see cref="Assigned"/> value is <see cref="ECssValueTypes.NONE"/>
        /// </summary>
        public override bool IsNone { get => Assigned.Type == ECssValueTypes.NONE; }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// </summary>
        public override bool IsAuto { get => Assigned.Type == ECssValueTypes.AUTO; }
        /// <summary>
        /// Returns TRUE if the assigned value is <see cref="CssValue.Inherit"/>
        /// </summary>
        public override bool IsInherited { get => Assigned.Type == ECssValueTypes.INHERIT; }
        /// <summary>
        /// Returns TRUE if the assigned value has the <see cref="ECssValueFlags.Depends"/> flag
        /// </summary>
        public override bool IsDependent { get => Assigned.Has_Flags(ECssValueFlags.Depends); }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// Returns TRUE if the assigned value has the <see cref="ECssValueFlags.Depends"/> flag
        /// </summary>
        public override bool IsDependentOrAuto { get => Assigned.Type == ECssValueTypes.AUTO || Assigned.Has_Flags(ECssValueFlags.Depends); }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// Returns TRUE if the assigned value type is a percentage
        /// </summary>
        public override bool IsPercentageOrAuto { get => Assigned.Type == ECssValueTypes.AUTO || Assigned.Type == ECssValueTypes.PERCENT; }

        /// <summary>
        /// All flags which are present for all currently computed <see cref="CssValue"/>'s
        /// </summary>
        public override ECssValueFlags Flags => Specified.Flags;
        #endregion

        #region Constructor
        public CssProperty(AtomicName<ECssPropertyID> CssName, ICssElement Owner, WeakReference<CssComputedStyle> Source, bool Locked)
            : base(CssName, Owner, Source, Locked)
        {
        }
        #endregion

        #region Reverting
        /// <summary>
        /// Causes this property to revert back to the computed stage such that it must re-interpret its Used and Actual values.
        /// </summary>
        /// <param name="suppress">Suppresses any change event from firing once the Used value gets re-interpreted</param>
        internal override void Revert(bool suppress = false)
        {
            _used = null;
            _actual = null;

            if (suppress)
            {
                oldUsed.Update(null, true);
            }
        }
        #endregion

        #region Interpreting

        /// <summary>
        /// Reinterprets our <see cref="Specified"/> value
        /// </summary>
        /// <param name="Auto_Interpret_Next">Determines whether the next value stage will also be re-interpreted if the current stage changes due to this re-interpretation</param>
        private void Reinterpret_Specified(bool Auto_Interpret_Next = true)
        {
            _specified = Assigned.Derive_SpecifiedValue(this);
            // detect changes, fire events
            if (ReferenceEquals(oldSpecified, null) || oldSpecified != _specified)
            {// the computed value changed
                oldSpecified.Update(_specified);
                // FireValueChangeEvent(ECssPropertyStage.Specified);

                // update the Computed value
                Reinterpret_Computed();
            }
        }

        /// <param name="Auto_Interpret_Next">Determines whether the next value stage will also be re-interpreted if the current stage changes due to this re-interpretation</param>
        private void Reinterpret_Computed(bool Auto_Interpret_Next = true)
        {
            _computed = Specified.Derive_ComputedValue(this);
            // detect changes, fire events
            if (ReferenceEquals(oldComputed, null) || oldComputed != _computed)
            {
                oldComputed.Update(_computed);
                // FireValueChangeEvent(ECssPropertyStage.Computed);

                // Update the Used value
                Reinterpret_Used();
            }
        }

        /// <param name="Auto_Interpret_Next">Determines whether the next value stage will also be re-interpreted if the current stage changes due to this re-interpretation</param>
        private void Reinterpret_Used(bool Auto_Interpret_Next = true)
        {
            _used = Computed.Derive_UsedValue(this);
            // detect changes, fire events
            if (ReferenceEquals(oldUsed, null) || oldUsed != _used)
            {
                oldUsed.Update(_used);
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
                oldActual.Update(_actual);
                FireValueChangeEvent(EPropertyStage.Actual);
            }
        }
        #endregion

        #region Unit Resolver
        /// <summary>
        /// Allows external code to notify this property that a certain unit type has changed scale and if we have a value which uses that unit-type we need to fire our Changed event because our Computed value will be different
        /// </summary>
        public override void Handle_Unit_Change(ECssUnit Unit)
        {
            if (Specified.Unit == Unit)
            {// We are using this unit and its change will affect our computed value
                FireValueChangeEvent(EPropertyStage.Computed);
            }
        }
        #endregion

        #region Cascade
        /// <summary>
        /// Overwrites the values of this instance with any values from another which aren't <see cref="CssValue.Null"/>
        /// </summary>
        /// <returns>Success</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Cascade(ICssProperty prop)
        {// Circumvents locking
            CssProperty o = prop as CssProperty;
            bool changes = false;
            if (o.Assigned.HasValue)
            {
                changes = true;
                // Don't make a copy of the value, they are readonly anyhow
                //_value = new CssValue(o.Assigned);
                _assigned = o.Assigned;

                SourcePtr = o.SourcePtr;
                Selector = o.Selector;
            }

            if (changes) Update();
            return changes;
        }

        /// <summary>
        /// Overwrites the values of this instance with any values from another which aren't <see cref="CssValue.Null"/>
        /// </summary>
        /// <returns>Success</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override async Task<bool> CascadeAsync(ICssProperty prop)
        {
            return await Task.Factory.StartNew(() => Cascade(prop)).ConfigureAwait(continueOnCapturedContext: false);
        }
        #endregion

        #region Overwrite
        /// <summary>
        /// Overwrites the assigned value of this instance with values from another if they are different
        /// </summary>
        /// <returns>Success</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Overwrite(ICssProperty prop)
        {
            CssProperty o = prop as CssProperty;
            bool changes = false;
            if (o.Assigned != Assigned)
            {
                changes = true;
                //_value = new CssValue(o.Assigned);
                // Don't make a copy of the value, they are readonly anyhow
                _assigned = o.Assigned;

                SourcePtr = o.SourcePtr;
                Selector = o.Selector;
            }

            if (changes) Update();
            return changes;
        }

        /// <summary>
        /// Overwrites the assigned value of this instance with values from another if they are different
        /// </summary>
        /// <returns>Success</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override async Task<bool> OverwriteAsync(ICssProperty prop)
        {
            return await Task.Factory.StartNew(() => Overwrite(prop)).ConfigureAwait(continueOnCapturedContext: false);
        }
        #endregion

        #region Has Flags
        public bool Has_Flags(ECssValueFlags Flags) { return (this.Flags & Flags) != 0; }
        #endregion

        #region ToString
        public override string ToString() { return $"{CssName}: {Assigned.ToString()}"; }
        #endregion

        #region Serialization
        public override string Serialize() { return $"{CssName}: {Assigned.ToString()}"; }
        #endregion

        #region Updating
        /// <summary>
        /// Resets all values back to the Declared and then recomputes them later
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
                oldAssigned.Update(Assigned);
                FireValueChangeEvent(EPropertyStage.Assigned);
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
        /// If the Declared value is one that depends on another value for its final value then
        /// Resets all values back to the Declared and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public override void UpdateDependent(bool ComputeNow = false)
        {
            if (IsDependent)
                Update(ComputeNow);
        }

        /// <summary>
        /// If the Declared value is one that depends on another value for its final value OR is <see cref="CssValue.Auto"/> then
        /// Resets all values back to the Declared and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public override void UpdateDependentOrAuto(bool ComputeNow = false)
        {
            if (IsDependentOrAuto)
                Update(ComputeNow);
        }

        /// <summary>
        /// If the Declared value is a percentage OR is <see cref="CssValue.Auto"/> then
        /// Resets all values back to the Declared and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public override void UpdatePercentageOrAuto(bool ComputeNow = false)
        {
            if (IsPercentageOrAuto)
                Update(ComputeNow);
        }
        #endregion

        #region Setter
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

        /// <summary>
        /// Allows us to overwrite the computed value in special circumstances such as with Box calculation.
        /// </summary>
        /// <param name="Used"></param>
        internal void Set_Computed_Value(CssValue Used)
        {
            Revert(true);
            _computed = Used;
            Update(true);
        }
        #endregion

    }
}
