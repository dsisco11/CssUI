
using System;
using CssUI.DOM.Nodes;

namespace CssUI.CSS
{
    public class PositionProperty : CssProperty
    {
        #region Value Overrides
        public new Point2f Actual => base.Actual.AsPosition();
        #endregion

        #region Constructor
        public PositionProperty(AtomicName<ECssPropertyID> CssName, ICssElement Owner, WeakReference<CssComputedStyle> Source, bool Locked) : base(CssName, Owner, Source, Locked)
        {
        }
        #endregion

        #region Setters
        /// <summary>
        /// Sets the <see cref="Assigned"/> value for this property to the given color
        /// </summary>
        /// <param name="value"></param>
        public void Set(CssValue Horizontal, CssValue Vertical = null)
        {
            var newValue = CssValue.From(Horizontal, Vertical ?? CssValue.Null);
            if (Assigned != newValue)
            {
                Assigned = newValue;
            }
        }
        #endregion
    }
}
