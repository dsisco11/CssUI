using System;
using CssUI.DOM.Nodes;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a styling property which always resolves to an floating point number
    /// </summary>
    public class NumberProperty : LengthProperty
    {
        #region Value Overrides
        public new double Actual => base.Actual.AsDecimal();
        #endregion

        #region Constructors
        public NumberProperty(AtomicName<ECssPropertyID> CssName, ICssElement Owner, WeakReference<CssComputedStyle> Source, bool Locked) 
            : base(CssName, Owner, Source, Locked)
        {
        }
        #endregion
    }
}
