using System;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a styling property which always resolves to a string
    /// </summary>
    public class StringProperty : CssProperty
    {
        #region Accessors
        public string Value
        {
            get
            {// Anything other than an actual string value returns NULL
                return base.Specified?.Get_String();
            }
        }
        #endregion

        #region Value Overrides
        public new string Actual
        {
            get
            {
                return base.Actual.Value;
            }
        }
        #endregion

        #region Constructors
        public StringProperty(string CssName, cssElement Owner, WeakReference<CssPropertySet> Source, bool Locked) 
            : base(CssName, Locked, Source, Owner)
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
