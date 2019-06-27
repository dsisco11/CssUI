using System;
using System.Collections.Generic;

namespace CssUI.CSS
{
    /// <summary>
    /// Defines all of the possible CSS styles properties and their default values / behaviour flags
    /// </summary>
    public class CssProperties
    {
        public static Dictionary<AtomicString, CssPropertyDefinition> Definitions = new Dictionary<AtomicString, CssPropertyDefinition>();

        static CssProperties()
        {// Create all of the property definitions, setting up their initial values aswell as specifying the method they use to resolve a percentage value
            
            new CssPropertyDefinition("display", false, EPropertyAffects.Block, CssValue.From_Enum(EDisplayMode.INLINE_BLOCK));// We differ from the spec because our implementation is for a UI
            new CssPropertyDefinition("box-sizing", false, EPropertyAffects.Block, CssValue.From_Enum(EBoxSizingMode.BORDER));// We differ from the spec because our implementation is for a UI

            new CssPropertyDefinition("opacity", false, EPropertyAffects.Visual, CssValue.From_Number(1.0));

            new CssPropertyDefinition("dpi-x", true, EPropertyAffects.Text | EPropertyAffects.Flow, CssValue.Null, (cssElement E, double Pct) => { return (Pct * 72.0); });
            new CssPropertyDefinition("dpi-y", true, EPropertyAffects.Text | EPropertyAffects.Flow, CssValue.Null, (cssElement E, double Pct) => { return (Pct * 72.0); });

            new CssPropertyDefinition("font-family", true, EPropertyAffects.Text | EPropertyAffects.Flow, CssValue.Null);
            new CssPropertyDefinition("font-weight", true, EPropertyAffects.Text | EPropertyAffects.Flow, CssValue.From_Int(400));
            new CssPropertyDefinition("font-style", true, EPropertyAffects.Text | EPropertyAffects.Flow, CssValue.From_Enum(EFontStyle.Normal));
            new CssPropertyDefinition("font-size", true, EPropertyAffects.Text | EPropertyAffects.Flow, CssValue.From_Number(12), 
                (cssElement E, double Pct) => {
                    if (E.Parent != null)
                    {
                        return (Pct * E.Parent.Style.FontSize);
                    }
                    else
                    {// fallback to definition
                        var def = CssProperties.Definitions[new AtomicString("font-size")];
                        double r = def.Initial.Resolve() ?? throw new Exception("Failed to resolve default value from 'font-size' definition");
                        return Pct * r;
                    }
                });

            new CssPropertyDefinition("text-align", true, EPropertyAffects.Flow, CssValue.From_Enum(ETextAlign.Start));

            // SEE: https://www.w3.org/TR/CSS2/visudet.html#propdef-line-height
            new CssPropertyDefinition("line-height", true, EPropertyAffects.Flow, CssValue.From_Number(1.0), (cssElement E, double Pct) => { return (Pct * E.Style.FontSize); });

            // Was concerned about using CssValue.None as the initial value but that what the specs say
            // SEE: https://www.w3.org/TR/css-transforms-1/#transform-property
            new CssPropertyDefinition("transform", false, EPropertyAffects.Visual, CssValue.None);

            new CssPropertyDefinition("top", false, EPropertyAffects.Block, CssValue.Auto, (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Height); }, true);
            new CssPropertyDefinition("right", false, EPropertyAffects.Block, CssValue.Auto, (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("bottom", false, EPropertyAffects.Block, CssValue.Auto, (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Height); }, true);
            new CssPropertyDefinition("left", false, EPropertyAffects.Block, CssValue.Auto, (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);


            new CssPropertyDefinition("width", false, EPropertyAffects.Block, CssValue.Auto, (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("height", false, EPropertyAffects.Block, CssValue.Auto, (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Height); }, true);


            new CssPropertyDefinition("min-width", false, EPropertyAffects.Block, CssValue.Zero, (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("min-height", false, EPropertyAffects.Block, CssValue.Zero, (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Height); }, true);

            new CssPropertyDefinition("max-width", false, EPropertyAffects.Block, CssValue.None, (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("max-height", false, EPropertyAffects.Block, CssValue.None, (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Height); }, true);


            // Padding percentages are all calculated against the containing block's width so it is possible to create elements which conform to an aspect ratio
            new CssPropertyDefinition("padding-top", false, EPropertyAffects.Block, CssValue.Zero, (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("padding-right", false, EPropertyAffects.Block, CssValue.Zero, (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("padding-bottom", false, EPropertyAffects.Block, CssValue.Zero, (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("padding-left", false, EPropertyAffects.Block, CssValue.Zero, (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);


            // Margin percentages are all calculated against the containing block's width so it is possible to create elements which conform to an aspect ratio
            new CssPropertyDefinition("margin-top", false, EPropertyAffects.Block, CssValue.Zero, (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("margin-right", false, EPropertyAffects.Block, CssValue.Zero, (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("margin-bottom", false, EPropertyAffects.Block, CssValue.Zero, (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("margin-left", false, EPropertyAffects.Block, CssValue.Zero, (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);

            new CssPropertyDefinition("border-top-color", false, EPropertyAffects.Block, CssValue.CurrentColor);
            new CssPropertyDefinition("border-right-color", false, EPropertyAffects.Block, CssValue.CurrentColor);
            new CssPropertyDefinition("border-bottom-color", false, EPropertyAffects.Block, CssValue.CurrentColor);
            new CssPropertyDefinition("border-left-color", false, EPropertyAffects.Block, CssValue.CurrentColor);

            new CssPropertyDefinition("border-top-style", false, EPropertyAffects.Block, CssValue.From_Enum(EBorderStyle.None));
            new CssPropertyDefinition("border-right-style", false, EPropertyAffects.Block, CssValue.From_Enum(EBorderStyle.None));
            new CssPropertyDefinition("border-bottom-style", false, EPropertyAffects.Block, CssValue.From_Enum(EBorderStyle.None));
            new CssPropertyDefinition("border-left-style", false, EPropertyAffects.Block, CssValue.From_Enum(EBorderStyle.None));

            new CssPropertyDefinition("border-top-width", false, EPropertyAffects.Block, CssValue.From_Int(2), (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("border-right-width", false, EPropertyAffects.Block, CssValue.From_Int(2), (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("border-bottom-width", false, EPropertyAffects.Block, CssValue.From_Int(2), (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("border-left-width", false, EPropertyAffects.Block, CssValue.From_Int(2), (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);

            new CssPropertyDefinition("overflow-x", false, EPropertyAffects.Block, CssValue.From_Enum(EOverflowMode.Visible));
            new CssPropertyDefinition("overflow-y", false, EPropertyAffects.Block, CssValue.From_Enum(EOverflowMode.Visible));

            new CssPropertyDefinition("positioning", false, EPropertyAffects.Block, CssValue.From_Enum(EPositioning.Relative));

            new CssPropertyDefinition("object-fit", false, EPropertyAffects.Block, CssValue.From_Enum(EObjectFit.Fill));
            
            new CssPropertyDefinition("object-position-x", false, EPropertyAffects.Block, CssValue.From_Percent(50.0));
            new CssPropertyDefinition("object-position-y", false, EPropertyAffects.Block, CssValue.From_Percent(50.0));

            new CssPropertyDefinition("intrinsic-width", false, EPropertyAffects.Block, CssValue.Null, true);
            new CssPropertyDefinition("intrinsic-height", false, EPropertyAffects.Block, CssValue.Null, true);
            new CssPropertyDefinition("intrinsic-ratio", false, EPropertyAffects.Block, CssValue.Null, true);


            new CssPropertyDefinition("content-width", false, EPropertyAffects.Block, CssValue.Null, true);
            new CssPropertyDefinition("content-height", false, EPropertyAffects.Block, CssValue.Null, true);
        }
    }
}
