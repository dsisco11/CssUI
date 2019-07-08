using CssUI.DOM.Enums;

namespace CssUI.DOM
{
    public class CDATASection : CharacterData
    {

        #region Node Overrides
        public override ENodeType nodeType => ENodeType.CDATA_SECTION_NODE;
        public override string nodeName => "#cdata-section";
        #endregion

        public CDATASection()
        {
        }
    }
}
