using System;
using EnumRecords;

namespace CssUI.CSS;

/// <summary>
/// Defines all the possible border styles
/// </summary>
[Flags, EnumRecord<KeywordProperties>]
public enum EBorderStyle : int
{
    /// <summary>
    /// No border, Color and width are ignored.
    /// </summary>
    [EnumData("none")]
    None = (1 << 0),
    /// <summary>
    /// Same as 'None' but with different conflict resolution for border-collapsed tables
    /// </summary>
    [EnumData("hidden")]
    Hidden = (1 << 1),
    [EnumData("dotted")]
    Dotted = (1 << 2),
    [EnumData("dashed")]
    Dashed = (1 << 3),
    [EnumData("solid")]
    Solid = (1 << 4),
    [EnumData("double")]
    Double = (1 << 5),
    [EnumData("groove")]
    Groove = (1 << 6),
    [EnumData("ridge")]
    Ridge = (1 << 7),
    [EnumData("inset")]
    Inset = (1 << 8),
    [EnumData("outset")]
    Outset = (1 << 9),
};

