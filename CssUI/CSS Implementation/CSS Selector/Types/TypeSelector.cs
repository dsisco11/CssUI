﻿using CssUI.DOM;
using CssUI.DOM.Nodes;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Selectors
{

    /// <summary>
    /// A type-selector matches an elements <see cref="cssElement.TypeName"/>
    /// </summary>
    public class TypeSelector : SimpleSelector
    {
        #region Properties
        /// <summary>
        /// The namespace to restrict this type matcher too.
        /// <para>'*' if it matches ANY namespace</para>
        /// <para></para>
        /// </summary>
        readonly string Namespace;
        readonly string TypeName;
        #endregion

        #region Constructors
        public TypeSelector(string TypeName) : base(ESimpleSelectorType.TypeSelector)
        {
            if (string.Compare("*", TypeName) == 0) throw new CssParserException("Caught attempt to create a TypeSelector with the UniversalSelector symbol(*)!");
            this.Namespace = "*";// Match ANY namespace
            this.TypeName = TypeName;
        }

        public TypeSelector(string Namespace, string TypeName) : base(ESimpleSelectorType.TypeSelector)
        {
            this.Namespace = Namespace;
            this.TypeName = TypeName;
        }

        public TypeSelector(NamespacePrefixToken Namespace, string TypeName) : base(ESimpleSelectorType.TypeSelector)
        {
            this.Namespace = Namespace?.Value;
            this.TypeName = TypeName;
        }
        #endregion

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        override public bool Matches(Element E, params Node[] scopeElements)
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

            return string.Compare(TypeName, E.tagName, true) == 0;
        }
    }
}
