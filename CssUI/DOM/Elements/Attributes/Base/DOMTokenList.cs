using CssUI.DOM.CustomElements;
using CssUI.DOM.Exceptions;
using CssUI.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace CssUI.DOM
{
    public class DOMTokenList : AttributeTokenList<AtomicString>
    {/* Docs: https://dom.spec.whatwg.org/#interface-domtokenlist */

        #region Constructor
        public DOMTokenList(Element ownerElement, AtomicName<EAttributeName> localName) : base(ownerElement, localName)
        {
        }

        public DOMTokenList(Element ownerElement, AtomicName<EAttributeName> localName, string[] supportedTokens) : base(ownerElement, localName, null)
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
