using System;

namespace CssUI.DOM.Enums
{
    /// <summary>
    /// Specifies a DOM element attribute type, which determines what kind of string value formats are allowed to be set for it.
    /// </summary>
    public enum EAttributeType : int
    {
        /// <summary>
        /// Any string is valid
        /// </summary>
        String,

        /// <summary>
        /// A number of attributes are boolean attributes. The presence of a boolean attribute on an element represents the true value, and the absence of the attribute represents the false value.
        /// If the attribute is present, its value must either be the empty string or a value that is an ASCII case-insensitive match for the attribute's canonical name, with no leading or trailing whitespace.
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#boolean-attributes
        Boolean,

        /// <summary>
        /// Enumerated attributes may only be assigned a specific set of keywords with special meaning
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#keywords-and-enumerated-attributes
        Enumerated,

        /// <summary>
        /// A string is a valid integer if it consists of one or more ASCII digits, optionally prefixed with a U+002D HYPHEN-MINUS character (-).
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#signed-integers
        Integer,

        /// <summary>
        /// A string is a valid non-negative integer if it consists of one or more ASCII digits.
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#non-negative-integers
        NonNegative_Integer,

        /// <summary>
        /// A string is a valid floating-point number if it consists of only an optional hypen(-), a series of digits, an optional period(.), followed by a series of digits
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#floating-point-numbers
        FloatingPoint,

        /// <summary>
        /// A length is a floating point number greater than or equal to 0.0
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#percentages-and-dimensions
        Length,

        /// <summary>
        /// A percentage is a floating point number greater than or equal to 0.0 followed by a percent sign (%)
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#percentages-and-dimensions
        Percentage,

        /// <summary>
        /// A non-zero length is a floating point number greater than 0.0
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#non-zero-percentages-and-lengths
        NonZero_Length,

        /// <summary>
        /// A non-zero percentage is a floating point number greater than 0.0 followed by a percent sign(%)
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#non-zero-percentages-and-lengths
        NonZero_Percentage,


        /// <summary>
        /// A simple color consists of three 8-bit numbers in the range 0..255, representing the red, green, and blue components of the color respectively, in the sRGB color space. [SRGB]
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#colours
        Color,


        /// <summary>
        /// A time consists of a specific time with no time-zone information, consisting of an hour, a minute, a second, and a fraction of a second.
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#times
        Time,
        /// <summary>
        /// A date consists of a specific proleptic-Gregorian date with no time-zone information, consisting of a year, a month, and a day. [GREGORIAN]
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#dates
        Date,
        /// <summary>
        /// A duration consists of a number of seconds.
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#durations
        Duration,

        /// <summary>
        /// KeyCombination attributes specify a combination of keypresses as a text string in a format defined in the HTML standards
        /// </summary>
        KeyCombo,
    }
}
