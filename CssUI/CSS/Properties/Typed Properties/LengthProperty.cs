using System;
using CssUI.DOM.Nodes;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a styling property which always resolves to an integer
    /// </summary>
    public abstract class LengthProperty : CssProperty
    {
        #region Constructors
        public LengthProperty(AtomicName<ECssPropertyID> CssName, ICssElement Owner, WeakReference<CssComputedStyle> Source, bool Locked)
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

        /// <summary>
        /// Sets the <see cref="Assigned"/> value for this property to the given length (in pixels)
        /// </summary>
        /// <param name="value"></param>
        public void Set(double? value)
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

