﻿using CssUI.CSS;
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
                new CssPropertyDefinition("line-height", true, EPropertyDirtFlags.Flow, CssValue.From_Number(1.0), Percentage_Resolver: (cssElement E, double Pct) => CssValue.From_Number(Pct * E.Style.FontSize)),

                new CssPropertyDefinition("font-family", true, EPropertyDirtFlags.Text | EPropertyDirtFlags.Flow, CssValue.From_Keyword(ECssGenericFontFamily.SansSerif),
                    Keywords: Keywords.Font_Familys,
                    AllowedTypes: EStyleDataType.STRING | EStyleDataType.KEYWORD,
                    Resolvers: new Tuple<ECssPropertyStage, PropertyResolverFunc>(ECssPropertyStage.Used, CssPropertyResolver.Font_Family_Used)
                    ),


                new CssPropertyDefinition("font-weight", true, EPropertyDirtFlags.Text | EPropertyDirtFlags.Flow, CssValue.From_Keyword("normal"),
                    DisallowedTypes: EStyleDataType.PERCENT,
                    AllowedTypes: EStyleDataType.KEYWORD | EStyleDataType.INTEGER,
                    Keywords: Keywords.Font_Weight,
                    Resolvers: new Tuple<ECssPropertyStage, PropertyResolverFunc>(ECssPropertyStage.Computed, CssPropertyResolver.Font_Weight_Computed)
                    ),


                new CssPropertyDefinition("font-style", true, EPropertyDirtFlags.Text | EPropertyDirtFlags.Flow, CssValue.From_Enum(EFontStyle.Normal)),
                new CssPropertyDefinition("font-size", true, EPropertyDirtFlags.Text | EPropertyDirtFlags.Flow, CssValue.From_Number(12),
                    Percentage_Resolver: (cssElement E, double Pct) => {
                        if (E.Parent != null)
                        {
                            return CssValue.From_Int((int)(Pct * E.Parent.Style.FontSize));
                        }
                        else
                        {// fallback to definition
                                var def = CssProperties.Definitions[new AtomicString("font-size")];
                            double r = def.Initial.Resolve() ?? throw new Exception("Failed to resolve default value from 'font-size' definition");
                            return CssValue.From_Int((int)(Pct * r));
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
                new CssPropertyDefinition("dpi-x", true, EPropertyDirtFlags.Text | EPropertyDirtFlags.Flow, CssValue.Null, Percentage_Resolver: (cssElement E, double Pct) => CssValue.From_Number(Pct * 72.0)),
                new CssPropertyDefinition("dpi-y", true, EPropertyDirtFlags.Text | EPropertyDirtFlags.Flow, CssValue.Null, Percentage_Resolver: (cssElement E, double Pct) => CssValue.From_Number(Pct * 72.0)),

                new CssPropertyDefinition("intrinsic-width", false, EPropertyDirtFlags.Content_Area | EPropertyDirtFlags.Replaced_Area, CssValue.Null, IsPrivate: true, DisallowedTypes: EStyleDataType.INHERIT | EStyleDataType.AUTO),
                new CssPropertyDefinition("intrinsic-height", false, EPropertyDirtFlags.Content_Area | EPropertyDirtFlags.Replaced_Area, CssValue.Null, IsPrivate: true, DisallowedTypes: EStyleDataType.INHERIT | EStyleDataType.AUTO),
                new CssPropertyDefinition("intrinsic-ratio", false, EPropertyDirtFlags.Content_Area | EPropertyDirtFlags.Replaced_Area, CssValue.Null, IsPrivate: true, DisallowedTypes: EStyleDataType.INHERIT | EStyleDataType.AUTO),

                new CssPropertyDefinition("content-width", false, EPropertyDirtFlags.Content_Area | EPropertyDirtFlags.Replaced_Area, CssValue.Null, IsPrivate: true, DisallowedTypes: EStyleDataType.AUTO | EStyleDataType.INHERIT | EStyleDataType.PERCENT),
                new CssPropertyDefinition("content-height", false, EPropertyDirtFlags.Content_Area | EPropertyDirtFlags.Replaced_Area, CssValue.Null, IsPrivate: true, DisallowedTypes: EStyleDataType.AUTO | EStyleDataType.INHERIT | EStyleDataType.PERCENT)
            });
        }

        /// <summary>
        /// Properties that affect the way something is rendered
        /// </summary>
        /// <returns></returns>
        private static LinkedList<CssPropertyDefinition> Create_Rendering_Property_Definitions()
        {
            return new LinkedList<CssPropertyDefinition>(new CssPropertyDefinition[] {
                new CssPropertyDefinition("overflow-x", false, EPropertyDirtFlags.Visual, CssValue.From_Enum(EOverflowMode.Visible)),
                new CssPropertyDefinition("overflow-y", false, EPropertyDirtFlags.Visual, CssValue.From_Enum(EOverflowMode.Visible)),

                new CssPropertyDefinition("opacity", false, EPropertyDirtFlags.Visual, CssValue.From_Number(1.0), DisallowedTypes: EStyleDataType.PERCENT),

                new CssPropertyDefinition("border-top-color", false, EPropertyDirtFlags.Visual, CssValue.CurrentColor),
                new CssPropertyDefinition("border-right-color", false, EPropertyDirtFlags.Visual, CssValue.CurrentColor),
                new CssPropertyDefinition("border-bottom-color", false, EPropertyDirtFlags.Visual, CssValue.CurrentColor),
                new CssPropertyDefinition("border-left-color", false, EPropertyDirtFlags.Visual, CssValue.CurrentColor),

                new CssPropertyDefinition("border-top-style", false, EPropertyDirtFlags.Visual | EPropertyDirtFlags.Border_Area, CssValue.From_Enum(EBorderStyle.None), EStyleDataType.KEYWORD),
                new CssPropertyDefinition("border-right-style", false, EPropertyDirtFlags.Visual | EPropertyDirtFlags.Border_Area, CssValue.From_Enum(EBorderStyle.None), EStyleDataType.KEYWORD),
                new CssPropertyDefinition("border-bottom-style", false, EPropertyDirtFlags.Visual | EPropertyDirtFlags.Border_Area, CssValue.From_Enum(EBorderStyle.None), EStyleDataType.KEYWORD),
                new CssPropertyDefinition("border-left-style", false, EPropertyDirtFlags.Visual | EPropertyDirtFlags.Border_Area, CssValue.From_Enum(EBorderStyle.None), EStyleDataType.KEYWORD),
                
                // Was concerned about using CssValue.None as the initial value but thats what the specs say
                // SEE: https://www.w3.org/TR/css-transforms-1/#transform-property
                // The transform property is completely visual and does not change an elements box
                new CssPropertyDefinition("transform", false, EPropertyDirtFlags.Visual, CssValue.None)
            });
        }

        /// <summary>
        /// Properties that affect the way elements position their sub elements
        /// </summary>
        /// <returns></returns>
        private static LinkedList<CssPropertyDefinition> Create_Layout_Property_Definitions()
        {
            return new LinkedList<CssPropertyDefinition>(new CssPropertyDefinition[] {
                new CssPropertyDefinition("direction", true, EPropertyDirtFlags.Flow, CssValue.From_Enum(ECssDirection.LTR), AllowedTypes: EStyleDataType.KEYWORD, Keywords: Keywords.Direction),
                new CssPropertyDefinition("writing-mode", true, EPropertyDirtFlags.Flow, CssValue.From_Enum(EWritingMode.Horizontal_TB), AllowedTypes: EStyleDataType.KEYWORD, Keywords: Keywords.Writing_Mode),
                new CssPropertyDefinition("text-align", true, EPropertyDirtFlags.Flow, CssValue.From_Enum(ETextAlign.Start))
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
                new CssPropertyDefinition("object-fit", false, EPropertyDirtFlags.Replaced_Area, CssValue.From_Enum(EObjectFit.Fill), DisallowedTypes: EStyleDataType.INHERIT | EStyleDataType.AUTO, AllowedTypes: EStyleDataType.KEYWORD)
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
                new CssPropertyDefinition("display", false, EPropertyDirtFlags.Box, CssValue.From_Enum(EDisplayMode.INLINE_BLOCK), DisallowedTypes: EStyleDataType.INHERIT),
                new CssPropertyDefinition("box-sizing", false, EPropertyDirtFlags.Content_Area | EPropertyDirtFlags.Border_Area, CssValue.From_Enum(EBoxSizingMode.BORDER), AllowedTypes: EStyleDataType.KEYWORD, Keywords: Keywords.Box_Sizing),

                new CssPropertyDefinition("positioning", false, EPropertyDirtFlags.Margin_Area, CssValue.From_Enum(EPositioning.Relative)),
                new CssPropertyDefinition("object-position-x", false, EPropertyDirtFlags.Replaced_Area, CssValue.From_Percent(50.0), DisallowedTypes: EStyleDataType.INHERIT, Percentage_Resolver: (cssElement E, double Pct) => CssValue.From_Int(CssAlgorithms.Solve_Object_Axis_Position((float)Pct, E.Box.Content.Get_Dimensions().Width, E.Box.Get_Replaced_Block_Size().Width))),
                new CssPropertyDefinition("object-position-y", false, EPropertyDirtFlags.Replaced_Area, CssValue.From_Percent(50.0), DisallowedTypes: EStyleDataType.INHERIT, Percentage_Resolver: (cssElement E, double Pct) => CssValue.From_Int(CssAlgorithms.Solve_Object_Axis_Position((float)Pct, E.Box.Content.Get_Dimensions().Height, E.Box.Get_Replaced_Block_Size().Height))),

                new CssPropertyDefinition("top", false, EPropertyDirtFlags.Margin_Area, CssValue.Auto, EStyleDataType.AUTO | EStyleDataType.DIMENSION | EStyleDataType.PERCENT, Percentage_Resolver: (cssElement E, double Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalHeight)),
                new CssPropertyDefinition("right", false, EPropertyDirtFlags.Margin_Area, CssValue.Auto, EStyleDataType.AUTO | EStyleDataType.DIMENSION | EStyleDataType.PERCENT, Percentage_Resolver: (cssElement E, double Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth)),
                new CssPropertyDefinition("bottom", false, EPropertyDirtFlags.Margin_Area, CssValue.Auto, EStyleDataType.AUTO | EStyleDataType.DIMENSION | EStyleDataType.PERCENT, Percentage_Resolver: (cssElement E, double Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalHeight)),
                new CssPropertyDefinition("left", false, EPropertyDirtFlags.Margin_Area, CssValue.Auto, EStyleDataType.AUTO | EStyleDataType.DIMENSION | EStyleDataType.PERCENT, Percentage_Resolver: (cssElement E, double Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth)),


                new CssPropertyDefinition("width", false, EPropertyDirtFlags.Content_Area, CssValue.Auto, EStyleDataType.DIMENSION | EStyleDataType.PERCENT | EStyleDataType.AUTO | EStyleDataType.INHERIT, 0x0, null, false, (cssElement E, double Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth)),
                new CssPropertyDefinition("height", false, EPropertyDirtFlags.Content_Area, CssValue.Auto, EStyleDataType.DIMENSION | EStyleDataType.PERCENT | EStyleDataType.AUTO | EStyleDataType.INHERIT, 0x0, null, false, (cssElement E, double Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalHeight)),


                new CssPropertyDefinition("min-width", false, EPropertyDirtFlags.Content_Area, CssValue.Auto, EStyleDataType.DIMENSION | EStyleDataType.PERCENT | EStyleDataType.AUTO, 0x0, Keywords.MinMaxSize, false, (cssElement E, double Pct) => !E.Box.Containing_Box_Explicit_Width ? CssValue.Zero : CssValue.From_Number(Pct * Math.Max(0, E.Box.Containing_Box.LogicalWidth)), new Tuple<ECssPropertyStage, PropertyResolverFunc>(ECssPropertyStage.Used, CssPropertyResolver.Min_Width_Used) ),
                new CssPropertyDefinition("min-height", false, EPropertyDirtFlags.Content_Area, CssValue.Auto, EStyleDataType.DIMENSION | EStyleDataType.PERCENT | EStyleDataType.AUTO, 0x0, Keywords.MinMaxSize, false, (cssElement E, double Pct) => (!E.Box.Containing_Box_Explicit_Height ? CssValue.Zero : CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalHeight)), new Tuple<ECssPropertyStage, PropertyResolverFunc>(ECssPropertyStage.Used, CssPropertyResolver.Min_Height_Used)),

                new CssPropertyDefinition("max-width", false, EPropertyDirtFlags.Content_Area, CssValue.None, EStyleDataType.DIMENSION | EStyleDataType.PERCENT | EStyleDataType.NONE, 0x0, Keywords.MinMaxSize, false, (cssElement E, double Pct) => !E.Box.Containing_Box_Explicit_Width ? CssValue.None : CssValue.From_Number(Pct * Math.Max(0, E.Box.Containing_Box.LogicalWidth)), new Tuple<ECssPropertyStage, PropertyResolverFunc>(ECssPropertyStage.Used, CssPropertyResolver.Max_Width_Used)),
                new CssPropertyDefinition("max-height", false, EPropertyDirtFlags.Content_Area, CssValue.None, EStyleDataType.DIMENSION | EStyleDataType.PERCENT | EStyleDataType.NONE, 0x0, Keywords.MinMaxSize, false, (cssElement E, double Pct) => (!E.Box.Containing_Box_Explicit_Height ? CssValue.None :  CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalHeight)), new Tuple<ECssPropertyStage, PropertyResolverFunc>(ECssPropertyStage.Used, CssPropertyResolver.Max_Height_Used)),


                // Padding percentages are all calculated against the containing block's width so it is possible to create elements which conform to an aspect ratio
                new CssPropertyDefinition("padding-top", false, EPropertyDirtFlags.Padding_Area, CssValue.Zero, AllowedTypes: EStyleDataType.PERCENT | EStyleDataType.DIMENSION, Percentage_Resolver: (cssElement E, double Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth)),
                new CssPropertyDefinition("padding-right", false, EPropertyDirtFlags.Padding_Area, CssValue.Zero, AllowedTypes: EStyleDataType.PERCENT | EStyleDataType.DIMENSION, Percentage_Resolver: (cssElement E, double Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth)),
                new CssPropertyDefinition("padding-bottom", false, EPropertyDirtFlags.Padding_Area, CssValue.Zero, AllowedTypes: EStyleDataType.PERCENT | EStyleDataType.DIMENSION, Percentage_Resolver: (cssElement E, double Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth)),
                new CssPropertyDefinition("padding-left", false, EPropertyDirtFlags.Padding_Area, CssValue.Zero, AllowedTypes: EStyleDataType.PERCENT | EStyleDataType.DIMENSION, Percentage_Resolver: (cssElement E, double Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth)),

                new CssPropertyDefinition("border-top-width", false, EPropertyDirtFlags.Border_Area, CssValue.From_Keyword("medium"), AllowedTypes: EStyleDataType.DIMENSION | EStyleDataType.KEYWORD, 0x0, Keywords.Border_Width, false, null, new Tuple<ECssPropertyStage, PropertyResolverFunc>(ECssPropertyStage.Used, CssPropertyResolver.Border_Width_Used)),
                new CssPropertyDefinition("border-right-width", false, EPropertyDirtFlags.Border_Area, CssValue.From_Keyword("medium"), AllowedTypes: EStyleDataType.DIMENSION | EStyleDataType.KEYWORD, 0x0, Keywords.Border_Width, false, null, new Tuple<ECssPropertyStage, PropertyResolverFunc>(ECssPropertyStage.Used, CssPropertyResolver.Border_Width_Used)),
                new CssPropertyDefinition("border-bottom-width", false, EPropertyDirtFlags.Border_Area, CssValue.From_Keyword("medium"), AllowedTypes: EStyleDataType.DIMENSION | EStyleDataType.KEYWORD, 0x0, Keywords.Border_Width, false, null, new Tuple<ECssPropertyStage, PropertyResolverFunc>(ECssPropertyStage.Used, CssPropertyResolver.Border_Width_Used)),
                new CssPropertyDefinition("border-left-width", false, EPropertyDirtFlags.Border_Area, CssValue.From_Keyword("medium"), AllowedTypes: EStyleDataType.DIMENSION | EStyleDataType.KEYWORD, 0x0, Keywords.Border_Width, false, null, new Tuple<ECssPropertyStage, PropertyResolverFunc>(ECssPropertyStage.Used, CssPropertyResolver.Border_Width_Used)),

                // Margin percentages are all calculated against the containing block's width so it is possible to create elements which conform to an aspect ratio
                new CssPropertyDefinition("margin-top", false, EPropertyDirtFlags.Margin_Area, CssValue.Zero, AllowedTypes: EStyleDataType.AUTO | EStyleDataType.PERCENT | EStyleDataType.DIMENSION, 0x0, null, false, (cssElement E, double Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth)),
                new CssPropertyDefinition("margin-right", false, EPropertyDirtFlags.Margin_Area, CssValue.Zero, AllowedTypes: EStyleDataType.AUTO | EStyleDataType.PERCENT | EStyleDataType.DIMENSION, 0x0, null, false, (cssElement E, double Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth)),
                new CssPropertyDefinition("margin-bottom", false, EPropertyDirtFlags.Margin_Area, CssValue.Zero, AllowedTypes: EStyleDataType.AUTO | EStyleDataType.PERCENT | EStyleDataType.DIMENSION, 0x0, null, false, (cssElement E, double Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth)),
                new CssPropertyDefinition("margin-left", false, EPropertyDirtFlags.Margin_Area, CssValue.Zero, AllowedTypes: EStyleDataType.AUTO | EStyleDataType.PERCENT | EStyleDataType.DIMENSION, 0x0, null, false, (cssElement E, double Pct) => CssValue.From_Number(Pct * E.Box.Containing_Box.LogicalWidth))
            }); ;
        }
        #endregion
    }
}
