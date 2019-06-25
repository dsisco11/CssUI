using System;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a styling property which holds an enum value
    /// </summary>
    /// <typeparam name="Ty">The enum type this property stores</typeparam>
    public class EnumProperty<Ty> : CssProperty where Ty : struct, IConvertible
    {
        static CssPropertyOptions DefaultOptions = new CssPropertyOptions() { AllowPercentage = false };
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
        public EnumProperty(string CssName) : base(CssName, DefaultOptions)
        {
        }

        public EnumProperty(bool Locked) : base(Locked, DefaultOptions)
        {
        }

        public EnumProperty(bool Locked, CssPropertyOptions Options) : base(Locked, new CssPropertyOptions(Options) { AllowPercentage = false })
        {
        }

        public EnumProperty(string CssName, bool Locked, CssPropertyOptions Options) : base(CssName, Locked, new CssPropertyOptions(Options) { AllowPercentage = false })
        {
        }

        [Obsolete("Please specify the properties Source")]
        public EnumProperty(string CssName, cssElement Owner, bool Locked, bool Unset, CssPropertyOptions Options) : base(CssName, Locked, Unset, Owner, new CssPropertyOptions(Options) { AllowPercentage = false })
        {
        }
        
        public EnumProperty(string CssName, cssElement Owner, WeakReference<CssPropertySet> Source, bool Locked, bool Unset, CssPropertyOptions Options) : base(CssName, Locked, Unset, Source, Owner, new CssPropertyOptions(Options) { AllowPercentage = false })
        {
        }

        public EnumProperty(CssValue initial) : base(initial, null, DefaultOptions)
        {
        }

        public EnumProperty(CssValue initial, bool Locked) : base(initial, Locked, DefaultOptions)
        {
        }

        public EnumProperty(CssValue initial, bool Locked, CssPropertyOptions Options) : base(initial, Locked, new CssPropertyOptions(Options) { AllowPercentage = false })
        {
        }

        public EnumProperty(Ty initial) : base(CssValue.From_Int(Convert.ToInt32(initial)), null, DefaultOptions)
        {
        }

        public EnumProperty(Ty initial, bool Locked) : base(CssValue.From_Int(Convert.ToInt32(initial)), Locked, DefaultOptions)
        {
        }

        public EnumProperty(Ty initial, bool Locked, CssPropertyOptions Options) : base(CssValue.From_Int(Convert.ToInt32(initial)), Locked, new CssPropertyOptions(Options) { AllowPercentage=false })
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
