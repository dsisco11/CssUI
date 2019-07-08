using System.Collections.Generic;

namespace CssUI.DOM
{
    // XXX: finish this implementation
    //SEE:  https://www.w3.org/TR/cssom-1/#parse-a-media-query-list
    public class MediaList
    {
        #region Properties
        private List<string> mediaQueries = new List<string>();
        public string mediaText;
        public readonly ulong length;
        #endregion

        #region Getters/Setters
        public string get_item(ulong i)
        {
            return mediaQueries[(int)i];
        }

        public string set_item(ulong i, string value)
        {
            return mediaQueries[(int)i] = value;
        }
        #endregion


        #region Constructors
        public MediaList(string mediaText)
        {
            this.mediaText = mediaText;
        }
        #endregion


    }
}
