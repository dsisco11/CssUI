using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CssUI.CSS;

namespace CssUI
{
    /// <summary>
    /// Represents a style property which consists of multiple <see cref="CssValue"/>s
    /// </summary>
    public abstract class MultiValueProperty : ICssProperty
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
        /// Callback for when the computed value of this property changes
        /// </summary>
        public event PropertyChangeDelegate onChanged;
        /// <summary>
        /// Tracks which styling rule block this property came from
        /// </summary>
        public WeakReference<CssPropertySet> Source { get; set; } = null;
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
        public virtual bool IsAuto { get { return Specified.FirstOrDefault().Type == EStyleDataType.AUTO; } }
        /// <summary>
        /// Returns TRUE if the assigned value is <see cref="CssValue.Inherit"/>
        /// </summary>
        public virtual bool IsInherited { get { return Specified.FirstOrDefault().Type == EStyleDataType.INHERIT; } }
        /// <summary>
        /// Returns TRUE if the assigned value has the <see cref="StyleValueFlags.Depends"/> flag
        /// </summary>
        public virtual bool HasDependent { get { return Specified.FirstOrDefault(o => o.Has_Flags(StyleValueFlags.Depends)) != null; } }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// Returns TRUE if the assigned value has the <see cref="StyleValueFlags.Depends"/> flag
        /// </summary>
        public virtual bool HasDependentOrAuto { get { return (Specified.FirstOrDefault().Type == EStyleDataType.AUTO || Specified.FirstOrDefault(o => o.Has_Flags(StyleValueFlags.Depends)) != null); } }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// Returns TRUE if the assigned value type is a percentage
        /// </summary>
        public virtual bool HasPercentageOrAuto { get { return (Specified.FirstOrDefault().Type == EStyleDataType.AUTO || Specified.FirstOrDefault(o => o.Type == EStyleDataType.PERCENT) != null); } }


        /// <summary>
        /// Returns whether or not the property has a set value that should take affect during cascading.
        /// </summary>
        public bool HasValue { get { return !Assigned.FirstOrDefault().IsNullOrUnset(); } }

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

        public CssPropertySet Get_Source()
        {
            this.Source.TryGetTarget(out CssPropertySet src);
            return src;
        }
        #endregion



        #region Cascade
        /// <summary>
        /// Overwrites the values of this instance with any values from another which aren't <see cref="CssValue.Null"/>
        /// </summary>
        /// <returns>Success</returns>
        [Obsolete("Non-asynchronous function are now obsolete, please use CascadeAsync instead.")]
        public bool Cascade(ICssProperty prop)
        {// Circumvents locking
            MultiValueProperty o = prop as MultiValueProperty;
            bool changes = false;
            var fval = o.Assigned.FirstOrDefault();
            if (fval != CssValue.Null && fval != CssValue.None)
            {
                changes = true;
                Values = new List<CssValue>(o.Assigned);
                this.Source = o.Source;
                this.Selector = o.Selector;
            }

            if (changes) onChanged?.Invoke(this);
            return changes;
        }

        /// <summary>
        /// Overwrites the values of this instance with any values from another which aren't <see cref="CssValue.Null"/>
        /// </summary>
        /// <returns>Success</returns>
        public async Task<bool> CascadeAsync(ICssProperty prop)
        {
            MultiValueProperty o = prop as MultiValueProperty;
            bool changes = false;
            var fval = o.Assigned.FirstOrDefault();
            if (fval != CssValue.Null && fval != CssValue.None)
            {
                changes = true;
                Values = new List<CssValue>(o.Assigned);
                this.Source = o.Source;
                this.Selector = o.Selector;
            }

            if (changes) onChanged?.Invoke(this);

            return await Task.FromResult(changes);
        }
        #endregion

        #region Overwrite
        /// <summary>
        /// Overwrites the assigned values of this instance with values from another if they are different
        /// </summary>
        /// <returns>Success</returns>
        [Obsolete("Non-asynchronous function are now obsolete, please use OverwriteAsync instead.")]
        public bool Overwrite(ICssProperty prop)
        {// Circumvents locking
            MultiValueProperty o = prop as MultiValueProperty;
            bool changes = false;
            if (o.Assigned != Assigned)
            {
                changes = true;
                Values = new List<CssValue>(o.Assigned);

                this.Source = o.Source;
                this.Selector = o.Selector;
            }

            if (changes) onChanged?.Invoke(this);
            return changes;
        }

        /// <summary>
        /// Overwrites the assigned values of this instance with values from another if they are different
        /// </summary>
        /// <returns>Success</returns>
        public async Task<bool> OverwriteAsync(ICssProperty prop)
        {// Circumvents locking
            MultiValueProperty o = prop as MultiValueProperty;
            bool changes = false;
            if (o.Assigned != Assigned)
            {
                changes = true;
                Values = new List<CssValue>(o.Assigned);

                this.Source = o.Source;
                this.Selector = o.Selector;
            }

            if (changes) onChanged?.Invoke(this);
            return await Task.FromResult(changes);
        }
        #endregion


        #region Unit Resolver
        /// <summary>
        /// Allows external code to notify this property that a certain unit type has changed scale and if we have a value which uses that unit-type we need to fire our Changed event because our Computed value will be different
        /// </summary>
        public void Notify_Unit_Change(EStyleUnit Unit)
        {
            if (Specified.FirstOrDefault(o => o.Unit == Unit) != null)
            {
                onChanged.Invoke(this);
            }
        }

        private double Get_Unit_Scale(EStyleUnit Unit)
        {
            return StyleUnitResolver.Get_Scale(Owner, this, Unit);
        }
        #endregion


        #region Notify_Change
        /// <summary>
        /// Allows external code to notify this property that its true value has changed without it knowing and it should be resolved again
        /// </summary>
        public void Notify_Change()
        {
            onChanged?.Invoke(this);
        }
        #endregion
    }
}
