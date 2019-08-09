using System;
using System.Collections.Generic;
using CssUI.DOM.Enums;
using CssUI.DOM.Exceptions;
using System.Runtime.CompilerServices;
using System.Linq;
using CssUI.HTML.Serialization;

namespace CssUI.DOM
{
    /// <summary>
    /// Holds all of, the specification defined, information about the valid values for a property and how to resolve said values into an absolute form.
    /// </summary>
    public class AttributeDefinition
    {
        #region Properties
        /// <summary>
        /// Name of the attribute
        /// </summary>
        public readonly AtomicName<EAttributeName> Name;

        public readonly EAttributeFlags Flags = 0x0;
        /// <summary>
        /// When the attribute is not specified, if there is a missing value default state defined, then that is the state represented by the (missing) attribute. Otherwise, the absence of the attribute means that there is no state represented.
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#missing-value-default
        public readonly AttributeValue MissingValueDefault = null;
        /// <summary>
        /// 
        /// </summary>
        public readonly AttributeValue InvalidValueDefault = null;
        /// <summary>
        /// Allowed datatypes
        /// </summary>
        public readonly EAttributeType Type = 0x0;
        /// <summary>
        /// A list of all keywords that can be assigned to this property
        /// </summary>
        public readonly HashSet<AtomicString> Keywords = null;
        /// <summary>
        /// A list of all tokens that can be assigned to this property
        /// </summary>
        public readonly HashSet<AtomicString> SupportedTokens = null;

        public readonly Type enumType = null;
        public readonly Type ElementType = null;

        /// <summary>
        /// The minimum value (if any) that can be assigned to this attribute
        /// </summary>
        public readonly dynamic LowerRange = null;
        /// <summary>
        /// The maximum value (if any) that can be assigned to this attribute
        /// </summary>
        public readonly dynamic UpperRange = null;
        #endregion

        #region Accessors
        /// <summary>
        /// If TRUE then this attribute will be inherited by default, meaning it's value is passed down to child elements rather then their matching property using it's 'initial' value
        /// </summary>
        public bool Inherited => 0 != (Flags & EAttributeFlags.Inherited);
        #endregion

        #region Constructors
        private AttributeDefinition(string[] Keywords, string[] SupportedTokens)
        {
            if (Keywords == null)
            {
                this.Keywords = new HashSet<AtomicString>(new AtomicString[0]);
            }
            else
            {
                var set = Keywords.Select(word => word.ToLowerInvariant()).Cast<AtomicString>();
                this.Keywords = new HashSet<AtomicString>(set);
            }

            if (SupportedTokens == null)
            {
                this.SupportedTokens = new HashSet<AtomicString>(new AtomicString[0]);
            }
            else
            {
                var set = SupportedTokens.Select(word => word.ToLowerInvariant()).Cast<AtomicString>();
                this.SupportedTokens = new HashSet<AtomicString>(set);
            }
        }

        /// <summary>
        /// Creates a DOM attribute definition
        /// </summary>
        /// <param name="Name">DOM attribute name</param>
        /// <param name="Flags">Indicates what aspects of an element this property affects</param>
        /// <param name="MissingValueDefault">Default value for the attribute</param>
        /// <param name="Keywords">List of keywords which can be assigned to this attribute</param>
        public AttributeDefinition(AtomicName<EAttributeName> Name, EAttributeType Type = 0x0, AttributeValue MissingValueDefault = null, AttributeValue InvalidValueDefault = null, EAttributeFlags Flags = 0x0, string[] Keywords = null, Type enumType = null, dynamic lowerRange = null, dynamic upperRange = null, string[] SupportedTokens = null)
            : this(Keywords, SupportedTokens)
        {
            this.ElementType = typeof(Element);
            this.Name = Name;
            this.Flags = Flags;
            this.MissingValueDefault = MissingValueDefault;
            this.InvalidValueDefault = InvalidValueDefault;
            this.enumType = enumType;
            this.LowerRange = lowerRange;
            this.UpperRange = upperRange;

            // Append the specified allowed types to our defaults
            this.Type |= Type;
        }

