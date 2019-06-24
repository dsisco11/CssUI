using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a styling property which always resolves to a string
    /// </summary>
    public class StringProperty : StyleProperty
    {
        static PropertyOptions DefaultOptions = new PropertyOptions() { AllowPercentage = false };
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

        public StringProperty(bool Locked, PropertyOptions Options) : base(Locked, new PropertyOptions(Options) { AllowPercentage = false })
        {
        }

        public StringProperty(string CssName, bool Locked, PropertyOptions Options) : base(CssName, Locked, new PropertyOptions(Options) { AllowPercentage = false })
        {
        }

        public StringProperty(string CssName, cssElement Owner, bool Locked, bool Unset, PropertyOptions Options) : base(CssName, Locked, Unset, Owner, new PropertyOptions(Options) { AllowPercentage = false })
        {
        }

        public StringProperty(CssValue initial) : base(initial, null, DefaultOptions)
        {
        }

        public StringProperty(CssValue initial, bool Locked) : base(initial, Locked, DefaultOptions)
        {
        }

        public StringProperty(CssValue initial, bool Locked, PropertyOptions Options) : base(initial, Locked, new PropertyOptions(Options) { AllowPercentage = false })
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
