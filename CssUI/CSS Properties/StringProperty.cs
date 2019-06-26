using System;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a styling property which always resolves to a string
    /// </summary>
    public class StringProperty : CssProperty
    {
        static CssPropertyOptions DefaultOptions = new CssPropertyOptions() { AllowPercentage = false };
        #region Accessors
        public string Value
        {
            get
            {// Anything other than an actual string value returns NULL
                return base.Specified?.AsString();
            }
        }
        #endregion

        #region Constructors

        public StringProperty() : base(DefaultOptions)
        {
        }

        public StringProperty(bool Locked) : base(Locked, DefaultOptions)
        {
        }

        public StringProperty(bool Locked, CssPropertyOptions Options) : base(Locked, new CssPropertyOptions(Options) { AllowPercentage = false })
        {
        }

        public StringProperty(string CssName, bool Locked, CssPropertyOptions Options) : base(CssName, Locked, new CssPropertyOptions(Options) { AllowPercentage = false })
        {
        }

        public StringProperty(string CssName, cssElement Owner, WeakReference<CssPropertySet> Source, bool Locked, bool Unset, CssPropertyOptions Options) : base(CssName, Locked, Unset, Source, Owner, new CssPropertyOptions(Options) { AllowPercentage = false })
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
