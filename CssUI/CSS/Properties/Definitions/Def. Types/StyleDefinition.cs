using System;
using CssUI.Internal;
using CssUI.CSS.Enums;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Collections.ObjectModel;

namespace CssUI.CSS.Internal
{
    /// <summary>
    /// Holds all of, the specification defined, information about the valid values for a property and how to resolve said values into an absolute form.
    /// </summary>
    public class StyleDefinition
    {
        #region Properties
        /// <summary>
        /// CSS Name of the property
        /// </summary>
        public AtomicName<ECssPropertyID> Name { get; }
        /// <summary>
        /// Value for this property which is treated as the 'specified' value if the property definition does not have 'Inherited' (inherited by default) set to true or if the owning elements has no parent elements.
        /// </summary>
        public CssValue Initial { get; } = CssValue.Null;
        /// <summary>
        /// The default CSS unit this property uses when resolving
        /// </summary>
        public ECssUnit DefaultUnit { get; } = ECssUnit.PX;
        /// <summary>
        /// If TRUE then this property will be inherited by default, meaning it's value is passed down to child elements rather then their matching property using it's 'initial' value
        /// </summary>
        public bool Inherited { get; } = false;
        /// <summary>
        /// Method used to resolve percentage values to computed values for this property, or NULL if percentages are not accepted.
        /// </summary>
        public CssPercentageResolver Percentage_Resolver { get; } = null;
        /// <summary>
        /// If TRUE then this property cannot be set from style-sheets
        /// </summary>
        public bool IsPrivate { get; } = false;

        /// <summary>
        /// A list of all keywords that can be assigned to this property
        /// </summary>
        private readonly string[] keywordWhitelist = null;
        /// <summary>
        /// A map of resolution delegates to <see cref="EPropertyStage"/> which the defined property uses to resolve property values
        /// </summary>
        private readonly PropertyResolverFunc[] propertyStageResolver = new PropertyResolverFunc[7];

        /// <summary>
        /// A list of all keywords that can be assigned to this property
        /// </summary>
        public ReadOnlyCollection<string> KeywordWhitelist => new ReadOnlyCollection<string>(keywordWhitelist);

        /// <summary>
        /// A map of resolution delegates to <see cref="EPropertyStage"/> which the defined property uses to resolve property values
        /// </summary>
        public ReadOnlyCollection<PropertyResolverFunc> PropertyStageResolver => new ReadOnlyCollection<PropertyResolverFunc>(propertyStageResolver);

        /// <summary>
        /// Allowed datatypes, when set this will override the disallowed list
        /// </summary>
        public ECssValueTypes AllowedTypes { get; } = ECssValueTypes.INITIAL | ECssValueTypes.INHERIT | ECssValueTypes.UNSET;

        /// <summary>
        /// Specifies which aspects of an elements state this property will affect
        /// </summary>
        public EPropertyDirtFlags Flags { get; } = EPropertyDirtFlags.Visual;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new CSS property definition
        /// </summary>
        /// <param name="Name">CSS property name</param>
        /// <param name="Inherited">Do child elements inherit this value if they are unset?</param>
        /// <param name="Flags">Indicates what aspects of an element this property affects</param>
        /// <param name="Initial">Default value for the property</param>
        /// <param name="Keywords">List of keywords which can be assigned to this property</param>
        /// <param name="IsPrivate">If TRUE then this property cannot be set from style-sheets</param>
        public StyleDefinition(AtomicName<ECssPropertyID> Name, bool Inherited, EPropertyDirtFlags Flags, CssValue Initial, ECssValueTypes AllowedTypes = 0x0, string[] Keywords = null, bool IsPrivate = false, CssPercentageResolver Percentage_Resolver = null, params Tuple<EPropertyStage, PropertyResolverFunc>[] Resolvers)
        {
            this.Name = Name;
            this.Flags = Flags;
            this.IsPrivate = IsPrivate;
            this.Inherited = Inherited;
            this.Initial = Initial;
            this.Percentage_Resolver = Percentage_Resolver;

            if (Keywords is null)
                keywordWhitelist = Array.Empty<string>();
            else
                keywordWhitelist = Keywords.ToArray();

            // Append the specified allowed types to our defaults
            this.AllowedTypes |= AllowedTypes;

            // Setup our resolver index
            foreach (var o in Resolvers)
            {
                propertyStageResolver[(int)o.Item1] = o.Item2;
            }

        }

        #endregion

        #region Checks
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
            if (Owner is null) throw new ArgumentNullException(nameof(Owner));
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
                throw new CssException($"The property({Name}) cannot be set to an {Enum.GetName(typeof(ECssValueTypes), Value.Type)}!");

            switch (Value.Type)
            {
                case ECssValueTypes.KEYWORD:
                    {// check this value against our keyword whitelist
                        if (KeywordWhitelist is object && KeywordWhitelist.Count > 0)
                        {
                            if (!KeywordWhitelist.Contains(Value.ToString()))
                                throw new CssException($"Property({Name}) does not accept '{Value.ToString()}' as a value!");
                        }
                    }
                    break;
            }
        }
        #endregion
    }
}
