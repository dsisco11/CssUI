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
    public class NumberProperty : StyleProperty
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

        public NumberProperty(bool Locked, PropertyOptions Options) : base(Locked, Options)
        {
        }

        public NumberProperty(string CssName, bool Locked, PropertyOptions Options) : base(CssName, Locked, Options)
        {
        }

        public NumberProperty(string CssName, uiElement Owner, bool Locked, bool Unset, PropertyOptions Options) : base(CssName, Locked, Unset, Owner, Options)
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
            var newValue = CSSValue.From_Number(value, CSSValue.Null);
            if (Assigned != newValue)
            {
                Assigned = newValue;
            }
        }
        #endregion
    }
}
