
namespace CssUI.CSS
{
    /// <summary>
    /// Holds all of the information about a CSS property
    /// </summary>
    public class CssPropertyDefinition
    {// SEE  : https://www.w3.org/TR/CSS2/about.html#property-defs

        public delegate double StyleValuePercentageResolver(cssElement E, double Percent);
        #region Properties
        /// <summary>
        /// CSS Name of the property
        /// </summary>
        public readonly AtomicString Name;
        /// <summary>
        /// List of all the style values this property can be assigned or NULL to ignore
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
        public readonly StyleValuePercentageResolver Percentage_Resolver = null;
        /// <summary>
        /// Whether percentage values for this property depend on another value which is not available at the cascading stage.
        /// </summary>
        public readonly bool Percentage_Dependent = false;
        /// <summary>
        /// If TRUE then this property cannot be set from style-sheets
        /// </summary>
        public readonly bool IsPrivate = false;
        /// <summary>
        /// Which 
        /// </summary>
        public readonly EPropertyAffects Flags = EPropertyAffects.Visual;
        #endregion

        private CssPropertyDefinition(string Name, EPropertyAffects Flags)
        {
            this.Name = new AtomicString(Name);
            this.Flags = Flags;
            CssProperties.Definitions.Add(this.Name, this);
        }
        /// <summary>
        /// Define a new Css Styling property
        /// </summary>
        /// <param name="Name">CSS property name</param>
        /// <param name="Inherited">Do child elements inherit this value if they are unset?</param>
        /// <param name="Initial">Default value for the property</param>
        /// <param name="IsPrivate">If TRUE then this property cannot be set from style-sheets</param>
        public CssPropertyDefinition(string Name, bool Inherited, EPropertyAffects Flags, CssValue Initial, bool IsPrivate) : this(Name, Flags)
        {
            this.Inherited = Inherited;
            this.Initial = Initial;
            this.IsPrivate = IsPrivate;
        }
        /// <summary>
        /// Define a new Css Styling property
        /// </summary>
        /// <param name="Name">CSS property name</param>
        /// <param name="Inherited">Do child elements inherit this value if they are unset?</param>
        /// <param name="Initial">Default value for the property</param>
        /// <param name="Percentage_Resolver">Method used to resolve a percentage value into an absolute one</param>
        public CssPropertyDefinition(string Name, bool Inherited, EPropertyAffects Flags, CssValue Initial, StyleValuePercentageResolver Percentage_Resolver) : this(Name, Flags)
        {
            this.Inherited = Inherited;
            this.Initial = Initial;
            this.Percentage_Resolver = Percentage_Resolver;
        }

        /// <summary>
        /// Define a new Css Styling property
        /// </summary>
        /// <param name="Name">CSS property name</param>
        /// <param name="Inherited">Do child elements inherit this value if they are unset?</param>
        /// <param name="Initial">Default value for the property</param>
        /// <param name="Percentage_Resolver">Method used to resolve a percentage value into an absolute one</param>
        /// <param name="Percentage_Dependent">Whether percentage values for this property depend on other values an thus cannot be resolved at the cascading stage</param>
        public CssPropertyDefinition(string Name, bool Inherited, EPropertyAffects Flags, CssValue Initial, StyleValuePercentageResolver Percentage_Resolver, bool Percentage_Dependent) : this(Name, Flags)
        {
            this.Inherited = Inherited;
            this.Initial = Initial;
            this.Percentage_Resolver = Percentage_Resolver;
            this.Percentage_Dependent = Percentage_Dependent;
        }

        /// <summary>
        /// Define a new Css Styling property
        /// </summary>
        /// <param name="Name">CSS property name</param>
        /// <param name="Inherited">Do child elements inherit this value if they are unset?</param>
        /// <param name="Initial">Default value for the property</param>
        public CssPropertyDefinition(string Name, bool Inherited, EPropertyAffects Flags, CssValue Initial) : this(Name, Flags)
        {
            this.Inherited = Inherited;
            this.Initial = Initial;
        }

        /// <summary>
        /// Define a new Css Styling property
        /// </summary>
        /// <param name="Name">CSS property name</param>
        /// <param name="Inherited">Do child elements inherit this value if they are unset?</param>
        /// <param name="Initial">Default value for the property</param>
        /// <param name="LegalValues">List of values this property can be set to</param>
        public CssPropertyDefinition(string Name, bool Inherited, EPropertyAffects Flags, CssValue Initial, CssValue[] LegalValues) : this(Name, Flags)
        {
            this.Inherited = Inherited;
            this.Initial = Initial;
            this.LegalValues = LegalValues;
        }

        /// <summary>
        /// Define a new Css Styling property
        /// </summary>
        /// <param name="Name">CSS property name</param>
        /// <param name="Inherited">Do child elements inherit this value if they are unset?</param>
        /// <param name="Initial">Default value for the property</param>
        /// <param name="LegalValues">List of values this property can be set to</param>
        /// <param name="Percentage_Resolver">Method used to resolve a percentage value into an absolute one</param>
        public CssPropertyDefinition(string Name, bool Inherited, EPropertyAffects Flags, CssValue Initial, CssValue[] LegalValues, StyleValuePercentageResolver Percentage_Resolver) : this(Name, Inherited, Flags, Initial, Percentage_Resolver)
        {
            this.LegalValues = LegalValues;
        }

        /// <summary>
        /// Define a new Css Styling property
        /// </summary>
        /// <param name="Name">CSS property name</param>
        /// <param name="Inherited">Do child elements inherit this value if they are unset?</param>
        /// <param name="Initial">Default value for the property</param>
        /// <param name="LegalValues">List of values this property can be set to</param>
        /// <param name="Percentage_Resolver">Method used to resolve a percentage value into an absolute one</param>
        /// <param name="Percentage_Dependent">Whether percentage values for this property depend on other values an thus cannot be resolved at the cascading stage</param>
        public CssPropertyDefinition(string Name, bool Inherited, EPropertyAffects Flags, CssValue Initial, CssValue[] LegalValues, StyleValuePercentageResolver Percentage_Resolver, bool Percentage_Dependent) : this(Name, Inherited, Flags, Initial, Percentage_Resolver, Percentage_Dependent)
        {
            this.LegalValues = LegalValues;
            this.Percentage_Dependent = Percentage_Dependent;
        }
    }
}
