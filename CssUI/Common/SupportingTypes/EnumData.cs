using System.Runtime.InteropServices;

namespace CssUI
{
    /// <summary>
    /// Holds ancillary data about an enum value
    /// </summary>
    public struct EnumData
    {
        #region Properties
        public readonly object[] Data;
        public readonly string Keyword;
        #endregion

        #region Constructor

        public EnumData(string keyword, params object[] data)
        {
            Keyword = keyword;
            Data = data;
        }
        #endregion

        public object this[int i]
        {
            get => Data[i];
        }

        public int Length => Data.Length;
    }
}
