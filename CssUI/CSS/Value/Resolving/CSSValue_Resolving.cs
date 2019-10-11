using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using System.Runtime.CompilerServices;

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
        {/* Docs:  https://www.w3.org/TR/css-cascade-3/#specified */
            StyleDefinition Def = Property.Definition;

            // CSS specs say if the cascade (assigned) resulted in a value, use it.
            if (!IsNull)
            {
                switch (Type)
                {
                    case ECssValueTypes.UNSET:/* Docs:  https://www.w3.org/TR/css-cascade-4/#valdef-all-unset  */
                        {// This property wants to act as though there is no decleration

                            //is this property inherited
                            if (Def != null && Def.Inherited)
                            {
                                return Property.Find_Inherited_Value();
                            }
                            // Not inherited, treat this situation like INITIAL
                            return new CssValue(Def.Initial);
                        }
                    case ECssValueTypes.INHERIT:
                        {
                            return Property.Find_Inherited_Value();
                        }
                    case ECssValueTypes.INITIAL:
                        {// If the Assigned value is the CssValue.Initial literal, then we use our definitions default
                            return Def.Initial;
                        }
                    default:
                        {
                            // Try and use a resolver if one is specified
                            var ResolutionDelegate = Def.PropertyStageResolver[(int)EPropertyStage.Specified];
                            if (ResolutionDelegate is object)
                            {
                                return (CssValue)ResolutionDelegate.Invoke(Property);
                            }

                            // Looks like this value doesn't need to change
                            return this;
                        }
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
                if (!Property.Owner.isRoot)
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
        {/* Docs:  https://www.w3.org/TR/css-cascade-3/#computed  */
            StyleDefinition Def = Property.Definition;

            // Resolve any relative values
            switch (Type)
            {
                case ECssValueTypes.INHERIT:// Docs:  https://www.w3.org/TR/CSS2/cascade.html#value-def-inherit
                    {
                        return Property.Find_Inherited_Value();
                    }
                case ECssValueTypes.PERCENT:
                    {
                        if (Def.Percentage_Resolver != null)
                        {
                            return Def.Percentage_Resolver(Property, (double)value);
                        }
                    }
                    break;
                case ECssValueTypes.DIMENSION:
                    {
                        double nv = Resolve(Property.Owner.ownerDocument.cssUnitResolver);
                        return CssValue.From(nv);
                    }
            }

            // If we havent resolved a value yet that means this was meant to be handled by a custom handler
            var ResolutionDelegate = Def.PropertyStageResolver[(int)EPropertyStage.Computed];
            if (ResolutionDelegate is object)
            {
                return (CssValue)ResolutionDelegate.Invoke(Property);
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
        {/* Docs:  https://www.w3.org/TR/css-cascade-3/#used */
            var ResolutionDelegate = Property.Definition.PropertyStageResolver[(int)EPropertyStage.Used];
            if (ResolutionDelegate is object)
            {
                return (CssValue)ResolutionDelegate.Invoke(Property);
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
        {/* Docs:  https://www.w3.org/TR/css-cascade-3/#actual */
         // the Actual value does not get 'resolved' it gets restricted.

            var ResolutionDelegate = Property.Definition.PropertyStageResolver[(int)EPropertyStage.Actual];
            if (ResolutionDelegate is object)
            {
                return (CssValue)ResolutionDelegate(Property);
            }

            return new CssValue(this);
        }
    }
}
