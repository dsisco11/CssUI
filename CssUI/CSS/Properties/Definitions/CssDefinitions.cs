using CssUI.CSS.Enums;
using CssUI.CSS.Media;
using CssUI.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CssUI.CSS.Internal
{
    /* XXX:
     * Refactor code to use an AtomicName<ECssProperty> for specifying CSS properties
     * https://www.w3.org/TR/css-sizing-3/
     */

    /// <summary>
    /// Defines all of the possible CSS styles properties and their default values / behaviour flags
    /// </summary>
    internal class CssDefinitions
    {
        internal static readonly ReadOnlyDictionary<AtomicName<ECssPropertyID>, StyleDefinition> StyleDefinitions = new ReadOnlyDictionary<AtomicName<ECssPropertyID>, StyleDefinition>(Create_Style_Definitions());
        internal static readonly ReadOnlyDictionary<AtomicName<EMediaFeatureName>, MediaDefinition> MediaDefinitions = new ReadOnlyDictionary<AtomicName<EMediaFeatureName>, MediaDefinition>(Create_Media_Definitions());

        
        #region Media Definitions
        static Dictionary<AtomicName<EMediaFeatureName>, MediaDefinition> Create_Media_Definitions()
        {
            var Definitions = new MediaDefinition[]
            {
                new MediaDefinition(EMediaFeatureName.Width, EMediaFeatureType.Range, ECssValueTypes.DIMENSION),
                new MediaDefinition(EMediaFeatureName.Min_Width, EMediaFeatureType.Range, ECssValueTypes.DIMENSION),
                new MediaDefinition(EMediaFeatureName.Max_Width, EMediaFeatureType.Range, ECssValueTypes.DIMENSION),

                new MediaDefinition(EMediaFeatureName.Height, EMediaFeatureType.Range, ECssValueTypes.DIMENSION),
                new MediaDefinition(EMediaFeatureName.Min_Height, EMediaFeatureType.Range, ECssValueTypes.DIMENSION),
                new MediaDefinition(EMediaFeatureName.Max_Height, EMediaFeatureType.Range, ECssValueTypes.DIMENSION),

                new MediaDefinition(EMediaFeatureName.AspectRatio, EMediaFeatureType.Range, ECssValueTypes.RATIO),
                new MediaDefinition(EMediaFeatureName.Min_AspectRatio, EMediaFeatureType.Range, ECssValueTypes.RATIO),
                new MediaDefinition(EMediaFeatureName.Max_AspectRatio, EMediaFeatureType.Range, ECssValueTypes.RATIO),

                new MediaDefinition(EMediaFeatureName.Orientation, EMediaFeatureType.Discreet, ECssValueTypes.KEYWORD, Lookup.Get_Keywords(typeof(EMediaOrientation))),

                new MediaDefinition(EMediaFeatureName.Resolution, EMediaFeatureType.Range, ECssValueTypes.KEYWORD | ECssValueTypes.RESOLUTION, Lookup.Get_Keywords(typeof(EMediaResolution))),
                new MediaDefinition(EMediaFeatureName.Min_Resolution, EMediaFeatureType.Range, ECssValueTypes.KEYWORD | ECssValueTypes.RESOLUTION, Lookup.Get_Keywords(typeof(EMediaResolution))),
                new MediaDefinition(EMediaFeatureName.Max_Resolution, EMediaFeatureType.Range, ECssValueTypes.KEYWORD | ECssValueTypes.RESOLUTION, Lookup.Get_Keywords(typeof(EMediaResolution))),

                new MediaDefinition(EMediaFeatureName.Scan, EMediaFeatureType.Discreet, ECssValueTypes.KEYWORD, Lookup.Get_Keywords(typeof(EMediaScan))),

                new MediaDefinition(EMediaFeatureName.Grid, EMediaFeatureType.Discreet, ECssValueTypes.INTEGER),

                new MediaDefinition(EMediaFeatureName.Update, EMediaFeatureType.Discreet, ECssValueTypes.KEYWORD, Lookup.Get_Keywords(typeof(EMediaUpdate))),

                new MediaDefinition(EMediaFeatureName.OverflowBlock, EMediaFeatureType.Discreet, ECssValueTypes.KEYWORD, Lookup.Get_Keywords(typeof(EMediaOverflowBlock))),
                new MediaDefinition(EMediaFeatureName.OverflowInline, EMediaFeatureType.Discreet, ECssValueTypes.KEYWORD, Lookup.Get_Keywords(typeof(EMediaOverflowInline))),

                new MediaDefinition(EMediaFeatureName.Color, EMediaFeatureType.Range, ECssValueTypes.INTEGER),

                new MediaDefinition(EMediaFeatureName.ColorIndex, EMediaFeatureType.Range, ECssValueTypes.INTEGER),

                new MediaDefinition(EMediaFeatureName.Monochrome, EMediaFeatureType.Range, ECssValueTypes.INTEGER),
                new MediaDefinition(EMediaFeatureName.Min_Monochrome, EMediaFeatureType.Range, ECssValueTypes.INTEGER),
                new MediaDefinition(EMediaFeatureName.Max_Monochrome, EMediaFeatureType.Range, ECssValueTypes.INTEGER),

                new MediaDefinition(EMediaFeatureName.ColorGamut, EMediaFeatureType.Discreet, ECssValueTypes.KEYWORD, Lookup.Get_Keywords(typeof(EMediaColorGamut))),
            };

            // Add all of our definitions to a backing dictionary
            var Dict = new Dictionary<AtomicName<EMediaFeatureName>, MediaDefinition>();
            foreach (var def in Definitions) { Dict.Add(def.Name, def); }

            return Dict;
        }
        #endregion


        #region Style Definitions
        static Dictionary<AtomicName<ECssPropertyID>, StyleDefinition> Create_Style_Definitions()
        {
            //Create a linked list we can populate with property definitions
            LinkedList<StyleDefinition> Definitions = new LinkedList<StyleDefinition>();
            foreach (var def in Create_Font_Property_Definitions()) { Definitions.AddLast(def); }
            foreach (var def in Create_System_Property_Definitions()) { Definitions.AddLast(def); }
            foreach (var def in Create_Rendering_Property_Definitions()) { Definitions.AddLast(def); }
            foreach (var def in Create_Layout_Property_Definitions()) { Definitions.AddLast(def); }
            foreach (var def in Create_Sizing_Property_Definitions()) { Definitions.AddLast(def); }
            foreach (var def in Create_Block_Property_Definitions()) { Definitions.AddLast(def); }

            // Add all of our definitions to a backing dictionary
            var Dict = new Dictionary<AtomicName<ECssPropertyID>, StyleDefinition>();
            foreach (var def in Definitions) { Dict.Add(def.Name, def); }

            return Dict;
        }


        /// <summary>
        /// Properties that are defined in the CSS fonts module
        /// </summary>
        static IEnumerable<StyleDefinition> Create_Font_Property_Definitions()
        {
            return new StyleDefinition[] {
                // SEE: https://www.w3.org/TR/CSS2/visudet.html#propdef-line-height
                new StyleDefinition(ECssPropertyID.LineHeight, true, EPropertyDirtFlags.Flow, CssValue.From(1.0), Percentage_Resolver: (Prop, Pct) => CssValue.From(Pct * Prop.Owner.Style.FontSize)),

                new StyleDefinition(ECssPropertyID.FontFamily, true, EPropertyDirtFlags.Text | EPropertyDirtFlags.Flow, CssValue.From(EGenericFontFamily.SansSerif), ECssValueTypes.STRING | ECssValueTypes.KEYWORD, Lookup.Get_Keywords<EGenericFontFamily>(), Resolvers: new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Font_Family_Used)
                    ),


                new StyleDefinition(ECssPropertyID.FontWeight, true, EPropertyDirtFlags.Text | EPropertyDirtFlags.Flow, CssValue.From(EFontWeight.Normal), ECssValueTypes.KEYWORD | ECssValueTypes.INTEGER, Lookup.Get_Keywords<EFontWeight>(), Resolvers: new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Computed, CssPropertyResolver.Font_Weight_Computed)
                    ),


                new StyleDefinition(ECssPropertyID.FontStyle, true, EPropertyDirtFlags.Text | EPropertyDirtFlags.Flow, CssValue.From(EFontStyle.Normal), ECssValueTypes.KEYWORD, Lookup.Get_Keywords<EFontStyle>()),
                //new StyleDefinition(ECssProperty.FontSize, true, EPropertyDirtFlags.Text | EPropertyDirtFlags.Flow, CssValue.From(12),
                new StyleDefinition(ECssPropertyID.FontSize, true, EPropertyDirtFlags.Text | EPropertyDirtFlags.Flow, CssValue.From(EFontSize.Medium), ECssValueTypes.DIMENSION | ECssValueTypes.PERCENT | ECssValueTypes.KEYWORD,  Lookup.Get_Keywords<EFontSize>(), false, Percentage_Resolver: (Prop, Pct) => {
                        if (!Prop.Owner.isRoot)
                        {
                            return CssValue.From(Pct * Prop.Owner.Style.FontSize);
                        }
                        else
                        {// fallback to definition
                            var def = StyleDefinitions[ECssPropertyID.FontSize];
                            //double r = def.Initial.Resolve() ?? throw new Exception("Failed to resolve default value from 'fontSize' definition");
                            var r = def.Initial.AsDecimal();
                            return CssValue.From(Pct * r);
                        }
                    },
                    new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Computed, CssPropertyResolver.Font_Size_Computed),
                    new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Font_Size_Used)
                    ),
            };
        }

        /// <summary>
        /// Properties that can ONLY be set by the CssUI system
        /// </summary>
        static IEnumerable<StyleDefinition> Create_System_Property_Definitions()
        {
            return new StyleDefinition[] {
                new StyleDefinition(ECssPropertyID.DpiX, true, EPropertyDirtFlags.Text | EPropertyDirtFlags.Flow, CssValue.Null, ECssValueTypes.NUMBER, null, false, (E, Pct) => CssValue.From(Pct * 72.0)),
                new StyleDefinition(ECssPropertyID.DpiY, true, EPropertyDirtFlags.Text | EPropertyDirtFlags.Flow, CssValue.Null, ECssValueTypes.NUMBER, null, false, (E, Pct) => CssValue.From(Pct * 72.0)),
            };
        }

        /// <summary>
        /// Properties that affect the way something is rendered
        /// </summary>
        static IEnumerable<StyleDefinition> Create_Rendering_Property_Definitions()
        {
            return new StyleDefinition[] {
                new StyleDefinition(ECssPropertyID.ScrollBehavior, false, EPropertyDirtFlags.Visual, CssValue.From(EScrollBehavior.Auto), ECssValueTypes.KEYWORD, Lookup.Get_Keywords<EScrollBehavior>()),
                new StyleDefinition(ECssPropertyID.OverflowX, false, EPropertyDirtFlags.Visual, CssValue.From(EOverflowMode.Visible), ECssValueTypes.KEYWORD, Lookup.Get_Keywords<EOverflowMode>()),
                new StyleDefinition(ECssPropertyID.OverflowY, false, EPropertyDirtFlags.Visual, CssValue.From(EOverflowMode.Visible), ECssValueTypes.KEYWORD, Lookup.Get_Keywords<EOverflowMode>()),

                new StyleDefinition(ECssPropertyID.Color, true, EPropertyDirtFlags.Visual, CssValue.From(EColor.Black), ECssValueTypes.KEYWORD | ECssValueTypes.COLOR | ECssValueTypes.INHERIT, Lookup.Get_Keywords<EColor>(), false, null, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Specified, CssPropertyResolver.Color_Specified), new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Computed, CssPropertyResolver.Color_Computed), new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Color_Used)),
                new StyleDefinition(ECssPropertyID.Opacity, false, EPropertyDirtFlags.Visual, CssValue.From(1.0), ECssValueTypes.NUMBER | ECssValueTypes.INTEGER, null, false, null, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Computed, CssPropertyResolver.Opacity_Computed)),

                new StyleDefinition(ECssPropertyID.BorderTopColor, false, EPropertyDirtFlags.Visual, CssValue.From(EColor.CurrentColor), ECssValueTypes.KEYWORD | ECssValueTypes.COLOR | ECssValueTypes.INHERIT, Lookup.Get_Keywords<EColor>(), false, null, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Specified, CssPropertyResolver.Color_Specified), new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Computed, CssPropertyResolver.Color_Computed), new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Color_Used)),
                new StyleDefinition(ECssPropertyID.BorderRightColor, false, EPropertyDirtFlags.Visual, CssValue.From(EColor.CurrentColor), ECssValueTypes.KEYWORD | ECssValueTypes.COLOR | ECssValueTypes.INHERIT, Lookup.Get_Keywords<EColor>(), false, null, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Specified, CssPropertyResolver.Color_Specified), new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Computed, CssPropertyResolver.Color_Computed), new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Color_Used)),
                new StyleDefinition(ECssPropertyID.BorderBottomColor, false, EPropertyDirtFlags.Visual, CssValue.From(EColor.CurrentColor), ECssValueTypes.KEYWORD | ECssValueTypes.COLOR | ECssValueTypes.INHERIT, Lookup.Get_Keywords<EColor>(), false, null, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Specified, CssPropertyResolver.Color_Specified), new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Computed, CssPropertyResolver.Color_Computed), new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Color_Used)),
                new StyleDefinition(ECssPropertyID.BorderLeftColor, false, EPropertyDirtFlags.Visual, CssValue.From(EColor.CurrentColor), ECssValueTypes.KEYWORD | ECssValueTypes.COLOR | ECssValueTypes.INHERIT, Lookup.Get_Keywords<EColor>(), false, null, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Specified, CssPropertyResolver.Color_Specified), new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Computed, CssPropertyResolver.Color_Computed), new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Color_Used)),

                new StyleDefinition(ECssPropertyID.BorderTopStyle, false, EPropertyDirtFlags.Visual | EPropertyDirtFlags.Border_Area, CssValue.From(EBorderStyle.None), ECssValueTypes.KEYWORD, Lookup.Get_Keywords<EBorderStyle>()),
                new StyleDefinition(ECssPropertyID.BorderRightStyle, false, EPropertyDirtFlags.Visual | EPropertyDirtFlags.Border_Area, CssValue.From(EBorderStyle.None), ECssValueTypes.KEYWORD, Lookup.Get_Keywords<EBorderStyle>()),
                new StyleDefinition(ECssPropertyID.BorderBottomStyle, false, EPropertyDirtFlags.Visual | EPropertyDirtFlags.Border_Area, CssValue.From(EBorderStyle.None), ECssValueTypes.KEYWORD, Lookup.Get_Keywords<EBorderStyle>()),
                new StyleDefinition(ECssPropertyID.BorderLeftStyle, false, EPropertyDirtFlags.Visual | EPropertyDirtFlags.Border_Area, CssValue.From(EBorderStyle.None), ECssValueTypes.KEYWORD, Lookup.Get_Keywords<EBorderStyle>()),
                
                // Was concerned about using CssValue.None as the initial value but thats what the specs say
                // SEE: https://www.w3.org/TR/css-transforms-1/#transform-property
                // The transform property is completely visual and does not change an elements box
                new StyleDefinition(ECssPropertyID.Transform, false, EPropertyDirtFlags.Visual, CssValue.None)
            };
        }

        /// <summary>
        /// Properties that affect the way elements position their sub elements
        /// </summary>
        static IEnumerable<StyleDefinition> Create_Layout_Property_Definitions()
        {
            return new StyleDefinition[] {
                new StyleDefinition(ECssPropertyID.Direction, true, EPropertyDirtFlags.Flow, CssValue.From(EDirection.LTR), ECssValueTypes.KEYWORD, Lookup.Get_Keywords<EDirection>()),
                new StyleDefinition(ECssPropertyID.WritingMode, true, EPropertyDirtFlags.Flow, CssValue.From(EWritingMode.Horizontal_TB), 0x0, Lookup.Get_Keywords<EWritingMode>()),
                new StyleDefinition(ECssPropertyID.TextAlign, true, EPropertyDirtFlags.Flow, CssValue.From(ETextAlign.Start), ECssValueTypes.KEYWORD, Lookup.Get_Keywords<ETextAlign>())
            };
        }

        /// <summary>
        /// Properties that affect the sizing of sub elements
        /// </summary>
        static IEnumerable<StyleDefinition> Create_Sizing_Property_Definitions()
        {
            return new StyleDefinition[] {
                // The ‘object-fit’ property specifies how the contents of a replaced element should be fitted to the box established by its used height and width.
                new StyleDefinition(ECssPropertyID.ObjectFit, false, EPropertyDirtFlags.Replaced_Area, CssValue.From(EObjectFit.Fill), ECssValueTypes.KEYWORD)
            };
        }

        /// <summary>
        /// Properties that directly determine the size or position of an elements box
        /// </summary>
        static IEnumerable<StyleDefinition> Create_Block_Property_Definitions()
        {
            return new StyleDefinition[] {

                new StyleDefinition(ECssPropertyID.Display, false, EPropertyDirtFlags.Box, CssValue.From(EDisplayMode.INLINE_BLOCK), ECssValueTypes.KEYWORD, Lookup.Get_Keywords<EDisplayMode>()),
                new StyleDefinition(ECssPropertyID.BoxSizing, false, EPropertyDirtFlags.Content_Area | EPropertyDirtFlags.Border_Area, CssValue.From(EBoxSizingMode.BorderBox), ECssValueTypes.KEYWORD, Lookup.Get_Keywords<EBoxSizingMode>()),

                new StyleDefinition(ECssPropertyID.Positioning, false, EPropertyDirtFlags.Margin_Area, CssValue.From(EBoxPositioning.Relative), ECssValueTypes.KEYWORD, Lookup.Get_Keywords<EBoxPositioning>()),

                new StyleDefinition(ECssPropertyID.ObjectPosition, false, EPropertyDirtFlags.Replaced_Area, CssValue.From(CssValue.Percent_50, CssValue.Percent_50), ECssValueTypes.COLLECTION | ECssValueTypes.KEYWORD | ECssValueTypes.INTEGER | ECssValueTypes.NUMBER | ECssValueTypes.PERCENT, Lookup.Get_Keywords<EPosition>(), false, Percentage_Resolver: (Prop, Pct) => CssValue.From(CssAlgorithms.Solve_Object_Axis_Position((float)Pct, Prop.Owner.Style.Box.Content.Width, Prop.Owner.Style.Box.Get_Replaced_Block_Size().Width)), new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Computed, CssPropertyResolver.Position_Computed), new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Position_Used)),
                // new StyleDefinition(ECssPropertyID.ObjectPositionX, false, EPropertyDirtFlags.Replaced_Area, CssValue.From_Percent(50.0), ECssValueTypes.POSITION, Percentage_Resolver: (Prop, Pct) => CssValue.From(CssAlgorithms.Solve_Object_Axis_Position((float)Pct, Prop.Owner.Style.Box.Content.Width, Prop.Owner.Style.Box.Get_Replaced_Block_Size().Width))),
                // new StyleDefinition(ECssPropertyID.ObjectPositionY, false, EPropertyDirtFlags.Replaced_Area, CssValue.From_Percent(50.0), ECssValueTypes.POSITION, Percentage_Resolver: (Prop, Pct) => CssValue.From(CssAlgorithms.Solve_Object_Axis_Position((float)Pct, Prop.Owner.Style.Box.Content.Height, Prop.Owner.Style.Box.Get_Replaced_Block_Size().Height))),

                new StyleDefinition(ECssPropertyID.Top, false, EPropertyDirtFlags.Margin_Area, CssValue.Auto, ECssValueTypes.AUTO | ECssValueTypes.DIMENSION | ECssValueTypes.PERCENT, null, false, CssPercentageResolvers.Containing_Block_Logical_Height, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Definite_Or_Zero_Used)),
                new StyleDefinition(ECssPropertyID.Right, false, EPropertyDirtFlags.Margin_Area, CssValue.Auto, ECssValueTypes.AUTO | ECssValueTypes.DIMENSION | ECssValueTypes.PERCENT, null, false, CssPercentageResolvers.Containing_Block_Logical_Width, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Definite_Or_Zero_Used)),
                new StyleDefinition(ECssPropertyID.Bottom, false, EPropertyDirtFlags.Margin_Area, CssValue.Auto, ECssValueTypes.AUTO | ECssValueTypes.DIMENSION | ECssValueTypes.PERCENT, null, false, CssPercentageResolvers.Containing_Block_Logical_Height, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Definite_Or_Zero_Used)),
                new StyleDefinition(ECssPropertyID.Left, false, EPropertyDirtFlags.Margin_Area, CssValue.Auto, ECssValueTypes.AUTO | ECssValueTypes.DIMENSION | ECssValueTypes.PERCENT, null, false, CssPercentageResolvers.Containing_Block_Logical_Width, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Definite_Or_Zero_Used)),


                new StyleDefinition(ECssPropertyID.Width, false, EPropertyDirtFlags.Content_Area, CssValue.Auto, ECssValueTypes.DIMENSION | ECssValueTypes.PERCENT | ECssValueTypes.AUTO | ECssValueTypes.INHERIT, null, false, CssPercentageResolvers.Containing_Block_Logical_Width),
                new StyleDefinition(ECssPropertyID.Height, false, EPropertyDirtFlags.Content_Area, CssValue.Auto, ECssValueTypes.DIMENSION | ECssValueTypes.PERCENT | ECssValueTypes.AUTO | ECssValueTypes.INHERIT, null, false, CssPercentageResolvers.Containing_Block_Logical_Height),


                new StyleDefinition(ECssPropertyID.MinWidth, false, EPropertyDirtFlags.Content_Area, CssValue.Auto, ECssValueTypes.DIMENSION | ECssValueTypes.PERCENT | ECssValueTypes.AUTO, Lookup.Get_Keywords<EBoxSize>(), false, CssPercentageResolvers.Containing_Block_Logical_Width, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Min_Width_Used) ),
                new StyleDefinition(ECssPropertyID.MinHeight, false, EPropertyDirtFlags.Content_Area, CssValue.Auto, ECssValueTypes.DIMENSION | ECssValueTypes.PERCENT | ECssValueTypes.AUTO, Lookup.Get_Keywords<EBoxSize>(), false, CssPercentageResolvers.Containing_Block_Logical_Height, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Min_Height_Used)),

                new StyleDefinition(ECssPropertyID.MaxWidth, false, EPropertyDirtFlags.Content_Area, CssValue.None, ECssValueTypes.DIMENSION | ECssValueTypes.PERCENT | ECssValueTypes.NONE, Lookup.Get_Keywords<EBoxSize>(), false, CssPercentageResolvers.Containing_Block_Logical_Width, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Max_Width_Used)),
                new StyleDefinition(ECssPropertyID.MaxHeight, false, EPropertyDirtFlags.Content_Area, CssValue.None, ECssValueTypes.DIMENSION | ECssValueTypes.PERCENT | ECssValueTypes.NONE, Lookup.Get_Keywords<EBoxSize>(), false, CssPercentageResolvers.Containing_Block_Logical_Height, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Max_Height_Used)),


                // Padding percentages are all calculated against the containing block's width so it is possible to create elements which conform to an aspect ratio
                new StyleDefinition(ECssPropertyID.PaddingTop, false, EPropertyDirtFlags.Padding_Area, CssValue.Zero, AllowedTypes: ECssValueTypes.PERCENT | ECssValueTypes.DIMENSION, Percentage_Resolver: CssPercentageResolvers.Containing_Block_Logical_Width),
                new StyleDefinition(ECssPropertyID.PaddingRight, false, EPropertyDirtFlags.Padding_Area, CssValue.Zero, AllowedTypes: ECssValueTypes.PERCENT | ECssValueTypes.DIMENSION, Percentage_Resolver: CssPercentageResolvers.Containing_Block_Logical_Width),
                new StyleDefinition(ECssPropertyID.PaddingBottom, false, EPropertyDirtFlags.Padding_Area, CssValue.Zero, AllowedTypes: ECssValueTypes.PERCENT | ECssValueTypes.DIMENSION, Percentage_Resolver: CssPercentageResolvers.Containing_Block_Logical_Width),
                new StyleDefinition(ECssPropertyID.PaddingLeft, false, EPropertyDirtFlags.Padding_Area, CssValue.Zero, AllowedTypes: ECssValueTypes.PERCENT | ECssValueTypes.DIMENSION, Percentage_Resolver: CssPercentageResolvers.Containing_Block_Logical_Width),

                new StyleDefinition(ECssPropertyID.BorderTopWidth, false, EPropertyDirtFlags.Border_Area, CssValue.From(EBorderSize.Medium), ECssValueTypes.DIMENSION | ECssValueTypes.KEYWORD, Lookup.Get_Keywords<EBorderSize>(), false, null, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Border_Width_Used)),
                new StyleDefinition(ECssPropertyID.BorderRightWidth, false, EPropertyDirtFlags.Border_Area, CssValue.From(EBorderSize.Medium), ECssValueTypes.DIMENSION | ECssValueTypes.KEYWORD, Lookup.Get_Keywords<EBorderSize>(), false, null, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Border_Width_Used)),
                new StyleDefinition(ECssPropertyID.BorderBottomWidth, false, EPropertyDirtFlags.Border_Area, CssValue.From(EBorderSize.Medium), ECssValueTypes.DIMENSION | ECssValueTypes.KEYWORD, Lookup.Get_Keywords<EBorderSize>(), false, null, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Border_Width_Used)),
                new StyleDefinition(ECssPropertyID.BorderLeftWidth, false, EPropertyDirtFlags.Border_Area, CssValue.From(EBorderSize.Medium), ECssValueTypes.DIMENSION | ECssValueTypes.KEYWORD, Lookup.Get_Keywords<EBorderSize>(), false, null, new Tuple<EPropertyStage, PropertyResolverFunc>(EPropertyStage.Used, CssPropertyResolver.Border_Width_Used)),

                // Margin percentages are all calculated against the containing block's width so it is possible to create elements which conform to an aspect ratio
                new StyleDefinition(ECssPropertyID.MarginTop, false, EPropertyDirtFlags.Margin_Area, CssValue.Zero, AllowedTypes: ECssValueTypes.AUTO | ECssValueTypes.PERCENT | ECssValueTypes.DIMENSION, Keywords: null, IsPrivate: false, Percentage_Resolver: CssPercentageResolvers.Containing_Block_Logical_Width),
                new StyleDefinition(ECssPropertyID.MarginRight, false, EPropertyDirtFlags.Margin_Area, CssValue.Zero, AllowedTypes: ECssValueTypes.AUTO | ECssValueTypes.PERCENT | ECssValueTypes.DIMENSION, Keywords: null, IsPrivate: false, Percentage_Resolver: CssPercentageResolvers.Containing_Block_Logical_Width),
                new StyleDefinition(ECssPropertyID.MarginBottom, false, EPropertyDirtFlags.Margin_Area, CssValue.Zero, AllowedTypes: ECssValueTypes.AUTO | ECssValueTypes.PERCENT | ECssValueTypes.DIMENSION, Keywords: null, IsPrivate: false, Percentage_Resolver: CssPercentageResolvers.Containing_Block_Logical_Width),
                new StyleDefinition(ECssPropertyID.MarginLeft, false, EPropertyDirtFlags.Margin_Area, CssValue.Zero, AllowedTypes: ECssValueTypes.AUTO | ECssValueTypes.PERCENT | ECssValueTypes.DIMENSION, Keywords: null, IsPrivate: false, Percentage_Resolver: CssPercentageResolvers.Containing_Block_Logical_Width)
            };
        }
#endregion
    }
}
