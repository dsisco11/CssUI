
namespace CssUI.CSS
{
    /// <summary>
    /// Represents a styling property which always resolves to an integer
    /// </summary>
    public class IntProperty : StyleProperty
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

        public IntProperty(string CssName, bool Locked, PropertyOptions Options) : base(CssName, Locked, Options)
        {
        }

        public IntProperty(string CssName, cssElement Owner, bool Locked, bool Unset, PropertyOptions Options) : base(CssName, Locked, Unset, Owner, Options)
        {
        }

        public IntProperty(string CssName, bool Locked, CSSValue Initial, PropertyOptions Options) : base(CssName, Locked, Initial, Options)
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
            var newValue = CSSValue.From_Int(value, CSSValue.Null);
            if (Assigned != newValue)
            {
                Assigned = newValue;
            }
        }
        #endregion
    }
}

