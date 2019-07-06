using System;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a styling property which always resolves to an integer
    /// </summary>
    public abstract class LengthProperty : CssProperty
    {
        #region Constructors
        public LengthProperty(string CssName, cssElement Owner, WeakReference<CssPropertySet> Source, bool Locked)
            : base(CssName, Locked, Source, Owner)
        {
        }
        #endregion

        #region Setters
        /// <summary>
        /// Sets the <see cref="Assigned"/> value for this property to the given length (in pixels)
        /// </summary>
        /// <param name="value"></param>
        public void Set(int? value)
        {
            var newValue = CssValue.From_Length(value, EStyleUnit.PX, CssValue.Null);
            if (Assigned != newValue)
            {
                Assigned = newValue;
            }
        }

        /// <summary>
        /// Sets the <see cref="Assigned"/> value for this property to the given length (in pixels)
        /// </summary>
        /// <param name="value"></param>
        public void Set(double? value)
        {
            var newValue = CssValue.From_Length(value, EStyleUnit.PX, CssValue.Null);
            if (Assigned != newValue)
            {
                Assigned = newValue;
            }
        }
        #endregion
    }
}

