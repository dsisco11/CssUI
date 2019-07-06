using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CssUI.CSS;
using CssUI.Internal;

namespace CssUI
{
    /// <summary>
    /// Represents a CSS property which consists of multiple <see cref="CssValue"/>s
    /// This is a base class to be used by more complex multi-value property types, like the 'font' property
    /// </summary>
    public abstract class CssMultiValueProperty : CssPropertyBase, ICssProperty
    {

        #region backing values
        /// <summary>
        /// Backing value for <see cref="Assigned"/>
        /// <para> Docs: https://www.w3.org/TR/css-cascade-3/#cascaded </para>
        /// </summary>
        CssValueList _assigned = new CssValueList(CssValue.Null);

        /// <summary>
        /// Backing value for <see cref="Specified"/>
        /// <para> Docs: https://www.w3.org/TR/css-cascade-3/#specified </para>
        /// </summary>
        CssValueList _specified = null;

        /// <summary>
        /// Backing value for <see cref="Computed"/>
        /// <para> Docs: https://www.w3.org/TR/css-cascade-3/#computed </para>
        /// </summary>
        CssValueList _computed = null;

        /// <summary>
        /// Backing value for <see cref="Used"/>
        /// <para> Docs: https://www.w3.org/TR/css-cascade-3/#used </para>
        /// </summary>
        CssValueList _used = null;

        /// <summary>
        /// Backing value for <see cref="Actual"/>
        /// <para> Docs: https://www.w3.org/TR/css-cascade-3/#actual </para>
        /// </summary>
        CssValueList _actual = null;
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
        public CssValueList Assigned
        {
            get { return _assigned; }
            set
            {
                if (Locked) throw new Exception("Cannot modify the value of a locked css property!");
                Definition.CheckAndThrow(this, value);
                // Translate a value of NULL to CSSValue.Null
                _assigned = (object.ReferenceEquals(value, null) ? null : value);
                //our assigned value has changed, this means our specified and computed valued are now incorrect.
                Update();
            }
        }

        /// <summary>
        /// The value we interpreted from <see cref="Assigned"/>
        /// </summary>
        public CssValueList Specified
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
        public CssValueList Computed
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
        public CssValueList Used
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
        public virtual CssValueList Actual
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
        /// Returns TRUE if the <see cref="Assigned"/> value is <see cref="ECssDataType.NONE"/>
        /// </summary>
        public override bool IsNone { get => (Assigned.FirstOrDefault().Type == ECssDataType.NONE); }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// </summary>
        public override bool IsAuto { get => Assigned.FirstOrDefault().Type == ECssDataType.AUTO; }
        /// <summary>
        /// Returns TRUE if the assigned value is <see cref="CssValue.Inherit"/>
        /// </summary>
        public override bool IsInherited { get => Assigned.FirstOrDefault().Type == ECssDataType.INHERIT; }
        /// <summary>
        /// Returns TRUE if this property is inheritable according to its definition
        /// </summary>
        public override bool IsInheritable { get => Definition.Inherited; }
        /// <summary>
        /// Returns TRUE if the assigned value has the <see cref="ECssValueFlags.Depends"/> flag
        /// </summary>
        public override bool IsDependent { get => Assigned.FirstOrDefault(o => o.Has_Flags(ECssValueFlags.Depends)) != null; }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// Returns TRUE if the assigned value has the <see cref="ECssValueFlags.Depends"/> flag
        /// </summary>
        public override bool IsDependentOrAuto { get => (Assigned.FirstOrDefault().Type == ECssDataType.AUTO || Assigned.FirstOrDefault(o => o.Has_Flags(ECssValueFlags.Depends)) != null); }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// Returns TRUE if the assigned value type is a percentage
        /// </summary>
        public override bool IsPercentageOrAuto { get => (Assigned.FirstOrDefault().Type == ECssDataType.AUTO || Assigned.FirstOrDefault(o => o.Type == ECssDataType.PERCENT) != null); }
        /// <summary>
        /// Returns whether or not the property has a set value that should take affect during cascading.
        /// </summary>
        public override bool HasValue { get { return Assigned.FirstOrDefault().HasValue; } }
        #endregion
        
        #region Constructor
        public CssMultiValueProperty(string CssName, bool Locked, WeakReference<CssPropertySet> Source, cssElement Owner)
            : base(CssName, Locked, Source, Owner)
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
                oldUsed.Set(null);
        }
        #endregion

        #region Inherited Value
        /// <summary>
        /// Returns the inherited value from the properties owners parent element
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new CssValueList Find_Inherited_Value()
        {
            if (ReferenceEquals(Owner.Parent, null))
            {// Root elements cannot inherit, they use the INITIAL value
                return new CssValueList(Definition.Initial);
            }
            else
            {// Take our parents computed value
                ICssProperty prop = Owner.Parent.Style.Cascaded.Get(CssName);
                if (!ReferenceEquals(prop, null))
                    return new CssValueList((prop as CssProperty).Computed);
                else
                    throw new CssPropertyException($"Cannot read parent element property: {CssName}");
            }
        }
        #endregion

