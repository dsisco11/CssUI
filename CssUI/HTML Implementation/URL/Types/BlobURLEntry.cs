using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CssUI.HTML
{
    public class BlobURLEntry
    {
        #region Static
        public static readonly Dictionary<string, BlobURLEntry> BlobURLStore = new Dictionary<string, BlobURLEntry>();

        public static bool Resolve(Url url, out BlobURLEntry outBlob)
        {/* Docs: https://w3c.github.io/FileAPI/#blob-url-resolve */
            Debug.Assert(url.Scheme == "blob");
            string urlString = url.Serialize(true);

            if (BlobURLStore.TryGetValue(urlString, out BlobURLEntry blob))
            {
                outBlob = blob;
                return true;
            }

            outBlob = null;
            return false;
        }

        public static string Generate_Blob_Url()
        {
            StringBuilder result = new StringBuilder();
            result.Append("blob:");
            /* 3) Let settings be the current settings object */
            /* 4) Let origin be settings’s origin. */
            /* 5) Let serialized be the ASCII serialization of origin. */
            /* 6) If serialized is "null", set it to an implementation-defined value. */
            /* 7) Append serialized to result. */
            result.Append("CSSUI");
            result.Append(UnicodeCommon.CHAR_SOLIDUS);
            result.Append(System.Guid.NewGuid().ToString());

            return result.ToString();
        }
        #endregion

        #region Properties
        public readonly dynamic Value;
        #endregion

        #region Constructors
        public BlobURLEntry(Blob Value)
        {
            this.Value = Value;
        }
        #endregion

        #region Implicit Casts
        public static implicit operator BlobURLEntry(Blob blob) => new BlobURLEntry(blob);
        #endregion
    }
}
