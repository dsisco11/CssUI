using System.Collections.Generic;

namespace CssUI.CSS
{
    /// <summary>
    /// Holds all of the CSS property definitions
    /// </summary>
    public class CssProperties
    {
        public static Dictionary<AtomicString, CssPropertyDefinition> Definitions = new Dictionary<AtomicString, CssPropertyDefinition>();

        static CssProperties()
        {// Create all of the property definitions, setting up their initial values aswell as specifying the method they use to resolve a percentage value
            
            new CssPropertyDefinition("display", false, EPropertyFlags.Block, CSSValue.From_Enum(EDisplayMode.INLINE_BLOCK));// We differ from the spec because our implementation is for a UI
            new CssPropertyDefinition("box-sizing", false, EPropertyFlags.Block, CSSValue.From_Enum(EBoxSizingMode.BORDER));// We differ from the spec because our implementation is for a UI

            new CssPropertyDefinition("opacity", false, EPropertyFlags.Visual, CSSValue.From_Number(1));

            new CssPropertyDefinition("font-family", true, EPropertyFlags.Font | EPropertyFlags.Flow, CSSValue.Null);
            new CssPropertyDefinition("font-weight", true, EPropertyFlags.Font | EPropertyFlags.Flow, CSSValue.From_Int(400));
            new CssPropertyDefinition("font-style", true, EPropertyFlags.Font | EPropertyFlags.Flow, CSSValue.From_Enum(EFontStyle.Normal));
            new CssPropertyDefinition("font-size", true, EPropertyFlags.Font | EPropertyFlags.Flow, CSSValue.From_Length(1.0, EStyleUnit.EM), (uiElement E, double Pct) => { return (Pct * E.Parent.Style.FontSize); });

            new CssPropertyDefinition("text-align", true, EPropertyFlags.Flow, CSSValue.From_Enum(ETextAlign.Start));

            // SEE: https://www.w3.org/TR/CSS2/visudet.html#propdef-line-height
            new CssPropertyDefinition("line-height", true, EPropertyFlags.Flow, CSSValue.From_Percent(100.0), (uiElement E, double Pct) => { return (Pct * E.Style.FontSize); });

            new CssPropertyDefinition("transform", false, EPropertyFlags.Visual, CSSValue.None);

            new CssPropertyDefinition("top", false, EPropertyFlags.Block, CSSValue.Auto, (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Height); }, true);
            new CssPropertyDefinition("right", false, EPropertyFlags.Block, CSSValue.Auto, (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("bottom", false, EPropertyFlags.Block, CSSValue.Auto, (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Height); }, true);
            new CssPropertyDefinition("left", false, EPropertyFlags.Block, CSSValue.Auto, (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);


            new CssPropertyDefinition("width", false, EPropertyFlags.Block, CSSValue.Auto, (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("height", false, EPropertyFlags.Block, CSSValue.Auto, (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Height); }, true);


            new CssPropertyDefinition("min-width", false, EPropertyFlags.Block, CSSValue.Zero, (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("min-height", false, EPropertyFlags.Block, CSSValue.Zero, (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Height); }, true);

            new CssPropertyDefinition("max-width", false, EPropertyFlags.Block, CSSValue.None, (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("max-height", false, EPropertyFlags.Block, CSSValue.None, (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Height); }, true);


            // Padding percentages are all calculated against the containing block's width so it is possible to create elements which conform to an aspect ratio
            new CssPropertyDefinition("padding-top", false, EPropertyFlags.Block, CSSValue.Zero, (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("padding-right", false, EPropertyFlags.Block, CSSValue.Zero, (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("padding-bottom", false, EPropertyFlags.Block, CSSValue.Zero, (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("padding-left", false, EPropertyFlags.Block, CSSValue.Zero, (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);


            // Margin percentages are all calculated against the containing block's width so it is possible to create elements which conform to an aspect ratio
            new CssPropertyDefinition("margin-top", false, EPropertyFlags.Block, CSSValue.Zero, (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("margin-right", false, EPropertyFlags.Block, CSSValue.Zero, (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("margin-bottom", false, EPropertyFlags.Block, CSSValue.Zero, (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("margin-left", false, EPropertyFlags.Block, CSSValue.Zero, (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);

            new CssPropertyDefinition("border-top-color", false, EPropertyFlags.Block, CSSValue.CurrentColor);
            new CssPropertyDefinition("border-right-color", false, EPropertyFlags.Block, CSSValue.CurrentColor);
            new CssPropertyDefinition("border-bottom-color", false, EPropertyFlags.Block, CSSValue.CurrentColor);
            new CssPropertyDefinition("border-left-color", false, EPropertyFlags.Block, CSSValue.CurrentColor);

            new CssPropertyDefinition("border-top-style", false, EPropertyFlags.Block, CSSValue.From_Enum(EBorderStyle.None));
            new CssPropertyDefinition("border-right-style", false, EPropertyFlags.Block, CSSValue.From_Enum(EBorderStyle.None));
            new CssPropertyDefinition("border-bottom-style", false, EPropertyFlags.Block, CSSValue.From_Enum(EBorderStyle.None));
            new CssPropertyDefinition("border-left-style", false, EPropertyFlags.Block, CSSValue.From_Enum(EBorderStyle.None));

            new CssPropertyDefinition("border-top-width", false, EPropertyFlags.Block, CSSValue.From_Int(2), (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("border-right-width", false, EPropertyFlags.Block, CSSValue.From_Int(2), (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("border-bottom-width", false, EPropertyFlags.Block, CSSValue.From_Int(2), (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);
            new CssPropertyDefinition("border-left-width", false, EPropertyFlags.Block, CSSValue.From_Int(2), (uiElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }, true);

            new CssPropertyDefinition("overflow-x", false, EPropertyFlags.Block, CSSValue.From_Enum(EOverflowMode.Visible));
            new CssPropertyDefinition("overflow-y", false, EPropertyFlags.Block, CSSValue.From_Enum(EOverflowMode.Visible));

            new CssPropertyDefinition("positioning", false, EPropertyFlags.Block, CSSValue.From_Enum(EPositioning.Relative));

            new CssPropertyDefinition("object-fit", false, EPropertyFlags.Block, CSSValue.From_Enum(EObjectFit.Fill));
            
            new CssPropertyDefinition("object-position-x", false, EPropertyFlags.Block, CSSValue.From_Percent(50.0));
            new CssPropertyDefinition("object-position-y", false, EPropertyFlags.Block, CSSValue.From_Percent(50.0));

            new CssPropertyDefinition("intrinsic-width", false, EPropertyFlags.Block, CSSValue.Null, true);
            new CssPropertyDefinition("intrinsic-height", false, EPropertyFlags.Block, CSSValue.Null, true);
            new CssPropertyDefinition("intrinsic-ratio", false, EPropertyFlags.Block, CSSValue.Null, true);


            new CssPropertyDefinition("content-width", false, EPropertyFlags.Block, CSSValue.Null, true);
            new CssPropertyDefinition("content-height", false, EPropertyFlags.Block, CSSValue.Null, true);
        }
    }
}
