using System;
using System.Runtime.CompilerServices;

namespace CssUI
{
    public static class StringExt
    {
        /// <summary>
        /// Transforms a <c>string</c> into a byte-array
        /// </summary>
        /// <returns>Byte-array containing the string data</returns>
        public static byte[] ToByteArray(this string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }


        /// <summary>
        /// Returns whether <paramref name="str1"/> and <paramref name="str2"/> contain the same values
        /// </summary>
        /// <returns>True if both strings are an exact match</returns>
        public static bool streq(ReadOnlySpan<char> str1, ReadOnlySpan<char> str2)
        {
            if (str1.Length != str2.Length)
                return false;

            for (int i = 0; i < str1.Length; i++)
            {
                if (str1[i] != str2[i])
                {
                    return false;
                }
            }

            return true;
        }

    }
}
