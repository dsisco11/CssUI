using CssUI.DOM;
using System;
using System.Collections.Generic;

namespace CssUI.HTML
{
    public struct MimeTypeRecord
    {
        #region Properties
        public readonly string Type;
        public readonly string SubType;
        public readonly Dictionary<string, string> Parameters;
        public readonly EMimeGroup Groups;
        #endregion

        #region Constructors
        public MimeTypeRecord(string type, string subType, Dictionary<string, string> parameters)
        {
            Type = type;
            SubType = subType;
            Parameters = parameters;
            Groups = MimeType.Determine_Groups(Type, SubType.AsMemory());
        }
        #endregion
    }
}
