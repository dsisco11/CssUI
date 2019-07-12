namespace CssUI.DOM.Internal
{
    /// <summary>
    /// Provides common functions outlined in the HTML specifications
    /// </summary>
    public static class HTMLCommon
    {
        #region Metadata
        /// <summary>
        /// Returns the official HTML namespace string
        /// </summary>
        public const string Namespace = "http://www.w3.org/1999/xhtml";
        #endregion

        public static string Uppercased_Name(string Name)
        {/* Docs: https://dom.spec.whatwg.org/#element-html-uppercased-qualified-name */

            char[] buf = new char[Name.Length];
            for (int c=0; c<buf.Length; c++)
            {
                buf[c] = ASCIICommon.To_ASCII_Upper_Alpha(Name[c]);
            }

            return new string(buf);
        }
    }
}
