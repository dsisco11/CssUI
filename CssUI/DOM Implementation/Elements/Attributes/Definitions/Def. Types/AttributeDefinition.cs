using System;
using CssUI.Internal;
using System.Collections.Generic;
using CssUI.DOM.Enums;
using CssUI.DOM.Exceptions;
using System.Runtime.CompilerServices;
using System.Linq;
using CssUI.DOM.Serialization;

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
        public readonly AtomicString MissingValueDefault = null;
        /// <summary>
        /// 
        /// </summary>
        public readonly AtomicString InvalidValueDefault = null;
        /// <summary>
        /// If TRUE then this attribute will be inherited by default, meaning it's value is passed down to child elements rather then their matching property using it's 'initial' value
        /// </summary>
        public readonly bool Inherited = false;

        /// <summary>
        /// Allowed datatypes
        /// </summary>
        public readonly EAttributeType Type = 0x0;

        /// <summary>
        /// A list of all keywords that can be assigned to this property
        /// </summary>
        public readonly HashSet<string> Keywords = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a DOM attribute definition
        /// </summary>
        /// <param name="Name">DOM attribute name</param>
        /// <param name="Inherited">Do child elements inherit this value if they are unset?</param>
        /// <param name="Flags">Indicates what aspects of an element this property affects</param>
        /// <param name="MissingValueDefault">Default value for the attribute</param>
        /// <param name="Keywords">List of keywords which can be assigned to this attribute</param>
        public AttributeDefinition(AtomicName<EAttributeName> Name, bool Inherited, EAttributeFlags Flags, EAttributeType Type = 0x0, string[] Keywords = null, AtomicString MissingValueDefault = null, AtomicString InvalidValueDefault = null)
        {
            this.Name = Name;
            this.Flags = Flags;
            this.Inherited = Inherited;
            this.MissingValueDefault = MissingValueDefault;
            this.InvalidValueDefault = InvalidValueDefault;

            if (ReferenceEquals(null, Keywords))
            {
                this.Keywords = new HashSet<string>(new string[0]);
            }
            else
            {
                this.Keywords = new HashSet<string>(Keywords.Select(word => word.ToLowerInvariant()));
            }

            // Append the specified allowed types to our defaults
            this.Type |= Type;

        }

        #endregion

        #region Checks
        
        /// <summary>
        /// Throws an exception if the value is invalid according to the currently set options
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public void CheckAndThrow(string Value)
        {
            switch (Type)
            {
                case EAttributeType.String:// strings accept any value
                    break;
                case EAttributeType.Boolean:// we need no verification for booleans. they dont care what the value use, only whether its null or not
                    break;
                case EAttributeType.Enumerated:
                    {
                        if (!ReferenceEquals(null, Keywords) && Keywords.Count > 0)
                        {
                            if (!Keywords.Contains(Value.ToLowerInvariant()))
                            {
                                throw new DomSyntaxError($"Attribute {Name}: \"{Value}\" is not an acceptable value");
                            }
                        }
                    }
                    break;
                case EAttributeType.Integer:
                    {
                        if (!DOMParser.Parse_Integer(ref Value, out _))
                        {
                            throw new DomSyntaxError($"Attribute {Name}: \"{Value}\" is not an valid integer");
                        }
                    }
                    break;
                case EAttributeType.NonNegative_Integer:
                    {
                        if (!DOMParser.Parse_Integer(ref Value, out long outVal) || outVal < 0)
                        {
                            throw new DomSyntaxError($"Attribute {Name}: \"{Value}\" is not an valid non-negative integer");
                        }
                    }
                    break;
                case EAttributeType.FloatingPoint:
                    {
                        if (!DOMParser.Parse_FloatingPoint(ref Value, out _))
                        {
                            throw new DomSyntaxError($"Attribute {Name}: \"{Value}\" is not an valid number");
                        }
                    }
                    break;

                case EAttributeType.Length:
                    {
                        if (!DOMParser.Parse_Length(ref Value, out _, out EAttributeType outTy) || outTy != EAttributeType.Length)
                        {
                            throw new DomSyntaxError($"Attribute {Name}: \"{Value}\" is not an valid length");
                        }
                    }
                    break;
                case EAttributeType.NonZero_Length:
                    {
                        if (!DOMParser.Parse_Length(ref Value, out var outVal, out EAttributeType outTy) || outVal <= 0d || outTy != EAttributeType.Length)
                        {
                            throw new DomSyntaxError($"Attribute {Name}: \"{Value}\" is not an valid non-zero length");
                        }
                    }
                    break;
                case EAttributeType.Percentage:
                    {
                        if (!DOMParser.Parse_Length(ref Value, out _, out EAttributeType outTy) || outTy != EAttributeType.Percentage)
                        {
                            throw new DomSyntaxError($"Attribute {Name}: \"{Value}\" is not an valid percentage");
                        }
                    }
                    break;
                case EAttributeType.NonZero_Percentage:
                    {
                        if (!DOMParser.Parse_Length(ref Value, out var outVal, out EAttributeType outTy) || outVal <= 0d || outTy != EAttributeType.Percentage)
                        {
                            throw new DomSyntaxError($"Attribute {Name}: \"{Value}\" is not an valid non-zero percentage");
                        }
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AttributeDefinition Lookup(AtomicName<EAttributeName> Name)
        {
            if (DomDefinitions.AttributeDefinitions.TryGetValue(Name, out AttributeDefinition def))
            {
                return def;
            }

            return null;
        }
    }
}
