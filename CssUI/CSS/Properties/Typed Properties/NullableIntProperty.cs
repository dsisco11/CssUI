using System;
using CssUI.DOM.Nodes;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a styling property which always resolves to an integer
    /// </summary>
    public class NullableIntProperty : CssProperty
    {

        #region Value Overrides
        public new int? Actual => base.Actual.AsIntegerN();
        #endregion

        #region Constructors
        public NullableIntProperty(AtomicName<ECssPropertyID> CssName, ICssElement Owner, WeakReference<CssComputedStyle> Source, bool Locked) 
            : base(CssName, Owner, Source, Locked)
        {
        }
        #endregion

        #region Setters
        /// <summary>
        /// Sets the <see cref="Assigned"/> value for this property
        /// </summary>
        /// <param name="value"></param>
        public void Set(int? value)
        {
            var newValue = CssValue.From(value, CssValue.Null);
            if (Assigned != newValue)
            {
                Assigned = newValue;
            }
        }
        #endregion
    }
}

