using CssUI.CSS.Parser;

namespace CssUI.CSS.Selectors
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
}
