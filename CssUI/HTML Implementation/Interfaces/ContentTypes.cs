namespace CssUI.HTML
{
    /* THIS FILE CONTAINS VARIOUS INTERFACES USED TO DISTINGUISH THE CONTENT TYPES OF HTML ELEMENTS */

    /// <summary>
    /// Metadata content is content that sets up the presentation or behavior of the rest of the content, or that sets up the relationship of the document with other documents, or that conveys other "out of band" information.
    /// </summary>
    public interface IPossibleMetadataContent
    {/* Docs: https://html.spec.whatwg.org/multipage/dom.html#metadata-content */
    }

    /// <summary>
    /// Most elements that are used in the body of documents and applications are categorized as flow content.
    /// </summary>
    public interface IPossibleFlowContent
    {/* Docs: https://html.spec.whatwg.org/multipage/dom.html#flow-content */
    }

    /// <summary>
    /// Sectioning content is content that defines the scope of headings and footers.
    /// </summary>
    public interface IPossibleSectioningContent
    {/* Docs: https://html.spec.whatwg.org/multipage/dom.html#sectioning-content */
    }

    /// <summary>
    /// Heading content defines the header of a section (whether explicitly marked up using sectioning content elements, or implied by the heading content itself).
    /// </summary>
    public interface IPossibleHeadingContent
    {/* Docs: https://html.spec.whatwg.org/multipage/dom.html#heading-content */
    }

    /// <summary>
    /// Phrasing content is the text of the document, as well as elements that mark up that text at the intra-paragraph level. Runs of phrasing content form paragraphs.
    /// </summary>
    public interface IPossiblePhrasingContent
    {/* Docs: https://html.spec.whatwg.org/multipage/dom.html#phrasing-content */
    }

    /// <summary>
    /// Embedded content is content that imports another resource into the document, or content from another vocabulary that is inserted into the document.
    /// </summary>
    public interface IPossibleEmbeddedContent
    {/* Docs: https://html.spec.whatwg.org/multipage/dom.html#embedded-content-2 */
    }

    /// <summary>
    /// Interactive content is content that is specifically intended for user interaction.
    /// </summary>
    public interface IPossibleInteractiveContent
    {/* Docs: https://html.spec.whatwg.org/multipage/dom.html#interactive-content */
    }

    /// <summary>
    /// Palpable content makes an element non-empty by providing either some descendant non-empty text, or else something users can hear (audio elements) or view (video or img or canvas elements) or otherwise interact with (for example, interactive form controls).
    /// </summary>
    public interface IPossiblePalpableContent
    {/* Docs: https://html.spec.whatwg.org/multipage/dom.html#palpable-content */
    }
}
