using System;
using CssUI.Internal;
using CssUI.CSS.Media;
using System.Diagnostics.Contracts;
using System.Collections.ObjectModel;

namespace CssUI.CSS.Internal
{
    /// <summary>
    /// Holds all of, the specification defined, information about the valid values for a property and how to resolve said values into an absolute form.
    /// </summary>
    public class MediaDefinition
    {
        private readonly string[] keywordWhitelist = null;

        #region Properties
        /// <summary>
        /// Name of the property
        /// </summary>
        public AtomicName<EMediaFeatureName> Name { get; }
        /// <summary>
        /// Specifies if this feature name is a Discreet or Range type
        /// </summary>
        public EMediaFeatureType Type { get; }

        /// <summary>
        /// Allowed datatypes, when set this will override the disallowed list
        /// </summary>
        public ECssValueTypes AllowedTypes { get; } = 0x0;

        /// <summary>
        /// A list of all keywords that can be assigned to this property
        /// </summary>
        public ReadOnlyCollection<string> KeywordWhitelist => new ReadOnlyCollection<string>(keywordWhitelist);
        public ECssUnit DefaultUnit { get; } = ECssUnit.PX;
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
        public MediaDefinition(AtomicName<EMediaFeatureName> Name, EMediaFeatureType Type, ECssValueTypes AllowedTypes, string[] Keywords = null)
        {
            this.Name = Name;
            this.Type = Type;

            if (Keywords is null)
                keywordWhitelist = Array.Empty<string>();
            else
                keywordWhitelist = Keywords;

            // Append the specified allowed types to our defaults
            this.AllowedTypes |= AllowedTypes;
        }

        #endregion

        #region Lookup
        public static MediaDefinition Lookup(EMediaFeatureName Name)
        {
            if (CssDefinitions.MediaDefinitions.TryGetValue(Name, out MediaDefinition def))
            {
                return def;
            }

            return null;
        }
        #endregion

        #region Value Checking

        /// <summary>
        /// Returns whether the specified value is valid according to the currently set options
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool Is_Valid_Value_Type(ECssValueTypes Type)
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
            if (Values is null) throw new ArgumentNullException(nameof(Values));
            Contract.EndContractBlock();

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
            if (Owner is null) throw new ArgumentNullException(nameof(Owner));
            if (Value is null) throw new ArgumentNullException(nameof(Value));
            Contract.EndContractBlock();

            if (!Is_Valid_Value_Type(Value.Type))
                throw new CssException($"The property({Enum.GetName(typeof(EMediaFeatureName), Name.Value)}) cannot be set to an {Enum.GetName(typeof(ECssValueTypes), Value.Type)}!");

            switch (Value.Type)
            {
                case ECssValueTypes.KEYWORD:
                    {// check this value against our keyword whitelist
                        if (KeywordWhitelist is object && KeywordWhitelist.Count > 0)
                        {
                            //if (!Array.Exists(keywordWhitelist, x => x.Equals(Value.AsString(), StringComparison.InvariantCultureIgnoreCase)))
                            if (!KeywordWhitelist.Contains(Value.AsString()))
                                throw new CssException($"Property({Enum.GetName(typeof(EMediaFeatureName), Name.Value)}) does not accept '{Value.AsString()}' as a value!");
                        }
                    }
                    break;
            }
        }
        #endregion
    }
}
