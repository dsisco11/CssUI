using CssUI.CSS.Internal;
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
        internal CssValue Derive_SpecifiedValue(ICssProperty Property)
        {// SEE:  https://www.w3.org/TR/css-cascade-3/#specified
            CssPropertyDefinition Def = Property.Definition;

            // CSS specs say if the cascade (assigned) resulted in a value, use it.
            if (!this.IsNull)
            {
                switch (this.Type)
                {
                    case ECssDataType.UNSET:// SEE:  https://www.w3.org/TR/css-cascade-4/#valdef-all-unset
                        {// This property wants to act as though there is no decleration

                            //is this property inherited
                            if (Def != null && Def.Inherited)
                            {
                                return Property.Find_Inherited_Value();
                            }
                            // Not inherited, treat this situation like INITIAL
                            return new CssValue(Def.Initial);
                        }
                        break;
                    case ECssDataType.INHERIT:
                        {
                            return Property.Find_Inherited_Value();
                        }
                        break;
                    case ECssDataType.INITIAL:
                        {// If the Assigned value is the CssValue.Initial literal, then we use our definitions default
                            return Def.Initial;
                        }
                        break;
                    default:
                        {
                            return this;
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
                    return Def.Initial;
                }

                return Property.Find_Inherited_Value();
            }
            /*
            * CSS Specs:
            * 3. Otherwise use the property's initial value. The initial value of each property is indicated in the property's definition.
            */
            return Def.Initial;
        }

        /// <summary>
        /// Derives a 'Computed' value from this one according to the CSS standards
        /// </summary>
        /// <param name="Property"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal CssValue Derive_ComputedValue(ICssProperty Property)
        {// SEE:  https://www.w3.org/TR/css-cascade-3/#computed
            CssPropertyDefinition Def = Property.Definition;

            // Resolve any relative values
            switch (this.Type)
            {
                case ECssDataType.INHERIT:// SEE:  https://www.w3.org/TR/CSS2/cascade.html#value-def-inherit
                    {
                        return Property.Find_Inherited_Value();
                    }
                    break;
                case ECssDataType.PERCENT:
                    {
                        if (Def.Percentage_Resolver != null)
                        {
                            return Def.Percentage_Resolver(Property.Owner, (double)this.Value);
                        }
                    }
                    break;
                case ECssDataType.DIMENSION:
                    {
                        double? nv = this.Resolve((Unit) => StyleUnitResolver.Get_Scale(Property.Owner, Property, Unit));
                        if (nv.HasValue)
                        {
                            return CssValue.From_Number(nv.Value);
                        }
                    }
                    break;
            }

            // If we havent resolved a value yet that means this was meant to be handled by a custom handler
            var ResolutionDelegate = Def.PropertyStageResolver[(int)ECssPropertyStage.Computed];
            if (!ReferenceEquals(ResolutionDelegate, null))
            {
                return ResolutionDelegate.Invoke(Property);
            }


            return this;
        }

        /// <summary>
        /// Derives a 'Used' value from this one according to the CSS standards
        /// </summary>
        /// <param name="Computed"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal CssValue Derive_UsedValue(ICssProperty Property)
        {// SEE:  https://www.w3.org/TR/css-cascade-3/#used
            var ResolutionDelegate = Property.Definition.PropertyStageResolver[(int)ECssPropertyStage.Used];
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
        internal CssValue Derive_ActualValue(ICssProperty Property)
        {// SEE:  https://www.w3.org/TR/css-cascade-3/#actual
         // the Actual value does not get 'resolved' it gets restricted.

            var ResolutionDelegate = Property.Definition.PropertyStageResolver[(int)ECssPropertyStage.Actual];
            if (!ReferenceEquals(ResolutionDelegate, null))
            {
                return ResolutionDelegate(Property);
            }

            return new CssValue(this);
        }
    }
}
