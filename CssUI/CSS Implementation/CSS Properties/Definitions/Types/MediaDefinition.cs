using System;
using CssUI.Internal;
using System.Collections.Generic;
using CssUI.CSS.Media;

namespace CssUI.CSS.Internal
{
    /// <summary>
    /// Holds all of, the specification defined, information about the valid values for a property and how to resolve said values into an absolute form.
    /// </summary>
    public class MediaDefinition
    {

        #region Properties
        /// <summary>
        /// Name of the property
        /// </summary>
        public readonly AtomicName<EMediaFeatureName> Name;
        /// <summary>
        /// Specifies if this feature name is a Discreet or Range type
        /// </summary>
        public readonly EMediaFeatureType Type;

        /// <summary>
        /// Allowed datatypes, when set this will override the disallowed list
        /// </summary>
        public readonly ECssValueType AllowedTypes = 0x0;

        /// <summary>
        /// A list of all keywords that can be assigned to this property
        /// </summary>
        public readonly List<string> KeywordWhitelist = null;

        public readonly EUnit DefaultUnit = EUnit.PX;
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
        public MediaDefinition(AtomicName<EMediaFeatureName> Name, EMediaFeatureType Type, ECssValueType AllowedTypes, string[] Keywords = null)
        {
            this.Name = Name;
            this.Type = Type;

            if (ReferenceEquals(Keywords, null))
                KeywordWhitelist = new List<string>();
            else
                KeywordWhitelist = new List<string>(Keywords);

            // Append the specified allowed types to our defaults
            this.AllowedTypes |= AllowedTypes;
        }

        #endregion

        #region Value Checking

        /// <summary>
        /// Returns whether the specified value is valid according to the currently set options
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool Is_Valid_Value_Type(ECssValueType Type)
        {
            return 0 != (AllowedTypes & Type);
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
            if (!Is_Valid_Value_Type(Value.Type))
                throw new CssException($"The property({Enum.GetName(typeof(EMediaFeatureName), Name.Value)}) cannot be set to an {Enum.GetName(typeof(ECssValueType), Value.Type)}!");

            switch (Value.Type)
            {
                case ECssValueType.KEYWORD:
                    {// check this value against our keyword whitelist
                        if (KeywordWhitelist != null && KeywordWhitelist.Count > 0)
                        {
                            if (!KeywordWhitelist.Contains(Value.Value as string))
                                throw new CssException($"Property({Enum.GetName(typeof(EMediaFeatureName), Name.Value)}) does not accept '{Value.Value as string}' as a value!");
                        }
                    }
                    break;
            }
        }
        #endregion
    }
}
