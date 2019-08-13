using System;
using System.Text;

namespace CssUI.HTML
{
    /// <summary>
    /// Contains 8-bit integers making up a single 32-bit integer which indicates a network address
    /// </summary>
    public struct IPV4Address
    {/* Docs: https://url.spec.whatwg.org/#concept-ipv4 */

        #region Properties
        public readonly byte[] Part;
        #endregion

        #region Constructors

        public IPV4Address(byte A, byte B, byte C, byte D)
        {
            Part = new byte[4] { A, B, C, D };
        }
        #endregion

        public UInt32 Address
        {
            get => (UInt32)((Part[0] << 0) | (Part[1] << 8) | (Part[2] << 16) | (Part[3] << 24));
        }

        public string Serialize()
        {/* Docs: https://url.spec.whatwg.org/#concept-ipv4-serializer */
            StringBuilder output = new StringBuilder();
            for (int i=0; i<4; i++)
            {
                output.Append(Part[i] % 256);
                if (i != 3) output.Append(UnicodeCommon.CHAR_FULL_STOP);
            }

            return output.ToString();
        }

    }
}
