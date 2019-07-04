using System;
using System.Threading.Tasks;
using CssUI.CSS;
using CssUI.Internal;

namespace CssUI
{
    /// <summary>
    /// Interface for css properties held within an styling rule block
    /// </summary>
    public interface ICssProperty
    {

        #region Events
        /// <summary>
        /// Callback for when any value stage of this property changes
        /// </summary>
        event Action<ECssPropertyStage, ICssProperty> onValueChange;
        #endregion

        #region Properties
        /// <summary>
        /// Tracks which styling rule block this property came from
        /// </summary>
        CssSelector Selector { get; set; }
        /// <summary>
        /// The property we belong to
        /// </summary>
        /// <returns></returns>
        CssPropertySet Source { get; }
        /// <summary>
        /// Returns the definition for this property
        /// </summary>
        /// <returns></returns>
        CssPropertyDefinition Definition { get; }
        #endregion

        #region Accessors
        cssElement Owner { get; }
        /// <summary>
        /// The properties identifier token in stylesheets.
        /// <para>EG; "box-sizing", "margin-left", "margin-top", etc </para>
        /// </summary>
        AtomicString CssName { get; }
        /// <summary>
        /// Tracks which styling rule block this property came from
        /// </summary>
        WeakReference<CssPropertySet> SourcePtr { get; set; }

        /// <summary>
        /// Returns whether or not the property has a set value that should take affect during cascading.
        /// </summary>
        bool HasValue { get; }
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// </summary>
        bool IsAuto { get; }

        /// <summary>
        /// Returns TRUE if the assigned value is <see cref="CssValue.Inherit"/>
        /// </summary>
        bool IsInherited { get; }
        /// <summary>
        /// Returns TRUE if this property is inheritable according to its definition
        /// </summary>
        bool IsInheritable { get; }
        #endregion

        #region Inherited Value
        /// <summary>
        /// Returns the inherited value from the properties owners parent element
        /// </summary>
        CssValue Find_Inherited_Value();
        #endregion

        #region Serialization
        string Serialize();
        #endregion

        #region Unit Resolver
        /// <summary>
        /// Allows external code to notify this property that a certain unit type has changed scale and if we have a value which uses that unit-type we need to fire our Changed event because our Computed value will be different
        /// </summary>
        void Handle_Unit_Change(EStyleUnit Unit);
        #endregion

        #region Cascading
        /// <summary>
        /// Overwrites the values of this instance with any values from another which aren't <see cref="CssValue.Null"/>
        /// </summary>
        /// <returns>Success</returns>
        bool Cascade(ICssProperty value);

        /// <summary>
        /// Asynchronously overwrites the values of this instance with any values from another which aren't <see cref="CssValue.Null"/>
        /// </summary>
        /// <returns>Success</returns>
        Task<bool> CascadeAsync(ICssProperty value);
        #endregion

        #region Overwriting
        /// <summary>
        /// Overwrites the assigned value of this instance with values from another if they are different
        /// </summary>
        /// <returns>Success</returns>
        bool Overwrite(ICssProperty value);

        /// <summary>
        /// Asynchronously overwrites the assigned value of this instance with values from another if they are different
        /// </summary>
        /// <returns>Success</returns>
        Task<bool> OverwriteAsync(ICssProperty value);
        #endregion

        #region Updating
        /// <summary>
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        void Update(bool ComputeNow = false);

        /// <summary>
        /// If the Assigned value is one that depends on another value for its final value then
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        void UpdateDependent(bool ComputeNow = false);

        /// <summary>
        /// If the Assigned value is one that depends on another value for its final value OR is <see cref="CssValue.Auto"/> then
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        void UpdateDependentOrAuto(bool ComputeNow = false);

        /// <summary>
        /// If the Assigned value is a percentage OR is <see cref="CssValue.Auto"/> then
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        void UpdatePercentageOrAuto(bool ComputeNow = false);

        #endregion

    }


}