        /// <summary>
        /// Creates a DOM attribute definition
        /// </summary>
        /// <param name="Name">DOM attribute name</param>
        /// <param name="Flags">Indicates what aspects of an element this property affects</param>
        /// <param name="MissingValueDefault">Default value for the attribute</param>
        /// <param name="Keywords">List of keywords which can be assigned to this attribute</param>
        public AttributeDefinition(Type ElementType, AtomicName<EAttributeName> Name, EAttributeType Type = 0x0, AttributeValue MissingValueDefault = null, AttributeValue InvalidValueDefault = null, EAttributeFlags Flags = 0x0, string[] Keywords = null, Type enumType = null, dynamic lowerRange = null, dynamic upperRange = null, string[] SupportedTokens = null)
            : this(Keywords, SupportedTokens)
        {
            this.ElementType = ElementType;
            this.Name = Name;
            this.Flags = Flags;
            this.MissingValueDefault = MissingValueDefault;
            this.InvalidValueDefault = InvalidValueDefault;
            this.enumType = enumType;
            this.LowerRange = lowerRange;
            this.UpperRange = upperRange;

            // Append the specified allowed types to our defaults
            this.Type |= Type;
        }

        #endregion

        #region Parsing
        public void Parse(string Input, out dynamic outValue)
        {
            switch (Type)
            {
                case EAttributeType.String:// strings accept any value
                    {
                        outValue = Input;
                    }
                    break;
                case EAttributeType.Boolean:// we need no verification for booleans. they dont care what the value use, only whether its null or not
                    {
                        outValue = !ReferenceEquals(null, Input);
                    }
                    break;
                case EAttributeType.Enumerated:
                    {
                        if (Keywords != null && Keywords.Count > 0)
                        {
                            string strLower = Input.ToLowerInvariant();
                            if (!Keywords.Contains(strLower))
                            {
                                throw new DomSyntaxError($"Attribute {Name}: \"{Input}\" is not an acceptable value, Acceptable values are: {StringCommon.Concat(@", ".AsMemory(), Keywords.Select(o => o.AsMemory()))}");
                            }

                            if (!CssUI.Lookup.TryEnum(enumType, strLower, out var outEnum))
                            {
                                outValue = strLower;
                                throw new Exception($"Unable to find keyword value for \"{strLower}\" in enum: {enumType.Name}");
                            }

                            outValue = outEnum;
                        }
                        else
                        {
                            throw new DomSyntaxError($"Definition for enumerated attribute \"{Name}\" does not have any keywords specified!");
                        }
                    }
                    break;
                case EAttributeType.Integer:
                    {
                        if (!HTMLParserCommon.Try_Parse_Integer(Input.AsMemory(), out long outVal))
                        {
                            throw new DomSyntaxError($"Attribute {Name}: \"{Input}\" is not an valid integer");
                        }

                        outValue = outVal;
                    }
                    break;
                case EAttributeType.NonNegative_Integer:
                    {
                        if (!HTMLParserCommon.Try_Parse_Integer(Input.AsMemory(), out long outVal) || outVal < 0)
                        {
                            throw new DomSyntaxError($"Attribute {Name}: \"{Input}\" is not an valid non-negative integer");
                        }

                        outValue = (ulong)outVal;
                    }
                    break;
                case EAttributeType.FloatingPoint:
                    {
                        if (!HTMLParserCommon.Try_Parse_FloatingPoint(Input.AsMemory(), out var outVal))
                        {
                            throw new DomSyntaxError($"Attribute {Name}: \"{Input}\" is not an valid number");
                        }

                        outValue = outVal;
                    }
                    break;

                case EAttributeType.Length:
                    {
                        if (!HTMLParserCommon.Try_Parse_Length(Input.AsMemory(), out var outVal, out EAttributeType outTy) || outTy != EAttributeType.Length)
                        {
                            throw new DomSyntaxError($"Attribute {Name}: \"{Input}\" is not an valid length");
                        }

                        outValue = outVal;
                    }
                    break;
                case EAttributeType.NonZero_Length:
                    {
                        if (!HTMLParserCommon.Try_Parse_Length(Input.AsMemory(), out var outVal, out EAttributeType outTy) || outVal <= 0d || outTy != EAttributeType.Length)
                        {
                            throw new DomSyntaxError($"Attribute {Name}: \"{Input}\" is not an valid non-zero length");
                        }

                        outValue = outVal;
                    }
                    break;
                case EAttributeType.Percentage:
                    {
                        if (!HTMLParserCommon.Try_Parse_Length(Input.AsMemory(), out var outVal, out EAttributeType outTy) || outTy != EAttributeType.Percentage)
                        {
                            throw new DomSyntaxError($"Attribute {Name}: \"{Input}\" is not an valid percentage");
                        }

                        outValue = outVal;
                    }
                    break;
                case EAttributeType.NonZero_Percentage:
                    {
                        if (!HTMLParserCommon.Try_Parse_Length(Input.AsMemory(), out var outVal, out EAttributeType outTy) || outVal <= 0d || outTy != EAttributeType.Percentage)
                        {
                            throw new DomSyntaxError($"Attribute {Name}: \"{Input}\" is not an valid non-zero percentage");
                        }

                        outValue = outVal;
                    }
                    break;
                case EAttributeType.KeyCombo:
                    {
                        throw new NotImplementedException();
                    }
                    break;
                default:
                    {
                        throw new NotImplementedException();
                    }
                    break;
            }
        }
        #endregion

