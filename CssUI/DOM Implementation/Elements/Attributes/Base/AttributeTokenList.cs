using CssUI.DOM.CustomElements;
using CssUI.DOM.Exceptions;
using CssUI.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CssUI.DOM
{
    public abstract class AttributeTokenList<TokenType> : IEnumerable<TokenType>, IAttributeTokenList
    {/* Docs: https://dom.spec.whatwg.org/#interface-domtokenlist */
        #region Backing List
        private List<TokenType> TokenSet = new List<TokenType>();
        #endregion

        #region Properties
        private readonly Element ownerElement;
        /// <summary>
        /// Name of the attribute this token list represents
        /// </summary>
        public readonly AtomicName<EAttributeName> localName;
        private readonly HashSet<TokenType> SupportedTokens = null;
        #endregion

        #region Constructor
        public AttributeTokenList(Element ownerElement, AtomicName<EAttributeName> localName, TokenType[] supportedTokens)
        {
            SupportedTokens = new HashSet<TokenType>(supportedTokens);
            /* 1) Let element be associated element. */
            this.ownerElement = ownerElement;
            /* 2) Let localName be associated attribute’s local name. */
            this.localName = localName;

            if (ownerElement.tokenListMap.ContainsKey(localName))
            {
                throw new DOMException($"Cannot link {nameof(DOMTokenList)} to element because it already has one defined for attribute \"{localName}\"");
            }
            else
            {
                ownerElement.tokenListMap.Add(localName, this);
            }
            /* 3) Let value be the result of getting an attribute value given element and localName. */
            this.ownerElement.find_attribute(localName, out Attr attr);
            //var value = attr?.Value.Get_String();
            /* 4) Run the attribute change steps for element, localName, value, value, and null. */
            run_attribute_change_steps(ownerElement, localName, attr.Value, attr.Value, null);
        }

        public AttributeTokenList(Element ownerElement, AtomicName<EAttributeName> localName) : this(ownerElement, localName, null)
        {
        }

        ~AttributeTokenList()
        {
            ownerElement?.tokenListMap.Remove(localName);
        }
        #endregion


        #region Utility
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void dtl_update()
        {
            /* A DOMTokenList object’s update steps are: */
            /* 1) If the associated element does not have an associated attribute and token set is empty, then return. */
            string name = localName.ToString();
            if (!ownerElement.AttributeList.ContainsKey(name) && TokenSet.Count <= 0)
                return;

            /* 2) Set an attribute value for the associated element using associated attribute’s local name and the result of running the ordered set serializer for token set. */
            string serialized = DOMCommon.Serialize_Ordered_Set(TokenSet.Select(tok => (Token_To_String(tok)).AsMemory()));
            ownerElement.setAttribute(localName, AttributeValue.From_String(serialized));
        }
        #endregion

        #region Accessors
        public int Length => TokenSet.Count;

        [CEReactions]
        public AtomicString Value
        {
            get => ownerElement.getAttribute(localName).Get_String();
            set => CEReactions.Wrap_CEReaction(ownerElement, () => ownerElement.setAttribute(localName, AttributeValue.From_String(value)));
        }

        public string item(int index)
        {
            /* The item(index) method, when invoked, must run these steps: */
            /* 1) If index is equal to or greater than context object’s token set’s size, then return null. */
            if (index >= Length)
                return null;

            /* 2) Return context object’s token set[index]. */
            return TokenSet[index];
        }

        #endregion

        #region Token Conversion
        protected abstract TokenType String_To_Token(ReadOnlyMemory<char> str);
        protected abstract string Token_To_String(TokenType token);
        #endregion

        public void run_attribute_change_steps(Element element, AtomicName<EAttributeName> localName, AttributeValue oldValue, AttributeValue newValue, ReadOnlyMemory<char> Namespace)
        {
            if (localName != this.localName) return;
            if (!ReferenceEquals(null, Namespace)) return;

            var value = newValue?.Get_String();
            /* 1) If localName is associated attribute’s local name, namespace is null, and value is null, then empty token set. */
            if (ReferenceEquals(null, value))
            {
                TokenSet = new List<TokenType>();
            }
            else
            {
                /* 2) Otherwise, if localName is associated attribute’s local name, namespace is null, then set token set to value, parsed. */
                TokenSet = new List<TokenType>();
                var valueMem = value.AsMemory();
                var rawTokens = DOMCommon.Parse_Ordered_Set(valueMem);

                foreach (var tokenMem in rawTokens)
                {
                    TokenSet.Add(String_To_Token(tokenMem));
                }
            }
        }



        public bool Contains(TokenType token)
        {
            /* The contains(token) method, when invoked, must return true if context object’s token set[token] exists, and false otherwise. */
            return TokenSet.Contains(token);
        }

        public bool ContainsAll(IEnumerable<TokenType> tokens)
        {
            /* The contains(token) method, when invoked, must return true if context object’s token set[token] exists, and false otherwise. */
            foreach (TokenType token in tokens)
            {
                if (!TokenSet.Contains(token))
                    return false;
            }

            return true;
        }

        public bool ContainsAll(params TokenType[] tokens)
        {
            /* The contains(token) method, when invoked, must return true if context object’s token set[token] exists, and false otherwise. */
            foreach (TokenType token in tokens)
            {
                if (!TokenSet.Contains(token))
                    return false;
            }

            return true;
        }

        public void Add(params TokenType[] tokens)
        {
            /* The add(tokens…) method, when invoked, must run these steps: */
            /* 1) For each token in tokens: */
            foreach (var token in tokens)
            {
                string tokenValue = Token_To_String(token);
                /* 1) If token is the empty string, then throw a "SyntaxError" DOMException. */
                if (ReferenceEquals(null, tokenValue) || tokenValue.Length <= 0)
                    throw new DomSyntaxError();
                /* 2) If token contains any ASCII whitespace, then throw an "InvalidCharacterError" DOMException. */
                bool hasWhitespace = default(char) != tokenValue.ToString().ToCharArray().SingleOrDefault(c => UnicodeCommon.Is_Ascii_Whitespace(c));
                if (hasWhitespace)
                    throw new InvalidCharacterError("tokens cannot contain whitespaces");
            }
            /* 2) For each token in tokens, append token to context object’s token set. */
            foreach (var token in tokens)
            {
                TokenSet.Add(token);
                //TokenSet.Add(new AtomicString(token, EAtomicStringFlags.CaseInsensitive));
            }
            /* 3) Run the update steps. */
            dtl_update();
        }

        public void Remove(params TokenType[] tokens)
        {
            /* The remove(tokens…) method, when invoked, must run these steps: */
            /* 1) For each token in tokens: */
            foreach (TokenType token1 in tokens)
            {
                string tokenValue = Token_To_String(token1);
                /* 1) If token is the empty string, then throw a "SyntaxError" DOMException. */
                if (ReferenceEquals(null, tokenValue) || tokenValue.Length <= 0)
                    throw new DomSyntaxError("Token cannot be null.");
                /* 2) If token contains any ASCII whitespace, then throw an "InvalidCharacterError" DOMException. */
                bool hasWhitespace = default(char) != tokenValue.ToString().ToCharArray().SingleOrDefault(c => UnicodeCommon.Is_Ascii_Whitespace(c));
                if (hasWhitespace)
                    throw new InvalidCharacterError("Tokens cannot contain whitespaces");
            }
            /* 2) For each token in tokens, remove token from context object’s token set. */
            foreach (TokenType token in tokens)
            {
                TokenSet.Remove(token);
            }
            /* 3) Run the update steps. */
            dtl_update();
        }

        public bool Toggle(TokenType token, bool? force = null)
        {
            /* The toggle(token, force) method, when invoked, must run these steps: */
            /* 1) If token is the empty string, then throw a "SyntaxError" DOMException. */
            string tokenValue = Token_To_String(token);
            if (string.IsNullOrEmpty(tokenValue))
                throw new DomSyntaxError("Token cannot be null or empty.");
            /* 2) If token contains any ASCII whitespace, then throw an "InvalidCharacterError" DOMException. */
            bool hasWhitespace = default(char) != tokenValue.ToString().ToCharArray().SingleOrDefault(c => UnicodeCommon.Is_Ascii_Whitespace(c));
            if (hasWhitespace)
                throw new InvalidCharacterError("Tokens cannot contain whitespaces");
            /* 3) If context object’s token set[token] exists, then: */
            if (TokenSet.Contains(token))
            {
                /* 1) If force is either not given or is false, then remove token from context object’s token set, run the update steps and return false. */
                if (!force.HasValue || !force.Value)
                {
                    dtl_update();
                    return false;
                }
                return true;
            }
            /* 4) Otherwise, if force not given or is true, append token to context object’s token set, run the update steps, and return true. */
            if (!force.HasValue || force.Value)
            {
                TokenSet.Add(token);
                //TokenSet.Add(new AtomicString(tokenValue, EAtomicStringFlags.CaseInsensitive));
                dtl_update();
                return true;
            }
            /* 5) Return false. */
            return false;
        }

        public bool Replace(TokenType token, TokenType newToken)
        {
            /* The replace(token, newToken) method, when invoked, must run these steps: */
            /* 1) If token is the empty string, then throw a "SyntaxError" DOMException. */
            string tokenValue = Token_To_String(token);
            if (ReferenceEquals(null, tokenValue) || tokenValue.Length <= 0)
                throw new DomSyntaxError("Token cannot be null or empty.");
            /* 2) If token contains any ASCII whitespace, then throw an "InvalidCharacterError" DOMException. */
            bool hasWhitespace = default(char) != tokenValue.ToString().ToCharArray().SingleOrDefault(c => UnicodeCommon.Is_Ascii_Whitespace(c));
            if (hasWhitespace)
                throw new InvalidCharacterError("Tokens cannot contain whitespaces");

            /* 3) If context object’s token set does not contain token, then return false. */
            if (!TokenSet.Contains(token))
                return false;
            /* 4) Replace token in context object’s token set with newToken. */
            TokenSet[TokenSet.IndexOf(token)] = newToken;// new AtomicString(newToken, EAtomicStringFlags.CaseInsensitive);
            /* 5) Run the update steps. */
            dtl_update();
            /* 6) Return true. */
            return true;
        }

        public bool Supports(TokenType token)
        {/* Docs: https://dom.spec.whatwg.org/#dom-domtokenlist-supports */
            if (SupportedTokens == null)
            {
                if (ownerElement.find_attribute(localName, out Attr outAttr))
                {
                    if (outAttr.Definition.SupportedTokens == null)
                    {
                        throw new TypeError($"Content attribute \"{localName}\" does not define supported tokens");
                    }

                    if (outAttr.Definition.SupportedTokens.Contains(Token_To_String(token)))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return SupportedTokens.Contains(token);
            }

            return false;
        }




        #region IEnumerable Implementation
        public IEnumerator<TokenType> GetEnumerator()
        {
            return ((IEnumerable<TokenType>)TokenSet).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TokenType>)TokenSet).GetEnumerator();
        }
        #endregion
    }
}
