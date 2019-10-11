using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CssUI.CSS.Parser
{
    /// <summary>
    /// Provides byte-stream filtering, correcting any faulty bytes so the <see cref="CssParser"/> may interpret it.
    /// <para>Docs: https://www.w3.org/TR/css-syntax-3/#input-preprocessing </para>
    /// </summary>
    public static class CssInput
    {
        #region Byte Order Mark
        const uint BOM_UTF8 = 0;
        const uint BOM_UTF16LE = 1;
        const uint BOM_UTF16BE = 2;

        static byte[][] BOM_HEADER = new byte[3][]
        {
            new byte[] { 0xEF, 0xBB, 0xBF },// UTF8
            new byte[] { 0xFF, 0xFE },// UTF16 LittleEndian
            new byte[] { 0xFE, 0xFF },// UTF16 BigEndian
        };

        static bool BOMCheck(List<byte> buf, byte[] BOM_BUFFER)
        {
            if (buf.Count < BOM_BUFFER.Length) return false;

            for (int i=0; i< BOM_BUFFER.Length; i++)
            {
                if (buf[i] != BOM_BUFFER[i]) return false;
            }

            return true;
        }
        #endregion

        /// <summary>
        /// Performs pre-processing on input text to remove invalid characters according to the CSS documentation
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string PreProcess(ReadOnlySpan<char> Text)
        {// SEE:  https://www.w3.org/TR/css-syntax-3/#input-preprocessing
            int bufLen = Encoding.Default.GetByteCount(Text);
            Span<byte> buf = new byte[bufLen];
            Encoding.Default.GetBytes(Text, buf);
            string str = Encoding.UTF8.GetString(buf);
            return str.Replace('\r', '\n').Replace('\f', '\n').Replace('\0', '\uFFFD');
        }

        public static string PreProcess(Stream stream)
        {// SEE:  https://encoding.spec.whatwg.org/#decode
            bool BOM_Seen = false;
            Encoding enc = null;
            List<byte> buf = new List<byte>();
            
            for (int i=0; i<3; i++)
            {
                int b = stream.ReadByte();
                if (b == -1) break;// EOF
                buf.Add((byte)b);
            }

            // Test for UTF-8
            if (BOMCheck(buf, BOM_HEADER[BOM_UTF8])) { enc = Encoding.UTF8; BOM_Seen = true; }
            else if (BOMCheck(buf, BOM_HEADER[BOM_UTF16BE])) { enc = Encoding.BigEndianUnicode; BOM_Seen = true; }
            else if (BOMCheck(buf, BOM_HEADER[BOM_UTF16LE])) { enc = Encoding.Unicode; BOM_Seen = true; }

            if (!BOM_Seen) stream.Seek(0, SeekOrigin.Begin);
            else if (BOM_Seen && enc != Encoding.UTF8 && buf.Count >= 3) stream.Seek(3, SeekOrigin.Begin);

            byte[] encBuf = new byte[stream.Length - stream.Position];
            int read = 0;
            while (read < encBuf.Length) { read += stream.Read(encBuf, 0, (encBuf.Length - read)); }

            string output = enc.GetString(encBuf);
            return output;
        }
    }
}
