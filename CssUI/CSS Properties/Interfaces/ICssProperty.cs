﻿using System;
using System.Threading.Tasks;
using CssUI.CSS;

namespace CssUI
{
    public delegate void PropertyChangeDelegate(ICssProperty Property);
    /// <summary>
    /// Interface for css properties held within an styling rule block
    /// </summary>
    public interface ICssProperty
    {
        cssElement Owner { get; }
        /// <summary>
        /// The propertys field-name in whatever class is holding it.
        /// <para>If FullName were "Margins.Left" then this would be "Left"</para>
        /// </summary>
        AtomicString FieldName { get; set; }
        /// <summary>
        /// The propertys identifier token in stylesheets.
        /// <para>EG; "box-sizing", "margin-left", "margin-top", etc </para>
        /// </summary>
        AtomicString CssName { get; }
        /// <summary>
        /// Tracks which styling rule block this property came from
        /// </summary>
        WeakReference<CssPropertySet> SourcePtr { get; set; }
        /// <summary>
        /// Tracks which styling rule block this property came from
        /// </summary>
        CssSelector Selector { get; set; }

        /// <summary>
        /// The assigned value of this property has changed
        /// </summary>
        event PropertyChangeDelegate onChanged;

        /// <summary>
        /// Returns whether or not the property has a set value that should take affect during cascading.
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// Returns TRUE if the assigned value is <see cref="CssValue.Inherit"/>
        /// </summary>
        bool IsInherited { get; }

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

        /// <summary>
        /// Asynchronously overwrites the values of this instance with any values from another which aren't <see cref="CssValue.Null"/>
        /// </summary>
        /// <returns>Success</returns>
        Task<bool> CascadeAsync(ICssProperty value);
        
        /// <summary>
        /// Asynchronously overwrites the assigned value of this instance with values from another if they are different
        /// </summary>
        /// <returns>Success</returns>
        Task<bool> OverwriteAsync(ICssProperty value);

        /// <summary>
        /// Calculates the 'Assigned' and 'Computed' values
        /// </summary>
        void Update();

        /// <summary>
        /// Allows external code to notify this property that a certain unit type has changed scale and if we have a value which uses that unit-type we need to fire our Changed event because our Computed value will be different
        /// </summary>
        void Notify_Unit_Change(EStyleUnit Unit);

    }


}
