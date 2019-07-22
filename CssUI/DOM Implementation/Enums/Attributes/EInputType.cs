using CssUI.DOM.Internal;

namespace CssUI.DOM
{
    /// <summary>
    /// The type attribute controls the data type (and associated control) of an input element.
    /// </summary>
    [DomEnum]
    public enum EInputType : int
    {/* Docs: https://html.spec.whatwg.org/multipage/input.html#attr-input-type */

        /// <summary>
        /// Arbitrart string
        /// </summary>
        [DomKeyword("hidden")]
        Hidden,

        /// <summary>
        /// Text with no line breaks
        /// </summary>
        [DomKeyword("text")]
        Text,

        /// <summary>
        /// Text with no line breaks
        /// </summary>
        [DomKeyword("search")]
        Search,

        /// <summary>
        /// Text with no line breaks
        /// </summary>
        [DomKeyword("tel")]
        Telephone,

        /// <summary>
        /// An absolute URL
        /// </summary>
        [DomKeyword("url")]
        Url,

        /// <summary>
        /// An e-mail address or list of e-mail addresses
        /// </summary>
        [DomKeyword("email")]
        Email,

        /// <summary>
        /// Text with no line breaks (sensitive information)
        /// </summary>
        [DomKeyword("password")]
        Password,

        /// <summary>
        /// A date (year, month, day) with no time zone
        /// </summary>
        [DomKeyword("date")]
        Date,

        /// <summary>
        /// A date consisting of a year and a month with no time zone
        /// </summary>
        [DomKeyword("month")]
        Month,

        /// <summary>
        /// A date consisting of a week-year number and a week number with no time zone
        /// </summary>
        [DomKeyword("week")]
        Week,

        /// <summary>
        /// A time (hour, minute, seconds, fractional seconds) with no time zone
        /// </summary>
        [DomKeyword("time")]
        Time,

        /// <summary>
        /// A date and time (year, month, day, hour, minute, second, fraction of a second) with no time zone
        /// </summary>
        [DomKeyword("datetime-local")]
        Local,

        /// <summary>
        /// A numerical value
        /// </summary>
        [DomKeyword("number")]
        Number,

        /// <summary>
        /// A numerical value, with the extra semantic that the exact value is not important
        /// </summary>
        [DomKeyword("range")]
        Range,

        /// <summary>
        /// An sRGB color with 8-bit red, green, and blue components
        /// </summary>
        [DomKeyword("color")]
        Color,

        /// <summary>
        /// A set of zero or more values from a predefined list
        /// </summary>
        [DomKeyword("checkbox")]
        Checkbox,

        /// <summary>
        /// An enumerated value
        /// </summary>
        [DomKeyword("radio")]
        Radio,

        /// <summary>
        /// Zero or more files each with a MIME type and optionally a file name
        /// </summary>
        [DomKeyword("file")]
        File,

        /// <summary>
        /// An enumerated value, with the extra semantic that it must be the last value selected and initiates form submission
        /// </summary>
        [DomKeyword("submit")]
        Submit,

        /// <summary>
        /// A coordinate, relative to a particular image's size, with the extra semantic that it must be the last value selected and initiates form submission
        /// </summary>
        [DomKeyword("image")]
        Image,

        /// <summary>
        /// n/a
        /// </summary>
        [DomKeyword("reset")]
        reset,

        /// <summary>
        /// n/a
        /// </summary>
        [DomKeyword("button")]
        button,
    }
}
