using CssUI.DOM.Nodes;
using System;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a styling property which always resolves to a string
    /// </summary>
    public class StringProperty : CssProperty
    {
        #region Accessors
        public string Value => Specified?.AsString();
        #endregion

        #region Value Overrides
        public new string Actual => base.Actual.AsString();
        #endregion

        #region Constructors
        public StringProperty(AtomicName<ECssPropertyID> CssName, ICssElement Owner, WeakReference<CssComputedStyle> Source, bool Locked) 
            : base(CssName, Owner, Source, Locked)
        {
        }
        #endregion

        #region Setters
        public void Set(string value)
        {
            var newValue = CssValue.From_String(value);
            if (base.Assigned != newValue)
            {
                base.Assigned = newValue;
            }
        }
        #endregion
        
    }
}
