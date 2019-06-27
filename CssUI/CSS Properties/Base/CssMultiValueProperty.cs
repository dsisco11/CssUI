using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CssUI.CSS;

namespace CssUI
{
    /// <summary>
    /// Represents a CSS property which consists of multiple <see cref="CssValue"/>s
    /// This is a base class to be used by more complex multi-value property types, like the 'font' property
    /// </summary>
    public abstract class CssMultiValueProperty : ICssProperty
    {
        #region Properties

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
        /// List of all our values
        /// </summary>
        protected List<CssValue> Values = new List<CssValue>(0);        
        #endregion

        #region Accessors

        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// </summary>
        public virtual bool IsAuto { get { return Assigned.FirstOrDefault().Type == EStyleDataType.AUTO; } }
        /// <summary>
        /// Returns TRUE if the assigned value is <see cref="CssValue.Inherit"/>
        /// </summary>
        public virtual bool IsInherited { get { return Assigned.FirstOrDefault().Type == EStyleDataType.INHERIT; } }
        /// <summary>
        /// Returns TRUE if the assigned value has the <see cref="StyleValueFlags.Depends"/> flag
        /// </summary>
        public virtual bool HasDependent { get { return Assigned.FirstOrDefault(o => o.Has_Flags(StyleValueFlags.Depends)) != null; } }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// Returns TRUE if the assigned value has the <see cref="StyleValueFlags.Depends"/> flag
        /// </summary>
        public virtual bool HasDependentOrAuto { get { return (Assigned.FirstOrDefault().Type == EStyleDataType.AUTO || Assigned.FirstOrDefault(o => o.Has_Flags(StyleValueFlags.Depends)) != null); } }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// Returns TRUE if the assigned value type is a percentage
        /// </summary>
        public virtual bool HasPercentageOrAuto { get { return (Assigned.FirstOrDefault().Type == EStyleDataType.AUTO || Assigned.FirstOrDefault(o => o.Type == EStyleDataType.PERCENT) != null); } }


        /// <summary>
        /// Returns whether or not the property has a set value that should take affect during cascading.
        /// </summary>
        public bool HasValue { get { return Assigned.FirstOrDefault().HasValue(); } }

        /// <summary>
        /// List of all the currently assigned values
        /// </summary>
        public List<CssValue> Assigned { get { return Values; } }
        /// <summary>
        /// Values we USE for the property, which can differ from assigned values.
        /// Eg: If no values are Assigned then the propertys defined initial value will be used.
        /// </summary>
        public abstract List<CssValue> Specified { get; }
        /// <summary>
        /// The Specified values after being resolved to an absolute values, if possible
        /// </summary>
        public abstract List<CssValue> Computed { get; }


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



        #region Cascade
        /// <summary>
        /// Overwrites the values of this instance with any values from another which aren't <see cref="CssValue.Null"/>
        /// </summary>
        /// <returns>Success</returns>
        public async Task<bool> CascadeAsync(ICssProperty prop)
        {
            CssMultiValueProperty o = prop as CssMultiValueProperty;
            bool changes = false;
            var fval = o.Assigned.FirstOrDefault();
            if (fval != CssValue.Null && fval != CssValue.None)
            {
                changes = true;
                Values = new List<CssValue>(o.Assigned);
                this.SourcePtr = o.SourcePtr;
                this.Selector = o.Selector;
            }

            if (changes) Update();

            return await Task.FromResult(changes);
        }
        #endregion

        #region Overwrite

        /// <summary>
        /// Overwrites the assigned values of this instance with values from another if they are different
        /// </summary>
        /// <returns>Success</returns>
        public async Task<bool> OverwriteAsync(ICssProperty prop)
        {// Circumvents locking
            CssMultiValueProperty o = prop as CssMultiValueProperty;
            bool changes = false;
            if (o.Assigned != Assigned)
            {
                changes = true;
                Values = new List<CssValue>(o.Assigned);

                this.SourcePtr = o.SourcePtr;
                this.Selector = o.Selector;
            }

            if (changes) await Update();
            return changes;
        }
        #endregion

        #region Updating
        /// <summary>
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public async Task Update(bool ComputeNow = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// If the Assigned value is one that depends on another value for its final value then
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public async Task UpdateDependent(bool ComputeNow = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// If the Assigned value is one that depends on another value for its final value OR is <see cref="CssValue.Auto"/> then
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public async Task UpdateDependentOrAuto(bool ComputeNow = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// If the Assigned value is a percentage OR is <see cref="CssValue.Auto"/> then
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public async Task UpdatePercentageOrAuto(bool ComputeNow = false)
        {
            throw new NotImplementedException();
        }
        #endregion
        
        #region Unit Resolver
        /// <summary>
        /// Allows external code to notify this property that a certain unit type has changed scale and if we have a value which uses that unit-type we need to fire our Changed event because our Computed value will be different
        /// </summary>
        public void Handle_Unit_Change(EStyleUnit Unit)
        {
            if (Specified.FirstOrDefault(o => o.Unit == Unit) != null)
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

    }
}
