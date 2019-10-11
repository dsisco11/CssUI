using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    public class DocumentType : Node
    {
        #region Instances
        public static DocumentType XML = new DocumentType("xml");
        public static DocumentType HTML = new DocumentType("html");
        #endregion

        #region Properties
        public string name { get; internal set; }
        public string publicId { get; internal set; }
        public string systemId { get; internal set; }
        #endregion

        #region Node Implementation
        public override ENodeType nodeType =>  ENodeType.DOCUMENT_TYPE_NODE;
        public override string nodeName => this.name;
        public override string nodeValue { get => null; set { /* Specifications say do nothing */ } }
        public override string textContent { get => null; set { /* Specifications say do nothing */ } }
        public override int nodeLength => 0;
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="publicId"></param>
        /// <param name="systemId"></param>
        /* THIS CONSTRUCTOR IS INTERNAL BECAUSE DOC-TYPES SHOULD ONLY BE CREATED THROUGH A DOCUMENT OBJECT! */
        internal DocumentType(string name, string publicId = "", string systemId = "")
        {
            this.name = name;
            this.publicId = publicId;
            this.systemId = systemId;
        }
        #endregion
    }
}
