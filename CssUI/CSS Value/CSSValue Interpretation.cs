using CssUI.Internal;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CssUI.CSS
{
    public partial class CssValue
    {

        /// <summary>
        /// Derives a 'Specified' value from this one according to the CSS standards
        /// </summary>
        /// <param name="Property"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal async Task<CssValue> Derive_SpecifiedValue(ICssProperty Property)
        {// SEE:  https://www.w3.org/TR/css-cascade-3/#specified
            CssPropertyDefinition Def = Property.Definition;

            // CSS specs say if the cascade (assigned) resulted in a value, use it.
            if (!this.IsNull())
            {
                switch (this.Type)
                {
                    case EStyleDataType.UNSET:// SEE:  https://www.w3.org/TR/css-cascade-4/#valdef-all-unset
                        {// This property wants to act as though there is no decleration

                            //is this property inherited
                            if (Def != null && Def.Inherited)
                            {
                                return new CssValue(Property.Find_Inherited_Value());
                            }
                            // Not inherited, treat this situation like INITIAL
                            return new CssValue(Def.Initial);
                        }
                        break;
                    case EStyleDataType.INHERIT:
                        {
                            return new CssValue(Property.Find_Inherited_Value());
                        }
                        break;
                    case EStyleDataType.INITIAL:
                        {// If the Assigned value is the CssValue.Initial literal, then we use our definitions default
                            return new CssValue(Def.Initial);
                        }
                        break;
                    default:
                        {
                            return new CssValue(this);
                        }
                        break;
                }

            }
            // Assigned value is NULL
            // Try to inherit from our parent, if that fails use the Initial value.

            /*
            * CSS Specs:
            * 2. if the property is inherited and the element is not the root of the document tree, use the computed value of the parent element.
            */
            if (Def != null && Def.Inherited)
            {
                if (!(Property.Owner is cssRootElement))
                {// Root elements cannot inherit, they use the INITIAL value
                    return new CssValue(Def.Initial);
                }

                return new CssValue(Property.Find_Inherited_Value());
            }
            /*
            * CSS Specs:
            * 3. Otherwise use the property's initial value. The initial value of each property is indicated in the property's definition.
            */
            return new CssValue(Def.Initial);
        }

        /// <summary>
        /// Derives a 'Computed' value from this one according to the CSS standards
        /// </summary>
        /// <param name="Property"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal async Task<CssValue> Derive_ComputedValue(ICssProperty Property)
        {// SEE:  https://www.w3.org/TR/css-cascade-3/#computed
            CssPropertyDefinition Def = Property.Definition;

            // Resolve any relative values
            switch (this.Type)
            {
                case EStyleDataType.PERCENT:
                    {
                        if (Def.Percentage_Resolver != null)
                        {
                            double resolved = this.Resolve(null, (double Pct) => Def.Percentage_Resolver(Property.Owner, Pct)).Value;
                            return CssValue.From_Number(resolved);
                        }
                    }
                    break;
                case EStyleDataType.DIMENSION:
                    {
                        double? nv = this.Resolve((Unit) => StyleUnitResolver.Get_Scale(Property.Owner, Property, Unit));
                        if (nv.HasValue)
                        {
                            return CssValue.From_Number(nv.Value);
                        }
                    }
                    break;
                case EStyleDataType.INHERIT:// SEE:  https://www.w3.org/TR/CSS2/cascade.html#value-def-inherit
                    {
                        return new CssValue(Property.Find_Inherited_Value());
                    }
                    break;
            }

            // If we havent resolved a value yet that means this was meant to be handled by a custom handler
            var ResolutionDelegate = CssPropertyResolver.Get(Property.CssName, ECssPropertyStage.Computed);
            if (!ReferenceEquals(ResolutionDelegate, null))
            {
                return ResolutionDelegate.Invoke(Property);
            }


            return new CssValue(this);
        }

        /// <summary>
        /// Derives a 'Used' value from this one according to the CSS standards
        /// </summary>
        /// <param name="Computed"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal async Task<CssValue> Derive_UsedValue(ICssProperty Property)
        {// SEE:  https://www.w3.org/TR/css-cascade-3/#used

            var ResolutionDelegate = CssPropertyResolver.Get(Property.CssName, ECssPropertyStage.Used);
            if (!ReferenceEquals(ResolutionDelegate, null))
            {
                return ResolutionDelegate.Invoke(Property);
            }

            return new CssValue(this);
        }

        /// <summary>
        /// Derives an 'Actual' value from this one according to the CSS standards
        /// </summary>
        /// <param name="Used"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal async Task<CssValue> Derive_ActualValue(ICssProperty Property)
        {// SEE:  https://www.w3.org/TR/css-cascade-3/#actual
            // the Actual value does not get 'resolved' it gets restricted.

            var ResolutionDelegate = CssPropertyResolver.Get(Property.CssName, ECssPropertyStage.Actual);
            if (!ReferenceEquals(ResolutionDelegate, null))
            {
                return ResolutionDelegate(Property);
            }

            return new CssValue(this);
        }
    }
}
