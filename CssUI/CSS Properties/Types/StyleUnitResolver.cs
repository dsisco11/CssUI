﻿using CssUI.CSS;
using CssUI.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Provides scale resolution for StyleValue unit types
    /// </summary>
    public static class StyleUnitResolver
    {

        const double Ratio_DegToRad = (Math.PI / 180.0);
        const double Ratio_GradToRad = (Math.PI / 200.0);
        const double Ratio_TurnToRad = (Math.PI / 0.5);
        public static double Get_Scale(cssElement Owner, ICssProperty Property, EStyleUnit Unit)
        {
            switch (Unit)
            {// SEE: https://www.w3.org/TR/css-values-3/#font-relative-lengths
                case EStyleUnit.PX:
                    {
                        // Officially this is defined in the specifications as 1/96th of 1 inch
                        return 1.0;
                    }
                case EStyleUnit.EM:
                    {
                        /*
                         * CSS Specs:
                         * When used in the value of the font-size property on the element they refer to, 
                         * these units refer to the computed font metrics of the parent element 
                         * (or the computed font metrics corresponding to the initial values of the font property, if the element has no parent). 
                         * When used outside the context of an element (such as in media queries), 
                         * these units refer to the computed font metrics corresponding to the initial values of the font property.
                         */

                        if (Property.CssName == "font-size" && Property.Owner == Owner)
                        {// We are being called from the font-size property

                            if (Owner.Parent != null)
                            {// Basically just try and inherit our parents unit scale
                                return Get_Scale(Owner.Parent, Property, Unit);
                            }
                            else// No parent
                            {
                                /*
                                 * CSS Specs:
                                 * When used in the value of the font-size property on the element they refer to, 
                                 * these units refer to the computed font metrics of the parent element 
                                 * (or the computed font metrics corresponding to the initial values of the font property, if the element has no parent)
                                 */

                                // I can only assume this means we default to the property declerations default/initial value
                                var def = CssProperties.Definitions[Property.CssName];
                                if (def != null)
                                {
                                    return def.Initial.Resolve() ?? throw new CssException($"Failed to resolve the default value specified in the '{Property.CssName}' property decleration to a number!");
                                }
                                
                            }
                        }

                        if (Owner.Style.Font != null)
                            return Owner.Style.Font.EmSize;
                        else
                            return Owner.Style.FontSize;
                    }
                case EStyleUnit.EX:
                    {
                        /*
                         * CSS Specs:
                         * The 'ex' unit is defined by the element's first available font. 
                         * The exception is when 'ex' occurs in the value of the 'font-size' property, 
                         * in which case it refers to the 'ex' of the parent element.
                         */
                        if (Property.CssName == "font-size" && Property.Owner == Owner)
                        {// We are being called from the font-size property

                            if (Owner.Parent != null)
                            {// Basically just try and inherit our parents unit scale
                                return Get_Scale(Owner.Parent, Property, Unit);
                            }
                            else// No parent
                            {
                                /*
                                 * CSS Specs:
                                 * When used in the value of the font-size property on the element they refer to, 
                                 * these units refer to the computed font metrics of the parent element 
                                 * (or the computed font metrics corresponding to the initial values of the font property, if the element has no parent)
                                 */

                                // I can only assume this means we default to the property declerations default/initial value
                                var def = CssProperties.Definitions[Property.CssName];
                                if (def != null)
                                {
                                    return def.Initial.Resolve() ?? throw new CssException($"Failed to resolve the default value specified in the '{Property.CssName}' property decleration to a number!");
                                }
                            }
                        }

                        if (Owner.Style.Font != null)
                        {
                            // XXX: implement logic to measure the 'x' height for our font. SEE: https://www.w3.org/TR/css-values-3/#font-relative-lengths
                            throw new NotImplementedException();
                        }
                        /*
                         * CSS Specs:
                         * In the cases where it is impossible or impractical to determine the x-height, a value of 0.5em should be used.
                         */
                        return Get_Scale(Owner, Property, EStyleUnit.EM) * 0.5;
                    }
                case EStyleUnit.CH:
                    throw new NotImplementedException($"CSS Unit type '{Enum.GetName(typeof(EStyleUnit), Unit)}' has not been implemented!");
                case EStyleUnit.REM:
                    return Owner.Root.Style.FontSize;
                case EStyleUnit.VMAX:
                    return Math.Max(Owner.Root.Get_Viewport().Area.Width, Owner.Root.Get_Viewport().Area.Height);
                case EStyleUnit.VMIN:
                    return Math.Min(Owner.Root.Get_Viewport().Area.Width, Owner.Root.Get_Viewport().Area.Height);
                case EStyleUnit.VW:
                    return Owner.Root.Get_Viewport().Area.Width;
                case EStyleUnit.VH:
                    return Owner.Root.Get_Viewport().Area.Height;
                case EStyleUnit.DEG:// Translate degrees to radians
                    return Ratio_DegToRad;
                case EStyleUnit.GRAD:
                    return Ratio_GradToRad;
                case EStyleUnit.RAD:
                    throw new NotImplementedException($"CSS Unit type '{Enum.GetName(typeof(EStyleUnit), Unit)}' has not been implemented!");
                case EStyleUnit.TURN:
                    return Ratio_TurnToRad;
                default:
                    throw new NotImplementedException($"CSS Unit type '{Enum.GetName(typeof(EStyleUnit), Unit)}' has not been implemented!");
            }
        }
    }
}
