
using System.Text;

namespace CssUI.Internal
{
    /// <summary>
    /// ".NET uses the UTF-16 encoding (represented by the UnicodeEncoding class) to represent characters and strings."
    /// We just need to implement certain Unicode standards for interpretation of said strings
    /// </summary>
    public class Unicode
    {
        #region Caseless Matching
        /* SOURCES
         * Normalization forms: https://en.wikipedia.org/wiki/Unicode_equivalence#Normal_forms
         * Formula: https://www.unicode.org/versions/Unicode12.0.0/UnicodeStandard-12.0.pdf -> Section: "Default Caseless Matching"
         */
        /// <summary>
        /// Performs a culture invariant case-insensitive match on two strings
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Match"></param>
        /// <returns></returns>
        public static int CaselessCompare(string Source, string Match)
        {
            /*
             * A string X is a canonical caseless match for a string Y if and only if:
             * NFD(toCasefold(NFD(X))) = NFD(toCasefold(NFD(Y)))
             */
            string A = Source.Normalize(NormalizationForm.FormD).ToLowerInvariant().Normalize(NormalizationForm.FormD);
            string B = Match.Normalize(NormalizationForm.FormD).ToLowerInvariant().Normalize(NormalizationForm.FormD);
            return A.CompareTo(B);
        }
        #endregion
    }
}
