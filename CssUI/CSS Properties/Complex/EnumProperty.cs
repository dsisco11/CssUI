using System;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a styling property which holds an enum value
    /// </summary>
    /// <typeparam name="Ty">The enum type this property stores</typeparam>
    public class EnumProperty<Ty> : CssProperty where Ty : struct, IConvertible
    {
        #region Accessors
        public Ty Value
        {
            get
            {
                if (base.Assigned.Type != EStyleDataType.INTEGER) base.Update();
                if (base.Assigned.Type != EStyleDataType.INTEGER) throw new ArgumentException("EnumPropertys cannot accept unit values other than int!");
                return (Ty)base.Assigned.Value;
            }
            set
            {
                base.Assigned = CssValue.From_Int(Convert.ToInt32(value));
            }
        }
        internal CssValue ExplicitValue { get { return base.Assigned; } }
        #endregion

        #region Constructors
        
        public EnumProperty(string CssName, cssElement Owner, WeakReference<CssPropertySet> Source, bool Locked) 
            : base(CssName, Locked, Source, Owner)
        {
        }
        #endregion

        #region Setters
        public void Set(Ty value)
        {
            base.Assigned = CssValue.From_Int(Convert.ToInt32(value));
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            if (Computed.Type == EStyleDataType.INTEGER)
            {
                return Enum.GetName(typeof(Ty), (Ty)Computed.Value);
            }

            return base.ToString();
        }
        #endregion
    }
}
