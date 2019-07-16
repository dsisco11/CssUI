
namespace CssUI.CSS.Selectors
{
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
}
