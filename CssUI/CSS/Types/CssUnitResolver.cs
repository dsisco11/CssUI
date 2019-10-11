using System;
using System.Linq;
using CssUI.DOM;
using CssUI.CSS;
using CssUI.CSS.Internal;

namespace CssUI
{
    /// <summary>
    /// Provides scale resolution for StyleValue unit types
    /// </summary>
    public class CssUnitResolver
    {
        #region Constants
        const double Ratio_DegToRad = (Math.PI / 180.0);
        const double Ratio_GradToRad = (Math.PI / 200.0);
        const double Ratio_TurnToRad = (Math.PI / 0.5);

        const double INCH_TO_PX = 1 / 96;
        const double CM_TO_PX = 96 / 2.54;
        const double MM_TO_PX = (1 / 10) / CM_TO_PX;
        const double PT_TO_PX = (1 / 72) / INCH_TO_PX;
        const double PC_TO_PX = (1 / 6) / INCH_TO_PX;
        const double Q_TO_PX = (1 / 40) / CM_TO_PX;

        const double DPPX_TO_DPI = 96;
        #endregion

        #region Static 
        private static int TABLE_SIZE = 0;
        static CssUnitResolver()
        {
            /* Find the max unit enum value */
            TABLE_SIZE = 1 + Enum.GetValues(typeof(ECssUnit)).Cast<byte>().Max();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Determines how the physical units are determined.
        /// If <c>True</c> the physical units will be anchored to the device DPI
        /// </summary>
        private bool anchor_to_dpi = true;
        private double[] SCALING_TABLE;
        Document document;
        #endregion

        #region Constructors
        public CssUnitResolver(Document document, bool anchor_to_dpi)
        {
            this.document = document;
            this.anchor_to_dpi = anchor_to_dpi;
            Compile_Table();
        }
        #endregion

        #region Table Compilation
        /// <summary>
        /// Compiles the PX_RELATIVE_SCALE table
        /// </summary>
        private void Compile_Table()
        {
            var unitList = Enum.GetValues(typeof(ECssUnit));
            SCALING_TABLE = new double[TABLE_SIZE];

            foreach (ECssUnit unit in unitList)
            {
                SCALING_TABLE[(int)unit] = Get_Scale(this.document, unit, this.anchor_to_dpi);
            }
        }
        #endregion

        /// <summary>
        /// Resolves a <paramref name="Value"/> from <paramref name="Unit"/> to its canonical form.
        /// </summary>
        /// <param name="Unit">Unit the value is specified in</param>
        /// <param name="Value">Value to convert</param>
        public double Resolve(double Value, ECssUnit Unit)
        {
            /* First we convert the unit value to the canonical unit (pixels for physical values) */
            return Value * SCALING_TABLE[(int)Unit];
        }

        /// <summary>
        /// Resolves a <paramref name="value"/> from <paramref name="unitFrom"/> to <paramref name="unitTo"/>
        /// </summary>
        /// <param name="unitFrom">The unit the value is specified in</param>
        /// <param name="unitTo">The unit to convert to</param>
        /// <param name="value">Value to convert</param>
        public double Resolve(double value, ECssUnit unitFrom, ECssUnit unitTo)
        {
            /* First we convert the unit value to the canonical unit (pixels for physical values) */
            double canonical = value * SCALING_TABLE[(int)unitFrom];
            /* Next we divide the canonical value to get the value in (unitTo) units */
            return canonical / SCALING_TABLE[(int)unitTo];
        }

        
        public static double Get_Font_Unit_Scale(Element Owner, ICssProperty Property, ECssUnit Unit)
        {
            switch (Unit)
            {
                case ECssUnit.CH:
                    throw new NotImplementedException($"CSS Unit type '{Enum.GetName(typeof(ECssUnit), Unit)}' has not been implemented!");
                case ECssUnit.EM:
                    {
                        /*
                         * CSS Specs:
                         * When used in the value of the font-size property on the element they refer to, 
                         * these units refer to the computed font metrics of the parent element 
                         * (or the computed font metrics corresponding to the initial values of the font property, if the element has no parent). 
                         * When used outside the context of an element (such as in media queries), 
                         * these units refer to the computed font metrics corresponding to the initial values of the font property.
                         */

                        if (Property.CssName == "font-size" && ReferenceEquals(Property.Owner, Owner))
                        {// We are being called from the font-size property

                            if (Property.Owner.parentElement is object)
                            {// Basically just try and inherit our parents unit scale
                                return Get_Font_Unit_Scale(Owner.parentElement, Property, Unit);
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
                                var def = CssDefinitions.StyleDefinitions[Property.CssName];
                                if (def != null)
                                {
                                    return def.Initial.Resolve() ?? throw new CssException($"Failed to resolve the default value specified in the '{Property.CssName}' property decleration to a number!");
                                }

                            }
                        }
#if DISABLE_FONT_SYSTEM
                        return 1;
#else 
                        if (Owner.Style.Font != null)
                            return Owner.Style.Font.EmSize;
                        else
                            return Owner.Style.FontSize;
#endif
                    }
                case ECssUnit.EX:
                    {
                        /*
                         * CSS Specs:
                         * The 'ex' unit is defined by the element's first available font. 
                         * The exception is when 'ex' occurs in the value of the 'font-size' property, 
                         * in which case it refers to the 'ex' of the parent element.
                         */
                        if (Property.CssName.Equals("font-size") && Property.Owner == Owner)
                        {// We are being called from the font-size property

                            if (Owner.parentElement != null)
                            {// Basically just try and inherit our parents unit scale
                                return Get_Font_Unit_Scale(Owner.parentElement, Property, Unit);
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
                                var def = CssDefinitions.StyleDefinitions[Property.CssName];
                                if (def != null)
                                {
                                    return def.Initial.Resolve() ?? throw new CssException($"Failed to resolve the default value specified in the '{Property.CssName}' property decleration to a number!");
                                }
                            }
                        }

#if DISABLE_FONT_SYSTEM
#else
                        if (Owner.Style.Font != null)
                        {
                            // XXX: implement logic to measure the 'x' height for our font. SEE: https://www.w3.org/TR/css-values-3/#font-relative-lengths
                            throw new NotImplementedException();
                        }
#endif
                        /*
                         * CSS Specs:
                         * In the cases where it is impossible or impractical to determine the x-height, a value of 0.5em should be used.
                         */
                        return Get_Font_Unit_Scale(Owner, Property, ECssUnit.EM) * 0.5;
                    }
                default:
                    throw new NotImplementedException($"CSS Unit type '{Enum.GetName(typeof(ECssUnit), Unit)}' has not been implemented!");
            }
        }

        /// <summary>
        /// Retreives the base scaling factor for the given unit relative to its canonical unit
        /// </summary>
        /// <param name="document"></param>
        /// <param name="Unit"></param>
        /// <returns></returns>
        private static double Get_Scale(Document document, ECssUnit Unit, bool anchor_to_dpi = false)
        {
            switch (Unit)
            {
                /* Physical Units */
                /* Docs: https://www.w3.org/TR/css3-values/#physical-units */
                case ECssUnit.PX:
                    {
                        return 1.0;
                    }
                case ECssUnit.CM:
                    {
                        if (anchor_to_dpi)
                        {
                            return (document.defaultView.screen.dpi / 2.54);
                        }

                        return CM_TO_PX;
                    }
                case ECssUnit.MM:
                    {
                        if (anchor_to_dpi)
                        {
                            return (document.defaultView.screen.dpi / 2.54) / 10.0;
                        }

                        return MM_TO_PX;
                    }
                case ECssUnit.Q:
                    {
                        if (anchor_to_dpi)
                        {
                            return (document.defaultView.screen.dpi / 2.54) / 40.0;
                        }

                        return Q_TO_PX;
                    }
                case ECssUnit.IN:
                    {
                        if (anchor_to_dpi)
                        {
                            return (1.0 / document.defaultView.screen.dpi);
                        }

                        return INCH_TO_PX;
                    }
                case ECssUnit.PC:
                    {
                        if (anchor_to_dpi)
                        {
                            return (1.0 / document.defaultView.screen.dpi) / 6.0;
                        }

                        return PC_TO_PX;
                    }
                case ECssUnit.PT:
                    {
                        if (anchor_to_dpi)
                        {
                            return (1.0 / document.defaultView.screen.dpi) / 72.0;
                        }

                        return PT_TO_PX;
                    }

                /* <Resolution> Units */
                /* Docs: https://www.w3.org/TR/css3-values/#resolution-value */
                case ECssUnit.DPI:
                    {
                        if (anchor_to_dpi)
                        {
                            return 1.0 / document.defaultView.screen.dpi;
                        }

                        return 1 / 96;
                    }
                case ECssUnit.DPCM:
                    {
                        if (anchor_to_dpi)
                        {
                            return (document.defaultView.screen.dpi / 2.54);
                        }

                        return PT_TO_PX;
                    }
                case ECssUnit.DPPX:
                    {
                        return 1;
                    }

                /* <Time> Units */
                /* Docs: https://www.w3.org/TR/css3-values/#time */
                case ECssUnit.S:
                    {
                        return 1.0;
                    }
                case ECssUnit.MS:
                    {
                        return (1 / 1000);
                    }

                /* <Frequency> Units */
                /* Docs: https://www.w3.org/TR/css3-values/#frequency */
                case ECssUnit.HZ:
                    {
                        return 1.0;
                    }
                case ECssUnit.KHZ:
                    {
                        return (1 / 1000);
                    }


                /* Angle Units */
                /* Docs: https://www.w3.org/TR/css3-values/#angles */
                /* Canonical unit: degrees */

                case ECssUnit.DEG:// Translate degrees to radians
                    {
                        return 1.0;
                    }
                case ECssUnit.GRAD:
                    {
                        return (400 / 360);
                    }
                case ECssUnit.RAD:
                    {
                        return (180.0 / Math.PI);
                    }
                case ECssUnit.TURN:
                    {
                        return 360.0;
                    }

                /* Font Units */
                /* Docs: https://www.w3.org/TR/css-values-3/#font-relative-lengths */
                case ECssUnit.REM:
                    {
                        return document.body.Style.FontSize;
                    }
                case ECssUnit.VMAX:
                    {
                        return Math.Max(document.defaultView.visualViewport.Width, document.defaultView.visualViewport.Height);
                    }
                case ECssUnit.VMIN:
                    {
                        return Math.Min(document.defaultView.visualViewport.Width, document.defaultView.visualViewport.Height);
                    }
                case ECssUnit.VW:
                    {
                        return document.defaultView.visualViewport.Width;
                    }
                case ECssUnit.VH:
                    {
                        return document.defaultView.visualViewport.Height;
                    }
                default:
                    {
                        throw new NotImplementedException($"CSS Unit type '{Enum.GetName(typeof(ECssUnit), Unit)}' has not been implemented!");
                    }
            }

        }

    }
}
