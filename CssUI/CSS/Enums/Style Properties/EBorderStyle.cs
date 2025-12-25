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
    [EnumRecordProperties("none")]
    None = (1 << 0),
    /// <summary>
    /// Same as 'None' but with different conflict resolution for border-collapsed tables
    /// </summary>
    [EnumRecordProperties("hidden")]
    Hidden = (1 << 1),
    [EnumRecordProperties("dotted")]
    Dotted = (1 << 2),
    [EnumRecordProperties("dashed")]
    Dashed = (1 << 3),
    [EnumRecordProperties("solid")]
    Solid = (1 << 4),
    [EnumRecordProperties("double")]
    Double = (1 << 5),
    [EnumRecordProperties("groove")]
    Groove = (1 << 6),
    [EnumRecordProperties("ridge")]
    Ridge = (1 << 7),
    [EnumRecordProperties("inset")]
    Inset = (1 << 8),
    [EnumRecordProperties("outset")]
    Outset = (1 << 9),
};

