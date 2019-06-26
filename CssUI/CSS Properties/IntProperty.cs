using System;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a styling property which always resolves to an integer
    /// </summary>
    public class IntProperty : CssProperty
    {
        #region Accessors
        //public Action<NamedProperty> onChanged { get { return this.onChanged; } set { this.onChanged = value; } }
        #endregion

        #region Constructors

        public IntProperty() : base()
        {
        }
        
        public IntProperty(string CssName) : base(CssName)
        {
        }

        public IntProperty(bool Locked) : base(Locked)
        {
        }

        public IntProperty(string CssName, bool Locked, CssPropertyOptions Options) : base(CssName, Locked, Options)
        {
        }

        public IntProperty(string CssName, cssElement Owner, WeakReference<CssPropertySet> Source, bool Locked, bool Unset, CssPropertyOptions Options) : base(CssName, Locked, Unset, Source, Owner, Options)
        {
        }

        #endregion

        #region Setters
        /// <summary>
        /// Sets the <see cref="Explicit"/> value for this property
        /// </summary>
        /// <param name="value"></param>
        public void Set(int? value)
        {
            var newValue = CssValue.From_Int(value, CssValue.Null);
            if (Assigned != newValue)
            {
                Assigned = newValue;
            }
        }
        #endregion
    }
}

