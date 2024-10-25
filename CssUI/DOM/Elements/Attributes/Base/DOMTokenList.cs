using CssUI.Enums;

using System;

namespace CssUI.DOM
{
    public class DOMTokenList : AttributeTokenList<AtomicString>
    {/* Docs: https://dom.spec.whatwg.org/#interface-domtokenlist */

        #region Constructor
        public DOMTokenList(Element ownerElement, AtomicName<EAttributeName> localName) : base(ownerElement, localName)
        {
        }

        public DOMTokenList(Element ownerElement, AtomicName<EAttributeName> localName, AtomicString[] supportedTokens) : base(ownerElement, localName, supportedTokens)
        {
        }
        #endregion


        protected override AtomicString String_To_Token(ReadOnlyMemory<char> data)
        {
            return new AtomicString(data, EAtomicStringFlags.CaseInsensitive);
        }

        protected override string Token_To_String(AtomicString token)
        {
            return token;
        }

    }
}
