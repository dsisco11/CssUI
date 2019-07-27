
namespace CssUI
{
    public class EnumData
    {
        #region Properties
        public readonly string Keyword;
        public readonly dynamic[] Data;
        #endregion

        #region Constructor

        public EnumData(string keyword, params dynamic[] data)
        {
            Keyword = keyword;
            Data = data;
        }
        #endregion

        public dynamic this[int i]
        {
            get => Data[i];
        }

        public int Count => ((IReadOnlyList<dynamic>)Data).Count;
    }
}
