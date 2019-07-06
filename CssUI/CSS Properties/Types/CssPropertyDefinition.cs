﻿using CssUI.CSS;
using System;
using System.Collections.Generic;

namespace CssUI.Internal
{
    /// <summary>
    /// Holds all, the specification defined, information about the valid values for a property and how to resolve said values into an absolute form.
    /// </summary>
    public class CssPropertyDefinition
    {

        #region Properties
        /// <summary>
        /// CSS Name of the property
        /// </summary>
        public readonly AtomicString Name;
        /// <summary>
        /// List of all the style values this property can be assigned or <c>null</c> to ignore
        /// </summary>
        public readonly CssValue[] LegalValues = null;
        /// <summary>
        /// Syntax for assigning to this property within a style sheet
        /// </summary>
        public readonly string Syntax = null;
        /// <summary>
        /// Value for this property which is treated as the 'specified' value if the property definition does not have 'Inherited' (inherited by default) set to true or if the owning elements has no parent elements.
        /// </summary>
        public readonly CssValue Initial = CssValue.Null;
        /// <summary>
        /// If TRUE then this property will be inherited by default, meaning it's value is passed down to child elements rather then their matching property using it's 'initial' value
        /// </summary>
        public readonly bool Inherited = false;
        /// <summary>
        /// Method used to resolve percentage values to computed values for this property, or NULL if percentages are not accepted.
        /// </summary>
        public readonly PercentageResolver Percentage_Resolver = null;
        /// <summary>
        /// If TRUE then this property cannot be set from style-sheets
        /// </summary>
        public readonly bool IsPrivate = false;
        /// <summary>
        /// Which 
        /// </summary>
        public readonly EPropertyDirtFlags Flags = EPropertyDirtFlags.Visual;
        /// <summary>
        /// All data types which are not allowed for this property
        /// </summary>
        public readonly ECssDataType DisallowedTypes = 0x0;

        /// <summary>
        /// Allowed datatypes, when set this will override the disallowed list
        /// </summary>
        public readonly ECssDataType AllowedTypes = (ECssDataType.INITIAL | ECssDataType.INHERIT | ECssDataType.UNSET);// By default we allow the CSS-wide identifiers. See: https://www.w3.org/TR/css-values-4/#common-keywords

        /// <summary>
        /// A list of all keywords that can be assigned to this property
        /// </summary>
        public readonly List<string> KeywordWhitelist = null;
        /// <summary>
        /// A map of resolution delegates to <see cref="ECssPropertyStage"/> which the defined property uses to resolve property values
        /// </summary>
        public readonly PropertyResolverFunc[] PropertyStageResolver = new PropertyResolverFunc[7];
        public readonly EStyleUnit DefaultUnit = EStyleUnit.PX;
        #endregion

        #region Constructors
        /// <summary>
        /// Define a new Css Styling property
        /// </summary>
        /// <param name="Name">CSS property name</param>
        /// <param name="Inherited">Do child elements inherit this value if they are unset?</param>
        /// <param name="Flags">Indicates what aspects of an element this property affects</param>
        /// <param name="Initial">Default value for the property</param>
        /// <param name="DisallowedTypes">Bitmask of all value data types which cannot be assigned to this property</param>
        /// <param name="Keywords">List of keywords which can be assigned to this property</param>
        /// <param name="IsPrivate">If TRUE then this property cannot be set from style-sheets</param>
        public CssPropertyDefinition(string Name, bool Inherited, EPropertyDirtFlags Flags, CssValue Initial, ECssDataType AllowedTypes = 0x0, ECssDataType DisallowedTypes = 0x0, string[] Keywords = null, bool IsPrivate = false, PercentageResolver Percentage_Resolver = null, params Tuple<ECssPropertyStage, PropertyResolverFunc>[] Resolvers)
        {
            this.Name = new AtomicString(Name);
            this.Flags = Flags;
            this.IsPrivate = IsPrivate;
            this.Inherited = Inherited;
            this.Initial = Initial;
            this.Percentage_Resolver = Percentage_Resolver;

            if (ReferenceEquals(Keywords, null))
                this.KeywordWhitelist = new List<string>();
            else
                this.KeywordWhitelist = new List<string>(Keywords);

            this.DisallowedTypes = DisallowedTypes;
            // Append the specified allowed types to our defaults
            this.AllowedTypes |= AllowedTypes;
            // Remove our disallowed types from our allowed ones
            this.AllowedTypes &= ~DisallowedTypes;

            // Setup our resolver index
            foreach(var o in Resolvers)
            {
                this.PropertyStageResolver[(int)o.Item1] = o.Item2;
            }
            
        }

        #endregion

        #region Value Checking

        /// <summary>
        /// Returns whether the specified value is valid according to the currently set options
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool IsValidDataType(ECssDataType Type)
        {
            if (AllowedTypes != 0x0)
                return (0 != (AllowedTypes & Type));
            // Bitwise check against our disallowed types
            return (0 == (DisallowedTypes & Type));
        }

        /// <summary>
        /// Throws an exception if any of the given values are invalid according to the currently set options
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public void CheckAndThrow(ICssProperty Owner, CssValueList Values)
        {
            foreach (CssValue Value in Values)
            {
                CheckAndThrow(Owner, Value);
            }
        }

        /// <summary>
        /// Throws an exception if the value is invalid according to the currently set options
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public void CheckAndThrow(ICssProperty Owner, CssValue Value)
        {
            if (!this.IsValidDataType(Value.Type))
                throw new CssException($"The property({Owner.CssName}) cannot be set to an {Enum.GetName(typeof(ECssDataType), Value.Type)}!");

            switch(Value.Type)
            {
                case ECssDataType.KEYWORD:
                    {// check this value against our keyword whitelist
                        if(KeywordWhitelist != null && KeywordWhitelist.Count > 0)
                        {
                            if (!KeywordWhitelist.Contains(Value.Value as string))
                                throw new CssException($"Property({Owner.CssName}) does not accept '{Value.Value as string}' as a value!");
                        }
                    }
                    break;
            }
        }
        #endregion
    }
}
