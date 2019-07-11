/* 
* =====================================================
*		   	THIS CODE IS GENERATED, 
*	ANY CHANGES ARE SUBJECT TO BEING OVERWRITTEN
* =====================================================
*/
using System;
using System.Linq;
using System.Collections.Generic;
using CssUI.Internal;
namespace CssUI.CSS.Internal
{
	internal static class CssEnumTables
	{
        public static int Get_Enum_Index<Ty>()
        {
            switch(typeof(Ty).Name)
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
			/* CssUI.EBorderStyle */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.EBorderStyle)).Cast<CssUI.EBorderStyle>().Max();
			TABLE[0] = new string[maxIndex+1];
			TABLE[0][(int)CssUI.EBorderStyle.None] = "none";
			TABLE[0][(int)CssUI.EBorderStyle.Hidden] = "hidden";
			TABLE[0][(int)CssUI.EBorderStyle.Dotted] = "dotted";
			TABLE[0][(int)CssUI.EBorderStyle.Dashed] = "dashed";
			TABLE[0][(int)CssUI.EBorderStyle.Solid] = "solid";
			TABLE[0][(int)CssUI.EBorderStyle.Double] = "double";
			TABLE[0][(int)CssUI.EBorderStyle.Groove] = "groove";
			TABLE[0][(int)CssUI.EBorderStyle.Ridge] = "ridge";
			TABLE[0][(int)CssUI.EBorderStyle.Inset] = "inset";
			TABLE[0][(int)CssUI.EBorderStyle.Outset] = "outset";


