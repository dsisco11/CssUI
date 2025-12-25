using EnumRecords;

namespace CssUI.HTML
{
    /// <summary>
    /// The type attribute controls the data type (and associated control) of an input element.
    /// </summary>
    [EnumRecord<KeywordProperties>]
    public enum EInputType : int
    {/* Docs: https://html.spec.whatwg.org/multipage/input.html#attr-input-type */

        /// <summary>
        /// Arbitrart string
        /// </summary>
        [EnumData("hidden")]
        Hidden,

        /// <summary>
        /// Text with no line breaks
        /// </summary>
        [EnumData("text")]
        Text,

        /// <summary>
        /// Text with no line breaks
        /// </summary>
        [EnumData("search")]
        Search,

        /// <summary>
        /// Text with no line breaks
        /// </summary>
        [EnumData("tel")]
        Telephone,

        /// <summary>
        /// An absolute URL
        /// </summary>
        [EnumData("url")]
        Url,

        /// <summary>
        /// An e-mail address or list of e-mail addresses
        /// </summary>
        [EnumData("email")]
        Email,

        /// <summary>
        /// Text with no line breaks (sensitive information)
        /// </summary>
        [EnumData("password")]
        Password,

        /// <summary>
        /// A date (year, month, day) with no time zone
        /// </summary>
        [EnumData("date")]
        Date,

        /// <summary>
        /// A date consisting of a year and a month with no time zone
        /// </summary>
        [EnumData("month")]
        Month,

        /// <summary>
        /// A date consisting of a week-year number and a week number with no time zone
        /// </summary>
        [EnumData("week")]
        Week,

        /// <summary>
        /// A time (hour, minute, seconds, fractional seconds) with no time zone
        /// </summary>
        [EnumData("time")]
        Time,

        /// <summary>
        /// A date and time (year, month, day, hour, minute, second, fraction of a second) with no time zone
        /// </summary>
        [EnumData("datetime-local")]
        Local,

        /// <summary>
        /// A numerical value
        /// </summary>
        [EnumData("number")]
        Number,

        /// <summary>
        /// A numerical value, with the extra semantic that the exact value is not important
        /// </summary>
        [EnumData("range")]
        Range,

        /// <summary>
        /// An sRGB color with 8-bit red, green, and blue components
        /// </summary>
        [EnumData("color")]
        Color,

        /// <summary>
        /// A set of zero or more values from a predefined list
        /// </summary>
        [EnumData("checkbox")]
        Checkbox,

        /// <summary>
        /// An enumerated value
        /// </summary>
        [EnumData("radio")]
        Radio,

        /// <summary>
        /// Zero or more files each with a MIME type and optionally a file name
        /// </summary>
        [EnumData("file")]
        File,

        /// <summary>
        /// An enumerated value, with the extra semantic that it must be the last value selected and initiates form submission
        /// </summary>
        [EnumData("submit")]
        Submit,

        /// <summary>
        /// A coordinate, relative to a particular image's size, with the extra semantic that it must be the last value selected and initiates form submission
        /// </summary>
        [EnumData("image")]
        Image,

        /// <summary>
        /// n/a
        /// </summary>
        [EnumData("reset")]
        Reset,

        /// <summary>
        /// n/a
        /// </summary>
        [EnumData("button")]
        Button,
    }
}

