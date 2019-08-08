using CssUI.Internal;

namespace CssUI.DOM
{
    /// <summary>
    /// The type attribute controls the data type (and associated control) of an input element.
    /// </summary>
    [MetaEnum]
    public enum EInputType : int
    {/* Docs: https://html.spec.whatwg.org/multipage/input.html#attr-input-type */

        /// <summary>
        /// Arbitrart string
        /// </summary>
        [MetaKeyword("hidden")]
        Hidden,

        /// <summary>
        /// Text with no line breaks
        /// </summary>
        [MetaKeyword("text")]
        Text,

        /// <summary>
        /// Text with no line breaks
        /// </summary>
        [MetaKeyword("search")]
        Search,

        /// <summary>
        /// Text with no line breaks
        /// </summary>
        [MetaKeyword("tel")]
        Telephone,

        /// <summary>
        /// An absolute URL
        /// </summary>
        [MetaKeyword("url")]
        Url,

        /// <summary>
        /// An e-mail address or list of e-mail addresses
        /// </summary>
        [MetaKeyword("email")]
        Email,

        /// <summary>
        /// Text with no line breaks (sensitive information)
        /// </summary>
        [MetaKeyword("password")]
        Password,

        /// <summary>
        /// A date (year, month, day) with no time zone
        /// </summary>
        [MetaKeyword("date")]
        Date,

        /// <summary>
        /// A date consisting of a year and a month with no time zone
        /// </summary>
        [MetaKeyword("month")]
        Month,

        /// <summary>
        /// A date consisting of a week-year number and a week number with no time zone
        /// </summary>
        [MetaKeyword("week")]
        Week,

        /// <summary>
        /// A time (hour, minute, seconds, fractional seconds) with no time zone
        /// </summary>
        [MetaKeyword("time")]
        Time,

        /// <summary>
        /// A date and time (year, month, day, hour, minute, second, fraction of a second) with no time zone
        /// </summary>
        [MetaKeyword("datetime-local")]
        Local,

        /// <summary>
        /// A numerical value
        /// </summary>
        [MetaKeyword("number")]
        Number,

        /// <summary>
        /// A numerical value, with the extra semantic that the exact value is not important
        /// </summary>
        [MetaKeyword("range")]
        Range,

        /// <summary>
        /// An sRGB color with 8-bit red, green, and blue components
        /// </summary>
        [MetaKeyword("color")]
        Color,

        /// <summary>
        /// A set of zero or more values from a predefined list
        /// </summary>
        [MetaKeyword("checkbox")]
        Checkbox,

        /// <summary>
        /// An enumerated value
        /// </summary>
        [MetaKeyword("radio")]
        Radio,

        /// <summary>
        /// Zero or more files each with a MIME type and optionally a file name
        /// </summary>
        [MetaKeyword("file")]
        File,

        /// <summary>
        /// An enumerated value, with the extra semantic that it must be the last value selected and initiates form submission
        /// </summary>
        [MetaKeyword("submit")]
        Submit,

        /// <summary>
        /// A coordinate, relative to a particular image's size, with the extra semantic that it must be the last value selected and initiates form submission
        /// </summary>
        [MetaKeyword("image")]
        Image,

        /// <summary>
        /// n/a
        /// </summary>
        [MetaKeyword("reset")]
        Reset,

        /// <summary>
        /// n/a
        /// </summary>
        [MetaKeyword("button")]
        Button,
    }
}
