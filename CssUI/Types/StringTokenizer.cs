namespace CssUI
{
    /// <summary>
    /// A generic string tokenizer
    /// </summary>
    public class StringTokenizer
    {
        #region Properties
        public int ReadPos { get; private set; }
        public readonly string Text;
        #endregion

        #region Constructors
        public StringTokenizer(string Text)
        {
            this.Text = Text;
        }
        #endregion

        #region Buffer Operations
        /// <summary>
        /// Returns the character at +Offset from the current read position within the text
        /// </summary>
        /// <param name="Offset">Distance from the current read position at which to peek</param>
        /// <returns></returns>
        public char Peek(int Offset = 0)
        {
            int i = (ReadPos + Offset);
            if (i >= Text.Length || i < 0) return (char)0;
            return Text[i];
        }

        /// <summary>
        /// Reads the current character from the text and progresses the current reading position
        /// </summary>
        public char Consume()
        {
            int EndPos = (ReadPos + 1);
            if (ReadPos >= Text.Length) return (char)0;

            char retVal = Text[ReadPos];
            ReadPos += 1;

            return retVal;
        }

        /// <summary>
        /// Reads the specified number of characters from the text and progresses the current reading position
        /// </summary>
        /// <param name="Count">Number of characters to consume</param>
        public string Consume(int Count = 1)
        {
            int EndPos = (ReadPos + Count);
            if (EndPos >= Text.Length) EndPos = (Text.Length - 1);

            string retVal = Text.Substring(ReadPos, (EndPos - ReadPos));
            ReadPos = EndPos;

            return retVal;
        }

        /// <summary>
        /// Pushes the given number of characters back onto the front of the "stream"
        /// </summary>
        /// <param name="Count"></param>
        public void Reconsume(int Count = 1)
        {// SEE:  https://www.w3.org/TR/css-syntax-3/#reconsume-the-current-input-code-point
            ReadPos -= Count;
            if (ReadPos < 0) ReadPos = 0;
        }
        #endregion

    }
}