        #region Interpreting

        #region Deriving
        private CssValue Derive_SpecifiedValue(CssValue AssignedValue, out bool Inherited)
        {
            CssPropertyDefinition Def = this.Definition;
            Inherited = false;

            if (!AssignedValue.IsNull)
            {
                switch (AssignedValue.Type)
                {
                    case ECssDataType.UNSET:// SEE:  https://www.w3.org/TR/css-cascade-4/#valdef-all-unset
                        {// This property wants to act as though there is no decleration

                            //is this property inherited
                            if (Def != null && Def.Inherited)
                            {
                                Inherited = true;
                                return null;
                            }
                            // Not inherited, treat this situation like INITIAL
                            return new CssValue(Def.Initial);
                        }
                        break;
                    case ECssDataType.INHERIT:
                        {
                            Inherited = true;
                            return null;
                        }
                        break;
                    case ECssDataType.INITIAL:
                        {// If the Assigned value is the CssValue.Initial literal, then we use our definitions default
                            return new CssValue(Def.Initial);
                        }
                        break;
                    default:
                        {
                            return new CssValue(AssignedValue);
                        }
                        break;
                }

            }
            // Assigned value is NULL
            // Try to inherit from our parent, if that fails use the Initial value.

            /*
            * CSS Specs:
            * 2. if the property is inherited and the element is not the root of the document tree, use the computed value of the parent element.
            */
            if (Def != null && Def.Inherited)
            {
                if (!(Owner is cssRootElement))
                {// Root elements cannot inherit, they use the INITIAL value
                    return new CssValue(Def.Initial);
                }

                Inherited = true;
                return null;
            }
            /*
            * CSS Specs:
            * 3. Otherwise use the property's initial value. The initial value of each property is indicated in the property's definition.
            */
            return new CssValue(Def.Initial);
        }
        #endregion

        private void Reinterpret_Specified()
        {
            _specified = null;
            // If we only have a SINGLE value assigned then handle it specially
            // because it might be some keyword like inherit or something
            if (Assigned.Count == 1)
            {
                CssValue newValue = Derive_SpecifiedValue(Assigned.First(), out bool Inherited);
                if (Inherited)
                {
                    if (ReferenceEquals(Owner.Parent, null))
                    {// Root element
                        _specified = new CssValueList(Definition.Initial);
                    }
                    else
                    {
                        var parentProperty = Owner.Parent?.Style.Cascaded.Get(CssName);
                        if (!ReferenceEquals(parentProperty, null))
                        {
                            _specified = new CssValueList((parentProperty as CssMultiValueProperty).Assigned);
                        }
                        else
                        {
                            throw new CssPropertyException($"Cannot read parent element property: {CssName}");
                        }
                    }
                }
                else
                {
                    _specified = new CssValueList(newValue);
                }
            }
            else
            {
                int Total = Assigned.Count;
                CssValue[] newValues = new CssValue[Total];
                for (int i = 0; i < Total; i++)
                {
                    newValues[i] = Assigned[i].Derive_SpecifiedValue(this);
                }
                _specified = new CssValueList(newValues);
            }

            // detect changes, fire events
            if (!oldSpecified.HasValue || oldSpecified != _specified)
            {// the computed value changed
                oldSpecified.Set(_specified);
                FireValueChangeEvent(ECssPropertyStage.Specified);

                // update the Computed value
                Reinterpret_Computed();
            }
        }

        private void Reinterpret_Computed()
        {
            _computed = null;
            if (Specified.Count == 1)
            {// The only thing that can make the computed value inherit, is being set to the INHERIT value
                if(Specified.First().Type == ECssDataType.INHERIT)
                {
                    if (ReferenceEquals(Owner.Parent, null))
                    {// Root element
                        _computed = new CssValueList(Definition.Initial);
                    }
                    else
                    {
                        var parentProperty = Owner.Parent?.Style.Cascaded.Get(CssName);
                        if (parentProperty != null)
                        {
                            _computed = new CssValueList((parentProperty as CssMultiValueProperty).Specified);
                        }
                        else
                        {
                            throw new CssPropertyException($"Cannot read parent element property: {CssName}");
                        }
                    }
                }
                else
                {// our single value isnt inherit so we just let it resolve itself into a new list
                    _computed = new CssValueList(Specified.First().Derive_ComputedValue(this));
                }
            }
            else
            {
                int Total = Specified.Count;
                CssValue[] newValues = new CssValue[Total];
                for (int i = 0; i < Total; i++)
                {
                    newValues[i] = Specified[i].Derive_ComputedValue(this);
                }
                _computed = new CssValueList(newValues);
            }

            // detect changes, fire events
            if (!oldComputed.HasValue || oldComputed != _computed)
            {// the computed value changed
                oldComputed.Set(_computed);
                FireValueChangeEvent(ECssPropertyStage.Computed);

                // update the Used value
                Reinterpret_Used();
            }
        }

