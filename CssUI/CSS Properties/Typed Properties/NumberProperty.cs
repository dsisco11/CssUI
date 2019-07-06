using System;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a styling property which always resolves to an floating point number
    /// </summary>
    public class NumberProperty : LengthProperty
    {
        #region Value Overrides
        public new double Actual
        {
            get
            {
                return (double)base.Actual.Value;
            }
        }
        #endregion

        #region Constructors
        public NumberProperty(string CssName, cssElement Owner, WeakReference<CssPropertySet> Source, bool Locked) 
            : base(CssName, Owner, Source, Locked)
        {
        }
        #endregion
    }
}
