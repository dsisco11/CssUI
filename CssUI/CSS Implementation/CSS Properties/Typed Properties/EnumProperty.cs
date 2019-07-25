using CssUI.Internal;
using System;
using System.Linq;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a styling property which holds an enum value
    /// </summary>
    /// <typeparam name="Ty">The enum type this property stores</typeparam>
    public class EnumProperty<Ty> : CssProperty where Ty : struct
    {

        #region Value Overrides
        public new Ty Actual
        {
            get
            {
                return CssLookup.Enum<Ty>((string)base.Actual.Value);
            }
        }
        #endregion

        #region Constructors

        public EnumProperty(string CssName, cssElement Owner, WeakReference<CssPropertySet> Source, bool Locked) 
            : base(CssName, Locked, Source, Owner)
        {
        }
        #endregion

        #region Setters
        public void Set(Ty Value)
        {
            /* Convert type value into its CSS Keyword */
            base.Assigned = CssValue.From_Keyword( CssLookup.Keyword<Ty>(Value) );
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            if (Computed.Type == ECssValueType.INTEGER)
            {
                return Enum.GetName(typeof(Ty), (Ty)Computed.Value);
            }

            return base.ToString();
        }
        #endregion
    }
}
