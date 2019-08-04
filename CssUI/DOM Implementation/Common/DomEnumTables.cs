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
namespace CssUI.DOM.Internal
{
	internal static class DomEnumTables
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
				case "EReactionName": return 0;
				case "EAutoCapitalizationHint": return 1;
				case "ECellScope": return 2;
				case "EDraggable": return 3;
				case "EEnterKeyHint": return 4;
				case "EInputMode": return 5;
				case "ESpellcheck": return 6;
				case "EElementTag": return 7;
				case "EQuirksMode": return 8;
				case "EScrollLogicalPosition": return 9;
				case "EShadowRootMode": return 10;
				default: return -1;//throw new NotImplementedException($"No lookup-index has defined for enum type '{typeof(Ty).Name}'");
            }
		}



		#region Static Enum Tables
		internal static readonly EnumData[][] TABLE;
		internal static readonly List<Dictionary<AtomicString, dynamic>> KEYWORD;

		static DomEnumTables()
		{
			TABLE = new EnumData[12][];
			KEYWORD = new List<Dictionary<AtomicString, dynamic>>(12);
			int maxIndex = 0;

			#region Table Data 
			/* CssUI.DOM.CustomElements.EReactionName */
			#region CssUI.DOM.CustomElements.EReactionName
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.CustomElements.EReactionName)).Cast<CssUI.DOM.CustomElements.EReactionName>().Max();
			TABLE[0] = new EnumData[maxIndex+1];
			TABLE[0][(int)CssUI.DOM.CustomElements.EReactionName.AttributeChanged] = new EnumData("attributeChangedCallback");
			TABLE[0][(int)CssUI.DOM.CustomElements.EReactionName.Connected] = new EnumData("connectedCallback");
			TABLE[0][(int)CssUI.DOM.CustomElements.EReactionName.Disconnected] = new EnumData("disconnectedCallback");
			TABLE[0][(int)CssUI.DOM.CustomElements.EReactionName.Adopted] = new EnumData("adoptedCallback");
			TABLE[0][(int)CssUI.DOM.CustomElements.EReactionName.FormAssociated] = new EnumData("formAssociatedCallback");
			TABLE[0][(int)CssUI.DOM.CustomElements.EReactionName.FormDisabled] = new EnumData("formDisabledCallback");
			TABLE[0][(int)CssUI.DOM.CustomElements.EReactionName.FormReset] = new EnumData("formResetCallback");
			TABLE[0][(int)CssUI.DOM.CustomElements.EReactionName.FormStateRestore] = new EnumData("formStateRestoreCallback");
			#endregion
			

			/* CssUI.DOM.EAutoCapitalizationHint */
			#region CssUI.DOM.EAutoCapitalizationHint
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EAutoCapitalizationHint)).Cast<CssUI.DOM.EAutoCapitalizationHint>().Max();
			TABLE[1] = new EnumData[maxIndex+1];
			TABLE[1][(int)CssUI.DOM.EAutoCapitalizationHint.Default] = new EnumData("");
			TABLE[1][(int)CssUI.DOM.EAutoCapitalizationHint.Off] = new EnumData("off");
			TABLE[1][(int)CssUI.DOM.EAutoCapitalizationHint.None] = new EnumData("none");
			TABLE[1][(int)CssUI.DOM.EAutoCapitalizationHint.On] = new EnumData("on");
			TABLE[1][(int)CssUI.DOM.EAutoCapitalizationHint.Sentences] = new EnumData("sentences");
			TABLE[1][(int)CssUI.DOM.EAutoCapitalizationHint.Words] = new EnumData("words");
			TABLE[1][(int)CssUI.DOM.EAutoCapitalizationHint.Characters] = new EnumData("characters");
			#endregion
			

			/* CssUI.DOM.ECellScope */
			#region CssUI.DOM.ECellScope
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.ECellScope)).Cast<CssUI.DOM.ECellScope>().Max();
			TABLE[2] = new EnumData[maxIndex+1];
			TABLE[2][(int)CssUI.DOM.ECellScope.Auto] = new EnumData("auto");
			TABLE[2][(int)CssUI.DOM.ECellScope.Row] = new EnumData("row");
			TABLE[2][(int)CssUI.DOM.ECellScope.Column] = new EnumData("col");
			TABLE[2][(int)CssUI.DOM.ECellScope.RowGroup] = new EnumData("rowgroup");
			TABLE[2][(int)CssUI.DOM.ECellScope.ColumnGroup] = new EnumData("colgroup");
			#endregion
			

			/* CssUI.DOM.EDraggable */
			#region CssUI.DOM.EDraggable
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EDraggable)).Cast<CssUI.DOM.EDraggable>().Max();
			TABLE[3] = new EnumData[maxIndex+1];
			TABLE[3][(int)CssUI.DOM.EDraggable.True] = new EnumData("true");
			TABLE[3][(int)CssUI.DOM.EDraggable.False] = new EnumData("false");
			#endregion
			

			/* CssUI.DOM.EEnterKeyHint */
			#region CssUI.DOM.EEnterKeyHint
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EEnterKeyHint)).Cast<CssUI.DOM.EEnterKeyHint>().Max();
			TABLE[4] = new EnumData[maxIndex+1];
			TABLE[4][(int)CssUI.DOM.EEnterKeyHint.Enter] = new EnumData("enter");
			TABLE[4][(int)CssUI.DOM.EEnterKeyHint.Done] = new EnumData("done");
			TABLE[4][(int)CssUI.DOM.EEnterKeyHint.Go] = new EnumData("go");
			TABLE[4][(int)CssUI.DOM.EEnterKeyHint.Next] = new EnumData("next");
			TABLE[4][(int)CssUI.DOM.EEnterKeyHint.Previous] = new EnumData("previous");
			TABLE[4][(int)CssUI.DOM.EEnterKeyHint.Search] = new EnumData("search");
			TABLE[4][(int)CssUI.DOM.EEnterKeyHint.Send] = new EnumData("send");
			#endregion
			

			/* CssUI.DOM.EInputMode */
			#region CssUI.DOM.EInputMode
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EInputMode)).Cast<CssUI.DOM.EInputMode>().Max();
			TABLE[5] = new EnumData[maxIndex+1];
			TABLE[5][(int)CssUI.DOM.EInputMode.None] = new EnumData("none");
			TABLE[5][(int)CssUI.DOM.EInputMode.Text] = new EnumData("text");
			TABLE[5][(int)CssUI.DOM.EInputMode.Tel] = new EnumData("tel");
			TABLE[5][(int)CssUI.DOM.EInputMode.Url] = new EnumData("url");
			TABLE[5][(int)CssUI.DOM.EInputMode.Email] = new EnumData("email");
			TABLE[5][(int)CssUI.DOM.EInputMode.Numeric] = new EnumData("numeric");
			TABLE[5][(int)CssUI.DOM.EInputMode.Decimal] = new EnumData("decimal");
			TABLE[5][(int)CssUI.DOM.EInputMode.Search] = new EnumData("search");
			#endregion
			

			/* CssUI.DOM.ESpellcheck */
			#region CssUI.DOM.ESpellcheck
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.ESpellcheck)).Cast<CssUI.DOM.ESpellcheck>().Max();
			TABLE[6] = new EnumData[maxIndex+1];
			TABLE[6][(int)CssUI.DOM.ESpellcheck.Default] = new EnumData("");
			TABLE[6][(int)CssUI.DOM.ESpellcheck.True] = new EnumData("true");
			TABLE[6][(int)CssUI.DOM.ESpellcheck.False] = new EnumData("false");
			#endregion
			

			/* CssUI.DOM.Enums.EElementTag */
			#region CssUI.DOM.Enums.EElementTag
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.Enums.EElementTag)).Cast<CssUI.DOM.Enums.EElementTag>().Max();
			TABLE[7] = new EnumData[maxIndex+1];
			TABLE[7][(int)CssUI.DOM.Enums.EElementTag.Div] = new EnumData("div");
			TABLE[7][(int)CssUI.DOM.Enums.EElementTag.Html] = new EnumData("html");
			TABLE[7][(int)CssUI.DOM.Enums.EElementTag.Body] = new EnumData("body");
			TABLE[7][(int)CssUI.DOM.Enums.EElementTag.Head] = new EnumData("head");
			TABLE[7][(int)CssUI.DOM.Enums.EElementTag.Template] = new EnumData("template");
			TABLE[7][(int)CssUI.DOM.Enums.EElementTag.Slot] = new EnumData("slot");
			#endregion
			

			/* CssUI.DOM.Enums.EQuirksMode */
			#region CssUI.DOM.Enums.EQuirksMode
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.Enums.EQuirksMode)).Cast<CssUI.DOM.Enums.EQuirksMode>().Max();
			TABLE[8] = new EnumData[maxIndex+1];
			TABLE[8][(int)CssUI.DOM.Enums.EQuirksMode.Quirks] = new EnumData("quirks");
			TABLE[8][(int)CssUI.DOM.Enums.EQuirksMode.NoQuirks] = new EnumData("no-quirks");
			TABLE[8][(int)CssUI.DOM.Enums.EQuirksMode.LimitedQuirks] = new EnumData("limited-quirks");
			#endregion
			

			/* CssUI.DOM.Enums.EScrollLogicalPosition */
			#region CssUI.DOM.Enums.EScrollLogicalPosition
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.Enums.EScrollLogicalPosition)).Cast<CssUI.DOM.Enums.EScrollLogicalPosition>().Max();
			TABLE[9] = new EnumData[maxIndex+1];
			TABLE[9][(int)CssUI.DOM.Enums.EScrollLogicalPosition.Start] = new EnumData("start");
			TABLE[9][(int)CssUI.DOM.Enums.EScrollLogicalPosition.Center] = new EnumData("center");
			TABLE[9][(int)CssUI.DOM.Enums.EScrollLogicalPosition.End] = new EnumData("end");
			TABLE[9][(int)CssUI.DOM.Enums.EScrollLogicalPosition.Nearest] = new EnumData("nearest");
			#endregion
			

			/* CssUI.DOM.Enums.EShadowRootMode */
			#region CssUI.DOM.Enums.EShadowRootMode
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.Enums.EShadowRootMode)).Cast<CssUI.DOM.Enums.EShadowRootMode>().Max();
			TABLE[10] = new EnumData[maxIndex+1];
			TABLE[10][(int)CssUI.DOM.Enums.EShadowRootMode.Open] = new EnumData("open");
			TABLE[10][(int)CssUI.DOM.Enums.EShadowRootMode.Closed] = new EnumData("closed");
			#endregion
			

		#endregion
		/* Generate Reverse lookup maps */
		#region Reverse Lookup
			/* CssUI.DOM.CustomElements.EReactionName */
			#region CssUI.DOM.CustomElements.EReactionName
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[0].Add(new AtomicString("attributeChangedCallback"), CssUI.DOM.CustomElements.EReactionName.AttributeChanged);
			KEYWORD[0].Add(new AtomicString("connectedCallback"), CssUI.DOM.CustomElements.EReactionName.Connected);
			KEYWORD[0].Add(new AtomicString("disconnectedCallback"), CssUI.DOM.CustomElements.EReactionName.Disconnected);
			KEYWORD[0].Add(new AtomicString("adoptedCallback"), CssUI.DOM.CustomElements.EReactionName.Adopted);
			KEYWORD[0].Add(new AtomicString("formAssociatedCallback"), CssUI.DOM.CustomElements.EReactionName.FormAssociated);
			KEYWORD[0].Add(new AtomicString("formDisabledCallback"), CssUI.DOM.CustomElements.EReactionName.FormDisabled);
			KEYWORD[0].Add(new AtomicString("formResetCallback"), CssUI.DOM.CustomElements.EReactionName.FormReset);
			KEYWORD[0].Add(new AtomicString("formStateRestoreCallback"), CssUI.DOM.CustomElements.EReactionName.FormStateRestore);
			#endregion
				

			/* CssUI.DOM.EAutoCapitalizationHint */
			#region CssUI.DOM.EAutoCapitalizationHint
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[1].Add(new AtomicString(""), CssUI.DOM.EAutoCapitalizationHint.Default);
			KEYWORD[1].Add(new AtomicString("off"), CssUI.DOM.EAutoCapitalizationHint.Off);
			KEYWORD[1].Add(new AtomicString("none"), CssUI.DOM.EAutoCapitalizationHint.None);
			KEYWORD[1].Add(new AtomicString("on"), CssUI.DOM.EAutoCapitalizationHint.On);
			KEYWORD[1].Add(new AtomicString("sentences"), CssUI.DOM.EAutoCapitalizationHint.Sentences);
			KEYWORD[1].Add(new AtomicString("words"), CssUI.DOM.EAutoCapitalizationHint.Words);
			KEYWORD[1].Add(new AtomicString("characters"), CssUI.DOM.EAutoCapitalizationHint.Characters);
			#endregion
				

			/* CssUI.DOM.ECellScope */
			#region CssUI.DOM.ECellScope
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[2].Add(new AtomicString("auto"), CssUI.DOM.ECellScope.Auto);
			KEYWORD[2].Add(new AtomicString("row"), CssUI.DOM.ECellScope.Row);
			KEYWORD[2].Add(new AtomicString("col"), CssUI.DOM.ECellScope.Column);
			KEYWORD[2].Add(new AtomicString("rowgroup"), CssUI.DOM.ECellScope.RowGroup);
			KEYWORD[2].Add(new AtomicString("colgroup"), CssUI.DOM.ECellScope.ColumnGroup);
			#endregion
				

			/* CssUI.DOM.EDraggable */
			#region CssUI.DOM.EDraggable
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[3].Add(new AtomicString("true"), CssUI.DOM.EDraggable.True);
			KEYWORD[3].Add(new AtomicString("false"), CssUI.DOM.EDraggable.False);
			#endregion
				

			/* CssUI.DOM.EEnterKeyHint */
			#region CssUI.DOM.EEnterKeyHint
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[4].Add(new AtomicString("enter"), CssUI.DOM.EEnterKeyHint.Enter);
			KEYWORD[4].Add(new AtomicString("done"), CssUI.DOM.EEnterKeyHint.Done);
			KEYWORD[4].Add(new AtomicString("go"), CssUI.DOM.EEnterKeyHint.Go);
			KEYWORD[4].Add(new AtomicString("next"), CssUI.DOM.EEnterKeyHint.Next);
			KEYWORD[4].Add(new AtomicString("previous"), CssUI.DOM.EEnterKeyHint.Previous);
			KEYWORD[4].Add(new AtomicString("search"), CssUI.DOM.EEnterKeyHint.Search);
			KEYWORD[4].Add(new AtomicString("send"), CssUI.DOM.EEnterKeyHint.Send);
			#endregion
				

			/* CssUI.DOM.EInputMode */
			#region CssUI.DOM.EInputMode
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[5].Add(new AtomicString("none"), CssUI.DOM.EInputMode.None);
			KEYWORD[5].Add(new AtomicString("text"), CssUI.DOM.EInputMode.Text);
			KEYWORD[5].Add(new AtomicString("tel"), CssUI.DOM.EInputMode.Tel);
			KEYWORD[5].Add(new AtomicString("url"), CssUI.DOM.EInputMode.Url);
			KEYWORD[5].Add(new AtomicString("email"), CssUI.DOM.EInputMode.Email);
			KEYWORD[5].Add(new AtomicString("numeric"), CssUI.DOM.EInputMode.Numeric);
			KEYWORD[5].Add(new AtomicString("decimal"), CssUI.DOM.EInputMode.Decimal);
			KEYWORD[5].Add(new AtomicString("search"), CssUI.DOM.EInputMode.Search);
			#endregion
				

			/* CssUI.DOM.ESpellcheck */
			#region CssUI.DOM.ESpellcheck
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[6].Add(new AtomicString(""), CssUI.DOM.ESpellcheck.Default);
			KEYWORD[6].Add(new AtomicString("true"), CssUI.DOM.ESpellcheck.True);
			KEYWORD[6].Add(new AtomicString("false"), CssUI.DOM.ESpellcheck.False);
			#endregion
				

			/* CssUI.DOM.Enums.EElementTag */
			#region CssUI.DOM.Enums.EElementTag
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[7].Add(new AtomicString("div"), CssUI.DOM.Enums.EElementTag.Div);
			KEYWORD[7].Add(new AtomicString("html"), CssUI.DOM.Enums.EElementTag.Html);
			KEYWORD[7].Add(new AtomicString("body"), CssUI.DOM.Enums.EElementTag.Body);
			KEYWORD[7].Add(new AtomicString("head"), CssUI.DOM.Enums.EElementTag.Head);
			KEYWORD[7].Add(new AtomicString("template"), CssUI.DOM.Enums.EElementTag.Template);
			KEYWORD[7].Add(new AtomicString("slot"), CssUI.DOM.Enums.EElementTag.Slot);
			#endregion
				

			/* CssUI.DOM.Enums.EQuirksMode */
			#region CssUI.DOM.Enums.EQuirksMode
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[8].Add(new AtomicString("quirks"), CssUI.DOM.Enums.EQuirksMode.Quirks);
			KEYWORD[8].Add(new AtomicString("no-quirks"), CssUI.DOM.Enums.EQuirksMode.NoQuirks);
			KEYWORD[8].Add(new AtomicString("limited-quirks"), CssUI.DOM.Enums.EQuirksMode.LimitedQuirks);
			#endregion
				

			/* CssUI.DOM.Enums.EScrollLogicalPosition */
			#region CssUI.DOM.Enums.EScrollLogicalPosition
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[9].Add(new AtomicString("start"), CssUI.DOM.Enums.EScrollLogicalPosition.Start);
			KEYWORD[9].Add(new AtomicString("center"), CssUI.DOM.Enums.EScrollLogicalPosition.Center);
			KEYWORD[9].Add(new AtomicString("end"), CssUI.DOM.Enums.EScrollLogicalPosition.End);
			KEYWORD[9].Add(new AtomicString("nearest"), CssUI.DOM.Enums.EScrollLogicalPosition.Nearest);
			#endregion
				

			/* CssUI.DOM.Enums.EShadowRootMode */
			#region CssUI.DOM.Enums.EShadowRootMode
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[10].Add(new AtomicString("open"), CssUI.DOM.Enums.EShadowRootMode.Open);
			KEYWORD[10].Add(new AtomicString("closed"), CssUI.DOM.Enums.EShadowRootMode.Closed);
			#endregion
				

		#endregion
		}
		#endregion
	}
}

