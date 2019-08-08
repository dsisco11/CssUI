using CssUI.Internal;

namespace CssUI.DOM
{
    [MetaEnum]
    public enum EFormMethod : int
    {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#attr-fs-formmethod */

        /// <summary>
        /// Indicates the HTTP GET method.
        /// </summary>
        [MetaKeyword("get")]
        Get,
        /// <summary>
        /// Indicates the HTTP POST method.
        /// </summary>
        [MetaKeyword("post")]
        Post,
        /// <summary>
        /// Indicates that submitting the form is intended to close the dialog box in which the form finds itself, if any, and otherwise not submit.
        /// </summary>
        [MetaKeyword("dialog")]
        Dialog,

    }
}
