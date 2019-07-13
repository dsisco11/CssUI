using CssUI.DOM.Exceptions;
using System.Collections.Generic;
using System.Text;

namespace CssUI.DOM
{
    public class DOMStringMap
    {/* DOcs: https://html.spec.whatwg.org/multipage/dom.html#domstringmap */
        #region Properties
        private readonly Element Owner;
        #endregion

        #region Constructors
        public DOMStringMap(Element Owner)
        {
            this.Owner = Owner;
        }
        #endregion

        #region Manipulation
        public string this[string Name]
        {
            get
            {
                string safeName = To_XML_Safe_Name(Name);
                string Value = Owner.Attributes.getNamedItem(safeName)?.Value;
                return ReferenceEquals(null, Value) ? string.Empty : Value;
            }
            set
            {
                string safeName = To_XML_Safe_Name(Name);
                Owner.setAttribute(safeName, value);
            }
        }

        /// <summary>
        /// Deletes a named value from the list.
        /// </summary>
        /// <param name="Name"></param>
        public void delete(string Name)
        {
            bool hasAlpha = false;
            for (int i = 0; i < Name.Length; i++) { if (ASCIICommon.Is_ASCII_Upper_Alpha(Name[i])) { hasAlpha = true; break; } }

            /* 1) For each ASCII upper alpha in name, insert a U+002D HYPHEN-MINUS character (-) before the character and replace the character with the same character converted to ASCII lowercase. */
            StringBuilder sb = new StringBuilder();
            if (hasAlpha)
            {
                for (int i = 0; i < Name.Length; i++)
                {
                    if (ASCIICommon.Is_ASCII_Upper_Alpha(Name[i]))
                    {
                        sb.Append('-');
                        sb.Append(ASCIICommon.To_ASCII_Lower_Alpha(Name[i]));
                    }
                    else
                    {
                        sb.Append(Name[i]);
                    }
                }
            }
            else
            {
                sb.Append(Name);
            }
            /* 2) Insert the string data- at the front of name. */
            sb.Insert(0, "data-");
            Name = sb.ToString();
            /* 3) Remove an attribute by name given name and the DOMStringMap's associated element. */
            if (Owner.find_attribute(Name, out Attr attr))
                Owner.remove_attribute(attr);
        }

        public IEnumerable<KeyValuePair<string, string>> GetKeyValuePairs()
        {
            LinkedList<KeyValuePair<string, string>> list = new LinkedList<KeyValuePair<string, string>>();
            /* 2) For each content attribute on the DOMStringMap's associated element whose first five characters are the string "data-" and whose remaining characters (if any) do not include any ASCII upper alphas, in the order that those attributes are listed in the element's attribute list, add a name-value pair to list whose name is the attribute's name with the first five characters removed and whose value is the attribute's value. */
            foreach (var attr in Owner.AttributeList)
            {
                /* 3) For each name in list, for each U+002D HYPHEN-MINUS character (-) in the name that is followed by an ASCII lower alpha, remove the U+002D HYPHEN-MINUS character (-) and replace the character that followed it by the same character converted to ASCII uppercase. */
                string unsafeName = From_XML_Safe_Name(attr.Name);
                if (!ReferenceEquals(null, unsafeName))
                {
                    list.AddLast(new KeyValuePair<string, string>(unsafeName, attr.Value));
                }
            }

            return list;
        }

        #endregion

        #region XML Safety
        private string To_XML_Safe_Name(string Name)
        {
            int idx = Name.IndexOf('-');
            if (idx >= 0 && Name.Length < 2)
                throw new SyntaxError();
            if (idx >= 0 && (Name[idx + 1] >= 'a' || Name[idx + 1] <= 'z'))
                throw new SyntaxError();

            bool hasAlpha = false;
            for (int i = 0; i < Name.Length; i++) { if (ASCIICommon.Is_ASCII_Upper_Alpha(Name[i])) { hasAlpha = true; break; } }

            /* 2) For each ASCII upper alpha in name, insert a U+002D HYPHEN-MINUS character (-) before the character and replace the character with the same character converted to ASCII lowercase. */
            StringBuilder sb = new StringBuilder();
            if (hasAlpha)
            {
                for (int i = 0; i < Name.Length; i++)
                {
                    if (ASCIICommon.Is_ASCII_Upper_Alpha(Name[i]))
                    {
                        sb.Append('-');
                        sb.Append(ASCIICommon.To_ASCII_Lower_Alpha(Name[i]));
                    }
                    else
                    {
                        sb.Append(Name[i]);
                    }
                }
            }
            else
            {
                sb.Append(Name);
            }
            /* 3) Insert the string data- at the front of name. */
            sb.Insert(0, "data-");

            string safeName = sb.ToString();
            /* 4) If name does not match the XML Name production, throw an "InvalidCharacterError" DOMException. */
            if (!XMLCommon.Is_Valid_Name(safeName))
                throw new InvalidCharacterError($"The Name '{safeName}' is not a valid XML name production.");

            return safeName;
        }

        private string From_XML_Safe_Name(string Name)
        {
            if (0 == Name.IndexOf("data-") && !ASCIICommon.Has_ASCII_Upper_Alpha(Name))
            {
                string str = Name.Substring(5);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == '-' && (i + 1) < str.Length && ASCIICommon.Is_ASCII_Lower_Alpha(str[i + 1]))
                    {
                        i++;// skip the hypen
                        sb.Append(ASCIICommon.To_ASCII_Upper_Alpha(str[i + 1]));
                    }
                    else
                    {
                        sb.Append(str[i]);
                    }
                }
                return sb.ToString();
            }

            return null;
        }
        #endregion
    }
}
