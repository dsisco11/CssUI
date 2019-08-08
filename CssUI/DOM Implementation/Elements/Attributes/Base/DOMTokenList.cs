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
    public class DOMTokenList : IEnumerable<AtomicString>
    {/* Docs: https://dom.spec.whatwg.org/#interface-domtokenlist */
        #region Backing List
        private List<AtomicString> TokenSet = new List<AtomicString>();
        #endregion

        #region Properties
        private readonly Element ownerElement;
        /// <summary>
        /// Name of the attribute this token list represents
        /// </summary>
        public readonly AtomicName<EAttributeName> localName;
        private readonly string[] SupportedTokens = null;
        #endregion

        #region Constructor
        public DOMTokenList(Element ownerElement, AtomicName<EAttributeName> localName)
        {
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
            var value = attr?.Value.Get_String();
            /* 4) Run the attribute change steps for element, localName, value, value, and null. */
            run_attribute_change_steps(ownerElement, localName, attr.Value, attr.Value, null);
        }

        public DOMTokenList(Element ownerElement, AtomicName<EAttributeName> localName, string[] supportedTokens)
        {
            SupportedTokens = supportedTokens;
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
            var value = attr?.Value.Get_String();
            /* 4) Run the attribute change steps for element, localName, value, value, and null. */
            run_attribute_change_steps(ownerElement, localName, attr.Value, attr.Value, null);
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
            ownerElement.setAttribute(localName, AttributeValue.From_String(DOMCommon.Serialize_Ordered_Set(TokenSet.Select(t => ((string)t).AsMemory()))));
        }
        #endregion

        #region Accessors
        public int Length => TokenSet.Count;

        [CEReactions] public AtomicString Value
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

        #region Internal
        internal void run_attribute_change_steps(Element element, AtomicName<EAttributeName> localName, AttributeValue oldValue, AttributeValue newValue, string Namespace)
        {
            if (localName != this.localName) return;
            if (!ReferenceEquals(null, Namespace)) return;

            var value = newValue.Get_String();
            /* 1) If localName is associated attribute’s local name, namespace is null, and value is null, then empty token set. */
            if (ReferenceEquals(null, value))
            {
                TokenSet = new List<AtomicString>();
            }
            else
            {
                /* 2) Otherwise, if localName is associated attribute’s local name, namespace is null, then set token set to value, parsed. */
                TokenSet = new List<AtomicString>();
                var valueMem = value.AsMemory();
                var rawTokens = DOMCommon.Parse_Ordered_Set(valueMem);
                var tokens = rawTokens.Select(o => o.ToString());

                foreach (var token in tokens)
                {
                    TokenSet.Add(new AtomicString(token, EAtomicStringFlags.CaseInsensitive));
                }
            }
        }
        #endregion



        public bool Contains(AtomicString token)
        {
            /* The contains(token) method, when invoked, must return true if context object’s token set[token] exists, and false otherwise. */
            return TokenSet.Contains(token);
        }

        public bool ContainsAll(IEnumerable<AtomicString> tokens)
        {
            /* The contains(token) method, when invoked, must return true if context object’s token set[token] exists, and false otherwise. */
            foreach(var token in tokens)
            {
                if (!TokenSet.Contains(token))
                    return false;
            }

            return true;
        }

        public void Add(params AtomicString[] tokens)
        {
            /* The add(tokens…) method, when invoked, must run these steps: */
            /* 1) For each token in tokens: */
            foreach(var token in tokens)
            {
                /* 1) If token is the empty string, then throw a "SyntaxError" DOMException. */
                if (string.IsNullOrEmpty(token))
                    throw new DomSyntaxError();
                /* 2) If token contains any ASCII whitespace, then throw an "InvalidCharacterError" DOMException. */
                bool hasWhitespace = default(char) != token.ToString().ToCharArray().SingleOrDefault(c => UnicodeCommon.Is_Ascii_Whitespace(c));
                if (hasWhitespace)
                    throw new InvalidCharacterError("tokens cannot contain whitespaces");
            }
            /* 2) For each token in tokens, append token to context object’s token set. */
            foreach(var token in tokens)
            {
                TokenSet.Add(new AtomicString(token, EAtomicStringFlags.CaseInsensitive));
            }
            //TokenSet.AddRange(tokens.Select(tok => tok.ToString().ToLowerInvariant()).Cast<AtomicString>());
            /* 3) Run the update steps. */
            dtl_update();
        }

        public void Remove(params AtomicString[] tokens)
        {
            /* The remove(tokens…) method, when invoked, must run these steps: */
            /* 1) For each token in tokens: */
            foreach (var token in tokens)
            {
                /* 1) If token is the empty string, then throw a "SyntaxError" DOMException. */
                if (string.IsNullOrEmpty(token))
                    throw new DomSyntaxError("Token cannot be null.");
                /* 2) If token contains any ASCII whitespace, then throw an "InvalidCharacterError" DOMException. */
                bool hasWhitespace = default(char) != token.ToString().ToCharArray().SingleOrDefault(c => UnicodeCommon.Is_Ascii_Whitespace(c));
                if (hasWhitespace)
                    throw new InvalidCharacterError("Tokens cannot contain whitespaces");
            }
            /* 2) For each token in tokens, remove token from context object’s token set. */
            foreach (var token in tokens)
            {
                TokenSet.Remove(token);
            }
            /* 3) Run the update steps. */
            dtl_update();
        }

        public bool Toggle(AtomicString token, bool? force = null)
        {
            /* The toggle(token, force) method, when invoked, must run these steps: */
            /* 1) If token is the empty string, then throw a "SyntaxError" DOMException. */
            if (string.IsNullOrEmpty(token))
                throw new DomSyntaxError("Token cannot be null or empty.");
            /* 2) If token contains any ASCII whitespace, then throw an "InvalidCharacterError" DOMException. */
            bool hasWhitespace = default(char) != token.ToString().ToCharArray().SingleOrDefault(c => UnicodeCommon.Is_Ascii_Whitespace(c));
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
                TokenSet.Add(new AtomicString(token, EAtomicStringFlags.CaseInsensitive));
                dtl_update();
                return true;
            }
            /* 5) Return false. */
            return false;
        }

        public bool Replace(AtomicString token, AtomicString newToken)
        {
            /* The replace(token, newToken) method, when invoked, must run these steps: */
            /* 1) If token is the empty string, then throw a "SyntaxError" DOMException. */
            if (string.IsNullOrEmpty(token))
                throw new DomSyntaxError("Token cannot be null or empty.");
            /* 2) If token contains any ASCII whitespace, then throw an "InvalidCharacterError" DOMException. */
            bool hasWhitespace = default(char) != token.ToString().ToCharArray().SingleOrDefault(c => UnicodeCommon.Is_Ascii_Whitespace(c));
            if (hasWhitespace)
                throw new InvalidCharacterError("Tokens cannot contain whitespaces");

            /* 3) If context object’s token set does not contain token, then return false. */
            if (!TokenSet.Contains(token))
                return false;
            /* 4) Replace token in context object’s token set with newToken. */
            TokenSet[TokenSet.IndexOf(token)] = new AtomicString(newToken, EAtomicStringFlags.CaseInsensitive);
            /* 5) Run the update steps. */
            dtl_update();
            /* 6) Return true. */
            return true;
        }

        public bool Supports(AtomicString token)
        {/* Docs: https://dom.spec.whatwg.org/#dom-domtokenlist-supports */
            if (ownerElement.find_attribute(localName, out Attr outAttr))
            {
                if (outAttr.Definition.SupportedTokens == null)
                {
                    throw new TypeError($"Content attribute \"{localName}\" does not define supported tokens");
                }

                if (outAttr.Definition.SupportedTokens.Contains(token))
                {
                    return true;
                }
            }

            return false;
        }




        #region IEnumerable Implementation
        public IEnumerator<AtomicString> GetEnumerator()
        {
            return ((IEnumerable<AtomicString>)TokenSet).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<AtomicString>)TokenSet).GetEnumerator();
        }
        #endregion
    }
}
