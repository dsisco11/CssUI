namespace CssUI.DOM
{
    public enum ESelectionMode : int
    {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#dom-selectionmode-select-dev */
        /// <summary>
        /// Selects the newly inserted text.
        /// </summary>
        Select,

        /// <summary>
        /// Moves the selection to just before the inserted text.
        /// </summary>
        Start,

        /// <summary>
        /// Moves the selection to just after the selected text.
        /// </summary>
        End,

        /// <summary>
        /// Attempts to preserve the selection. This is the default.
        /// </summary>
        Preserve,
    }
}