        #region Checks

        /// <summary>
        /// Throws an exception if the value is invalid according to the currently set options
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public void CheckAndThrow(string Value) => Parse(Value, out _);

        /// <summary>
        /// Throws an exception if the value is invalid according to the currently set options
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public void CheckAndThrow(AttributeValue Value)
        {
            if (Type == Value.Type)
                return;// We're good

            /* Our types dont exactly match so we need to just check some edge cases, like Integer vs. NonNegative_Integer */
            switch (Type)
            {
                case EAttributeType.Integer:
                case EAttributeType.NonNegative_Integer:
                    {
                        if (Value.Type == EAttributeType.Integer || Value.Type == EAttributeType.NonNegative_Integer)
                        {
                            return;// This case is fine
                        }
                    }
                    break;
                case EAttributeType.Length:
                case EAttributeType.NonZero_Length:
                    {
                        if (Value.Type == EAttributeType.Length || Value.Type == EAttributeType.NonZero_Length)
                        {
                            return;// This case is fine
                        }
                    }
                    break;
                case EAttributeType.Percentage:
                case EAttributeType.NonZero_Percentage:
                    {
                        if (Value.Type == EAttributeType.Percentage || Value.Type == EAttributeType.NonZero_Percentage)
                        {
                            return;// This case is fine
                        }
                    }
                    break;
            }

            throw new Exception($"A {Enum.GetName(typeof(EAttributeType), this.Type)} attribute cannot be assigned a {Enum.GetName(typeof(EAttributeType), Value.Type)} value!");
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AttributeDefinition Lookup(AtomicName<EAttributeName> Name)
        {
            if (DomDefinitions.AttributeDefinitions.TryGetValue(Name, out List<AttributeDefinition> definitionList))
            {
                if (definitionList.Count > 1)
                {
                    throw new Exception($"The content attribute \"{Name}\" has multiple definitions, an element type must be specified to determine the correct definition");
                }

                return definitionList[0];
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AttributeDefinition Lookup(AtomicName<EAttributeName> Name, Type elementType)
        {
            if (elementType == null)
            {
                throw new ArgumentNullException(nameof(elementType));
            }

            if (DomDefinitions.AttributeDefinitions.TryGetValue(Name, out List<AttributeDefinition> definitionList))
            {
                foreach (var def in definitionList)
                {
                    if (elementType.IsAssignableFrom(def.ElementType))
                    {
                        return def;
                    }
                }

                throw new Exception($"Unable to find the content attribute(\"{Name}\") definition for the element type \"{eType.Name}\"");
            }

            return null;
        }
    }
}
