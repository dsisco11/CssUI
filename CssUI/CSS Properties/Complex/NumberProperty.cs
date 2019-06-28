using System;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a styling property which always resolves to an floating point number
    /// </summary>
    public class NumberProperty : CssProperty
    {
        #region Properties
        /// <summary>
        /// Stores the resolved value of this property
        /// </summary>
        public double? Resolved;
        #endregion
        
        #region Constructors

        public NumberProperty(string CssName, cssElement Owner, WeakReference<CssPropertySet> Source, bool Locked) 
            : base(CssName, Locked,  Source, Owner)
        {
        }

        #endregion

        #region Setters
        /// <summary>
        /// Sets the <see cref="Explicit"/> value for this property
        /// </summary>
        /// <param name="value"></param>
        public void Set(double? value)
        {
            var newValue = CssValue.From_Number(value, CssValue.Null);
            if (Assigned != newValue)
            {
                Assigned = newValue;
            }
        }
        #endregion
    }
}
