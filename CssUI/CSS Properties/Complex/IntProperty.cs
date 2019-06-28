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

        public IntProperty(string CssName, cssElement Owner, WeakReference<CssPropertySet> Source, bool Locked) 
            : base(CssName, Locked, Source, Owner)
        {
        }

        #endregion

        #region Setters
        /// <summary>
        /// Sets the <see cref="Assigned"/> value for this property
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

