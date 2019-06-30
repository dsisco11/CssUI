using CssUI.CSS;
using CssUI.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CssUI.Internal
{
    /// <summary>
    /// Defines all of the possible CSS styles properties and their default values / behaviour flags
    /// </summary>
    public class CssProperties
    {
        public static readonly ReadOnlyDictionary<AtomicString, CssPropertyDefinition> Definitions;

        static CssProperties()
        {
            //Create a linked list we can populate with property definitions
            LinkedList<CssPropertyDefinition> Defs = new LinkedList<CssPropertyDefinition>();
            foreach (var def in Create_Font_Property_Definitions()) { Defs.AddLast(def); }
            foreach (var def in Create_System_Property_Definitions()) { Defs.AddLast(def); }
            foreach (var def in Create_Rendering_Property_Definitions()) { Defs.AddLast(def); }
            foreach (var def in Create_Layout_Property_Definitions()) { Defs.AddLast(def); }
            foreach (var def in Create_Sizing_Property_Definitions()) { Defs.AddLast(def); }
            foreach (var def in Create_Block_Property_Definitions()) { Defs.AddLast(def); }

            // Add all of out definitions to a backing dictionary
            var Dict = new Dictionary<AtomicString, CssPropertyDefinition>();
            foreach (var def in Defs) { Dict.Add(def.Name, def); }

            // Finally all of our definitions go into this global readonly map of Css property definitions!
            Definitions = new ReadOnlyDictionary<AtomicString, CssPropertyDefinition>(Dict);

        }

        #region Definitions
        /// <summary>
        /// Properties that are defined in the CSS fonts module
        /// </summary>
        /// <returns></returns>
        private static LinkedList<CssPropertyDefinition> Create_Font_Property_Definitions()
        {
            return new LinkedList<CssPropertyDefinition>(new CssPropertyDefinition[] {
                // SEE: https://www.w3.org/TR/CSS2/visudet.html#propdef-line-height
                new CssPropertyDefinition("line-height", true, EPropertyAffects.Flow, CssValue.From_Number(1.0), Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Style.FontSize); }),

                new CssPropertyDefinition("font-family", true, EPropertyAffects.Text | EPropertyAffects.Flow, CssValue.From_Keyword(ECssGenericFontFamily.SansSerif),
                    Keywords: new string[] { "serif", "sans-serif", "cursive", "fantasy", "monospace" },
                    AllowedTypes: EStyleDataType.STRING | EStyleDataType.KEYWORD,
                    Resolvers: new Tuple<ECssPropertyStage, PropertyResolverFunc>(ECssPropertyStage.Used, CssPropertyResolver.Font_Family_Used)
                    ),


                new CssPropertyDefinition("font-weight", true, EPropertyAffects.Text | EPropertyAffects.Flow, CssValue.From_Keyword("normal"),
                    DisallowedTypes: EStyleDataType.PERCENT,
                    AllowedTypes: EStyleDataType.KEYWORD | EStyleDataType.INTEGER,
                    Keywords: new string[] { "normal", "bold", "bolder", "lighter" },
                    Resolvers: new Tuple<ECssPropertyStage, PropertyResolverFunc>(ECssPropertyStage.Computed, CssPropertyResolver.Font_Weight_Computed)
                    ),


                new CssPropertyDefinition("font-style", true, EPropertyAffects.Text | EPropertyAffects.Flow, CssValue.From_Enum(EFontStyle.Normal)),
                new CssPropertyDefinition("font-size", true, EPropertyAffects.Text | EPropertyAffects.Flow, CssValue.From_Number(12),
                    Percentage_Resolver: (cssElement E, double Pct) => {
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
                    },
                    Resolvers: new Tuple<ECssPropertyStage, PropertyResolverFunc>(ECssPropertyStage.Used, CssPropertyResolver.Font_Size_Used)
                    ),
            });
        }

        /// <summary>
        /// Properties that can ONLY be set by the CssUI system
        /// </summary>
        /// <returns></returns>
        private static LinkedList<CssPropertyDefinition> Create_System_Property_Definitions()
        {
            return new LinkedList<CssPropertyDefinition>(new CssPropertyDefinition[] {
                new CssPropertyDefinition("dpi-x", true, EPropertyAffects.Text | EPropertyAffects.Flow, CssValue.Null, Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * 72.0); }),
                new CssPropertyDefinition("dpi-y", true, EPropertyAffects.Text | EPropertyAffects.Flow, CssValue.Null, Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * 72.0); }),

                new CssPropertyDefinition("intrinsic-width", false, EPropertyAffects.Block, CssValue.Null, IsPrivate: true, DisallowedTypes: EStyleDataType.INHERIT | EStyleDataType.AUTO),
                new CssPropertyDefinition("intrinsic-height", false, EPropertyAffects.Block, CssValue.Null, IsPrivate: true, DisallowedTypes: EStyleDataType.INHERIT | EStyleDataType.AUTO),
                new CssPropertyDefinition("intrinsic-ratio", false, EPropertyAffects.Block, CssValue.Null, IsPrivate: true, DisallowedTypes: EStyleDataType.INHERIT | EStyleDataType.AUTO),

                new CssPropertyDefinition("content-width", false, EPropertyAffects.Block, CssValue.Null, IsPrivate: true, DisallowedTypes: EStyleDataType.AUTO | EStyleDataType.INHERIT | EStyleDataType.PERCENT),
                new CssPropertyDefinition("content-height", false, EPropertyAffects.Block, CssValue.Null, IsPrivate: true, DisallowedTypes: EStyleDataType.AUTO | EStyleDataType.INHERIT | EStyleDataType.PERCENT)
            });
        }

        /// <summary>
        /// Properties that affect the way something is rendered
        /// </summary>
        /// <returns></returns>
        private static LinkedList<CssPropertyDefinition> Create_Rendering_Property_Definitions()
        {
            return new LinkedList<CssPropertyDefinition>(new CssPropertyDefinition[] {
                new CssPropertyDefinition("overflow-x", false, EPropertyAffects.Visual, CssValue.From_Enum(EOverflowMode.Visible)),
                new CssPropertyDefinition("overflow-y", false, EPropertyAffects.Visual, CssValue.From_Enum(EOverflowMode.Visible)),

                new CssPropertyDefinition("opacity", false, EPropertyAffects.Visual, CssValue.From_Number(1.0), DisallowedTypes: EStyleDataType.PERCENT),

                new CssPropertyDefinition("border-top-color", false, EPropertyAffects.Visual, CssValue.CurrentColor),
                new CssPropertyDefinition("border-right-color", false, EPropertyAffects.Visual, CssValue.CurrentColor),
                new CssPropertyDefinition("border-bottom-color", false, EPropertyAffects.Visual, CssValue.CurrentColor),
                new CssPropertyDefinition("border-left-color", false, EPropertyAffects.Visual, CssValue.CurrentColor),

                new CssPropertyDefinition("border-top-style", false, EPropertyAffects.Visual, CssValue.From_Enum(EBorderStyle.None)),
                new CssPropertyDefinition("border-right-style", false, EPropertyAffects.Visual, CssValue.From_Enum(EBorderStyle.None)),
                new CssPropertyDefinition("border-bottom-style", false, EPropertyAffects.Visual, CssValue.From_Enum(EBorderStyle.None)),
                new CssPropertyDefinition("border-left-style", false, EPropertyAffects.Visual, CssValue.From_Enum(EBorderStyle.None)),
                
                // Was concerned about using CssValue.None as the initial value but thats what the specs say
                // SEE: https://www.w3.org/TR/css-transforms-1/#transform-property
                // The transform property is completely visual and does not change an elements box
                new CssPropertyDefinition("transform", false, EPropertyAffects.Visual, CssValue.None)
            });
        }

        /// <summary>
        /// Properties that affect the way elements position their sub elements
        /// </summary>
        /// <returns></returns>
        private static LinkedList<CssPropertyDefinition> Create_Layout_Property_Definitions()
        {
            return new LinkedList<CssPropertyDefinition>(new CssPropertyDefinition[] {
                new CssPropertyDefinition("text-align", true, EPropertyAffects.Flow, CssValue.From_Enum(ETextAlign.Start))
            });
        }

        /// <summary>
        /// Properties that affect the sizing of sub elements
        /// </summary>
        /// <returns></returns>
        private static LinkedList<CssPropertyDefinition> Create_Sizing_Property_Definitions()
        {
            return new LinkedList<CssPropertyDefinition>(new CssPropertyDefinition[] {
                // The ‘object-fit’ property specifies how the contents of a replaced element should be fitted to the box established by its used height and width.
                new CssPropertyDefinition("object-fit", false, EPropertyAffects.Block, CssValue.From_Enum(EObjectFit.Fill), DisallowedTypes: EStyleDataType.INHERIT | EStyleDataType.AUTO, AllowedTypes: EStyleDataType.KEYWORD)
            });
        }

        /// <summary>
        /// Properties that directly determine the size or position of an elements box
        /// </summary>
        /// <returns></returns>
        private static LinkedList<CssPropertyDefinition> Create_Block_Property_Definitions()
        {
            return new LinkedList<CssPropertyDefinition>(new CssPropertyDefinition[] {
                
                // We differ from the for dirplays initial value because our implementation is for a UI
                new CssPropertyDefinition("display", false, EPropertyAffects.Block, CssValue.From_Enum(EDisplayMode.INLINE_BLOCK), DisallowedTypes: EStyleDataType.INHERIT),
                new CssPropertyDefinition("box-sizing", false, EPropertyAffects.Block, CssValue.From_Enum(EBoxSizingMode.BORDER)),

                new CssPropertyDefinition("positioning", false, EPropertyAffects.Block, CssValue.From_Enum(EPositioning.Relative)),
                new CssPropertyDefinition("object-position-x", false, EPropertyAffects.Block, CssValue.From_Percent(50.0), DisallowedTypes: EStyleDataType.INHERIT),
                new CssPropertyDefinition("object-position-y", false, EPropertyAffects.Block, CssValue.From_Percent(50.0), DisallowedTypes: EStyleDataType.INHERIT),

                new CssPropertyDefinition("top", false, EPropertyAffects.Block, CssValue.Auto, Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Height); }),
                new CssPropertyDefinition("right", false, EPropertyAffects.Block, CssValue.Auto, Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }),
                new CssPropertyDefinition("bottom", false, EPropertyAffects.Block, CssValue.Auto, Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Height); }),
                new CssPropertyDefinition("left", false, EPropertyAffects.Block, CssValue.Auto, Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }),


                new CssPropertyDefinition("width", false, EPropertyAffects.Block, CssValue.Auto, Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }),
                new CssPropertyDefinition("height", false, EPropertyAffects.Block, CssValue.Auto, Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Height); }),


                new CssPropertyDefinition("min-width", false, EPropertyAffects.Block, CssValue.Zero, Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }),
                new CssPropertyDefinition("min-height", false, EPropertyAffects.Block, CssValue.Zero, Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Height); }),

                new CssPropertyDefinition("max-width", false, EPropertyAffects.Block, CssValue.None, Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }),
                new CssPropertyDefinition("max-height", false, EPropertyAffects.Block, CssValue.None, Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Height); }),


                // Padding percentages are all calculated against the containing block's width so it is possible to create elements which conform to an aspect ratio
                new CssPropertyDefinition("padding-top", false, EPropertyAffects.Block, CssValue.Zero, Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }),
                new CssPropertyDefinition("padding-right", false, EPropertyAffects.Block, CssValue.Zero, Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }),
                new CssPropertyDefinition("padding-bottom", false, EPropertyAffects.Block, CssValue.Zero, Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }),
                new CssPropertyDefinition("padding-left", false, EPropertyAffects.Block, CssValue.Zero, Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }),


                // Margin percentages are all calculated against the containing block's width so it is possible to create elements which conform to an aspect ratio
                new CssPropertyDefinition("margin-top", false, EPropertyAffects.Block, CssValue.Zero, Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }),
                new CssPropertyDefinition("margin-right", false, EPropertyAffects.Block, CssValue.Zero, Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }),
                new CssPropertyDefinition("margin-bottom", false, EPropertyAffects.Block, CssValue.Zero, Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }),
                new CssPropertyDefinition("margin-left", false, EPropertyAffects.Block, CssValue.Zero, Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }),

                new CssPropertyDefinition("border-top-width", false, EPropertyAffects.Block, CssValue.From_Int(2), Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }),
                new CssPropertyDefinition("border-right-width", false, EPropertyAffects.Block, CssValue.From_Int(2), Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }),
                new CssPropertyDefinition("border-bottom-width", false, EPropertyAffects.Block, CssValue.From_Int(2), Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); }),
                new CssPropertyDefinition("border-left-width", false, EPropertyAffects.Block, CssValue.From_Int(2), Percentage_Resolver: (cssElement E, double Pct) => { return (Pct * E.Block_Containing.Width); })
            });
        }
        #endregion
    }
}
