using CssUI.DOM.Exceptions;
using System;
using System.Text;

namespace CssUI.DOM.Serialization
{
    public class DOMParser
    {/* Docs: https://www.w3.org/TR/DOM-Parsing/ */
        /* https://html.spec.whatwg.org/multipage/parsing.html#overview-of-the-parsing-model */



        public static int Parse_Integer(string input)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#rules-for-parsing-integers */

            int position = 0;
            bool sign = true;
            StringBuilder sb = new StringBuilder();

            /* SKip ASCII whitespace */
            for (; position<input.Length; position++)
            {
                if (!ASCIICommon.Is_Ascii_Whitespace(input[position]))
                    break;
            }

            if (position >= input.Length) throw new DomSyntaxError();

            if (input[position] == 0x002D)
            {
                sign = false;
                position++;
                if (position >= input.Length) throw new DomSyntaxError();
            }
            else if (input[position] == 0x002B)
            {
                position++;
                if (position >= input.Length) throw new DomSyntaxError();
            }

            /* Collect sequence of ASCII digit codepoints */

            for (; position < input.Length; position++)
            {
                if (ASCIICommon.Is_Ascii_Digit(input[position]))
                {
                    sb.Append(input[position]);
                    continue;
                }

                throw new DomSyntaxError("Encountered non-digit while parsing integer.");
            }

            int parsed = Int32.Parse(sb.ToString());
            return sign ? parsed : (0 - parsed);
        }
    }
}
