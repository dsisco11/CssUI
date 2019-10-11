using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CssUI.CSS.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.CSS.Internal
{
    public abstract class CssPropertyBase : ICssProperty
    {
        #region Universal Class Properties
        /// <summary>
        /// The UI element which contains this property
        /// </summary>
        public ICssElement Owner { get; protected set; } = null;
        /// <summary>
        /// The propertys identifier token in stylesheets.
        /// <para>EG; "box-sizing", "margin-left", "margin-top", etc </para>
        /// </summary>
        public AtomicName<ECssPropertyID> CssName { get; protected set; } = null;

        /// <summary>
        /// Tracks which styling rule block this property came from
        /// </summary>
        public WeakReference<CssComputedStyle> SourcePtr { get; set; } = null;
        /// <summary>
        /// Tracks which styling rule block this property came from
        /// </summary>
        public CssSelector Selector { get; set; } = null;

        /// <summary>
        /// If true then this propertys values cannot be set externally
        /// </summary>
        public readonly bool Locked = false;

        /// <summary>
        /// All flags which are present for all currently computed <see cref="CssValue"/>'s
        /// </summary>
        public virtual ECssValueFlags Flags { get => ECssValueFlags.None; }
        #endregion

        #region Universal Class Accessors
        /// <summary>
        /// Returns TRUE if this property is inheritable according to its definition
        /// </summary>
        public virtual bool IsInheritable { get => Definition.Inherited; }

        public CssComputedStyle Source
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                SourcePtr.TryGetTarget(out CssComputedStyle src);
                return src;
            }
        }

        public StyleDefinition Definition
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (CssName is null)
                    return null;
                return CssDefinitions.StyleDefinitions[CssName];
            }
        }
        #endregion

        #region Abstract Accessors

        /// <summary>
        /// Returns TRUE if the <see cref="Assigned"/> value is non-null
        /// </summary>
        public abstract bool HasValue { get; }
        /// <summary>
        /// Returns TRUE if the <see cref="Assigned"/> value is <see cref="ECssValueTypes.NONE"/>
        /// </summary>
        public abstract bool IsNone { get; }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// </summary>
        public abstract bool IsAuto { get; }
        /// <summary>
        /// Returns TRUE if the assigned value is <see cref="CssValue.Inherit"/>
        /// </summary>
        public abstract bool IsInherited { get; }
        /// <summary>
        /// Returns TRUE if the assigned value has the <see cref="ECssValueFlags.Depends"/> flag
        /// </summary>
        public abstract bool IsDependent { get; }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// Returns TRUE if the assigned value has the <see cref="ECssValueFlags.Depends"/> flag
        /// </summary>
        public abstract bool IsDependentOrAuto { get; }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// Returns TRUE if the assigned value type is a percentage
        /// </summary>
        public abstract bool IsPercentageOrAuto { get; }

        #endregion

        #region Value Change
        /// <summary>
        /// Callback for when any value stage of this property changes
        /// </summary>
        public event Action<EPropertyStage, ICssProperty> onValueChange;

        protected void FireValueChangeEvent(EPropertyStage Stage)
        {
            onValueChange?.Invoke(Stage, this);
        }
        #endregion

        #region Inherited Value
        /// <summary>
        /// Returns the inherited value from the properties owners parent element
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CssValue Find_Inherited_Value()
        {
            if (Owner.parentNode is null)
            {// Root elements cannot inherit, they use the INITIAL value
                return new CssValue(Definition.Initial);
            }
            else
            {// Take our parents computed value
                ICssProperty prop = Owner.parentElement.Style.Cascaded.Get(CssName);
                if (prop is object)
                    return new CssValue((prop as CssProperty).Computed);
                else
                    throw new CssPropertyException($"Cannot read parent element property: {CssName}");
            }
        }
        #endregion

        #region Constructor
        public CssPropertyBase(AtomicName<ECssPropertyID> CssName, ICssElement Owner, WeakReference<CssComputedStyle> Source, bool Locked)
        {
            this.CssName = CssName;
            this.Owner = Owner;
            SourcePtr = Source;
            this.Locked = Locked;
        }
        #endregion

        #region Reverting
        /// <summary>
        /// Causes this property to revert back to the computed stage such that it must re-interpret its Used and Actual values.
        /// </summary>
        /// <param name="suppress">Suppresses any change event from firing once the Used value gets re-interpreted</param>
        internal abstract void Revert(bool suppress = false);
        #endregion

        #region Serialization
        public abstract string Serialize();
        #endregion

        #region Unit Resolver
        /// <summary>
        /// Allows external code to notify this property that a certain unit type has changed scale and if we have a value which uses that unit-type we need to fire our Changed event because our Computed value will be different
        /// </summary>
        public abstract void Handle_Unit_Change(ECssUnit Unit);
        #endregion

        #region Cascading
        /// <summary>
        /// Overwrites the values of this instance with any values from another which aren't <see cref="CssValue.Null"/>
        /// </summary>
        /// <returns>Success</returns>
        public abstract bool Cascade(ICssProperty value);

        /// <summary>
        /// Asynchronously overwrites the values of this instance with any values from another which aren't <see cref="CssValue.Null"/>
        /// </summary>
        /// <returns>Success</returns>
        public abstract Task<bool> CascadeAsync(ICssProperty value);
        #endregion

        #region Overwriting
        /// <summary>
        /// Ooverwrites the assigned value of this instance with values from another if they are different
        /// </summary>
        /// <returns>Success</returns>
        public abstract bool Overwrite(ICssProperty value);

        /// <summary>
        /// Asynchronously overwrites the assigned value of this instance with values from another if they are different
        /// </summary>
        /// <returns>Success</returns>
        public abstract Task<bool> OverwriteAsync(ICssProperty value);
        #endregion

        #region Updating
        /// <summary>
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public abstract void Update(bool ComputeNow = false);

        /// <summary>
        /// If the Assigned value is one that depends on another value for its final value then
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public abstract void UpdateDependent(bool ComputeNow = false);

        /// <summary>
        /// If the Assigned value is one that depends on another value for its final value OR is <see cref="CssValue.Auto"/> then
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public abstract void UpdateDependentOrAuto(bool ComputeNow = false);

        /// <summary>
        /// If the Assigned value is a percentage OR is <see cref="CssValue.Auto"/> then
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public abstract void UpdatePercentageOrAuto(bool ComputeNow = false);

        #endregion

    }
}
