using CssUI.CSS;
using CssUI.CSS.Media;
using CssUI.Enums;
using CssUI.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Internal
{
    /// <summary>
    /// Defines all of the possible CSS styles properties and their default values / behaviour flags
    /// </summary>
    internal class CssDefinitions
    {
        internal static readonly ReadOnlyDictionary<AtomicString, StyleDefinition> StyleDefinitions;
        internal static readonly ReadOnlyDictionary<AtomicName<EMediaFeatureName>, MediaDefinition> MediaDefinitions;

        static CssDefinitions()
        {
            /* STYLE */

            var StyleDefs = Create_Style_Definitions();
            // Add all of our definitions to a backing dictionary
            var StyleDict = new Dictionary<AtomicString, StyleDefinition>();
            foreach (var def in StyleDefs) { StyleDict.Add(def.Name, def); }
            // Finally all of our definitions go into this global readonly map of Css property definitions!
            StyleDefinitions = new ReadOnlyDictionary<AtomicString, StyleDefinition>(StyleDict);


            /* MEDIA */

            var MediaDefs = Create_Media_Definitions();
            // Add all of our definitions to a backing dictionary
            var MediaDict = new Dictionary<AtomicName<EMediaFeatureName>, MediaDefinition>();
            foreach (var def in MediaDefs) { MediaDict.Add(def.Name, def); }
            // Finally all of our definitions go into this global readonly map of Css property definitions!
            MediaDefinitions = new ReadOnlyDictionary<AtomicName<EMediaFeatureName>, MediaDefinition>(MediaDict);
        }
        
        #region Media Definitions
        static IEnumerable<MediaDefinition> Create_Media_Definitions()
        {
            return new MediaDefinition[]
            {
                new MediaDefinition(EMediaFeatureName.Width, EMediaFeatureType.Range, ECssValueType.DIMENSION),
                new MediaDefinition(EMediaFeatureName.Min_Width, EMediaFeatureType.Range, ECssValueType.DIMENSION),
                new MediaDefinition(EMediaFeatureName.Max_Width, EMediaFeatureType.Range, ECssValueType.DIMENSION),

                new MediaDefinition(EMediaFeatureName.Height, EMediaFeatureType.Range, ECssValueType.DIMENSION),
                new MediaDefinition(EMediaFeatureName.Min_Height, EMediaFeatureType.Range, ECssValueType.DIMENSION),
                new MediaDefinition(EMediaFeatureName.Max_Height, EMediaFeatureType.Range, ECssValueType.DIMENSION),

                new MediaDefinition(EMediaFeatureName.AspectRatio, EMediaFeatureType.Range, ECssValueType.RATIO),
                new MediaDefinition(EMediaFeatureName.Min_AspectRatio, EMediaFeatureType.Range, ECssValueType.RATIO),
                new MediaDefinition(EMediaFeatureName.Max_AspectRatio, EMediaFeatureType.Range, ECssValueType.RATIO),

                new MediaDefinition(EMediaFeatureName.Orientation, EMediaFeatureType.Discreet, ECssValueType.KEYWORD, CssLookup.Get_Keywords(typeof(EMediaOrientation))),

                new MediaDefinition(EMediaFeatureName.Resolution, EMediaFeatureType.Range, ECssValueType.KEYWORD | ECssValueType.RESOLUTION, CssLookup.Get_Keywords(typeof(EMediaResolution))),
                new MediaDefinition(EMediaFeatureName.Min_Resolution, EMediaFeatureType.Range, ECssValueType.KEYWORD | ECssValueType.RESOLUTION, CssLookup.Get_Keywords(typeof(EMediaResolution))),
                new MediaDefinition(EMediaFeatureName.Max_Resolution, EMediaFeatureType.Range, ECssValueType.KEYWORD | ECssValueType.RESOLUTION, CssLookup.Get_Keywords(typeof(EMediaResolution))),

                new MediaDefinition(EMediaFeatureName.Scan, EMediaFeatureType.Discreet, ECssValueType.KEYWORD, CssLookup.Get_Keywords(typeof(EMediaScan))),

                new MediaDefinition(EMediaFeatureName.Grid, EMediaFeatureType.Discreet, ECssValueType.INTEGER),

                new MediaDefinition(EMediaFeatureName.Update, EMediaFeatureType.Discreet, ECssValueType.KEYWORD, CssLookup.Get_Keywords(typeof(EMediaUpdate))),

                new MediaDefinition(EMediaFeatureName.OverflowBlock, EMediaFeatureType.Discreet, ECssValueType.KEYWORD, CssLookup.Get_Keywords(typeof(EMediaOverflowBlock))),
                new MediaDefinition(EMediaFeatureName.OverflowInline, EMediaFeatureType.Discreet, ECssValueType.KEYWORD, CssLookup.Get_Keywords(typeof(EMediaOverflowInline))),

                new MediaDefinition(EMediaFeatureName.Color, EMediaFeatureType.Range, ECssValueType.INTEGER),

                new MediaDefinition(EMediaFeatureName.ColorIndex, EMediaFeatureType.Range, ECssValueType.INTEGER),

                new MediaDefinition(EMediaFeatureName.Monochrome, EMediaFeatureType.Range, ECssValueType.INTEGER),
                new MediaDefinition(EMediaFeatureName.Min_Monochrome, EMediaFeatureType.Range, ECssValueType.INTEGER),
                new MediaDefinition(EMediaFeatureName.Max_Monochrome, EMediaFeatureType.Range, ECssValueType.INTEGER),

                new MediaDefinition(EMediaFeatureName.ColorGamut, EMediaFeatureType.Discreet, ECssValueType.KEYWORD, CssLookup.Get_Keywords(typeof(EMediaColorGamut))),
            };
        }
        #endregion


        #region Style Definitions
        static IEnumerable<StyleDefinition> Create_Style_Definitions()
        {
            //Create a linked list we can populate with property definitions
            LinkedList<StyleDefinition> Definitions = new LinkedList<StyleDefinition>();
            foreach (var def in Create_Font_Property_Definitions()) { Definitions.AddLast(def); }
            foreach (var def in Create_System_Property_Definitions()) { Definitions.AddLast(def); }
            foreach (var def in Create_Rendering_Property_Definitions()) { Definitions.AddLast(def); }
            foreach (var def in Create_Layout_Property_Definitions()) { Definitions.AddLast(def); }
            foreach (var def in Create_Sizing_Property_Definitions()) { Definitions.AddLast(def); }
            foreach (var def in Create_Block_Property_Definitions()) { Definitions.AddLast(def); }

            return Definitions;
        }


        /// <summary>
        /// Properties that are defined in the CSS fonts module
        /// </summary>
        static IEnumerable<StyleDefinition> Create_Font_Property_Definitions()
        {
            return new StyleDefinition[] {
                // SEE: https://www.w3.org/TR/CSS2/visudet.html#propdef-line-height
                new StyleDefinition("line-height", true, EPropertyDirtFlags.Flow, CssValue.From_Number(1.0), Percentage_Resolver: (E, Pct) => CssValue.From_Number(Pct * E.Style.FontSize)),

                new StyleDefinition("font-family", true, EPropertyDirtFlags.Text | EPropertyDirtFlags.Flow, CssValue.From_Enum(ECssGenericFontFamily.SansSerif),
                    Keywords: Keywords.Font_Familys,
                    AllowedTypes: ECssValueType.STRING | ECssValueType.KEYWORD,
                    Resolvers: new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Font_Family_Used)
                    ),


                new StyleDefinition("font-weight", true, EPropertyDirtFlags.Text | EPropertyDirtFlags.Flow, CssValue.From_Keyword("normal"),
                    DisallowedTypes: ECssValueType.PERCENT,
                    AllowedTypes: ECssValueType.KEYWORD | ECssValueType.INTEGER,
                    Keywords: Keywords.Font_Weight,
                    Resolvers: new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Computed, CssPropertyResolver.Font_Weight_Computed)
                    ),


                new StyleDefinition("font-style", true, EPropertyDirtFlags.Text | EPropertyDirtFlags.Flow, CssValue.From_Enum(EFontStyle.Normal)),
                new StyleDefinition("font-size", true, EPropertyDirtFlags.Text | EPropertyDirtFlags.Flow, CssValue.From_Number(12),
                    Percentage_Resolver: (E, Pct) => {
                        if (E.Parent != null)
                        {
                            return CssValue.From_Int((int)(Pct * E.Parent.Style.FontSize));
                        }
                        else
                        {// fallback to definition
                            var def = StyleDefinitions[new AtomicString("font-size")];
                            //double r = def.Initial.Resolve() ?? throw new Exception("Failed to resolve default value from 'font-size' definition");
                            double r = (double)def.Initial.Value;
                            return CssValue.From_Int((int)(Pct * r));
                        }
                    },
                    Resolvers: new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Font_Size_Used)
                    ),
            };
        }

        /// <summary>
        /// Properties that can ONLY be set by the CssUI system
        /// </summary>
        static IEnumerable<StyleDefinition> Create_System_Property_Definitions()
        {
            return new StyleDefinition[] {
                new StyleDefinition("dpi-x", true, EPropertyDirtFlags.Text | EPropertyDirtFlags.Flow, CssValue.Null, ECssValueType.NUMBER, 0x0, null, false, (E, Pct) => CssValue.From_Number(Pct * 72.0)),
                new StyleDefinition("dpi-y", true, EPropertyDirtFlags.Text | EPropertyDirtFlags.Flow, CssValue.Null, ECssValueType.NUMBER, 0x0, null, false, (E, Pct) => CssValue.From_Number(Pct * 72.0)),
            };
        }

        /// <summary>
        /// Properties that affect the way something is rendered
        /// </summary>
        static IEnumerable<StyleDefinition> Create_Rendering_Property_Definitions()
        {
            return new StyleDefinition[] {
                new StyleDefinition("overflow-x", false, EPropertyDirtFlags.Visual, CssValue.From_Enum(EOverflowMode.Visible), ECssValueType.KEYWORD, 0x0, Keywords.Overflow),
                new StyleDefinition("overflow-y", false, EPropertyDirtFlags.Visual, CssValue.From_Enum(EOverflowMode.Visible), ECssValueType.KEYWORD, 0x0, Keywords.Overflow),

                new StyleDefinition("opacity", false, EPropertyDirtFlags.Visual, CssValue.From_Number(1.0), DisallowedTypes: ECssValueType.PERCENT),

                new StyleDefinition("border-top-color", false, EPropertyDirtFlags.Visual, CssValue.CurrentColor),
                new StyleDefinition("border-right-color", false, EPropertyDirtFlags.Visual, CssValue.CurrentColor),
                new StyleDefinition("border-bottom-color", false, EPropertyDirtFlags.Visual, CssValue.CurrentColor),
                new StyleDefinition("border-left-color", false, EPropertyDirtFlags.Visual, CssValue.CurrentColor),

                new StyleDefinition("border-top-style", false, EPropertyDirtFlags.Visual | EPropertyDirtFlags.Border_Area, CssValue.From_Enum(EBorderStyle.None), ECssValueType.KEYWORD),
                new StyleDefinition("border-right-style", false, EPropertyDirtFlags.Visual | EPropertyDirtFlags.Border_Area, CssValue.From_Enum(EBorderStyle.None), ECssValueType.KEYWORD),
                new StyleDefinition("border-bottom-style", false, EPropertyDirtFlags.Visual | EPropertyDirtFlags.Border_Area, CssValue.From_Enum(EBorderStyle.None), ECssValueType.KEYWORD),
                new StyleDefinition("border-left-style", false, EPropertyDirtFlags.Visual | EPropertyDirtFlags.Border_Area, CssValue.From_Enum(EBorderStyle.None), ECssValueType.KEYWORD),
                
                // Was concerned about using CssValue.None as the initial value but thats what the specs say
                // SEE: https://www.w3.org/TR/css-transforms-1/#transform-property
                // The transform property is completely visual and does not change an elements box
                new StyleDefinition("transform", false, EPropertyDirtFlags.Visual, CssValue.None)
            };
        }

        /// <summary>
        /// Properties that affect the way elements position their sub elements
        /// </summary>
        static IEnumerable<StyleDefinition> Create_Layout_Property_Definitions()
        {
            return new StyleDefinition[] {
                new StyleDefinition("direction", true, EPropertyDirtFlags.Flow, CssValue.From_Enum(EDirection.LTR), AllowedTypes: ECssValueType.KEYWORD, Keywords: Keywords.Direction),
                new StyleDefinition("writing-mode", true, EPropertyDirtFlags.Flow, CssValue.From_Enum(EWritingMode.Horizontal_TB), AllowedTypes: ECssValueType.KEYWORD, Keywords: Keywords.Writing_Mode),
                new StyleDefinition("text-align", true, EPropertyDirtFlags.Flow, CssValue.From_Enum(ETextAlign.Start), ECssValueType.KEYWORD, 0x0, Keywords.Text_Align)
            };
        }

        /// <summary>
        /// Properties that affect the sizing of sub elements
        /// </summary>
        static IEnumerable<StyleDefinition> Create_Sizing_Property_Definitions()
        {
            return new StyleDefinition[] {
                // The ‘object-fit’ property specifies how the contents of a replaced element should be fitted to the box established by its used height and width.
                new StyleDefinition("object-fit", false, EPropertyDirtFlags.Replaced_Area, CssValue.From_Enum(EObjectFit.Fill), ECssValueType.KEYWORD, ECssValueType.INHERIT | ECssValueType.AUTO)
            };
        }

        /// <summary>
        /// Properties that directly determine the size or position of an elements box
        /// </summary>
        static IEnumerable<StyleDefinition> Create_Block_Property_Definitions()
        {
            return new StyleDefinition[] {

                new StyleDefinition("display", false, EPropertyDirtFlags.Box, CssValue.From_Enum(EDisplayMode.INLINE_BLOCK), ECssValueType.KEYWORD, ECssValueType.INHERIT, Keywords.DisplayMode),
                new StyleDefinition("box-sizing", false, EPropertyDirtFlags.Content_Area | EPropertyDirtFlags.Border_Area, CssValue.From_Enum(EBoxSizingMode.BorderBox), ECssValueType.KEYWORD, Keywords: Keywords.Box_Sizing),

                new StyleDefinition("positioning", false, EPropertyDirtFlags.Margin_Area, CssValue.From_Enum(EPositioning.Relative)),
                new StyleDefinition("object-position-x", false, EPropertyDirtFlags.Replaced_Area, CssValue.From_Percent(50.0), DisallowedTypes: ECssValueType.INHERIT, Percentage_Resolver: (E, Pct) => CssValue.From_Int(CssAlgorithms.Solve_Object_Axis_Position((float)Pct, E.Box.Content.Get_Dimensions().Width, E.Box.Get_Replaced_Block_Size().Width))),
                new StyleDefinition("object-position-y", false, EPropertyDirtFlags.Replaced_Area, CssValue.From_Percent(50.0), DisallowedTypes: ECssValueType.INHERIT, Percentage_Resolver: (E, Pct) => CssValue.From_Int(CssAlgorithms.Solve_Object_Axis_Position((float)Pct, E.Box.Content.Get_Dimensions().Height, E.Box.Get_Replaced_Block_Size().Height))),

                new StyleDefinition("top", false, EPropertyDirtFlags.Margin_Area, CssValue.Auto, ECssValueType.AUTO | ECssValueType.DIMENSION | ECssValueType.PERCENT, 0x0, null, false, (E, Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalHeight), new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Box_Top_Used)),
                new StyleDefinition("right", false, EPropertyDirtFlags.Margin_Area, CssValue.Auto, ECssValueType.AUTO | ECssValueType.DIMENSION | ECssValueType.PERCENT, 0x0, null, false, (E, Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth), new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Box_Right_Used)),
                new StyleDefinition("bottom", false, EPropertyDirtFlags.Margin_Area, CssValue.Auto, ECssValueType.AUTO | ECssValueType.DIMENSION | ECssValueType.PERCENT, 0x0, null, false, (E, Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalHeight), new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Box_Bottom_Used)),
                new StyleDefinition("left", false, EPropertyDirtFlags.Margin_Area, CssValue.Auto, ECssValueType.AUTO | ECssValueType.DIMENSION | ECssValueType.PERCENT, 0x0, null, false, (E, Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth), new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Box_Left_Used)),


                new StyleDefinition("width", false, EPropertyDirtFlags.Content_Area, CssValue.Auto, ECssValueType.DIMENSION | ECssValueType.PERCENT | ECssValueType.AUTO | ECssValueType.INHERIT, 0x0, null, false, (E, Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth)),
                new StyleDefinition("height", false, EPropertyDirtFlags.Content_Area, CssValue.Auto, ECssValueType.DIMENSION | ECssValueType.PERCENT | ECssValueType.AUTO | ECssValueType.INHERIT, 0x0, null, false, (E, Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalHeight)),


                new StyleDefinition("min-width", false, EPropertyDirtFlags.Content_Area, CssValue.Auto, ECssValueType.DIMENSION | ECssValueType.PERCENT | ECssValueType.AUTO, 0x0, Keywords.MinMaxSize, false, (E, Pct) => !E.Box.Containing_Box_Explicit_Width ? CssValue.Zero : CssValue.From_Number(Pct * Math.Max(0, E.Box.Containing_Box.LogicalWidth)), new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Min_Width_Used) ),
                new StyleDefinition("min-height", false, EPropertyDirtFlags.Content_Area, CssValue.Auto, ECssValueType.DIMENSION | ECssValueType.PERCENT | ECssValueType.AUTO, 0x0, Keywords.MinMaxSize, false, (E, Pct) => !E.Box.Containing_Box_Explicit_Height ? CssValue.Zero : CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalHeight), new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Min_Height_Used)),

                new StyleDefinition("max-width", false, EPropertyDirtFlags.Content_Area, CssValue.None, ECssValueType.DIMENSION | ECssValueType.PERCENT | ECssValueType.NONE, 0x0, Keywords.MinMaxSize, false, (E, Pct) => !E.Box.Containing_Box_Explicit_Width ? CssValue.None : CssValue.From_Number(Pct * Math.Max(0, E.Box.Containing_Box.LogicalWidth)), new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Max_Width_Used)),
                new StyleDefinition("max-height", false, EPropertyDirtFlags.Content_Area, CssValue.None, ECssValueType.DIMENSION | ECssValueType.PERCENT | ECssValueType.NONE, 0x0, Keywords.MinMaxSize, false, (E, Pct) => !E.Box.Containing_Box_Explicit_Height ? CssValue.None :  CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalHeight), new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Max_Height_Used)),


                // Padding percentages are all calculated against the containing block's width so it is possible to create elements which conform to an aspect ratio
                new StyleDefinition("padding-top", false, EPropertyDirtFlags.Padding_Area, CssValue.Zero, AllowedTypes: ECssValueType.PERCENT | ECssValueType.DIMENSION, Percentage_Resolver: (E, Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth)),
                new StyleDefinition("padding-right", false, EPropertyDirtFlags.Padding_Area, CssValue.Zero, AllowedTypes: ECssValueType.PERCENT | ECssValueType.DIMENSION, Percentage_Resolver: (E, Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth)),
                new StyleDefinition("padding-bottom", false, EPropertyDirtFlags.Padding_Area, CssValue.Zero, AllowedTypes: ECssValueType.PERCENT | ECssValueType.DIMENSION, Percentage_Resolver: (E, Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth)),
                new StyleDefinition("padding-left", false, EPropertyDirtFlags.Padding_Area, CssValue.Zero, AllowedTypes: ECssValueType.PERCENT | ECssValueType.DIMENSION, Percentage_Resolver: (E, Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth)),

                new StyleDefinition("border-top-width", false, EPropertyDirtFlags.Border_Area, CssValue.From_Keyword("medium"), AllowedTypes: ECssValueType.DIMENSION | ECssValueType.KEYWORD, 0x0, Keywords.Border_Width, false, null, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Border_Width_Used)),
                new StyleDefinition("border-right-width", false, EPropertyDirtFlags.Border_Area, CssValue.From_Keyword("medium"), AllowedTypes: ECssValueType.DIMENSION | ECssValueType.KEYWORD, 0x0, Keywords.Border_Width, false, null, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Border_Width_Used)),
                new StyleDefinition("border-bottom-width", false, EPropertyDirtFlags.Border_Area, CssValue.From_Keyword("medium"), AllowedTypes: ECssValueType.DIMENSION | ECssValueType.KEYWORD, 0x0, Keywords.Border_Width, false, null, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Border_Width_Used)),
                new StyleDefinition("border-left-width", false, EPropertyDirtFlags.Border_Area, CssValue.From_Keyword("medium"), AllowedTypes: ECssValueType.DIMENSION | ECssValueType.KEYWORD, 0x0, Keywords.Border_Width, false, null, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Border_Width_Used)),

                // Margin percentages are all calculated against the containing block's width so it is possible to create elements which conform to an aspect ratio
                new StyleDefinition("margin-top", false, EPropertyDirtFlags.Margin_Area, CssValue.Zero, AllowedTypes: ECssValueType.AUTO | ECssValueType.PERCENT | ECssValueType.DIMENSION, 0x0, null, false, (E, Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth)),
                new StyleDefinition("margin-right", false, EPropertyDirtFlags.Margin_Area, CssValue.Zero, AllowedTypes: ECssValueType.AUTO | ECssValueType.PERCENT | ECssValueType.DIMENSION, 0x0, null, false, (E, Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth)),
                new StyleDefinition("margin-bottom", false, EPropertyDirtFlags.Margin_Area, CssValue.Zero, AllowedTypes: ECssValueType.AUTO | ECssValueType.PERCENT | ECssValueType.DIMENSION, 0x0, null, false, (E, Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth)),
                new StyleDefinition("margin-left", false, EPropertyDirtFlags.Margin_Area, CssValue.Zero, AllowedTypes: ECssValueType.AUTO | ECssValueType.PERCENT | ECssValueType.DIMENSION, 0x0, null, false, (E, Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth))
            };
        }
        #endregion
    }
}
