using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a styling property which always resolves to an floating point number
    /// </summary>
    public class NumberProperty : CssProperty
    {
        #region Properties
        /// <summary>
        /// Stores the resolved value of this property
        /// </summary>
        public double? Resolved;
        #endregion

        #region Accessors
        //public event Action<NamedProperty> onChanged;// { get { return this.onChanged; } set { this.onChanged = value; } }
        #endregion

        #region Constructors
        public NumberProperty() : base()
        {
        }

        public NumberProperty(bool Locked) : base(Locked)
        {
        }

        public NumberProperty(bool Locked, CssPropertyOptions Options) : base(Locked, Options)
        {
        }

        public NumberProperty(string CssName, bool Locked, CssPropertyOptions Options) : base(CssName, Locked, Options)
        {
        }

        public NumberProperty(string CssName, cssElement Owner, bool Locked, bool Unset, CssPropertyOptions Options) : base(CssName, Locked, Unset, Owner, Options)
        {
        }

        #endregion

        #region Setters
        /// <summary>
        /// Sets the <see cref="Explicit"/> value for this property
        /// </summary>
        /// <param name="value"></param>
        public void Set(double? value)
        {
            var newValue = CssValue.From_Number(value, CssValue.Null);
            if (Assigned != newValue)
            {
                Assigned = newValue;
            }
        }
        #endregion
    }
}
