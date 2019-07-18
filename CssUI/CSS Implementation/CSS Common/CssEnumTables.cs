/* 
* =====================================================
*		   	THIS CODE IS GENERATED, 
*	ANY CHANGES ARE SUBJECT TO BEING OVERWRITTEN
* =====================================================
*/
using System;
using System.Linq;
using CssUI.Internal;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace CssUI.CSS.Internal
{
	internal static class CssEnumTables
	{
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Get_Enum_Index<Ty>()
        {
			return Lookup_Enum_Index(typeof(Ty).Name);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Lookup_Enum_Index(string Name)
        {
            switch(Name)
            {
				case "EBorderStyle": return 0;
				case "EBoxSizingMode": return 1;
				case "ECssDirection": return 2;
				case "ECssGenericFontFamily": return 3;
				case "ECssUnit": return 4;
				case "EDisplayMode": return 5;
				case "EFontStyle": return 6;
				case "EObjectFit": return 7;
				case "EOverflowMode": return 8;
				case "EPositioning": return 9;
				case "EPositioningScheme": return 10;
				case "ETextAlign": return 11;
				case "EWritingMode": return 12;
				default: return -1; //throw new NotImplementedException($"No lookup-index has defined for enum type '{typeof(Ty).Name}'");
            }
        }


		#region Static Enum Tables
		internal static readonly string[][] TABLE;
		internal static readonly List<Dictionary<AtomicString, dynamic>> KEYWORD;

		static CssEnumTables()
		{
			TABLE = new string[14][];
			KEYWORD = new List<Dictionary<AtomicString, dynamic>>(14);
			int maxIndex = 0;
			/* CssUI.CSS.Enums.EBorderStyle */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.CSS.Enums.EBorderStyle)).Cast<CssUI.CSS.Enums.EBorderStyle>().Max();
			TABLE[0] = new string[maxIndex+1];
			TABLE[0][(int)CssUI.CSS.Enums.EBorderStyle.None] = "none";
			TABLE[0][(int)CssUI.CSS.Enums.EBorderStyle.Hidden] = "hidden";
			TABLE[0][(int)CssUI.CSS.Enums.EBorderStyle.Dotted] = "dotted";
			TABLE[0][(int)CssUI.CSS.Enums.EBorderStyle.Dashed] = "dashed";
			TABLE[0][(int)CssUI.CSS.Enums.EBorderStyle.Solid] = "solid";
			TABLE[0][(int)CssUI.CSS.Enums.EBorderStyle.Double] = "double";
			TABLE[0][(int)CssUI.CSS.Enums.EBorderStyle.Groove] = "groove";
			TABLE[0][(int)CssUI.CSS.Enums.EBorderStyle.Ridge] = "ridge";
			TABLE[0][(int)CssUI.CSS.Enums.EBorderStyle.Inset] = "inset";
			TABLE[0][(int)CssUI.CSS.Enums.EBorderStyle.Outset] = "outset";


			/* CssUI.CSS.EBoxSizingMode */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.CSS.EBoxSizingMode)).Cast<CssUI.CSS.EBoxSizingMode>().Max();
			TABLE[1] = new string[maxIndex+1];
			TABLE[1][(int)CssUI.CSS.EBoxSizingMode.ContentBox] = "content-box";
			TABLE[1][(int)CssUI.CSS.EBoxSizingMode.BorderBox] = "border-box";


			/* CssUI.CSS.ECssDirection */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.CSS.EDirection)).Cast<CssUI.CSS.EDirection>().Max();
			TABLE[2] = new string[maxIndex+1];
			TABLE[2][(int)CssUI.CSS.EDirection.LTR] = "ltr";
			TABLE[2][(int)CssUI.CSS.EDirection.RTL] = "rtl";


			/* CssUI.CSS.ECssGenericFontFamily */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.CSS.EGenericFontFamily)).Cast<CssUI.CSS.EGenericFontFamily>().Max();
			TABLE[3] = new string[maxIndex+1];
			TABLE[3][(int)CssUI.CSS.EGenericFontFamily.Serif] = "serif";
			TABLE[3][(int)CssUI.CSS.EGenericFontFamily.SansSerif] = "sans-serif";
			TABLE[3][(int)CssUI.CSS.EGenericFontFamily.Cursive] = "cursive";
			TABLE[3][(int)CssUI.CSS.EGenericFontFamily.Fantasy] = "fantasy";
			TABLE[3][(int)CssUI.CSS.EGenericFontFamily.Monospace] = "monospace";


			/* CssUI.CSS.ECssUnit */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.CSS.EUnit)).Cast<CssUI.CSS.EUnit>().Max();
			TABLE[4] = new string[maxIndex+1];
			TABLE[4][(int)CssUI.CSS.EUnit.None] = "";
			TABLE[4][(int)CssUI.CSS.EUnit.PX] = "px";
			TABLE[4][(int)CssUI.CSS.EUnit.EM] = "em";
			TABLE[4][(int)CssUI.CSS.EUnit.EX] = "ex";
			TABLE[4][(int)CssUI.CSS.EUnit.CH] = "ch";
			TABLE[4][(int)CssUI.CSS.EUnit.REM] = "rem";
			TABLE[4][(int)CssUI.CSS.EUnit.VW] = "vw";
			TABLE[4][(int)CssUI.CSS.EUnit.VH] = "vh";
			TABLE[4][(int)CssUI.CSS.EUnit.VMIN] = "vmin";
			TABLE[4][(int)CssUI.CSS.EUnit.VMAX] = "vmax";
			TABLE[4][(int)CssUI.CSS.EUnit.DEG] = "deg";
			TABLE[4][(int)CssUI.CSS.EUnit.GRAD] = "grad";
			TABLE[4][(int)CssUI.CSS.EUnit.RAD] = "rad";
			TABLE[4][(int)CssUI.CSS.EUnit.TURN] = "turn";


			/* CssUI.CSS.EDisplayMode */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.CSS.EDisplayMode)).Cast<CssUI.CSS.EDisplayMode>().Max();
			TABLE[5] = new string[maxIndex+1];
			TABLE[5][(int)CssUI.CSS.EDisplayMode.NONE] = "none";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.CONTENT] = "content";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.INLINE] = "inline";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.RUN_IN] = "run-in";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.BLOCK] = "block";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.LIST_ITEM] = "list-item";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.FLEX] = "flex";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.FLOW] = "flow";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.FLOW_ROOT] = "flow-root";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.GRID] = "grid";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.TABLE] = "table";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.TABLE_ROW_GROUP] = "table-row-group";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.TABLE_HEADER_GROUP] = "table-header-group";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.TABLE_FOOTER_GROUP] = "table-footer-group";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.TABLE_ROW] = "table-row";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.TABLE_CELL] = "table-cell";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.TABLE_COLUMN_GROUP] = "table-column-group";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.TABLE_COLUMN] = "table-column";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.TABLE_CAPTION] = "table-caption";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.INLINE_BLOCK] = "inline-block";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.INLINE_TABLE] = "inline-table";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.INLINE_FLEX] = "inline-flex";
			TABLE[5][(int)CssUI.CSS.EDisplayMode.INLINE_GRID] = "inline-grid";


			/* CssUI.CSS.EFontStyle */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.CSS.EFontStyle)).Cast<CssUI.CSS.EFontStyle>().Max();
			TABLE[6] = new string[maxIndex+1];
			TABLE[6][(int)CssUI.CSS.EFontStyle.Normal] = "normal";
			TABLE[6][(int)CssUI.CSS.EFontStyle.Italic] = "italic";
			TABLE[6][(int)CssUI.CSS.EFontStyle.Oblique] = "oblique";


			/* CssUI.CSS.EObjectFit */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.CSS.EObjectFit)).Cast<CssUI.CSS.EObjectFit>().Max();
			TABLE[7] = new string[maxIndex+1];
			TABLE[7][(int)CssUI.CSS.EObjectFit.Fill] = "fill";
			TABLE[7][(int)CssUI.CSS.EObjectFit.Contain] = "contain";
			TABLE[7][(int)CssUI.CSS.EObjectFit.Cover] = "cover";
			TABLE[7][(int)CssUI.CSS.EObjectFit.None] = "none";
			TABLE[7][(int)CssUI.CSS.EObjectFit.Scale_Down] = "scale-down";


			/* CssUI.CSS.EOverflowMode */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.CSS.EOverflowMode)).Cast<CssUI.CSS.EOverflowMode>().Max();
			TABLE[8] = new string[maxIndex+1];
			TABLE[8][(int)CssUI.CSS.EOverflowMode.Visible] = "visible";
			TABLE[8][(int)CssUI.CSS.EOverflowMode.Hidden] = "hidden";
			TABLE[8][(int)CssUI.CSS.EOverflowMode.Clip] = "clip";
			TABLE[8][(int)CssUI.CSS.EOverflowMode.Scroll] = "scroll";
			TABLE[8][(int)CssUI.CSS.EOverflowMode.Auto] = "auto";


			/* CssUI.CSS.EPositioning */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.CSS.EPositioning)).Cast<CssUI.CSS.EPositioning>().Max();
			TABLE[9] = new string[maxIndex+1];
			TABLE[9][(int)CssUI.CSS.EPositioning.Static] = "static";
			TABLE[9][(int)CssUI.CSS.EPositioning.Relative] = "relative";
			TABLE[9][(int)CssUI.CSS.EPositioning.Absolute] = "absolute";
			TABLE[9][(int)CssUI.CSS.EPositioning.Fixed] = "fixed";


			/* CssUI.CSS.EPositioningScheme */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.CSS.EPositioningScheme)).Cast<CssUI.CSS.EPositioningScheme>().Max();
			TABLE[10] = new string[maxIndex+1];
			TABLE[10][(int)CssUI.CSS.EPositioningScheme.Normal] = "normal";
			TABLE[10][(int)CssUI.CSS.EPositioningScheme.Float] = "float";
			TABLE[10][(int)CssUI.CSS.EPositioningScheme.Absolute] = "absolute";


			/* CssUI.CSS.ETextAlign */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.CSS.ETextAlign)).Cast<CssUI.CSS.ETextAlign>().Max();
			TABLE[11] = new string[maxIndex+1];
			TABLE[11][(int)CssUI.CSS.ETextAlign.Start] = "start";
			TABLE[11][(int)CssUI.CSS.ETextAlign.End] = "end";
			TABLE[11][(int)CssUI.CSS.ETextAlign.Left] = "left";
			TABLE[11][(int)CssUI.CSS.ETextAlign.Right] = "right";
			TABLE[11][(int)CssUI.CSS.ETextAlign.Center] = "center";
			TABLE[11][(int)CssUI.CSS.ETextAlign.Justify] = "justify";
			TABLE[11][(int)CssUI.CSS.ETextAlign.MatchParent] = "match-parent";
			TABLE[11][(int)CssUI.CSS.ETextAlign.StartEnd] = "start-end";


			/* CssUI.CSS.EWritingMode */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.CSS.EWritingMode)).Cast<CssUI.CSS.EWritingMode>().Max();
			TABLE[12] = new string[maxIndex+1];
			TABLE[12][(int)CssUI.CSS.EWritingMode.Horizontal_TB] = "horizontal-tb";
			TABLE[12][(int)CssUI.CSS.EWritingMode.Vertical_RL] = "vertical-rl";
			TABLE[12][(int)CssUI.CSS.EWritingMode.Vertical_LR] = "vertical-lr";
			TABLE[12][(int)CssUI.CSS.EWritingMode.Sideways_RL] = "sideways-rl";
			TABLE[12][(int)CssUI.CSS.EWritingMode.Sideways_LR] = "sideways-lr";



		/* Generate Reverse lookup maps */
			/* CssUI.CSS.Enums.EBorderStyle */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[0].Add(new AtomicString("none"), CssUI.CSS.Enums.EBorderStyle.None);
			KEYWORD[0].Add(new AtomicString("hidden"), CssUI.CSS.Enums.EBorderStyle.Hidden);
			KEYWORD[0].Add(new AtomicString("dotted"), CssUI.CSS.Enums.EBorderStyle.Dotted);
			KEYWORD[0].Add(new AtomicString("dashed"), CssUI.CSS.Enums.EBorderStyle.Dashed);
			KEYWORD[0].Add(new AtomicString("solid"), CssUI.CSS.Enums.EBorderStyle.Solid);
			KEYWORD[0].Add(new AtomicString("double"), CssUI.CSS.Enums.EBorderStyle.Double);
			KEYWORD[0].Add(new AtomicString("groove"), CssUI.CSS.Enums.EBorderStyle.Groove);
			KEYWORD[0].Add(new AtomicString("ridge"), CssUI.CSS.Enums.EBorderStyle.Ridge);
			KEYWORD[0].Add(new AtomicString("inset"), CssUI.CSS.Enums.EBorderStyle.Inset);
			KEYWORD[0].Add(new AtomicString("outset"), CssUI.CSS.Enums.EBorderStyle.Outset);


			/* CssUI.CSS.EBoxSizingMode */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[1].Add(new AtomicString("content-box"), CssUI.CSS.EBoxSizingMode.ContentBox);
			KEYWORD[1].Add(new AtomicString("border-box"), CssUI.CSS.EBoxSizingMode.BorderBox);


			/* CssUI.CSS.ECssDirection */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[2].Add(new AtomicString("ltr"), CssUI.CSS.EDirection.LTR);
			KEYWORD[2].Add(new AtomicString("rtl"), CssUI.CSS.EDirection.RTL);


			/* CssUI.CSS.ECssGenericFontFamily */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[3].Add(new AtomicString("serif"), CssUI.CSS.EGenericFontFamily.Serif);
			KEYWORD[3].Add(new AtomicString("sans-serif"), CssUI.CSS.EGenericFontFamily.SansSerif);
			KEYWORD[3].Add(new AtomicString("cursive"), CssUI.CSS.EGenericFontFamily.Cursive);
			KEYWORD[3].Add(new AtomicString("fantasy"), CssUI.CSS.EGenericFontFamily.Fantasy);
			KEYWORD[3].Add(new AtomicString("monospace"), CssUI.CSS.EGenericFontFamily.Monospace);


			/* CssUI.CSS.ECssUnit */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[4].Add(new AtomicString(""), CssUI.CSS.EUnit.None);
			KEYWORD[4].Add(new AtomicString("px"), CssUI.CSS.EUnit.PX);
			KEYWORD[4].Add(new AtomicString("em"), CssUI.CSS.EUnit.EM);
			KEYWORD[4].Add(new AtomicString("ex"), CssUI.CSS.EUnit.EX);
			KEYWORD[4].Add(new AtomicString("ch"), CssUI.CSS.EUnit.CH);
			KEYWORD[4].Add(new AtomicString("rem"), CssUI.CSS.EUnit.REM);
			KEYWORD[4].Add(new AtomicString("vw"), CssUI.CSS.EUnit.VW);
			KEYWORD[4].Add(new AtomicString("vh"), CssUI.CSS.EUnit.VH);
			KEYWORD[4].Add(new AtomicString("vmin"), CssUI.CSS.EUnit.VMIN);
			KEYWORD[4].Add(new AtomicString("vmax"), CssUI.CSS.EUnit.VMAX);
			KEYWORD[4].Add(new AtomicString("deg"), CssUI.CSS.EUnit.DEG);
			KEYWORD[4].Add(new AtomicString("grad"), CssUI.CSS.EUnit.GRAD);
			KEYWORD[4].Add(new AtomicString("rad"), CssUI.CSS.EUnit.RAD);
			KEYWORD[4].Add(new AtomicString("turn"), CssUI.CSS.EUnit.TURN);


			/* CssUI.CSS.EDisplayMode */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[5].Add(new AtomicString("none"), CssUI.CSS.EDisplayMode.NONE);
			KEYWORD[5].Add(new AtomicString("content"), CssUI.CSS.EDisplayMode.CONTENT);
			KEYWORD[5].Add(new AtomicString("inline"), CssUI.CSS.EDisplayMode.INLINE);
			KEYWORD[5].Add(new AtomicString("run-in"), CssUI.CSS.EDisplayMode.RUN_IN);
			KEYWORD[5].Add(new AtomicString("block"), CssUI.CSS.EDisplayMode.BLOCK);
			KEYWORD[5].Add(new AtomicString("list-item"), CssUI.CSS.EDisplayMode.LIST_ITEM);
			KEYWORD[5].Add(new AtomicString("flex"), CssUI.CSS.EDisplayMode.FLEX);
			KEYWORD[5].Add(new AtomicString("flow"), CssUI.CSS.EDisplayMode.FLOW);
			KEYWORD[5].Add(new AtomicString("flow-root"), CssUI.CSS.EDisplayMode.FLOW_ROOT);
			KEYWORD[5].Add(new AtomicString("grid"), CssUI.CSS.EDisplayMode.GRID);
			KEYWORD[5].Add(new AtomicString("table"), CssUI.CSS.EDisplayMode.TABLE);
			KEYWORD[5].Add(new AtomicString("table-row-group"), CssUI.CSS.EDisplayMode.TABLE_ROW_GROUP);
			KEYWORD[5].Add(new AtomicString("table-header-group"), CssUI.CSS.EDisplayMode.TABLE_HEADER_GROUP);
			KEYWORD[5].Add(new AtomicString("table-footer-group"), CssUI.CSS.EDisplayMode.TABLE_FOOTER_GROUP);
			KEYWORD[5].Add(new AtomicString("table-row"), CssUI.CSS.EDisplayMode.TABLE_ROW);
			KEYWORD[5].Add(new AtomicString("table-cell"), CssUI.CSS.EDisplayMode.TABLE_CELL);
			KEYWORD[5].Add(new AtomicString("table-column-group"), CssUI.CSS.EDisplayMode.TABLE_COLUMN_GROUP);
			KEYWORD[5].Add(new AtomicString("table-column"), CssUI.CSS.EDisplayMode.TABLE_COLUMN);
			KEYWORD[5].Add(new AtomicString("table-caption"), CssUI.CSS.EDisplayMode.TABLE_CAPTION);
			KEYWORD[5].Add(new AtomicString("inline-block"), CssUI.CSS.EDisplayMode.INLINE_BLOCK);
			KEYWORD[5].Add(new AtomicString("inline-table"), CssUI.CSS.EDisplayMode.INLINE_TABLE);
			KEYWORD[5].Add(new AtomicString("inline-flex"), CssUI.CSS.EDisplayMode.INLINE_FLEX);
			KEYWORD[5].Add(new AtomicString("inline-grid"), CssUI.CSS.EDisplayMode.INLINE_GRID);


			/* CssUI.CSS.EFontStyle */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[6].Add(new AtomicString("normal"), CssUI.CSS.EFontStyle.Normal);
			KEYWORD[6].Add(new AtomicString("italic"), CssUI.CSS.EFontStyle.Italic);
			KEYWORD[6].Add(new AtomicString("oblique"), CssUI.CSS.EFontStyle.Oblique);


			/* CssUI.CSS.EObjectFit */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[7].Add(new AtomicString("fill"), CssUI.CSS.EObjectFit.Fill);
			KEYWORD[7].Add(new AtomicString("contain"), CssUI.CSS.EObjectFit.Contain);
			KEYWORD[7].Add(new AtomicString("cover"), CssUI.CSS.EObjectFit.Cover);
			KEYWORD[7].Add(new AtomicString("none"), CssUI.CSS.EObjectFit.None);
			KEYWORD[7].Add(new AtomicString("scale-down"), CssUI.CSS.EObjectFit.Scale_Down);


			/* CssUI.CSS.EOverflowMode */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[8].Add(new AtomicString("visible"), CssUI.CSS.EOverflowMode.Visible);
			KEYWORD[8].Add(new AtomicString("hidden"), CssUI.CSS.EOverflowMode.Hidden);
			KEYWORD[8].Add(new AtomicString("clip"), CssUI.CSS.EOverflowMode.Clip);
			KEYWORD[8].Add(new AtomicString("scroll"), CssUI.CSS.EOverflowMode.Scroll);
			KEYWORD[8].Add(new AtomicString("auto"), CssUI.CSS.EOverflowMode.Auto);


			/* CssUI.CSS.EPositioning */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[9].Add(new AtomicString("static"), CssUI.CSS.EPositioning.Static);
			KEYWORD[9].Add(new AtomicString("relative"), CssUI.CSS.EPositioning.Relative);
			KEYWORD[9].Add(new AtomicString("absolute"), CssUI.CSS.EPositioning.Absolute);
			KEYWORD[9].Add(new AtomicString("fixed"), CssUI.CSS.EPositioning.Fixed);


			/* CssUI.CSS.EPositioningScheme */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[10].Add(new AtomicString("normal"), CssUI.CSS.EPositioningScheme.Normal);
			KEYWORD[10].Add(new AtomicString("float"), CssUI.CSS.EPositioningScheme.Float);
			KEYWORD[10].Add(new AtomicString("absolute"), CssUI.CSS.EPositioningScheme.Absolute);


			/* CssUI.CSS.ETextAlign */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[11].Add(new AtomicString("start"), CssUI.CSS.ETextAlign.Start);
			KEYWORD[11].Add(new AtomicString("end"), CssUI.CSS.ETextAlign.End);
			KEYWORD[11].Add(new AtomicString("left"), CssUI.CSS.ETextAlign.Left);
			KEYWORD[11].Add(new AtomicString("right"), CssUI.CSS.ETextAlign.Right);
			KEYWORD[11].Add(new AtomicString("center"), CssUI.CSS.ETextAlign.Center);
			KEYWORD[11].Add(new AtomicString("justify"), CssUI.CSS.ETextAlign.Justify);
			KEYWORD[11].Add(new AtomicString("match-parent"), CssUI.CSS.ETextAlign.MatchParent);
			KEYWORD[11].Add(new AtomicString("start-end"), CssUI.CSS.ETextAlign.StartEnd);


			/* CssUI.CSS.EWritingMode */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[12].Add(new AtomicString("horizontal-tb"), CssUI.CSS.EWritingMode.Horizontal_TB);
			KEYWORD[12].Add(new AtomicString("vertical-rl"), CssUI.CSS.EWritingMode.Vertical_RL);
			KEYWORD[12].Add(new AtomicString("vertical-lr"), CssUI.CSS.EWritingMode.Vertical_LR);
			KEYWORD[12].Add(new AtomicString("sideways-rl"), CssUI.CSS.EWritingMode.Sideways_RL);
			KEYWORD[12].Add(new AtomicString("sideways-lr"), CssUI.CSS.EWritingMode.Sideways_LR);


		}
		#endregion
	}
}

