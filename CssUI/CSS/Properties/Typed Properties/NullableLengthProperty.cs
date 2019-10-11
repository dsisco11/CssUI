using System;
using CssUI.DOM.Nodes;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a CSS length that can be assigned a null value
    /// </summary>
    public class NullableLengthProperty : CssProperty
    {
        #region Value Overrides
        public new int? Actual => base.Actual.AsIntegerN();
        #endregion
        
        #region Constructors
        public NullableLengthProperty(AtomicName<ECssPropertyID> CssName, ICssElement Owner, WeakReference<CssComputedStyle> Source, bool Locked)
            : base(CssName, Owner, Source, Locked)
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
            var newValue = CssValue.From(value, ECssUnit.PX, CssValue.Null);
            if (Assigned != newValue)
            {
                Assigned = newValue;
            }
        }
        #endregion
    }
}
