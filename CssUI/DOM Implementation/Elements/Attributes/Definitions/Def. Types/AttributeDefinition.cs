using System;
using CssUI.Internal;
using System.Collections.Generic;
using CssUI.DOM.Enums;
using CssUI.DOM.Exceptions;
using System.Runtime.CompilerServices;

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
        /// Value for this attribute which is treated as the 'specified' value if the definition does not have a value assigned to the attribute
        /// </summary>
        public readonly AtomicString Initial = null;
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
        /// <param name="Initial">Default value for the attribute</param>
        /// <param name="Keywords">List of keywords which can be assigned to this attribute</param>
        public AttributeDefinition(AtomicName<EAttributeName> Name, bool Inherited, EAttributeFlags Flags, AtomicString Initial, EAttributeType AllowedTypes = 0x0, string[] Keywords = null)
        {
            this.Name = Name;
            this.Flags = Flags;
            this.Inherited = Inherited;
            this.Initial = Initial;

            if (ReferenceEquals(null, Keywords))
            {
                this.Keywords = new HashSet<string>(new string[0]);
            }
            else
            {
                this.Keywords = new HashSet<string>(Keywords);
            }

            // Append the specified allowed types to our defaults
            this.Type |= AllowedTypes;

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
                            if (!Keywords.Contains(Value))
                            {
                                throw new DomSyntaxError($"Attribute {Name}: \"{Value}\" is not an acceptable value");
                            }
                        }
                    }
                    break;
                case EAttributeType.Numeric:
                    {
                        if (!Int32.TryParse(Value, out int _))
                        {
                            throw new DomSyntaxError($"Attribute {Name}: \"{Value}\" is not an valid number");
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
