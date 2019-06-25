using System;
using System.Collections.Generic;

namespace CssUI.CSS
{
    public class NamespacePrefixToken : CssToken
    {
        public readonly string Value;
        public NamespacePrefixToken(string Value) : base(ECssTokenType.NamespacePrefix)
        {
            this.Value = Value;
        }

        public override string Encode()
        {
            return Value;
        }
    }

    /// <summary>
    /// A qualified name token will fundamentally always contain the namespace directly before it
    /// </summary>
    public class QualifiedNameToken : CssToken
    {
        public readonly NamespacePrefixToken Namespace;
        public readonly string Value;
        public QualifiedNameToken(string Value, NamespacePrefixToken Namespace) : base(ECssTokenType.QualifiedName)
        {
            this.Value = Value;
            this.Namespace = Namespace;
        }

        public override string Encode()
        {
            if (Namespace == null) return Value;
            return string.Concat(Namespace.Value, " ", Value);
        }
    }

    public class CombinatorToken : CssToken
    {
        public readonly string Value;
        public CombinatorToken(string Value) : base(ECssTokenType.Combinator)
        {
            this.Value = Value;
        }

        public override string Encode()
        {
            return Value;
        }
    }
}