			/* CssUI.Enums.EBoxSizingMode */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.Enums.EBoxSizingMode)).Cast<CssUI.Enums.EBoxSizingMode>().Max();
			TABLE[1] = new string[maxIndex+1];
			TABLE[1][(int)CssUI.Enums.EBoxSizingMode.ContentBox] = "content-box";
			TABLE[1][(int)CssUI.Enums.EBoxSizingMode.BorderBox] = "border-box";


			/* CssUI.Enums.ECssDirection */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.Enums.ECssDirection)).Cast<CssUI.Enums.ECssDirection>().Max();
			TABLE[2] = new string[maxIndex+1];
			TABLE[2][(int)CssUI.Enums.ECssDirection.LTR] = "ltr";
			TABLE[2][(int)CssUI.Enums.ECssDirection.RTL] = "rtl";


			/* CssUI.Enums.ECssGenericFontFamily */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.Enums.ECssGenericFontFamily)).Cast<CssUI.Enums.ECssGenericFontFamily>().Max();
			TABLE[3] = new string[maxIndex+1];
			TABLE[3][(int)CssUI.Enums.ECssGenericFontFamily.Serif] = "serif";
			TABLE[3][(int)CssUI.Enums.ECssGenericFontFamily.SansSerif] = "sans-serif";
			TABLE[3][(int)CssUI.Enums.ECssGenericFontFamily.Cursive] = "cursive";
			TABLE[3][(int)CssUI.Enums.ECssGenericFontFamily.Fantasy] = "fantasy";
			TABLE[3][(int)CssUI.Enums.ECssGenericFontFamily.Monospace] = "monospace";


			/* CssUI.ECssUnit */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.ECssUnit)).Cast<CssUI.ECssUnit>().Max();
			TABLE[4] = new string[maxIndex+1];
			TABLE[4][(int)CssUI.ECssUnit.None] = "";
			TABLE[4][(int)CssUI.ECssUnit.PX] = "px";
			TABLE[4][(int)CssUI.ECssUnit.EM] = "em";
			TABLE[4][(int)CssUI.ECssUnit.EX] = "ex";
			TABLE[4][(int)CssUI.ECssUnit.CH] = "ch";
			TABLE[4][(int)CssUI.ECssUnit.REM] = "rem";
			TABLE[4][(int)CssUI.ECssUnit.VW] = "vw";
			TABLE[4][(int)CssUI.ECssUnit.VH] = "vh";
			TABLE[4][(int)CssUI.ECssUnit.VMIN] = "vmin";
			TABLE[4][(int)CssUI.ECssUnit.VMAX] = "vmax";
			TABLE[4][(int)CssUI.ECssUnit.DEG] = "deg";
			TABLE[4][(int)CssUI.ECssUnit.GRAD] = "grad";
			TABLE[4][(int)CssUI.ECssUnit.RAD] = "rad";
			TABLE[4][(int)CssUI.ECssUnit.TURN] = "turn";


			/* CssUI.Enums.EDisplayMode */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.Enums.EDisplayMode)).Cast<CssUI.Enums.EDisplayMode>().Max();
			TABLE[5] = new string[maxIndex+1];
			TABLE[5][(int)CssUI.Enums.EDisplayMode.NONE] = "none";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.CONTENT] = "content";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.INLINE] = "inline";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.RUN_IN] = "run-in";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.BLOCK] = "block";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.LIST_ITEM] = "list-item";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.FLEX] = "flex";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.FLOW] = "flow";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.FLOW_ROOT] = "flow-root";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.GRID] = "grid";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.TABLE] = "table";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.TABLE_ROW_GROUP] = "table-row-group";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.TABLE_HEADER_GROUP] = "table-header-group";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.TABLE_FOOTER_GROUP] = "table-footer-group";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.TABLE_ROW] = "table-row";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.TABLE_CELL] = "table-cell";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.TABLE_COLUMN_GROUP] = "table-column-group";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.TABLE_COLUMN] = "table-column";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.TABLE_CAPTION] = "table-caption";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.INLINE_BLOCK] = "inline-block";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.INLINE_TABLE] = "inline-table";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.INLINE_FLEX] = "inline-flex";
			TABLE[5][(int)CssUI.Enums.EDisplayMode.INLINE_GRID] = "inline-grid";


			/* CssUI.Enums.EFontStyle */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.Enums.EFontStyle)).Cast<CssUI.Enums.EFontStyle>().Max();
			TABLE[6] = new string[maxIndex+1];
			TABLE[6][(int)CssUI.Enums.EFontStyle.Normal] = "normal";
			TABLE[6][(int)CssUI.Enums.EFontStyle.Italic] = "italic";
			TABLE[6][(int)CssUI.Enums.EFontStyle.Oblique] = "oblique";


			/* CssUI.Enums.EObjectFit */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.Enums.EObjectFit)).Cast<CssUI.Enums.EObjectFit>().Max();
			TABLE[7] = new string[maxIndex+1];
			TABLE[7][(int)CssUI.Enums.EObjectFit.Fill] = "fill";
			TABLE[7][(int)CssUI.Enums.EObjectFit.Contain] = "contain";
			TABLE[7][(int)CssUI.Enums.EObjectFit.Cover] = "cover";
			TABLE[7][(int)CssUI.Enums.EObjectFit.None] = "none";
			TABLE[7][(int)CssUI.Enums.EObjectFit.Scale_Down] = "scale-down";


			/* CssUI.Enums.EOverflowMode */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.Enums.EOverflowMode)).Cast<CssUI.Enums.EOverflowMode>().Max();
			TABLE[8] = new string[maxIndex+1];
			TABLE[8][(int)CssUI.Enums.EOverflowMode.Visible] = "visible";
			TABLE[8][(int)CssUI.Enums.EOverflowMode.Hidden] = "hidden";
			TABLE[8][(int)CssUI.Enums.EOverflowMode.Clip] = "clip";
			TABLE[8][(int)CssUI.Enums.EOverflowMode.Scroll] = "scroll";
			TABLE[8][(int)CssUI.Enums.EOverflowMode.Auto] = "auto";


			/* CssUI.Enums.EPositioning */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.Enums.EPositioning)).Cast<CssUI.Enums.EPositioning>().Max();
			TABLE[9] = new string[maxIndex+1];
			TABLE[9][(int)CssUI.Enums.EPositioning.Static] = "static";
			TABLE[9][(int)CssUI.Enums.EPositioning.Relative] = "relative";
			TABLE[9][(int)CssUI.Enums.EPositioning.Absolute] = "absolute";
			TABLE[9][(int)CssUI.Enums.EPositioning.Fixed] = "fixed";


			/* CssUI.Enums.EPositioningScheme */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.Enums.EPositioningScheme)).Cast<CssUI.Enums.EPositioningScheme>().Max();
			TABLE[10] = new string[maxIndex+1];
			TABLE[10][(int)CssUI.Enums.EPositioningScheme.Normal] = "normal";
			TABLE[10][(int)CssUI.Enums.EPositioningScheme.Float] = "float";
			TABLE[10][(int)CssUI.Enums.EPositioningScheme.Absolute] = "absolute";


			/* CssUI.Enums.ETextAlign */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.Enums.ETextAlign)).Cast<CssUI.Enums.ETextAlign>().Max();
			TABLE[11] = new string[maxIndex+1];
			TABLE[11][(int)CssUI.Enums.ETextAlign.Start] = "start";
			TABLE[11][(int)CssUI.Enums.ETextAlign.End] = "end";
			TABLE[11][(int)CssUI.Enums.ETextAlign.Left] = "left";
			TABLE[11][(int)CssUI.Enums.ETextAlign.Right] = "right";
			TABLE[11][(int)CssUI.Enums.ETextAlign.Center] = "center";
			TABLE[11][(int)CssUI.Enums.ETextAlign.Justify] = "justify";
			TABLE[11][(int)CssUI.Enums.ETextAlign.MatchParent] = "match-parent";
			TABLE[11][(int)CssUI.Enums.ETextAlign.StartEnd] = "start-end";


			/* CssUI.Enums.EWritingMode */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.Enums.EWritingMode)).Cast<CssUI.Enums.EWritingMode>().Max();
			TABLE[12] = new string[maxIndex+1];
			TABLE[12][(int)CssUI.Enums.EWritingMode.Horizontal_TB] = "horizontal-tb";
			TABLE[12][(int)CssUI.Enums.EWritingMode.Vertical_RL] = "vertical-rl";
			TABLE[12][(int)CssUI.Enums.EWritingMode.Vertical_LR] = "vertical-lr";
			TABLE[12][(int)CssUI.Enums.EWritingMode.Sideways_RL] = "sideways-rl";
			TABLE[12][(int)CssUI.Enums.EWritingMode.Sideways_LR] = "sideways-lr";



		/* Generate Reverse lookup maps */
			/* CssUI.EBorderStyle */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[0].Add(new AtomicString("none"), CssUI.EBorderStyle.None);
			KEYWORD[0].Add(new AtomicString("hidden"), CssUI.EBorderStyle.Hidden);
			KEYWORD[0].Add(new AtomicString("dotted"), CssUI.EBorderStyle.Dotted);
			KEYWORD[0].Add(new AtomicString("dashed"), CssUI.EBorderStyle.Dashed);
			KEYWORD[0].Add(new AtomicString("solid"), CssUI.EBorderStyle.Solid);
			KEYWORD[0].Add(new AtomicString("double"), CssUI.EBorderStyle.Double);
			KEYWORD[0].Add(new AtomicString("groove"), CssUI.EBorderStyle.Groove);
			KEYWORD[0].Add(new AtomicString("ridge"), CssUI.EBorderStyle.Ridge);
			KEYWORD[0].Add(new AtomicString("inset"), CssUI.EBorderStyle.Inset);
			KEYWORD[0].Add(new AtomicString("outset"), CssUI.EBorderStyle.Outset);


			/* CssUI.Enums.EBoxSizingMode */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[1].Add(new AtomicString("content-box"), CssUI.Enums.EBoxSizingMode.ContentBox);
			KEYWORD[1].Add(new AtomicString("border-box"), CssUI.Enums.EBoxSizingMode.BorderBox);


			/* CssUI.Enums.ECssDirection */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[2].Add(new AtomicString("ltr"), CssUI.Enums.ECssDirection.LTR);
			KEYWORD[2].Add(new AtomicString("rtl"), CssUI.Enums.ECssDirection.RTL);


			/* CssUI.Enums.ECssGenericFontFamily */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[3].Add(new AtomicString("serif"), CssUI.Enums.ECssGenericFontFamily.Serif);
			KEYWORD[3].Add(new AtomicString("sans-serif"), CssUI.Enums.ECssGenericFontFamily.SansSerif);
			KEYWORD[3].Add(new AtomicString("cursive"), CssUI.Enums.ECssGenericFontFamily.Cursive);
			KEYWORD[3].Add(new AtomicString("fantasy"), CssUI.Enums.ECssGenericFontFamily.Fantasy);
			KEYWORD[3].Add(new AtomicString("monospace"), CssUI.Enums.ECssGenericFontFamily.Monospace);


			/* CssUI.ECssUnit */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[4].Add(new AtomicString(""), CssUI.ECssUnit.None);
			KEYWORD[4].Add(new AtomicString("px"), CssUI.ECssUnit.PX);
			KEYWORD[4].Add(new AtomicString("em"), CssUI.ECssUnit.EM);
			KEYWORD[4].Add(new AtomicString("ex"), CssUI.ECssUnit.EX);
			KEYWORD[4].Add(new AtomicString("ch"), CssUI.ECssUnit.CH);
			KEYWORD[4].Add(new AtomicString("rem"), CssUI.ECssUnit.REM);
			KEYWORD[4].Add(new AtomicString("vw"), CssUI.ECssUnit.VW);
			KEYWORD[4].Add(new AtomicString("vh"), CssUI.ECssUnit.VH);
			KEYWORD[4].Add(new AtomicString("vmin"), CssUI.ECssUnit.VMIN);
			KEYWORD[4].Add(new AtomicString("vmax"), CssUI.ECssUnit.VMAX);
			KEYWORD[4].Add(new AtomicString("deg"), CssUI.ECssUnit.DEG);
			KEYWORD[4].Add(new AtomicString("grad"), CssUI.ECssUnit.GRAD);
			KEYWORD[4].Add(new AtomicString("rad"), CssUI.ECssUnit.RAD);
			KEYWORD[4].Add(new AtomicString("turn"), CssUI.ECssUnit.TURN);


			/* CssUI.Enums.EDisplayMode */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[5].Add(new AtomicString("none"), CssUI.Enums.EDisplayMode.NONE);
			KEYWORD[5].Add(new AtomicString("content"), CssUI.Enums.EDisplayMode.CONTENT);
			KEYWORD[5].Add(new AtomicString("inline"), CssUI.Enums.EDisplayMode.INLINE);
			KEYWORD[5].Add(new AtomicString("run-in"), CssUI.Enums.EDisplayMode.RUN_IN);
			KEYWORD[5].Add(new AtomicString("block"), CssUI.Enums.EDisplayMode.BLOCK);
			KEYWORD[5].Add(new AtomicString("list-item"), CssUI.Enums.EDisplayMode.LIST_ITEM);
			KEYWORD[5].Add(new AtomicString("flex"), CssUI.Enums.EDisplayMode.FLEX);
			KEYWORD[5].Add(new AtomicString("flow"), CssUI.Enums.EDisplayMode.FLOW);
			KEYWORD[5].Add(new AtomicString("flow-root"), CssUI.Enums.EDisplayMode.FLOW_ROOT);
			KEYWORD[5].Add(new AtomicString("grid"), CssUI.Enums.EDisplayMode.GRID);
			KEYWORD[5].Add(new AtomicString("table"), CssUI.Enums.EDisplayMode.TABLE);
			KEYWORD[5].Add(new AtomicString("table-row-group"), CssUI.Enums.EDisplayMode.TABLE_ROW_GROUP);
			KEYWORD[5].Add(new AtomicString("table-header-group"), CssUI.Enums.EDisplayMode.TABLE_HEADER_GROUP);
			KEYWORD[5].Add(new AtomicString("table-footer-group"), CssUI.Enums.EDisplayMode.TABLE_FOOTER_GROUP);
			KEYWORD[5].Add(new AtomicString("table-row"), CssUI.Enums.EDisplayMode.TABLE_ROW);
			KEYWORD[5].Add(new AtomicString("table-cell"), CssUI.Enums.EDisplayMode.TABLE_CELL);
			KEYWORD[5].Add(new AtomicString("table-column-group"), CssUI.Enums.EDisplayMode.TABLE_COLUMN_GROUP);
			KEYWORD[5].Add(new AtomicString("table-column"), CssUI.Enums.EDisplayMode.TABLE_COLUMN);
			KEYWORD[5].Add(new AtomicString("table-caption"), CssUI.Enums.EDisplayMode.TABLE_CAPTION);
			KEYWORD[5].Add(new AtomicString("inline-block"), CssUI.Enums.EDisplayMode.INLINE_BLOCK);
			KEYWORD[5].Add(new AtomicString("inline-table"), CssUI.Enums.EDisplayMode.INLINE_TABLE);
			KEYWORD[5].Add(new AtomicString("inline-flex"), CssUI.Enums.EDisplayMode.INLINE_FLEX);
			KEYWORD[5].Add(new AtomicString("inline-grid"), CssUI.Enums.EDisplayMode.INLINE_GRID);


			/* CssUI.Enums.EFontStyle */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[6].Add(new AtomicString("normal"), CssUI.Enums.EFontStyle.Normal);
			KEYWORD[6].Add(new AtomicString("italic"), CssUI.Enums.EFontStyle.Italic);
			KEYWORD[6].Add(new AtomicString("oblique"), CssUI.Enums.EFontStyle.Oblique);


			/* CssUI.Enums.EObjectFit */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[7].Add(new AtomicString("fill"), CssUI.Enums.EObjectFit.Fill);
			KEYWORD[7].Add(new AtomicString("contain"), CssUI.Enums.EObjectFit.Contain);
			KEYWORD[7].Add(new AtomicString("cover"), CssUI.Enums.EObjectFit.Cover);
			KEYWORD[7].Add(new AtomicString("none"), CssUI.Enums.EObjectFit.None);
			KEYWORD[7].Add(new AtomicString("scale-down"), CssUI.Enums.EObjectFit.Scale_Down);


			/* CssUI.Enums.EOverflowMode */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[8].Add(new AtomicString("visible"), CssUI.Enums.EOverflowMode.Visible);
			KEYWORD[8].Add(new AtomicString("hidden"), CssUI.Enums.EOverflowMode.Hidden);
			KEYWORD[8].Add(new AtomicString("clip"), CssUI.Enums.EOverflowMode.Clip);
			KEYWORD[8].Add(new AtomicString("scroll"), CssUI.Enums.EOverflowMode.Scroll);
			KEYWORD[8].Add(new AtomicString("auto"), CssUI.Enums.EOverflowMode.Auto);


			/* CssUI.Enums.EPositioning */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[9].Add(new AtomicString("static"), CssUI.Enums.EPositioning.Static);
			KEYWORD[9].Add(new AtomicString("relative"), CssUI.Enums.EPositioning.Relative);
			KEYWORD[9].Add(new AtomicString("absolute"), CssUI.Enums.EPositioning.Absolute);
			KEYWORD[9].Add(new AtomicString("fixed"), CssUI.Enums.EPositioning.Fixed);


			/* CssUI.Enums.EPositioningScheme */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[10].Add(new AtomicString("normal"), CssUI.Enums.EPositioningScheme.Normal);
			KEYWORD[10].Add(new AtomicString("float"), CssUI.Enums.EPositioningScheme.Float);
			KEYWORD[10].Add(new AtomicString("absolute"), CssUI.Enums.EPositioningScheme.Absolute);


			/* CssUI.Enums.ETextAlign */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[11].Add(new AtomicString("start"), CssUI.Enums.ETextAlign.Start);
			KEYWORD[11].Add(new AtomicString("end"), CssUI.Enums.ETextAlign.End);
			KEYWORD[11].Add(new AtomicString("left"), CssUI.Enums.ETextAlign.Left);
			KEYWORD[11].Add(new AtomicString("right"), CssUI.Enums.ETextAlign.Right);
			KEYWORD[11].Add(new AtomicString("center"), CssUI.Enums.ETextAlign.Center);
			KEYWORD[11].Add(new AtomicString("justify"), CssUI.Enums.ETextAlign.Justify);
			KEYWORD[11].Add(new AtomicString("match-parent"), CssUI.Enums.ETextAlign.MatchParent);
			KEYWORD[11].Add(new AtomicString("start-end"), CssUI.Enums.ETextAlign.StartEnd);


			/* CssUI.Enums.EWritingMode */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[12].Add(new AtomicString("horizontal-tb"), CssUI.Enums.EWritingMode.Horizontal_TB);
			KEYWORD[12].Add(new AtomicString("vertical-rl"), CssUI.Enums.EWritingMode.Vertical_RL);
			KEYWORD[12].Add(new AtomicString("vertical-lr"), CssUI.Enums.EWritingMode.Vertical_LR);
			KEYWORD[12].Add(new AtomicString("sideways-rl"), CssUI.Enums.EWritingMode.Sideways_RL);
			KEYWORD[12].Add(new AtomicString("sideways-lr"), CssUI.Enums.EWritingMode.Sideways_LR);


		}
		#endregion
	}
}

