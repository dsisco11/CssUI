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
    [EnumData("currentcolor", 0x000000, 0, 0, 0)]
    CurrentColor,

    /// <summary>
    /// Fully transparent. This keyword can be considered a shorthand for transparent black, rgba(0,0,0,0), which is its computed value.
    /// </summary>
    /* Docs: https://www.w3.org/TR/css-color-3/#transparent-def */
    [EnumData("transparent", 0x000000, 0, 0, 0)]
    Transparent,

    /// <summary>
    ///
    /// </summary>
    [EnumData("aliceblue", 0xF0F8FF, 240, 248, 255)]
    Aliceblue,

    /// <summary>
    ///
    /// </summary>
    [EnumData("antiquewhite", 0xFAEBD7, 250, 235, 215)]
    Antiquewhite,

    /// <summary>
    ///
    /// </summary>
    [EnumData("aqua", 0x00FFFF, 0, 255, 255)]
    Aqua,

    /// <summary>
    ///
    /// </summary>
    [EnumData("aquamarine", 0x7FFFD4, 127, 255, 212)]
    Aquamarine,

    /// <summary>
    ///
    /// </summary>
    [EnumData("azure", 0xF0FFFF, 240, 255, 255)]
    Azure,

    /// <summary>
    ///
    /// </summary>
    [EnumData("beige", 0xF5F5DC, 245, 245, 220)]
    Beige,

    /// <summary>
    ///
    /// </summary>
    [EnumData("bisque", 0xFFE4C4, 255, 228, 196)]
    Bisque,

    /// <summary>
    ///
    /// </summary>
    [EnumData("black", 0x000000, 0, 0, 0)]
    Black,

    /// <summary>
    ///
    /// </summary>
    [EnumData("blanchedalmond", 0xFFEBCD, 255, 235, 205)]
    Blanchedalmond,

    /// <summary>
    ///
    /// </summary>
    [EnumData("blue", 0x0000FF, 0, 0, 255)]
    Blue,

    /// <summary>
    ///
    /// </summary>
    [EnumData("blueviolet", 0x8A2BE2, 138, 43, 226)]
    Blueviolet,

    /// <summary>
    ///
    /// </summary>
    [EnumData("brown", 0xA52A2A, 165, 42, 42)]
    Brown,

    /// <summary>
    ///
    /// </summary>
    [EnumData("burlywood", 0xDEB887, 222, 184, 135)]
    Burlywood,

    /// <summary>
    ///
    /// </summary>
    [EnumData("cadetblue", 0x5F9EA0, 95, 158, 160)]
    Cadetblue,

    /// <summary>
    ///
    /// </summary>
    [EnumData("chartreuse", 0x7FFF00, 127, 255, 0)]
    Chartreuse,

    /// <summary>
    ///
    /// </summary>
    [EnumData("chocolate", 0xD2691E, 210, 105, 30)]
    Chocolate,

    /// <summary>
    ///
    /// </summary>
    [EnumData("coral", 0xFF7F50, 255, 127, 80)]
    Coral,

    /// <summary>
    ///
    /// </summary>
    [EnumData("cornflowerblue", 0x6495ED, 100, 149, 237)]
    Cornflowerblue,

    /// <summary>
    ///
    /// </summary>
    [EnumData("cornsilk", 0xFFF8DC, 255, 248, 220)]
    Cornsilk,

    /// <summary>
    ///
    /// </summary>
    [EnumData("crimson", 0xDC143C, 220, 20, 60)]
    Crimson,

    /// <summary>
    ///
    /// </summary>
    [EnumData("cyan", 0x00FFFF, 0, 255, 255)]
    Cyan,

    /// <summary>
    ///
    /// </summary>
    [EnumData("darkblue", 0x00008B, 0, 0, 139)]
    Darkblue,

    /// <summary>
    ///
    /// </summary>
    [EnumData("darkcyan", 0x008B8B, 0, 139, 139)]
    Darkcyan,

    /// <summary>
    ///
    /// </summary>
    [EnumData("darkgoldenrod", 0xB8860B, 184, 134, 11)]
    Darkgoldenrod,

    /// <summary>
    ///
    /// </summary>
    [EnumData("darkgray", 0xA9A9A9, 169, 169, 169)]
    Darkgray,

    /// <summary>
    ///
    /// </summary>
    [EnumData("darkgreen", 0x006400, 0, 100, 0)]
    Darkgreen,

    /// <summary>
    ///
    /// </summary>
    [EnumData("darkgrey", 0xA9A9A9, 169, 169, 169)]
    Darkgrey,

    /// <summary>
    ///
    /// </summary>
    [EnumData("darkkhaki", 0xBDB76B, 189, 183, 107)]
    Darkkhaki,

    /// <summary>
    ///
    /// </summary>
    [EnumData("darkmagenta", 0x8B008B, 139, 0, 139)]
    Darkmagenta,

    /// <summary>
    ///
    /// </summary>
    [EnumData("darkolivegreen", 0x556B2F, 85, 107, 47)]
    Darkolivegreen,

    /// <summary>
    ///
    /// </summary>
    [EnumData("darkorange", 0xFF8C00, 255, 140, 0)]
    Darkorange,

    /// <summary>
    ///
    /// </summary>
    [EnumData("darkorchid", 0x9932CC, 153, 50, 204)]
    Darkorchid,

    /// <summary>
    ///
    /// </summary>
    [EnumData("darkred", 0x8B0000, 139, 0, 0)]
    Darkred,

    /// <summary>
    ///
    /// </summary>
    [EnumData("darksalmon", 0xE9967A, 233, 150, 122)]
    Darksalmon,

    /// <summary>
    ///
    /// </summary>
    [EnumData("darkseagreen", 0x8FBC8F, 143, 188, 143)]
    Darkseagreen,

    /// <summary>
    ///
    /// </summary>
    [EnumData("darkslateblue", 0x483D8B, 72, 61, 139)]
    Darkslateblue,

    /// <summary>
    ///
    /// </summary>
    [EnumData("darkslategray", 0x2F4F4F, 47, 79, 79)]
    Darkslategray,

    /// <summary>
    ///
    /// </summary>
    [EnumData("darkslategrey", 0x2F4F4F, 47, 79, 79)]
    Darkslategrey,

    /// <summary>
    ///
    /// </summary>
    [EnumData("darkturquoise", 0x00CED1, 0, 206, 209)]
    Darkturquoise,

    /// <summary>
    ///
    /// </summary>
    [EnumData("darkviolet", 0x9400D3, 148, 0, 211)]
    Darkviolet,

    /// <summary>
    ///
    /// </summary>
    [EnumData("deeppink", 0xFF1493, 255, 20, 147)]
    Deeppink,

    /// <summary>
    ///
    /// </summary>
    [EnumData("deepskyblue", 0x00BFFF, 0, 191, 255)]
    Deepskyblue,

    /// <summary>
    ///
    /// </summary>
    [EnumData("dimgray", 0x696969, 105, 105, 105)]
    Dimgray,

    /// <summary>
    ///
    /// </summary>
    [EnumData("dimgrey", 0x696969, 105, 105, 105)]
    Dimgrey,

    /// <summary>
    ///
    /// </summary>
    [EnumData("dodgerblue", 0x1E90FF, 30, 144, 255)]
    Dodgerblue,

    /// <summary>
    ///
    /// </summary>
    [EnumData("firebrick", 0xB22222, 178, 34, 34)]
    Firebrick,

    /// <summary>
    ///
    /// </summary>
    [EnumData("floralwhite", 0xFFFAF0, 255, 250, 240)]
    Floralwhite,

    /// <summary>
    ///
    /// </summary>
    [EnumData("forestgreen", 0x228B22, 34, 139, 34)]
    Forestgreen,

    /// <summary>
    ///
    /// </summary>
    [EnumData("fuchsia", 0xFF00FF, 255, 0, 255)]
    Fuchsia,

    /// <summary>
    ///
    /// </summary>
    [EnumData("gainsboro", 0xDCDCDC, 220, 220, 220)]
    Gainsboro,

    /// <summary>
    ///
    /// </summary>
    [EnumData("ghostwhite", 0xF8F8FF, 248, 248, 255)]
    Ghostwhite,

    /// <summary>
    ///
    /// </summary>
    [EnumData("gold", 0xFFD700, 255, 215, 0)]
    Gold,

    /// <summary>
    ///
    /// </summary>
    [EnumData("goldenrod", 0xDAA520, 218, 165, 32)]
    Goldenrod,

    /// <summary>
    ///
    /// </summary>
    [EnumData("gray", 0x808080, 128, 128, 128)]
    Gray,

    /// <summary>
    ///
    /// </summary>
    [EnumData("green", 0x008000, 0, 128, 0)]
    Green,

    /// <summary>
    ///
    /// </summary>
    [EnumData("greenyellow", 0xADFF2F, 173, 255, 47)]
    Greenyellow,

    /// <summary>
    ///
    /// </summary>
    [EnumData("grey", 0x808080, 128, 128, 128)]
    Grey,

    /// <summary>
    ///
    /// </summary>
    [EnumData("honeydew", 0xF0FFF0, 240, 255, 240)]
    Honeydew,

    /// <summary>
    ///
    /// </summary>
    [EnumData("hotpink", 0xFF69B4, 255, 105, 180)]
    Hotpink,

    /// <summary>
    ///
    /// </summary>
    [EnumData("indianred", 0xCD5C5C, 205, 92, 92)]
    Indianred,

    /// <summary>
    ///
    /// </summary>
    [EnumData("indigo", 0x4B0082, 75, 0, 130)]
    Indigo,

    /// <summary>
    ///
    /// </summary>
    [EnumData("ivory", 0xFFFFF0, 255, 255, 240)]
    Ivory,

    /// <summary>
    ///
    /// </summary>
    [EnumData("khaki", 0xF0E68C, 240, 230, 140)]
    Khaki,

    /// <summary>
    ///
    /// </summary>
    [EnumData("lavender", 0xE6E6FA, 230, 230, 250)]
    Lavender,

    /// <summary>
    ///
    /// </summary>
    [EnumData("lavenderblush", 0xFFF0F5, 255, 240, 245)]
    Lavenderblush,

    /// <summary>
    ///
    /// </summary>
    [EnumData("lawngreen", 0x7CFC00, 124, 252, 0)]
    Lawngreen,

    /// <summary>
    ///
    /// </summary>
    [EnumData("lemonchiffon", 0xFFFACD, 255, 250, 205)]
    Lemonchiffon,

    /// <summary>
    ///
    /// </summary>
    [EnumData("lightblue", 0xADD8E6, 173, 216, 230)]
    Lightblue,

    /// <summary>
    ///
    /// </summary>
    [EnumData("lightcoral", 0xF08080, 240, 128, 128)]
    Lightcoral,

    /// <summary>
    ///
    /// </summary>
    [EnumData("lightcyan", 0xE0FFFF, 224, 255, 255)]
    Lightcyan,

    /// <summary>
    ///
    /// </summary>
    [EnumData("lightgoldenrodyellow", 0xFAFAD2, 250, 250, 210)]
    Lightgoldenrodyellow,

    /// <summary>
    ///
    /// </summary>
    [EnumData("lightgray", 0xD3D3D3, 211, 211, 211)]
    Lightgray,

    /// <summary>
    ///
    /// </summary>
    [EnumData("lightgreen", 0x90EE90, 144, 238, 144)]
    Lightgreen,

    /// <summary>
    ///
    /// </summary>
    [EnumData("lightgrey", 0xD3D3D3, 211, 211, 211)]
    Lightgrey,

    /// <summary>
    ///
    /// </summary>
    [EnumData("lightpink", 0xFFB6C1, 255, 182, 193)]
    Lightpink,

    /// <summary>
    ///
    /// </summary>
    [EnumData("lightsalmon", 0xFFA07A, 255, 160, 122)]
    Lightsalmon,

    /// <summary>
    ///
    /// </summary>
    [EnumData("lightseagreen", 0x20B2AA, 32, 178, 170)]
    Lightseagreen,

    /// <summary>
    ///
    /// </summary>
    [EnumData("lightskyblue", 0x87CEFA, 135, 206, 250)]
    Lightskyblue,

    /// <summary>
    ///
    /// </summary>
    [EnumData("lightslategray", 0x778899, 119, 136, 153)]
    Lightslategray,

    /// <summary>
    ///
    /// </summary>
    [EnumData("lightslategrey", 0x778899, 119, 136, 153)]
    Lightslategrey,

    /// <summary>
    ///
    /// </summary>
    [EnumData("lightsteelblue", 0xB0C4DE, 176, 196, 222)]
    Lightsteelblue,

    /// <summary>
    ///
    /// </summary>
    [EnumData("lightyellow", 0xFFFFE0, 255, 255, 224)]
    Lightyellow,

    /// <summary>
    ///
    /// </summary>
    [EnumData("lime", 0x00FF00, 0, 255, 0)]
    Lime,

    /// <summary>
    ///
    /// </summary>
    [EnumData("limegreen", 0x32CD32, 50, 205, 50)]
    Limegreen,

    /// <summary>
    ///
    /// </summary>
    [EnumData("linen", 0xFAF0E6, 250, 240, 230)]
    Linen,

    /// <summary>
    ///
    /// </summary>
    [EnumData("magenta", 0xFF00FF, 255, 0, 255)]
    Magenta,

    /// <summary>
    ///
    /// </summary>
    [EnumData("maroon", 0x800000, 128, 0, 0)]
    Maroon,

    /// <summary>
    ///
    /// </summary>
    [EnumData("mediumaquamarine", 0x66CDAA, 102, 205, 170)]
    Mediumaquamarine,

    /// <summary>
    ///
    /// </summary>
    [EnumData("mediumblue", 0x0000CD, 0, 0, 205)]
    Mediumblue,

    /// <summary>
    ///
    /// </summary>
    [EnumData("mediumorchid", 0xBA55D3, 186, 85, 211)]
    Mediumorchid,

    /// <summary>
    ///
    /// </summary>
    [EnumData("mediumpurple", 0x9370DB, 147, 112, 219)]
    Mediumpurple,

    /// <summary>
    ///
    /// </summary>
    [EnumData("mediumseagreen", 0x3CB371, 60, 179, 113)]
    Mediumseagreen,

    /// <summary>
    ///
    /// </summary>
    [EnumData("mediumslateblue", 0x7B68EE, 123, 104, 238)]
    Mediumslateblue,

    /// <summary>
    ///
    /// </summary>
    [EnumData("mediumspringgreen", 0x00FA9A, 0, 250, 154)]
    Mediumspringgreen,

    /// <summary>
    ///
    /// </summary>
    [EnumData("mediumturquoise", 0x48D1CC, 72, 209, 204)]
    Mediumturquoise,

    /// <summary>
    ///
    /// </summary>
    [EnumData("mediumvioletred", 0xC71585, 199, 21, 133)]
    Mediumvioletred,

    /// <summary>
    ///
    /// </summary>
    [EnumData("midnightblue", 0x191970, 25, 25, 112)]
    Midnightblue,

    /// <summary>
    ///
    /// </summary>
    [EnumData("mintcream", 0xF5FFFA, 245, 255, 250)]
    Mintcream,

    /// <summary>
    ///
    /// </summary>
    [EnumData("mistyrose", 0xFFE4E1, 255, 228, 225)]
    Mistyrose,

    /// <summary>
    ///
    /// </summary>
    [EnumData("moccasin", 0xFFE4B5, 255, 228, 181)]
    Moccasin,

    /// <summary>
    ///
    /// </summary>
    [EnumData("navajowhite", 0xFFDEAD, 255, 222, 173)]
    Navajowhite,

    /// <summary>
    ///
    /// </summary>
    [EnumData("navy", 0x000080, 0, 0, 128)]
    Navy,

    /// <summary>
    ///
    /// </summary>
    [EnumData("oldlace", 0xFDF5E6, 253, 245, 230)]
    Oldlace,

    /// <summary>
    ///
    /// </summary>
    [EnumData("olive", 0x808000, 128, 128, 0)]
    Olive,

    /// <summary>
    ///
    /// </summary>
    [EnumData("olivedrab", 0x6B8E23, 107, 142, 35)]
    Olivedrab,

    /// <summary>
    ///
    /// </summary>
    [EnumData("orange", 0xFFA500, 255, 165, 0)]
    Orange,

    /// <summary>
    ///
    /// </summary>
    [EnumData("orangered", 0xFF4500, 255, 69, 0)]
    Orangered,

    /// <summary>
    ///
    /// </summary>
    [EnumData("orchid", 0xDA70D6, 218, 112, 214)]
    Orchid,

    /// <summary>
    ///
    /// </summary>
    [EnumData("palegoldenrod", 0xEEE8AA, 238, 232, 170)]
    Palegoldenrod,

    /// <summary>
    ///
    /// </summary>
    [EnumData("palegreen", 0x98FB98, 152, 251, 152)]
    Palegreen,

    /// <summary>
    ///
    /// </summary>
    [EnumData("paleturquoise", 0xAFEEEE, 175, 238, 238)]
    Paleturquoise,

    /// <summary>
    ///
    /// </summary>
    [EnumData("palevioletred", 0xDB7093, 219, 112, 147)]
    Palevioletred,

    /// <summary>
    ///
    /// </summary>
    [EnumData("papayawhip", 0xFFEFD5, 255, 239, 213)]
    Papayawhip,

    /// <summary>
    ///
    /// </summary>
    [EnumData("peachpuff", 0xFFDAB9, 255, 218, 185)]
    Peachpuff,

    /// <summary>
    ///
    /// </summary>
    [EnumData("peru", 0xCD853F, 205, 133, 63)]
    Peru,

    /// <summary>
    ///
    /// </summary>
    [EnumData("pink", 0xFFC0CB, 255, 192, 203)]
    Pink,

    /// <summary>
    ///
    /// </summary>
    [EnumData("plum", 0xDDA0DD, 221, 160, 221)]
    Plum,

    /// <summary>
    ///
    /// </summary>
    [EnumData("powderblue", 0xB0E0E6, 176, 224, 230)]
    Powderblue,

    /// <summary>
    ///
    /// </summary>
    [EnumData("purple", 0x800080, 128, 0, 128)]
    Purple,

    /// <summary>
    ///
    /// </summary>
    [EnumData("rebeccapurple", 0x663399, 102, 51, 153)]
    Rebeccapurple,

    /// <summary>
    ///
    /// </summary>
    [EnumData("red", 0xFF0000, 255, 0, 0)]
    Red,

    /// <summary>
    ///
    /// </summary>
    [EnumData("rosybrown", 0xBC8F8F, 188, 143, 143)]
    Rosybrown,

    /// <summary>
    ///
    /// </summary>
    [EnumData("royalblue", 0x4169E1, 65, 105, 225)]
    Royalblue,

    /// <summary>
    ///
    /// </summary>
    [EnumData("saddlebrown", 0x8B4513, 139, 69, 19)]
    Saddlebrown,

    /// <summary>
    ///
    /// </summary>
    [EnumData("salmon", 0xFA8072, 250, 128, 114)]
    Salmon,

    /// <summary>
    ///
    /// </summary>
    [EnumData("sandybrown", 0xF4A460, 244, 164, 96)]
    Sandybrown,

    /// <summary>
    ///
    /// </summary>
    [EnumData("seagreen", 0x2E8B57, 46, 139, 87)]
    Seagreen,

    /// <summary>
    ///
    /// </summary>
    [EnumData("seashell", 0xFFF5EE, 255, 245, 238)]
    Seashell,

    /// <summary>
    ///
    /// </summary>
    [EnumData("sienna", 0xA0522D, 160, 82, 45)]
    Sienna,

    /// <summary>
    ///
    /// </summary>
    [EnumData("silver", 0xC0C0C0, 192, 192, 192)]
    Silver,

    /// <summary>
    ///
    /// </summary>
    [EnumData("skyblue", 0x87CEEB, 135, 206, 235)]
    Skyblue,

    /// <summary>
    ///
    /// </summary>
    [EnumData("slateblue", 0x6A5ACD, 106, 90, 205)]
    Slateblue,

    /// <summary>
    ///
    /// </summary>
    [EnumData("slategray", 0x708090, 112, 128, 144)]
    Slategray,

    /// <summary>
    ///
    /// </summary>
    [EnumData("slategrey", 0x708090, 112, 128, 144)]
    Slategrey,

    /// <summary>
    ///
    /// </summary>
    [EnumData("snow", 0xFFFAFA, 255, 250, 250)]
    Snow,

    /// <summary>
    ///
    /// </summary>
    [EnumData("springgreen", 0x00FF7F, 0, 255, 127)]
    Springgreen,

    /// <summary>
    ///
    /// </summary>
    [EnumData("steelblue", 0x4682B4, 70, 130, 180)]
    Steelblue,

    /// <summary>
    ///
    /// </summary>
    [EnumData("tan", 0xD2B48C, 210, 180, 140)]
    Tan,

    /// <summary>
    ///
    /// </summary>
    [EnumData("teal", 0x008080, 0, 128, 128)]
    Teal,

    /// <summary>
    ///
    /// </summary>
    [EnumData("thistle", 0xD8BFD8, 216, 191, 216)]
    Thistle,

    /// <summary>
    ///
    /// </summary>
    [EnumData("tomato", 0xFF6347, 255, 99, 71)]
    Tomato,

    /// <summary>
    ///
    /// </summary>
    [EnumData("turquoise", 0x40E0D0, 64, 224, 208)]
    Turquoise,

    /// <summary>
    ///
    /// </summary>
    [EnumData("violet", 0xEE82EE, 238, 130, 238)]
    Violet,

    /// <summary>
    ///
    /// </summary>
    [EnumData("wheat", 0xF5DEB3, 245, 222, 179)]
    Wheat,

    /// <summary>
    ///
    /// </summary>
    [EnumData("white", 0xFFFFFF, 255, 255, 255)]
    White,

    /// <summary>
    ///
    /// </summary>
    [EnumData("whitesmoke", 0xF5F5F5, 245, 245, 245)]
    Whitesmoke,

    /// <summary>
    ///
    /// </summary>
    [EnumData("yellow", 0xFFFF00, 255, 255, 0)]
    Yellow,

    /// <summary>
    ///
    /// </summary>
    [EnumData("yellowgreen", 0x9ACD32, 154, 205, 50)]
    Yellowgreen,

}

