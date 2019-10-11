using System;
using CssUI.DOM.Nodes;

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
                return base.Actual.AsInteger();
            }
        }
        #endregion

        #region Constructors
        public IntProperty(AtomicName<ECssPropertyID> CssName, ICssElement Owner, WeakReference<CssComputedStyle> Source, bool Locked) 
            : base(CssName, Owner, Source, Locked)
        {
        }
        #endregion

    }
}

