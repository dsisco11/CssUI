using EnumRecords;

namespace CssUI.CSS;

/// <summary>
/// Lists all of the named colors for CSS/HTML
/// </summary>
[EnumRecord<NamedColorProperties>]
public enum EColor : int
{
    /// <summary>
    /// The value of the ‘color’ property.
    /// The used value of the ‘currentColor’ keyword is the computed value of the ‘color’ property.
    /// If the ‘currentColor’ keyword is set on the ‘color’ property itself, it is treated as ‘color: inherit’.
    /// </summary>
    /* Docs: https://www.w3.org/TR/css-color-3/#currentcolor */
    [EnumRecordProperties("currentcolor", 0x000000, 0, 0, 0)]
    CurrentColor,

    /// <summary>
    /// Fully transparent. This keyword can be considered a shorthand for transparent black, rgba(0,0,0,0), which is its computed value.
    /// </summary>
    /* Docs: https://www.w3.org/TR/css-color-3/#transparent-def */
    [EnumRecordProperties("transparent", 0x000000, 0, 0, 0)]
    Transparent,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("aliceblue", 0xF0F8FF, 240, 248, 255)]
    Aliceblue,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("antiquewhite", 0xFAEBD7, 250, 235, 215)]
    Antiquewhite,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("aqua", 0x00FFFF, 0, 255, 255)]
    Aqua,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("aquamarine", 0x7FFFD4, 127, 255, 212)]
    Aquamarine,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("azure", 0xF0FFFF, 240, 255, 255)]
    Azure,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("beige", 0xF5F5DC, 245, 245, 220)]
    Beige,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("bisque", 0xFFE4C4, 255, 228, 196)]
    Bisque,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("black", 0x000000, 0, 0, 0)]
    Black,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("blanchedalmond", 0xFFEBCD, 255, 235, 205)]
    Blanchedalmond,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("blue", 0x0000FF, 0, 0, 255)]
    Blue,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("blueviolet", 0x8A2BE2, 138, 43, 226)]
    Blueviolet,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("brown", 0xA52A2A, 165, 42, 42)]
    Brown,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("burlywood", 0xDEB887, 222, 184, 135)]
    Burlywood,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("cadetblue", 0x5F9EA0, 95, 158, 160)]
    Cadetblue,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("chartreuse", 0x7FFF00, 127, 255, 0)]
    Chartreuse,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("chocolate", 0xD2691E, 210, 105, 30)]
    Chocolate,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("coral", 0xFF7F50, 255, 127, 80)]
    Coral,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("cornflowerblue", 0x6495ED, 100, 149, 237)]
    Cornflowerblue,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("cornsilk", 0xFFF8DC, 255, 248, 220)]
    Cornsilk,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("crimson", 0xDC143C, 220, 20, 60)]
    Crimson,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("cyan", 0x00FFFF, 0, 255, 255)]
    Cyan,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("darkblue", 0x00008B, 0, 0, 139)]
    Darkblue,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("darkcyan", 0x008B8B, 0, 139, 139)]
    Darkcyan,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("darkgoldenrod", 0xB8860B, 184, 134, 11)]
    Darkgoldenrod,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("darkgray", 0xA9A9A9, 169, 169, 169)]
    Darkgray,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("darkgreen", 0x006400, 0, 100, 0)]
    Darkgreen,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("darkgrey", 0xA9A9A9, 169, 169, 169)]
    Darkgrey,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("darkkhaki", 0xBDB76B, 189, 183, 107)]
    Darkkhaki,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("darkmagenta", 0x8B008B, 139, 0, 139)]
    Darkmagenta,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("darkolivegreen", 0x556B2F, 85, 107, 47)]
    Darkolivegreen,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("darkorange", 0xFF8C00, 255, 140, 0)]
    Darkorange,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("darkorchid", 0x9932CC, 153, 50, 204)]
    Darkorchid,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("darkred", 0x8B0000, 139, 0, 0)]
    Darkred,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("darksalmon", 0xE9967A, 233, 150, 122)]
    Darksalmon,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("darkseagreen", 0x8FBC8F, 143, 188, 143)]
    Darkseagreen,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("darkslateblue", 0x483D8B, 72, 61, 139)]
    Darkslateblue,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("darkslategray", 0x2F4F4F, 47, 79, 79)]
    Darkslategray,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("darkslategrey", 0x2F4F4F, 47, 79, 79)]
    Darkslategrey,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("darkturquoise", 0x00CED1, 0, 206, 209)]
    Darkturquoise,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("darkviolet", 0x9400D3, 148, 0, 211)]
    Darkviolet,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("deeppink", 0xFF1493, 255, 20, 147)]
    Deeppink,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("deepskyblue", 0x00BFFF, 0, 191, 255)]
    Deepskyblue,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("dimgray", 0x696969, 105, 105, 105)]
    Dimgray,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("dimgrey", 0x696969, 105, 105, 105)]
    Dimgrey,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("dodgerblue", 0x1E90FF, 30, 144, 255)]
    Dodgerblue,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("firebrick", 0xB22222, 178, 34, 34)]
    Firebrick,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("floralwhite", 0xFFFAF0, 255, 250, 240)]
    Floralwhite,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("forestgreen", 0x228B22, 34, 139, 34)]
    Forestgreen,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("fuchsia", 0xFF00FF, 255, 0, 255)]
    Fuchsia,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("gainsboro", 0xDCDCDC, 220, 220, 220)]
    Gainsboro,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("ghostwhite", 0xF8F8FF, 248, 248, 255)]
    Ghostwhite,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("gold", 0xFFD700, 255, 215, 0)]
    Gold,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("goldenrod", 0xDAA520, 218, 165, 32)]
    Goldenrod,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("gray", 0x808080, 128, 128, 128)]
    Gray,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("green", 0x008000, 0, 128, 0)]
    Green,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("greenyellow", 0xADFF2F, 173, 255, 47)]
    Greenyellow,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("grey", 0x808080, 128, 128, 128)]
    Grey,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("honeydew", 0xF0FFF0, 240, 255, 240)]
    Honeydew,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("hotpink", 0xFF69B4, 255, 105, 180)]
    Hotpink,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("indianred", 0xCD5C5C, 205, 92, 92)]
    Indianred,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("indigo", 0x4B0082, 75, 0, 130)]
    Indigo,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("ivory", 0xFFFFF0, 255, 255, 240)]
    Ivory,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("khaki", 0xF0E68C, 240, 230, 140)]
    Khaki,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("lavender", 0xE6E6FA, 230, 230, 250)]
    Lavender,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("lavenderblush", 0xFFF0F5, 255, 240, 245)]
    Lavenderblush,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("lawngreen", 0x7CFC00, 124, 252, 0)]
    Lawngreen,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("lemonchiffon", 0xFFFACD, 255, 250, 205)]
    Lemonchiffon,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("lightblue", 0xADD8E6, 173, 216, 230)]
    Lightblue,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("lightcoral", 0xF08080, 240, 128, 128)]
    Lightcoral,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("lightcyan", 0xE0FFFF, 224, 255, 255)]
    Lightcyan,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("lightgoldenrodyellow", 0xFAFAD2, 250, 250, 210)]
    Lightgoldenrodyellow,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("lightgray", 0xD3D3D3, 211, 211, 211)]
    Lightgray,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("lightgreen", 0x90EE90, 144, 238, 144)]
    Lightgreen,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("lightgrey", 0xD3D3D3, 211, 211, 211)]
    Lightgrey,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("lightpink", 0xFFB6C1, 255, 182, 193)]
    Lightpink,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("lightsalmon", 0xFFA07A, 255, 160, 122)]
    Lightsalmon,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("lightseagreen", 0x20B2AA, 32, 178, 170)]
    Lightseagreen,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("lightskyblue", 0x87CEFA, 135, 206, 250)]
    Lightskyblue,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("lightslategray", 0x778899, 119, 136, 153)]
    Lightslategray,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("lightslategrey", 0x778899, 119, 136, 153)]
    Lightslategrey,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("lightsteelblue", 0xB0C4DE, 176, 196, 222)]
    Lightsteelblue,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("lightyellow", 0xFFFFE0, 255, 255, 224)]
    Lightyellow,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("lime", 0x00FF00, 0, 255, 0)]
    Lime,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("limegreen", 0x32CD32, 50, 205, 50)]
    Limegreen,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("linen", 0xFAF0E6, 250, 240, 230)]
    Linen,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("magenta", 0xFF00FF, 255, 0, 255)]
    Magenta,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("maroon", 0x800000, 128, 0, 0)]
    Maroon,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("mediumaquamarine", 0x66CDAA, 102, 205, 170)]
    Mediumaquamarine,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("mediumblue", 0x0000CD, 0, 0, 205)]
    Mediumblue,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("mediumorchid", 0xBA55D3, 186, 85, 211)]
    Mediumorchid,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("mediumpurple", 0x9370DB, 147, 112, 219)]
    Mediumpurple,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("mediumseagreen", 0x3CB371, 60, 179, 113)]
    Mediumseagreen,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("mediumslateblue", 0x7B68EE, 123, 104, 238)]
    Mediumslateblue,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("mediumspringgreen", 0x00FA9A, 0, 250, 154)]
    Mediumspringgreen,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("mediumturquoise", 0x48D1CC, 72, 209, 204)]
    Mediumturquoise,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("mediumvioletred", 0xC71585, 199, 21, 133)]
    Mediumvioletred,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("midnightblue", 0x191970, 25, 25, 112)]
    Midnightblue,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("mintcream", 0xF5FFFA, 245, 255, 250)]
    Mintcream,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("mistyrose", 0xFFE4E1, 255, 228, 225)]
    Mistyrose,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("moccasin", 0xFFE4B5, 255, 228, 181)]
    Moccasin,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("navajowhite", 0xFFDEAD, 255, 222, 173)]
    Navajowhite,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("navy", 0x000080, 0, 0, 128)]
    Navy,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("oldlace", 0xFDF5E6, 253, 245, 230)]
    Oldlace,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("olive", 0x808000, 128, 128, 0)]
    Olive,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("olivedrab", 0x6B8E23, 107, 142, 35)]
    Olivedrab,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("orange", 0xFFA500, 255, 165, 0)]
    Orange,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("orangered", 0xFF4500, 255, 69, 0)]
    Orangered,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("orchid", 0xDA70D6, 218, 112, 214)]
    Orchid,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("palegoldenrod", 0xEEE8AA, 238, 232, 170)]
    Palegoldenrod,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("palegreen", 0x98FB98, 152, 251, 152)]
    Palegreen,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("paleturquoise", 0xAFEEEE, 175, 238, 238)]
    Paleturquoise,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("palevioletred", 0xDB7093, 219, 112, 147)]
    Palevioletred,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("papayawhip", 0xFFEFD5, 255, 239, 213)]
    Papayawhip,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("peachpuff", 0xFFDAB9, 255, 218, 185)]
    Peachpuff,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("peru", 0xCD853F, 205, 133, 63)]
    Peru,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("pink", 0xFFC0CB, 255, 192, 203)]
    Pink,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("plum", 0xDDA0DD, 221, 160, 221)]
    Plum,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("powderblue", 0xB0E0E6, 176, 224, 230)]
    Powderblue,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("purple", 0x800080, 128, 0, 128)]
    Purple,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("rebeccapurple", 0x663399, 102, 51, 153)]
    Rebeccapurple,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("red", 0xFF0000, 255, 0, 0)]
    Red,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("rosybrown", 0xBC8F8F, 188, 143, 143)]
    Rosybrown,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("royalblue", 0x4169E1, 65, 105, 225)]
    Royalblue,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("saddlebrown", 0x8B4513, 139, 69, 19)]
    Saddlebrown,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("salmon", 0xFA8072, 250, 128, 114)]
    Salmon,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("sandybrown", 0xF4A460, 244, 164, 96)]
    Sandybrown,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("seagreen", 0x2E8B57, 46, 139, 87)]
    Seagreen,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("seashell", 0xFFF5EE, 255, 245, 238)]
    Seashell,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("sienna", 0xA0522D, 160, 82, 45)]
    Sienna,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("silver", 0xC0C0C0, 192, 192, 192)]
    Silver,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("skyblue", 0x87CEEB, 135, 206, 235)]
    Skyblue,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("slateblue", 0x6A5ACD, 106, 90, 205)]
    Slateblue,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("slategray", 0x708090, 112, 128, 144)]
    Slategray,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("slategrey", 0x708090, 112, 128, 144)]
    Slategrey,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("snow", 0xFFFAFA, 255, 250, 250)]
    Snow,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("springgreen", 0x00FF7F, 0, 255, 127)]
    Springgreen,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("steelblue", 0x4682B4, 70, 130, 180)]
    Steelblue,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("tan", 0xD2B48C, 210, 180, 140)]
    Tan,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("teal", 0x008080, 0, 128, 128)]
    Teal,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("thistle", 0xD8BFD8, 216, 191, 216)]
    Thistle,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("tomato", 0xFF6347, 255, 99, 71)]
    Tomato,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("turquoise", 0x40E0D0, 64, 224, 208)]
    Turquoise,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("violet", 0xEE82EE, 238, 130, 238)]
    Violet,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("wheat", 0xF5DEB3, 245, 222, 179)]
    Wheat,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("white", 0xFFFFFF, 255, 255, 255)]
    White,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("whitesmoke", 0xF5F5F5, 245, 245, 245)]
    Whitesmoke,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("yellow", 0xFFFF00, 255, 255, 0)]
    Yellow,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("yellowgreen", 0x9ACD32, 154, 205, 50)]
    Yellowgreen,

}

