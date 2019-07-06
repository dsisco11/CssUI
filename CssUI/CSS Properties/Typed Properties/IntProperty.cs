using System;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a styling property which always resolves to an integer
    /// </summary>
    public class IntProperty : LengthProperty
    {
        #region Value Overrides
        public new int Actual
        {
            get
            {
                return (int)base.Actual.Value;
            }
        }
        #endregion

        #region Constructors
        public IntProperty(string CssName, cssElement Owner, WeakReference<CssPropertySet> Source, bool Locked) 
            : base(CssName, Owner, Source, Locked)
        {
        }
        #endregion

    }
}