        private void Reinterpret_Used()
        {
            _used = null;
            //var ResolutionDelegate = CssPropertyResolver.Get(CssName, ECssPropertyStage.Used);
            var ResolutionDelegate = Definition.PropertyStageResolver[(int)ECssPropertyStage.Used];
            if (!ReferenceEquals(ResolutionDelegate, null))
            {
                _used = (CssValueList)ResolutionDelegate.Invoke(this);
            }
            else
            {
                _used = new CssValueList(Computed);
            }

            // detect changes, fire events
            if (!oldUsed.HasValue || oldUsed != _used)
            {
                oldUsed.Set(_used);
                FireValueChangeEvent(ECssPropertyStage.Used);

                // update the Actual value
                Reinterpret_Actual();
            }
        }

        private void Reinterpret_Actual()
        {
            _actual = null;
            //var ResolutionDelegate = CssPropertyResolver.Get(CssName, ECssPropertyStage.Actual);
            var ResolutionDelegate = Definition.PropertyStageResolver[(int)ECssPropertyStage.Actual];
            if (!ReferenceEquals(ResolutionDelegate, null))
            {
                _actual = (CssValueList)ResolutionDelegate.Invoke(this);
            }
            else
            {
                _actual = new CssValueList(Used);
            }

            // detect changes, fire events
            if (!oldActual.HasValue || oldActual != _actual)
            {
                oldActual.Set(_actual);
                FireValueChangeEvent(ECssPropertyStage.Actual);
            }
        }
        #endregion

        #region Cascade
        /// <summary>
        /// Overwrites the values of this instance with any values from another which aren't <see cref="CssValue.Null"/>
        /// </summary>
        /// <returns>Success</returns>
        public override bool Cascade(ICssProperty prop)
        {
            CssMultiValueProperty Property = prop as CssMultiValueProperty;
            bool changes = false;
            if (Property.Assigned.FirstOrDefault().HasValue)
            {
                changes = true;
                _assigned = new CssValueList(Property.Assigned);
                this.SourcePtr = Property.SourcePtr;
                this.Selector = Property.Selector;
            }

            if (changes)
                Update();

            return changes;
        }

        /// <summary>
        /// Overwrites the values of this instance with any values from another which aren't <see cref="CssValue.Null"/>
        /// </summary>
        /// <returns>Success</returns>
        public override async Task<bool> CascadeAsync(ICssProperty prop)
        {
            return await Task.Factory.StartNew(() => Cascade(prop)).ConfigureAwait(continueOnCapturedContext: false);
        }
        #endregion

        #region Overwrite
        /// <summary>
        /// Overwrites the assigned values of this instance with values from another if they are different
        /// </summary>
        /// <returns>Success</returns>
        public override bool Overwrite(ICssProperty prop)
        {// Circumvents locking
            CssMultiValueProperty Property = prop as CssMultiValueProperty;
            bool changes = false;
            if (Property.Assigned != Assigned)
            {
                changes = true;
                _assigned = new CssValueList(Property.Assigned);

                this.SourcePtr = Property.SourcePtr;
                this.Selector = Property.Selector;
            }

            if (changes)
                Update();
            return changes;
        }

        /// <summary>
        /// Overwrites the assigned values of this instance with values from another if they are different
        /// </summary>
        /// <returns>Success</returns>
        public override async Task<bool> OverwriteAsync(ICssProperty prop)
        {
            return await Task.Factory.StartNew(() => Overwrite(prop)).ConfigureAwait(continueOnCapturedContext: false);
        }
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

        #region Unit Resolver
        /// <summary>
        /// Allows external code to notify this property that a certain unit type has changed scale and if we have a value which uses that unit-type we need to fire our Changed event because our Computed value will be different
        /// </summary>
        public override void Handle_Unit_Change(ECssUnit Unit)
        {
            if (Specified.FirstOrDefault(o => o.Unit == Unit) != null)
            {
                // This unit change will affect our computed value
                FireValueChangeEvent(ECssPropertyStage.Computed);
            }
        }
        #endregion



        #region ToString
        public override string ToString() { return $"{CssName}: {string.Join(", ", Assigned.Select(x => x.ToString()))}"; }
        #endregion

        #region Serialization
        public override string Serialize() { return $"{CssName}: {string.Join(", ", Assigned.Select(x => x.ToString()))}"; }
        #endregion

    }
}
