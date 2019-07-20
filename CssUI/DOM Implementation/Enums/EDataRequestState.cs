namespace CssUI.DOM.Enums
{
    public enum EDataRequestState : int
    {
        /// <summary>
        /// The user agent hasn't obtained any image data, or has obtained some or all of the image data but hasn't yet decoded enough of the image to get the image dimensions.
        /// </summary>
        Unavailable,

        /// <summary>
        /// The user agent has obtained some of the image data and at least the image dimensions are available.
        /// </summary>
        PartiallyAvailable,

        /// <summary>
        /// The user agent has obtained all of the image data and at least the image dimensions are available.
        /// </summary>
        CompletelyAvailable,

        /// <summary>
        /// The user agent has obtained all of the image data that it can, but it cannot even decode the image enough to get the image dimensions (e.g. the image is corrupted, or the format is not supported, or no data could be obtained).
        /// </summary>
        Broken,
    }
}
