using CssUI.CSS.Internal;
using CssUI.CSS.Serialization;
using CssUI.DOM;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace CssUI.CSS.Media
{
    /// <summary>
    /// </summary>
    public class MediaFeature : IMediaCondition, ICssSerializeable
    {/* Docs: https://www.w3.org/TR/mediaqueries-4/#mq-features */
        #region Properties
        private readonly EMediaCombinator Combinator = EMediaCombinator.None;
        private readonly EMediaFeatureContext Context;

        private readonly CssValue[] Values;
        private readonly EMediaOperator[] Operators;
        private bool IsValid = false;
        private bool IsNegated = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new 'boolean' type media feature
        /// </summary>
        /// <param name="name"></param>
        public MediaFeature(EMediaFeatureName Name)
        {
            Context = EMediaFeatureContext.Boolean;

            MediaDefinition def = MediaDefinition.Lookup(Name);
            if (def != null)
            {
                IsValid = true;
                /* if an allowed type is keywords and 'none' is an option then we match it against that keyword */
                if (0 != (def.AllowedTypes & ECssValueType.KEYWORD) && def.KeywordWhitelist.Contains("none"))
                {
                    Values = new CssValue[] { CssValue.From_Enum(Name), CssValue.From_Keyword("none") };
                }
                else if (0 != (def.AllowedTypes & ECssValueType.INTEGER))
                {
                    Values = new CssValue[] { CssValue.From_Enum(Name), CssValue.From_Int(0) };
                }
                else if (0 != (def.AllowedTypes & ECssValueType.DIMENSION))
                {
                    Values = new CssValue[] { CssValue.From_Enum(Name), CssValue.From_Length(0, EUnit.PX) };
                }
                else if (0 != (def.AllowedTypes & ECssValueType.RESOLUTION))
                {
                    Values = new CssValue[] { CssValue.From_Enum(Name), CssValue.From_Length(0, EUnit.DPPX) };
                }
            }
            else
            {
                /* This isnt a supported feature, therefore it should always evaluate to false */
                IsValid = false;
            }

            /* For sanity just catch the case where we dont have values to compare here */
            if (Values.Length <= 0)
            {
                IsValid = false;
            }
        }
        
        /// <summary>
        /// Creates a new 'range' type media feature
        /// </summary>
        /// <param name="name"></param>
        /// <param name="comparator"></param>
        /// <param name="value"></param>
        public MediaFeature(CssValue[] values, EMediaOperator[] ops)
        {
            Context = EMediaFeatureContext.Range;
            this.Values = values;
            this.Operators = ops;
            IsValid = true;
        }
        #endregion


        #region Matching
        public bool Matches(Document document)
        {
            if (!IsValid)
            {
                return false;
            }

            bool matched = Try_Match(document);
            switch (Combinator)
            {
                case EMediaCombinator.NOT:
                    {
                        return !matched;
                    }
                default:
                    {
                        return matched;
                    }
            }
        }

        private bool Try_Match(Document document)
        {
            switch (Context)
            {
                case EMediaFeatureContext.Boolean:
                    {
                        /*
                         * ...the media feature is evaluated in a boolean context. 
                         * If the feature would be true for any value other than the number 0, a <dimension> with the value 0, or the keyword none, the media feature evaluates to true. 
                         * Otherwise, it evaluates to false.
                         */
                        CssValue value = Values[0];
                        if (!Lookup.TryEnum((string)value.Value, out EMediaFeatureName nameLookup))
                        {/* This feature is not supported (or maybe valid) so we need to treat it as if it simply doesnt match */
                            IsValid = false;
                            return false;
                        }

                        CssValue valueA = Values[0];
                        CssValue valueB = Values[1];

                        switch (valueB.Type)
                        {
                            case ECssValueType.KEYWORD:
                                {
                                    return Compare_Keyword(document, nameLookup, (string)valueB.Value);
                                }
                            case ECssValueType.INTEGER:
                            case ECssValueType.NUMBER:
                                {
                                    double A = Resolve_Media_Name_Value(document, nameLookup);
                                    double B = (double)valueB.Value;
                                    return !MathExt.floatEq(A, (double)valueB.Value);
                                }
                            case ECssValueType.DIMENSION:
                            case ECssValueType.RESOLUTION:
                                {
                                    double A = Resolve_Media_Name_Value(document, nameLookup);
                                    double B = valueB.Resolve(document.cssUnitResolver);
                                    return !MathExt.floatEq(A, (double)valueB.Value);
                                }
                        }
                    }
                    break;
                case EMediaFeatureContext.Range:
                    {
                        /* Compare all value/operator pairs  */
                        for (int i=1; i<Operators.Length; i++)
                        {
                            CssValue A = Values[i - 1];
                            CssValue B = Values[i];
                            var op = Operators[i - 1];

                            if (!Compare(document, A, B, op))
                            {
                                return false;
                            }
                        }

                        return true;
                    }
            }

            return false;
        }


        bool Compare_Keyword(Document document, EMediaFeatureName Name, string keyword)
        {
            switch (Name)
            {
                case EMediaFeatureName.Orientation:
                    {
                        if (DomLookup.TryEnum(keyword, out EMediaOrientation outOrientation))
                        {
                            double width = Resolve_Media_Name_Value(document, EMediaFeatureName.Width);
                            double height = Resolve_Media_Name_Value(document, EMediaFeatureName.Height);

                            return (outOrientation == EMediaOrientation.Landscape) ? width >= height: height >= width;
                        }
                    }
                    break;
                case EMediaFeatureName.Update:
                    {
                        if (DomLookup.TryEnum(keyword, out EMediaUpdate outUpdateRate))
                        {
                            double refreshRate = Resolve_Media_Name_Value(document, Name);
                            switch (outUpdateRate)
                            {
                                case EMediaUpdate.None:
                                    {
                                        return MathExt.floatEq(0.0, refreshRate);
                                    }
                                case EMediaUpdate.Slow:
                                    {
                                        return refreshRate < 20;
                                    }
                                case EMediaUpdate.Fast:
                                    {
                                        return refreshRate > 20;
                                    }
                            }
                        }
                    }
                    break;
                case EMediaFeatureName.Color:
                case EMediaFeatureName.Min_Color:
                case EMediaFeatureName.Max_Color:
                    {
                        return Resolve_Media_Name_Value(document, Name) > 0;
                    }
                case EMediaFeatureName.Monochrome:
                case EMediaFeatureName.Min_Monochrome:
                case EMediaFeatureName.Max_Monochrome:
                    {
                        return Resolve_Media_Name_Value(document, Name) > 0;
                    }
                case EMediaFeatureName.ColorIndex:
                case EMediaFeatureName.Min_ColorIndex:
                case EMediaFeatureName.Max_ColorIndex:
                    {
                        return Resolve_Media_Name_Value(document, Name) > 0;
                    }
                case EMediaFeatureName.Grid:
                    {
                        return Resolve_Media_Name_Value(document, Name) > 0;
                    }
                case EMediaFeatureName.Width:
                case EMediaFeatureName.Min_Width:
                case EMediaFeatureName.Max_Width:
                    {
                        return Resolve_Media_Name_Value(document, Name) > 0;
                    }
                case EMediaFeatureName.Height:
                case EMediaFeatureName.Min_Height:
                case EMediaFeatureName.Max_Height:
                    {
                        return Resolve_Media_Name_Value(document, Name) > 0;
                    }
                case EMediaFeatureName.AspectRatio:
                case EMediaFeatureName.Min_AspectRatio:
                case EMediaFeatureName.Max_AspectRatio:
                    {
                        return Resolve_Media_Name_Value(document, Name) > 0;
                    }
            }

            return false;
        }

        bool Compare(Document document, CssValue ValueA, CssValue ValueB, EMediaOperator Op)
        {

            /* An Excerpt:
             * "If a media feature references a concept which does not exist on the device where the UA is running (for example, speech UAs do not have a concept of “width”), the media feature must always evaluate to false."
             */
            double A, B;
            /* First resolve these values to numbers we can compare */

            if (ValueA.Type == ECssValueType.KEYWORD)
            {
                if (!Lookup.TryEnum<EMediaFeatureName>(ValueA.Value, out EMediaFeatureName nameLookup))
                {/* If we cant find this keyword then we treat it as if it were an unsupported feature */
                    return false;
                }

                A = Resolve_Media_Name_Value(document, ValueA.Value);
            }
            else
            {
                A = (double)ValueA.Value;
            }


            if (ValueB.Type == ECssValueType.KEYWORD)
            {
                if (!Lookup.TryEnum<EMediaFeatureName>(ValueB.Value, out EMediaFeatureName nameLookup))
                {/* If we cant find this keyword then we treat it as if it were an unsupported feature */
                    return false;
                }

                B = Resolve_Media_Name_Value(document, ValueB.Value);
            }
            else
            {
                B = (double)ValueB.Value;
            }


            /* Compare them and return the result */
            switch (Op)
            {
                case EMediaOperator.EqualTo:
                    {
                        return MathExt.floatEq(A, B);
                    }
                case EMediaOperator.LessThan:
                    {
                        return (A < B);
                    }
                case EMediaOperator.GreaterThan:
                    {
                        return (A > B);
                    }
                case EMediaOperator.LessThanEq:
                    {
                        return (A < B || MathExt.floatEq(A, B));
                    }
                case EMediaOperator.GreaterThanEq:
                    {
                        return (A > B || MathExt.floatEq(A, B));
                    }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        dynamic Resolve_Media_Name_Value(Document document, EMediaFeatureName Name)
        {
            Screen screen = document.defaultView.screen;
            switch (Name)
            {
                case EMediaFeatureName.AspectRatio:
                case EMediaFeatureName.Min_AspectRatio:
                case EMediaFeatureName.Max_AspectRatio:
                    {
                        return (double)screen.width / (double)screen.height;
                    }
                case EMediaFeatureName.Width:
                case EMediaFeatureName.Min_Width:
                case EMediaFeatureName.Max_Width:
                    {
                        return screen.width;
                    }
                case EMediaFeatureName.Height:
                case EMediaFeatureName.Min_Height:
                case EMediaFeatureName.Max_Height:
                    {
                        return screen.height;
                    }
                case EMediaFeatureName.Color:
                case EMediaFeatureName.Min_Color:
                case EMediaFeatureName.Max_Color:
                    {
                        return screen.colorDepth;
                    }
                case EMediaFeatureName.Monochrome:
                case EMediaFeatureName.Min_Monochrome:
                case EMediaFeatureName.Max_Monochrome:
                    {
                        return screen.monochromeDepth;
                    }
                case EMediaFeatureName.ColorIndex:
                case EMediaFeatureName.Min_ColorIndex:
                case EMediaFeatureName.Max_ColorIndex:
                    {
                        return screen.colorIndex;
                    }
                case EMediaFeatureName.Grid:
                    {
                        return screen.isGrid;
                    }
                case EMediaFeatureName.Update:
                    {
                        return screen.refreshRate;
                    }
                default:
                    {
                        throw new NotImplementedException($"Media feature \"{Enum.GetName(typeof(EMediaFeatureName), Name)}\" is not implemented");
                    }
            }
        }

        public string Serialize()
        {
            if (!IsValid)
            {
                return string.Empty;
            }

            if (!Lookup.TryKeyword(Values[0].AsEnum<EMediaFeatureName>(), out string Name))
            {
                throw new CssSyntaxErrorException($"Could not find media-feature name");
            }


            StringBuilder sb = new StringBuilder();
            /* 1) Append a "(" (U+0028), followed by the media feature name, converted to ASCII lowercase, to s. */
            sb.Append(UnicodeCommon.CHAR_LEFT_PARENTHESES);

            if (Context == EMediaFeatureContext.Boolean)
            {
                sb.Append(Name);
            }
            else if (Context == EMediaFeatureContext.Range)
            {
                sb.Append(Name);

                if (Operators.Length == 0 && Values.Length == 2)
                {
                    /* If a value is given append a ":" (U+003A), followed by a single SPACE (U+0020), followed by the serialized media feature value. */
                    sb.Append(UnicodeCommon.CHAR_COLON);
                    sb.Append(UnicodeCommon.CHAR_SPACE);
                    sb.Append(Values[1].Serialize());
                }
                else
                {
                    /* Serialize all value/operator pairs  */
                    for (int i = 1; i < Operators.Length; i++)
                    {
                        CssValue B = Values[i];
                        EMediaOperator op = Operators[i - 1];
                        if (!Lookup.TryKeyword(op, out string outComparator))
                        {
                            throw new CssSyntaxErrorException("Unable to find the specified comparator within the CSS enum LUT");
                        }

                        sb.Append(B.Serialize());
                        sb.Append(outComparator);
                    }
                }
            }
            /* 3) Append a ")" (U+0029) to s. */
            sb.Append(UnicodeCommon.CHAR_RIGHT_PARENTHESES);

            return sb.ToString();
        }

        #endregion




    }
}
