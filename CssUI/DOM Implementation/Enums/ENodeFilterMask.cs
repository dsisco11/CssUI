using System;

namespace CssUI.DOM.Enums
{
    [Flags]
    public enum ENodeFilterMask : ulong
    {
        SHOW_ALL = 0xFFFFFFFF,
        SHOW_ELEMENT = 0x1,
        SHOW_ATTRIBUTE = 0x2,
        SHOW_TEXT = 0x4,
        SHOW_CDATA_SECTION = 0x8,
        SHOW_ENTITY_REFERENCE = 0x10, // historical
        SHOW_ENTITY = 0x20, // historical
        SHOW_PROCESSING_INSTRUCTION = 0x40,
        SHOW_COMMENT = 0x80,
        SHOW_DOCUMENT = 0x100,
        SHOW_DOCUMENT_TYPE = 0x200,
        SHOW_DOCUMENT_FRAGMENT = 0x400,
        SHOW_NOTATION = 0x800 // historical
    }
}
