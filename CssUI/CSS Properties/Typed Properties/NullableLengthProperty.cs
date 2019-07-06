using System;

namespace CssUI.CSS
{
    public class NullableLengthProperty : CssProperty
    {
        #region Value Overrides
        public new int? Actual { get => (base.Actual.HasValue ? base.Actual.Value : null); }
        #endregion


        #region Constructors
        public NullableLengthProperty(string CssName, cssElement Owner, WeakReference<CssPropertySet> Source, bool Locked)
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
        #endregion
    }
}
