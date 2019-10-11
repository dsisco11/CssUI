using System;
using CssUI.DOM.Nodes;
using CssUI.Rendering;

namespace CssUI.CSS
{
    public class ColorProperty : CssProperty
    {
        #region Value Overrides
        public new ReadOnlyColor Actual => base.Actual.AsColor();
        #endregion

        #region Constructors
        public ColorProperty(AtomicName<ECssPropertyID> CssName, ICssElement Owner, WeakReference<CssComputedStyle> Source, bool Locked)
            : base(CssName, Owner, Source, Locked)
        {
        }
        #endregion

        #region Setters
        /// <summary>
        /// Sets the <see cref="Assigned"/> value for this property to the given color
        /// </summary>
        /// <param name="value"></param>
        public void Set(Color value)
        {
            var newValue = CssValue.From(value);
            if (Assigned != newValue)
            {
                Assigned = newValue;
            }
        }
        #endregion
    }
}
