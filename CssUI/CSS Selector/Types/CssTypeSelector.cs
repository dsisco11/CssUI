
namespace CssUI.CSS
{

    /// <summary>
    /// A type-selector matches an elements <see cref="cssElement.TypeName"/>
    /// </summary>
    public class CssTypeSelector : CssSimpleSelector
    {
        /// <summary>
        /// The namespace to restrict this type matcher too.
        /// <para>'*' if it matches ANY namespace</para>
        /// <para></para>
        /// </summary>
        readonly string Namespace;
        readonly string TypeName;

        public CssTypeSelector(string TypeName) : base(ECssSimpleSelectorType.TypeSelector)
        {
            if (string.Compare("*", TypeName) == 0) throw new CssParserException("Caught attempt to create a TypeSelector with the UniversalSelector symbol(*)!");
            this.Namespace = "*";// Match ANY namespace
            this.TypeName = TypeName;
        }

        public CssTypeSelector(string Namespace, string TypeName) : base(ECssSimpleSelectorType.TypeSelector)
        {
            this.Namespace = Namespace;
            this.TypeName = TypeName;
        }

        public CssTypeSelector(NamespacePrefixToken Namespace, string TypeName) : base(ECssSimpleSelectorType.TypeSelector)
        {
            this.Namespace = Namespace?.Value;
            this.TypeName = TypeName;
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        override public bool Matches(cssElement E)
        {
            // Note: not even sure the UI system will ever HAVE the concept of "namespaces" as those are really for web domain names
            if (Namespace != null)
            {
                if (string.Compare(Namespace, "*") != 0)
                {// Perform namespace matching
                }
            }
            else
            {// ONLY match null namespaces (what?!?)
            }

            return string.Compare(TypeName, E.TypeName, true) == 0;
        }
    }
}
