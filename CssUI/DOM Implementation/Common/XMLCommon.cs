using CssUI.DOM.Exceptions;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CssUI.DOM
{
    public static class XMLCommon
    {

        #region Name Verification
        /* Docs: https://www.w3.org/TR/xml/#NT-Name */

        /// <summary>
        /// Returns True if the given string follows the XML Name format
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Valid_Name(string name)
        {
            for(int i=0; i<name.Length; i++)
            {
                if ( !Is_NameChar(name[i]) )
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Returns True if the given string follows the XML qName format
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Valid_QName(string name)
        {/* Docs: https://www.w3.org/TR/xml-names/#NT-QName */
            bool prefix = false;
            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] == ':')
                {
                    if (prefix) return false;// Can only have 1 prefix

                    prefix = true;
                    if (i == 0) return false;// prefix must have a name before it, if the prefix char is at index 0 then it doesnt.
                }
                if (!Is_NameChar(name[i]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// True is char is a valid XML name-start character
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_NameStartChar(char c)
        {
            return (c == ':' || c == '_') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= 0xC0 && c <= 0xD6) || (c >= 0xD8 && c <= 0xF6) || (c >= 0xF8 && c <= 0x2FF) || (c >= 0x370 && c <= 0x37D) || (c >= 0x37F && c <= 0x1FFF) || (c >= 0x200C && c <= 0x200D) || (c >= 0x2070 && c <= 0x218F) || (c >= 0x2C00 && c <= 0x2FEF) || (c >= 0x3001 && c <= 0xD7FF) || (c >= 0xF900 && c <= 0xFDCF) || (c >= 0xFDF0 && c <= 0xFFFD) || (c >= 0x10000 && c <= 0xEFFFF);
        }

        /// <summary>
        /// True is char is a valid XML name character
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_NameChar(char c)
        {
            return Is_NameStartChar(c) || (c >= '0' && c <= '9') || c == 0xB7 || (c >= 0x0300 && c <= 0x036F) || (c >= 0x203F && c <= 0x2040);
        }

        /// <summary>
        /// True is char is a valid XML character
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_NTChar(char c)
        {/* Docs: https://www.w3.org/TR/REC-xml/#NT-Char */
            /* any Unicode character, excluding the surrogate blocks, FFFE, and FFFF. */
            return c == 0x9 || c == 0xA || c == 0xD || (c >= 0x20 && c <= 0xD7FF) || (c >= 0xE000 && c <= 0xFFFD) || (c >= 0x10000 && c <= 0x10FFFF);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns whether the given name follows the XML name production format
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Valid(string Name)
        {/* Docs: https://dom.spec.whatwg.org/#validate */
            if (!Is_Valid_QName(Name) && !Is_Valid_Name(Name))
            {
                return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Validate(string Name)
        {/* Docs: https://dom.spec.whatwg.org/#validate */
            if (!Is_Valid_QName(Name) && !Is_Valid_Name(Name))
                throw new InvalidCharacterError();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Validate_And_Extract(string Namespace, string qualifiedName, out string outPrefix, out string outLocalName)
        {/* Docs: https://dom.spec.whatwg.org/#validate-and-extract */
            /* 1) If namespace is the empty string, set it to null. */
            if (Namespace.Length <= 0)
                Namespace = null;

            /* 2) Validate qualifiedName. */
            Validate(qualifiedName);

            string prefix = null;
            string localName = qualifiedName;
            /* 5) If qualifiedName contains a ":" (U+003E), then split the string on it and set prefix to the part before and localName to the part after. */
            if (qualifiedName.Contains(':'))
            {
                string[] tok = qualifiedName.Split(':');
                prefix = tok[0];
                localName = tok[1];
            }

            if (prefix != null && Namespace == null)
                throw new NamespaceError($"The qualified name contains a prefix but no namespace was specified");

            /* 7) If prefix is "xml" and namespace is not the XML namespace, then throw a "NamespaceError" DOMException. */
            if (0 == string.Compare("xml", prefix) && 0 != string.Compare(Namespace, DOMCommon.XMLNamespace))
                throw new NamespaceError($"The qualified names' prefix \"{qualifiedName}\" does not match the namespace specified \"{Namespace}\"");

            /* 8) If either qualifiedName or prefix is "xmlns" and namespace is not the XMLNS namespace, then throw a "NamespaceError" DOMException. */
            if ((0 == string.Compare("xmlns", qualifiedName) || 0 == string.Compare("xmlns", prefix)) && 0 != string.Compare(Namespace, DOMCommon.XMLNSNamespace))
                throw new NamespaceError($"The qualified names' prefix \"{qualifiedName}\" does not match the namespace specified \"{Namespace}\"");

            /* 9) If namespace is the XMLNS namespace and neither qualifiedName nor prefix is "xmlns", then throw a "NamespaceError" DOMException. */
            if ((0 != string.Compare("xmlns", qualifiedName) || 0 != string.Compare("xmlns", prefix)) && 0 == string.Compare(Namespace, DOMCommon.XMLNSNamespace))
                throw new NamespaceError($"The qualified names' prefix \"{qualifiedName}\" does not match the namespace specified \"{Namespace}\"");

            outPrefix = prefix;
            outLocalName = localName;
        }
        #endregion
    }
}
