using System;
using CssUI.DOM.Nodes;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a styling property which holds an enum value
    /// </summary>
    /// <typeparam name="Ty">The enum type this property stores</typeparam>
    public class EnumProperty<Ty> : CssProperty where Ty : struct, IConvertible
    {

        #region Value Overrides
        public new Ty Actual
        {
            get
            {
                return base.Actual.AsEnum<Ty>();
            }
        }
        #endregion

        #region Constructors

        public EnumProperty(AtomicName<ECssPropertyID> CssName, ICssElement Owner, WeakReference<CssComputedStyle> Source, bool Locked) 
            : base(CssName, Owner, Source, Locked)
        {
        }
        #endregion

        #region Setters
        public void Set(Ty Value)
        {
            Assigned = CssValue.From( Value );
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            if (Computed.Type == ECssValueTypes.INTEGER)
            {
                return Enum.GetName(typeof(Ty), Computed.AsEnum<Ty>());
            }

            return base.ToString();
        }
        #endregion
    }
}
