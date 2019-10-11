using System;

namespace CssUI.HTML
{
    [Flags]
    public enum EContentCategories : int
    {
        /// <summary>
        /// When an element's content model is nothing, the element must contain no Text nodes (other than inter-element whitespace) and no element nodes.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Metadata content is content that sets up the presentation or behavior of the rest of the content, or that sets up the relationship of the document with other documents, or that conveys other "out of band" information.
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/dom.html#metadata-content
        MetaData = (1 << 1),

        /// <summary>
        /// Most elements that are used in the body of documents and applications are categorized as flow content.
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/dom.html#flow-content
        Flow = (1 << 2),

        /// <summary>
        /// Sectioning content is content that defines the scope of headings and footers.
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/dom.html#sectioning-content
        Sectioning = (1 << 3),

        /// <summary>
        /// Heading content defines the header of a section (whether explicitly marked up using sectioning content elements, or implied by the heading content itself).
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/dom.html#heading-content
        Heading = (1 << 4),

        /// <summary>
        /// Phrasing content is the text of the document, as well as elements that mark up that text at the intra-paragraph level. Runs of phrasing content form paragraphs.
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/dom.html#phrasing-content
        Phrasing = (1 << 5),

        /// <summary>
        /// Embedded content is content that imports another resource into the document, or content from another vocabulary that is inserted into the document.
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/dom.html#embedded-content-2
        Embedded = (1 << 6),

        /// <summary>
        /// Interactive content is content that is specifically intended for user interaction.
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/dom.html#interactive-content
        Interactive = (1 << 7),

        /// <summary>
        /// Palpable content makes an element non-empty by providing either some descendant non-empty text, or else something users can hear (audio elements) or view (video or img or canvas elements) or otherwise interact with (for example, interactive form controls).
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/dom.html#palpable-content
        Palpable = (1 << 8),

        /// <summary>
        /// Certain elements are said to be sectioning roots, including blockquote and td elements. These elements can have their own outlines, but the sections and headings inside these elements do not contribute to the outlines of their ancestors.
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/sections.html#sectioning-root
        SectioningRoot = (1 << 9),

        /// <summary>
        /// Script-supporting elements are those that do not represent anything themselves (i.e. they are not rendered), but are used to support scripts, e.g. to provide functionality for the user.
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/dom.html#script-supporting-elements-2
        ScriptSupporting = (1 << 10),
    }
}
