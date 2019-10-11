using System;
using System.Runtime.CompilerServices;
using CssUI.CSS.Enums;

namespace CssUI.CSS
{
    public struct DisplayType
    {
        #region Properties
        /// <summary>
        /// The 'Inner Display Type'
        /// Defines the *-level of this box, whether it is block-level, inline-level, or other.
        /// Dictates how the principal box itself participates in flow layout.
        /// </summary>
        public readonly EOuterDisplayType Outer;
        /// <summary>
        /// The 'Inner Display Type'
        /// Defines (if it is a non-replaced element) the kind of formatting context it generates, dictating how its descendant boxes are laid out. (The inner display of a replaced element is outside the scope of CSS.)
        /// </summary>
        public readonly EInnerDisplayType Inner;
        #endregion

        #region Accessors
        /// <summary>
        /// A block container either contains only inline-level boxes participating in an inline formatting context, 
        /// or contains only block-level boxes participating in a block formatting context (possibly generating anonymous block boxes to ensure this constraint, as defined in CSS2§9.2.1.1).
        /// </summary>
        public bool IsBlockContainer => (Inner == EInnerDisplayType.Flow_Root);

        /// <summary> 
        /// Content that participates in block layout. Specifically, block-level boxes have an Outer display type of 'Block'
        /// </summary>
        public bool IsBlockLevel => (Outer == EOuterDisplayType.Block);
        /// <summary>
        /// Content that participates in inline layout. Specifically, inline-level boxes and text runs.
        /// </summary>
        public bool IsInlineLevel => (Outer == EOuterDisplayType.Inline || Outer == EOuterDisplayType.Run_In);
        #endregion

        #region Constructors
        public DisplayType(EOuterDisplayType outer, EInnerDisplayType inner)
        {
            Outer = outer;
            Inner = inner;
        }

        public DisplayType(EDisplayMode DisplayMode)
        {
            switch (DisplayMode)
            {
                case EDisplayMode.NONE:
                    Outer = EOuterDisplayType.None;
                    Inner = EInnerDisplayType.None;
                    break;
                case EDisplayMode.BLOCK:
                    Outer = EOuterDisplayType.Block;
                    Inner = EInnerDisplayType.Flow_Root;
                    break;
                case EDisplayMode.FLOW_ROOT:
                    Outer = EOuterDisplayType.Block;
                    Inner = EInnerDisplayType.Flow_Root;
                    break;
                case EDisplayMode.INLINE:
                    Outer = EOuterDisplayType.Inline;
                    Inner = EInnerDisplayType.Flow;
                    break;
                case EDisplayMode.INLINE_BLOCK:
                    Outer = EOuterDisplayType.Inline;
                    Inner = EInnerDisplayType.Flow_Root;
                    break;
                case EDisplayMode.RUN_IN:
                    Outer = EOuterDisplayType.Run_In;
                    Inner = EInnerDisplayType.Flow;
                    break;
                case EDisplayMode.LIST_ITEM:
                    Outer = EOuterDisplayType.Block;
                    Inner = EInnerDisplayType.Flow_Root;
                    break;
                case EDisplayMode.FLEX:
                    Outer = EOuterDisplayType.Block;
                    Inner = EInnerDisplayType.Flex;
                    break;
                case EDisplayMode.INLINE_FLEX:
                    Outer = EOuterDisplayType.Inline;
                    Inner = EInnerDisplayType.Flex;
                    break;
                case EDisplayMode.GRID:
                    Outer = EOuterDisplayType.Block;
                    Inner = EInnerDisplayType.Grid;
                    break;
                case EDisplayMode.INLINE_GRID:
                    Outer = EOuterDisplayType.Inline;
                    Inner = EInnerDisplayType.Grid;
                    break;
                case EDisplayMode.TABLE:
                    Outer = EOuterDisplayType.Block;
                    Inner = EInnerDisplayType.Table;
                    break;
                case EDisplayMode.INLINE_TABLE:
                    Outer = EOuterDisplayType.Inline;
                    Inner = EInnerDisplayType.Table;
                    break;
                default:
                    throw new NotImplementedException($"Display type \"{Lookup.Keyword(DisplayMode)}\" has not been implemented yet");
            }
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DisplayType From(EDisplayMode DisplayMode) => new DisplayType(DisplayMode);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EOuterDisplayType Get_Outer(EDisplayMode DisplayMode) => new DisplayType(DisplayMode).Outer;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EInnerDisplayType Get_Inner(EDisplayMode DisplayMode) => new DisplayType(DisplayMode).Inner;

    }
}
