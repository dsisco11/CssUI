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
				case "EAttributeName": return 1;
				case "EAutoCapitalizationHint": return 2;
				case "ECellScope": return 3;
				case "EContentEditable": return 4;
				case "EDir": return 5;
				case "EDraggable": return 6;
				case "EEnterKeyHint": return 7;
				case "EInputMode": return 8;
				case "EInputType": return 9;
				case "ESpellcheck": return 10;
				case "EElementTag": return 11;
				case "EQuirksMode": return 12;
				case "EScrollLogicalPosition": return 13;
				case "EShadowRootMode": return 14;
				case "EEventName": return 15;
				case "EKeyboardCode": return 16;
				default: return -1;//throw new NotImplementedException($"No lookup-index has defined for enum type '{typeof(Ty).Name}'");
            }
		}



		#region Static Enum Tables
		internal static readonly EnumData[][] TABLE;
		internal static readonly List<Dictionary<AtomicString, dynamic>> KEYWORD;

		static DomEnumTables()
		{
			TABLE = new EnumData[18][];
			KEYWORD = new List<Dictionary<AtomicString, dynamic>>(18);
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
			

			/* CssUI.DOM.EAttributeName */
			#region CssUI.DOM.EAttributeName
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EAttributeName)).Cast<CssUI.DOM.EAttributeName>().Max();
			TABLE[1] = new EnumData[maxIndex+1];
			TABLE[1][(int)CssUI.DOM.EAttributeName.ID] = new EnumData("id");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Name] = new EnumData("name");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Class] = new EnumData("class");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Width] = new EnumData("width");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Height] = new EnumData("height");
			TABLE[1][(int)CssUI.DOM.EAttributeName.IS] = new EnumData("is");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Title] = new EnumData("title");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Lang] = new EnumData("lang");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Dir] = new EnumData("dir");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Translate] = new EnumData("translate");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Nonce] = new EnumData("nonce");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Text] = new EnumData("text");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Label] = new EnumData("label");
			TABLE[1][(int)CssUI.DOM.EAttributeName.AccessKey] = new EnumData("accesskey");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Spellcheck] = new EnumData("spellcheck");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Autocapitalize] = new EnumData("autocapitalize");
			TABLE[1][(int)CssUI.DOM.EAttributeName.ContentEditable] = new EnumData("contenteditable");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Draggable] = new EnumData("draggable");
			TABLE[1][(int)CssUI.DOM.EAttributeName.TabIndex] = new EnumData("tabindex");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Disabled] = new EnumData("disabled");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Hidden] = new EnumData("hidden");
			TABLE[1][(int)CssUI.DOM.EAttributeName.HREF] = new EnumData("href");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Type] = new EnumData("type");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Slot] = new EnumData("slot");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Media] = new EnumData("media");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Dropzone] = new EnumData("dropzone");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Alt] = new EnumData("alt");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Src] = new EnumData("src");
			TABLE[1][(int)CssUI.DOM.EAttributeName.SrcSet] = new EnumData("srcset");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Href] = new EnumData("href");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Sizes] = new EnumData("sizes");
			TABLE[1][(int)CssUI.DOM.EAttributeName.UseMap] = new EnumData("usemap");
			TABLE[1][(int)CssUI.DOM.EAttributeName.IsMap] = new EnumData("ismap");
			TABLE[1][(int)CssUI.DOM.EAttributeName.CrossOrigin] = new EnumData("crossorigin");
			TABLE[1][(int)CssUI.DOM.EAttributeName.ReferrerPolicy] = new EnumData("referrerpolicy");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Decoding] = new EnumData("decoding");
			TABLE[1][(int)CssUI.DOM.EAttributeName.For] = new EnumData("for");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Span] = new EnumData("span");
			TABLE[1][(int)CssUI.DOM.EAttributeName.ColSpan] = new EnumData("colspan");
			TABLE[1][(int)CssUI.DOM.EAttributeName.RowSpan] = new EnumData("rowspan");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Headers] = new EnumData("headers");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Scope] = new EnumData("scope");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Abbr] = new EnumData("abbr");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Checked] = new EnumData("checked");
			TABLE[1][(int)CssUI.DOM.EAttributeName.InputMode] = new EnumData("inputmode");
			TABLE[1][(int)CssUI.DOM.EAttributeName.EnterKeyHint] = new EnumData("enterkeyhint");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Autocomplete] = new EnumData("autocomplete");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Form] = new EnumData("form");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Value] = new EnumData("value");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Selected] = new EnumData("selected");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Required] = new EnumData("required");
			TABLE[1][(int)CssUI.DOM.EAttributeName.Dirname] = new EnumData("dirname");
			#endregion
			

			/* CssUI.DOM.EAutoCapitalizationHint */
			#region CssUI.DOM.EAutoCapitalizationHint
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EAutoCapitalizationHint)).Cast<CssUI.DOM.EAutoCapitalizationHint>().Max();
			TABLE[2] = new EnumData[maxIndex+1];
			TABLE[2][(int)CssUI.DOM.EAutoCapitalizationHint.Default] = new EnumData("");
			TABLE[2][(int)CssUI.DOM.EAutoCapitalizationHint.Off] = new EnumData("off");
			TABLE[2][(int)CssUI.DOM.EAutoCapitalizationHint.None] = new EnumData("none");
			TABLE[2][(int)CssUI.DOM.EAutoCapitalizationHint.On] = new EnumData("on");
			TABLE[2][(int)CssUI.DOM.EAutoCapitalizationHint.Sentences] = new EnumData("sentences");
			TABLE[2][(int)CssUI.DOM.EAutoCapitalizationHint.Words] = new EnumData("words");
			TABLE[2][(int)CssUI.DOM.EAutoCapitalizationHint.Characters] = new EnumData("characters");
			#endregion
			

			/* CssUI.DOM.ECellScope */
			#region CssUI.DOM.ECellScope
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.ECellScope)).Cast<CssUI.DOM.ECellScope>().Max();
			TABLE[3] = new EnumData[maxIndex+1];
			TABLE[3][(int)CssUI.DOM.ECellScope.Auto] = new EnumData("auto");
			TABLE[3][(int)CssUI.DOM.ECellScope.Row] = new EnumData("row");
			TABLE[3][(int)CssUI.DOM.ECellScope.Column] = new EnumData("col");
			TABLE[3][(int)CssUI.DOM.ECellScope.RowGroup] = new EnumData("rowgroup");
			TABLE[3][(int)CssUI.DOM.ECellScope.ColumnGroup] = new EnumData("colgroup");
			#endregion
			

			/* CssUI.DOM.EContentEditable */
			#region CssUI.DOM.EContentEditable
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EContentEditable)).Cast<CssUI.DOM.EContentEditable>().Max();
			TABLE[4] = new EnumData[maxIndex+1];
			TABLE[4][(int)CssUI.DOM.EContentEditable.True] = new EnumData("true");
			TABLE[4][(int)CssUI.DOM.EContentEditable.False] = new EnumData("false");
			TABLE[4][(int)CssUI.DOM.EContentEditable.Inherit] = new EnumData("inherit");
			#endregion
			

			/* CssUI.DOM.EDir */
			#region CssUI.DOM.EDir
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EDir)).Cast<CssUI.DOM.EDir>().Max();
			TABLE[5] = new EnumData[maxIndex+1];
			TABLE[5][(int)CssUI.DOM.EDir.Ltr] = new EnumData("ltr");
			TABLE[5][(int)CssUI.DOM.EDir.Rtl] = new EnumData("rtl");
			TABLE[5][(int)CssUI.DOM.EDir.Auto] = new EnumData("auto");
			#endregion
			

			/* CssUI.DOM.EDraggable */
			#region CssUI.DOM.EDraggable
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EDraggable)).Cast<CssUI.DOM.EDraggable>().Max();
			TABLE[6] = new EnumData[maxIndex+1];
			TABLE[6][(int)CssUI.DOM.EDraggable.True] = new EnumData("true");
			TABLE[6][(int)CssUI.DOM.EDraggable.False] = new EnumData("false");
			#endregion
			

			/* CssUI.DOM.EEnterKeyHint */
			#region CssUI.DOM.EEnterKeyHint
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EEnterKeyHint)).Cast<CssUI.DOM.EEnterKeyHint>().Max();
			TABLE[7] = new EnumData[maxIndex+1];
			TABLE[7][(int)CssUI.DOM.EEnterKeyHint.Enter] = new EnumData("enter");
			TABLE[7][(int)CssUI.DOM.EEnterKeyHint.Done] = new EnumData("done");
			TABLE[7][(int)CssUI.DOM.EEnterKeyHint.Go] = new EnumData("go");
			TABLE[7][(int)CssUI.DOM.EEnterKeyHint.Next] = new EnumData("next");
			TABLE[7][(int)CssUI.DOM.EEnterKeyHint.Previous] = new EnumData("previous");
			TABLE[7][(int)CssUI.DOM.EEnterKeyHint.Search] = new EnumData("search");
			TABLE[7][(int)CssUI.DOM.EEnterKeyHint.Send] = new EnumData("send");
			#endregion
			

			/* CssUI.DOM.EInputMode */
			#region CssUI.DOM.EInputMode
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EInputMode)).Cast<CssUI.DOM.EInputMode>().Max();
			TABLE[8] = new EnumData[maxIndex+1];
			TABLE[8][(int)CssUI.DOM.EInputMode.None] = new EnumData("none");
			TABLE[8][(int)CssUI.DOM.EInputMode.Text] = new EnumData("text");
			TABLE[8][(int)CssUI.DOM.EInputMode.Tel] = new EnumData("tel");
			TABLE[8][(int)CssUI.DOM.EInputMode.Url] = new EnumData("url");
			TABLE[8][(int)CssUI.DOM.EInputMode.Email] = new EnumData("email");
			TABLE[8][(int)CssUI.DOM.EInputMode.Numeric] = new EnumData("numeric");
			TABLE[8][(int)CssUI.DOM.EInputMode.Decimal] = new EnumData("decimal");
			TABLE[8][(int)CssUI.DOM.EInputMode.Search] = new EnumData("search");
			#endregion
			

			/* CssUI.DOM.EInputType */
			#region CssUI.DOM.EInputType
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EInputType)).Cast<CssUI.DOM.EInputType>().Max();
			TABLE[9] = new EnumData[maxIndex+1];
			TABLE[9][(int)CssUI.DOM.EInputType.Hidden] = new EnumData("hidden");
			TABLE[9][(int)CssUI.DOM.EInputType.Text] = new EnumData("text");
			TABLE[9][(int)CssUI.DOM.EInputType.Search] = new EnumData("search");
			TABLE[9][(int)CssUI.DOM.EInputType.Telephone] = new EnumData("tel");
			TABLE[9][(int)CssUI.DOM.EInputType.Url] = new EnumData("url");
			TABLE[9][(int)CssUI.DOM.EInputType.Email] = new EnumData("email");
			TABLE[9][(int)CssUI.DOM.EInputType.Password] = new EnumData("password");
			TABLE[9][(int)CssUI.DOM.EInputType.Date] = new EnumData("date");
			TABLE[9][(int)CssUI.DOM.EInputType.Month] = new EnumData("month");
			TABLE[9][(int)CssUI.DOM.EInputType.Week] = new EnumData("week");
			TABLE[9][(int)CssUI.DOM.EInputType.Time] = new EnumData("time");
			TABLE[9][(int)CssUI.DOM.EInputType.Local] = new EnumData("datetime-local");
			TABLE[9][(int)CssUI.DOM.EInputType.Number] = new EnumData("number");
			TABLE[9][(int)CssUI.DOM.EInputType.Range] = new EnumData("range");
			TABLE[9][(int)CssUI.DOM.EInputType.Color] = new EnumData("color");
			TABLE[9][(int)CssUI.DOM.EInputType.Checkbox] = new EnumData("checkbox");
			TABLE[9][(int)CssUI.DOM.EInputType.Radio] = new EnumData("radio");
			TABLE[9][(int)CssUI.DOM.EInputType.File] = new EnumData("file");
			TABLE[9][(int)CssUI.DOM.EInputType.Submit] = new EnumData("submit");
			TABLE[9][(int)CssUI.DOM.EInputType.Image] = new EnumData("image");
			TABLE[9][(int)CssUI.DOM.EInputType.reset] = new EnumData("reset");
			TABLE[9][(int)CssUI.DOM.EInputType.button] = new EnumData("button");
			#endregion
			

			/* CssUI.DOM.ESpellcheck */
			#region CssUI.DOM.ESpellcheck
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.ESpellcheck)).Cast<CssUI.DOM.ESpellcheck>().Max();
			TABLE[10] = new EnumData[maxIndex+1];
			TABLE[10][(int)CssUI.DOM.ESpellcheck.Default] = new EnumData("");
			TABLE[10][(int)CssUI.DOM.ESpellcheck.True] = new EnumData("true");
			TABLE[10][(int)CssUI.DOM.ESpellcheck.False] = new EnumData("false");
			#endregion
			

			/* CssUI.DOM.Enums.EElementTag */
			#region CssUI.DOM.Enums.EElementTag
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.Enums.EElementTag)).Cast<CssUI.DOM.Enums.EElementTag>().Max();
			TABLE[11] = new EnumData[maxIndex+1];
			TABLE[11][(int)CssUI.DOM.Enums.EElementTag.Div] = new EnumData("div");
			TABLE[11][(int)CssUI.DOM.Enums.EElementTag.Html] = new EnumData("html");
			TABLE[11][(int)CssUI.DOM.Enums.EElementTag.Body] = new EnumData("body");
			TABLE[11][(int)CssUI.DOM.Enums.EElementTag.Head] = new EnumData("head");
			TABLE[11][(int)CssUI.DOM.Enums.EElementTag.Template] = new EnumData("template");
			TABLE[11][(int)CssUI.DOM.Enums.EElementTag.Slot] = new EnumData("slot");
			#endregion
			

			/* CssUI.DOM.Enums.EQuirksMode */
			#region CssUI.DOM.Enums.EQuirksMode
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.Enums.EQuirksMode)).Cast<CssUI.DOM.Enums.EQuirksMode>().Max();
			TABLE[12] = new EnumData[maxIndex+1];
			TABLE[12][(int)CssUI.DOM.Enums.EQuirksMode.Quirks] = new EnumData("quirks");
			TABLE[12][(int)CssUI.DOM.Enums.EQuirksMode.NoQuirks] = new EnumData("no-quirks");
			TABLE[12][(int)CssUI.DOM.Enums.EQuirksMode.LimitedQuirks] = new EnumData("limited-quirks");
			#endregion
			

			/* CssUI.DOM.Enums.EScrollLogicalPosition */
			#region CssUI.DOM.Enums.EScrollLogicalPosition
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.Enums.EScrollLogicalPosition)).Cast<CssUI.DOM.Enums.EScrollLogicalPosition>().Max();
			TABLE[13] = new EnumData[maxIndex+1];
			TABLE[13][(int)CssUI.DOM.Enums.EScrollLogicalPosition.Start] = new EnumData("start");
			TABLE[13][(int)CssUI.DOM.Enums.EScrollLogicalPosition.Center] = new EnumData("center");
			TABLE[13][(int)CssUI.DOM.Enums.EScrollLogicalPosition.End] = new EnumData("end");
			TABLE[13][(int)CssUI.DOM.Enums.EScrollLogicalPosition.Nearest] = new EnumData("nearest");
			#endregion
			

			/* CssUI.DOM.Enums.EShadowRootMode */
			#region CssUI.DOM.Enums.EShadowRootMode
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.Enums.EShadowRootMode)).Cast<CssUI.DOM.Enums.EShadowRootMode>().Max();
			TABLE[14] = new EnumData[maxIndex+1];
			TABLE[14][(int)CssUI.DOM.Enums.EShadowRootMode.Open] = new EnumData("open");
			TABLE[14][(int)CssUI.DOM.Enums.EShadowRootMode.Closed] = new EnumData("closed");
			#endregion
			

			/* CssUI.DOM.Events.EEventName */
			#region CssUI.DOM.Events.EEventName
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.Events.EEventName)).Cast<CssUI.DOM.Events.EEventName>().Max();
			TABLE[15] = new EnumData[maxIndex+1];
			TABLE[15][(int)CssUI.DOM.Events.EEventName.None] = new EnumData("");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Abort] = new EnumData("abort");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.AuxClick] = new EnumData("auxclick");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Blur] = new EnumData("blur");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Cancel] = new EnumData("cancel");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.CanPlay] = new EnumData("canplay");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.CanPlayThrough] = new EnumData("canplaythrough");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Change] = new EnumData("change");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Click] = new EnumData("click");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Close] = new EnumData("close");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.ContextMenu] = new EnumData("contextmenu");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.CueChange] = new EnumData("cuechange");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.DoubleClick] = new EnumData("dblclick");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Drag] = new EnumData("drag");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.DragEnd] = new EnumData("dragend");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.DragEnter] = new EnumData("dragenter");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.DragExit] = new EnumData("dragexit");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.DragLeave] = new EnumData("dragleave");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.DragOver] = new EnumData("dragover");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.DragStart] = new EnumData("dragstart");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Drop] = new EnumData("drop");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.DurationChange] = new EnumData("durationchange");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Emptied] = new EnumData("emptied");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Ended] = new EnumData("ended");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Error] = new EnumData("error");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Focus] = new EnumData("focus");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.FormData] = new EnumData("formdata");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Input] = new EnumData("input");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Invalid] = new EnumData("invalid");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.KeyDown] = new EnumData("keydown");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.KeyPress] = new EnumData("keypress");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.KeyUp] = new EnumData("keyup");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Load] = new EnumData("load");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.LoadedData] = new EnumData("loadeddata");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.LoadedMetadata] = new EnumData("loadedmetadata");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.LoadEnd] = new EnumData("loadend");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.LoadStart] = new EnumData("loadstart");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.MouseDown] = new EnumData("mousedown");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.MouseEnter] = new EnumData("mouseenter");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.MouseLeave] = new EnumData("mouseleave");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.MouseMove] = new EnumData("mousemove");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.MouseOut] = new EnumData("mouseout");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.MouseOver] = new EnumData("mouseover");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.MouseUp] = new EnumData("mouseup");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Wheel] = new EnumData("wheel");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Pause] = new EnumData("pause");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Play] = new EnumData("play");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Playing] = new EnumData("playing");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Progress] = new EnumData("progress");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.RateChange] = new EnumData("ratechange");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Reset] = new EnumData("reset");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Resize] = new EnumData("resize");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Scroll] = new EnumData("scroll");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.SecurityPolicyViolation] = new EnumData("securitypolicyviolation");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Seeked] = new EnumData("seeked");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Seeking] = new EnumData("seeking");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Select] = new EnumData("select");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Stalled] = new EnumData("stalled");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Submit] = new EnumData("submit");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Suspend] = new EnumData("suspend");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.TimeUpdate] = new EnumData("timeupdate");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Toggle] = new EnumData("toggle");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.VolumeChange] = new EnumData("volumechange");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Waiting] = new EnumData("waiting");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.SelectStart] = new EnumData("selectstart");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.SelectionChange] = new EnumData("selectionchange");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Copy] = new EnumData("copy");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Cut] = new EnumData("cut");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Paste] = new EnumData("paste");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.AfterPrint] = new EnumData("afterprint");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.BeforePrint] = new EnumData("beforeprint");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.BeforeUnload] = new EnumData("beforeunload");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.HashChange] = new EnumData("hashchange");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.LanguageChange] = new EnumData("languagechange");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Message] = new EnumData("message");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.MessageError] = new EnumData("messageerror");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Offline] = new EnumData("offline");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Online] = new EnumData("online");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.PageHide] = new EnumData("pagehide");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.PageShow] = new EnumData("pageshow");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.PopState] = new EnumData("popstate");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.RejectionHandled] = new EnumData("rejectionhandled");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Storage] = new EnumData("storage");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.UnhandledRejection] = new EnumData("unhandledrejection");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.Unload] = new EnumData("unload");
			TABLE[15][(int)CssUI.DOM.Events.EEventName.SlotChange] = new EnumData("slotchange");
			#endregion
			

			/* CssUI.DOM.Events.EKeyboardCode */
			#region CssUI.DOM.Events.EKeyboardCode
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.Events.EKeyboardCode)).Cast<CssUI.DOM.Events.EKeyboardCode>().Max();
			TABLE[16] = new EnumData[maxIndex+1];
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Invalid] = new EnumData("");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Backquote] = new EnumData("Backquote");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Backslash] = new EnumData("Backslash");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Backspace] = new EnumData("Backspace");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.BracketLeft] = new EnumData("BracketLeft");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.BracketRight] = new EnumData("BracketRight");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Comma] = new EnumData("Comma");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Digit0] = new EnumData("Digit0");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Digit1] = new EnumData("Digit1");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Digit2] = new EnumData("Digit2");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Digit3] = new EnumData("Digit3");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Digit4] = new EnumData("Digit4");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Digit5] = new EnumData("Digit5");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Digit6] = new EnumData("Digit6");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Digit7] = new EnumData("Digit7");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Digit8] = new EnumData("Digit8");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Digit9] = new EnumData("Digit9");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Equal] = new EnumData("Equal");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.IntlBackslash] = new EnumData("IntlBackslash");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.IntlRo] = new EnumData("IntlRo");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.IntlYen] = new EnumData("IntlYen");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyA] = new EnumData("KeyA");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyB] = new EnumData("KeyB");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyC] = new EnumData("KeyC");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyD] = new EnumData("KeyD");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyE] = new EnumData("KeyE");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyF] = new EnumData("KeyF");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyG] = new EnumData("KeyG");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyH] = new EnumData("KeyH");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyI] = new EnumData("KeyI");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyJ] = new EnumData("KeyJ");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyK] = new EnumData("KeyK");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyL] = new EnumData("KeyL");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyM] = new EnumData("KeyM");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyN] = new EnumData("KeyN");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyO] = new EnumData("KeyO");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyP] = new EnumData("KeyP");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyQ] = new EnumData("KeyQ");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyR] = new EnumData("KeyR");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyS] = new EnumData("KeyS");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyT] = new EnumData("KeyT");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyU] = new EnumData("KeyU");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyV] = new EnumData("KeyV");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyW] = new EnumData("KeyW");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyX] = new EnumData("KeyX");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyY] = new EnumData("KeyY");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KeyZ] = new EnumData("KeyZ");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Minus] = new EnumData("Minus");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Period] = new EnumData("Period");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Quote] = new EnumData("Quote");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Semicolon] = new EnumData("Semicolon");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Slash] = new EnumData("Slash");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.AltLeft] = new EnumData("AltLeft");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.AltRight] = new EnumData("AltRight");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.CapsLock] = new EnumData("CapsLock");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.ContextMenu] = new EnumData("ContextMenu");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.ControlLeft] = new EnumData("ControlLeft");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.ControlRight] = new EnumData("ControlRight");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Enter] = new EnumData("Enter");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.MetaLeft] = new EnumData("MetaLeft");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.MetaRight] = new EnumData("MetaRight");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.ShiftLeft] = new EnumData("ShiftLeft");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.ShiftRight] = new EnumData("ShiftRight");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Space] = new EnumData("Space");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Tab] = new EnumData("Tab");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Convert] = new EnumData("Convert");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.KanaMode] = new EnumData("KanaMode");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Lang1] = new EnumData("Lang1");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Lang2] = new EnumData("Lang2");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Lang3] = new EnumData("Lang3");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Lang4] = new EnumData("Lang4");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Lang5] = new EnumData("Lang5");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NonConvert] = new EnumData("NonConvert");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Delete] = new EnumData("Delete");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.End] = new EnumData("End");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Help] = new EnumData("Help");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Home] = new EnumData("Home");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Insert] = new EnumData("Insert");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.PageDown] = new EnumData("PageDown");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.PageUp] = new EnumData("PageUp");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.ArrowDown] = new EnumData("ArrowDown");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.ArrowLeft] = new EnumData("ArrowLeft");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.ArrowRight] = new EnumData("ArrowRight");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.ArrowUp] = new EnumData("ArrowUp");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NumLock] = new EnumData("NumLock");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Numpad0] = new EnumData("Numpad0");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Numpad1] = new EnumData("Numpad1");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Numpad2] = new EnumData("Numpad2");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Numpad3] = new EnumData("Numpad3");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Numpad4] = new EnumData("Numpad4");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Numpad5] = new EnumData("Numpad5");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Numpad6] = new EnumData("Numpad6");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Numpad7] = new EnumData("Numpad7");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Numpad8] = new EnumData("Numpad8");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Numpad9] = new EnumData("Numpad9");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NumpadAdd] = new EnumData("NumpadAdd");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NumpadBackspace] = new EnumData("NumpadBackspace");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NumpadClear] = new EnumData("NumpadClear");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NumpadClearEntry] = new EnumData("NumpadClearEntry");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NumpadComma] = new EnumData("NumpadComma");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NumpadDecimal] = new EnumData("NumpadDecimal");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NumpadDivide] = new EnumData("NumpadDivide");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.] = new EnumData("NumpadEnter");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NumpadEqual] = new EnumData("NumpadEqual");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NumpadHash] = new EnumData("NumpadHash");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NumpadMemoryAdd] = new EnumData("NumpadMemoryAdd");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NumpadMemoryClear] = new EnumData("NumpadMemoryClear");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NumpadMemoryRecall] = new EnumData("NumpadMemoryRecall");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NumpadMemoryStore] = new EnumData("NumpadMemoryStore");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NumpadMemorySubtract] = new EnumData("NumpadMemorySubtract");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NumpadMultiply] = new EnumData("NumpadMultiply");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NumpadParenLeft] = new EnumData("NumpadParenLeft");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NumpadParenRight] = new EnumData("NumpadParenRight");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NumpadStar] = new EnumData("NumpadStar");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.NumpadSubtract] = new EnumData("NumpadSubtract");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Escape] = new EnumData("Escape");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.F1] = new EnumData("F1");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.F2] = new EnumData("F2");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.F3] = new EnumData("F3");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.F4] = new EnumData("F4");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.F5] = new EnumData("F5");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.F6] = new EnumData("F6");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.F7] = new EnumData("F7");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.F8] = new EnumData("F8");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.F9] = new EnumData("F9");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.F10] = new EnumData("F10");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.F11] = new EnumData("F11");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.F12] = new EnumData("F12");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Fn] = new EnumData("Fn");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.FnLock] = new EnumData("FnLock");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.PrintScreen] = new EnumData("PrintScreen");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.ScrollLock] = new EnumData("ScrollLock");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Pause] = new EnumData("Pause");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.BrowserBack] = new EnumData("BrowserBack");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.BrowserFavorites] = new EnumData("BrowserFavorites");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.BrowserForward] = new EnumData("BrowserForward");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.BrowserHome] = new EnumData("BrowserHome");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.BrowserRefresh] = new EnumData("BrowserRefresh");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.BrowserSearch] = new EnumData("BrowserSearch");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.BrowserStop] = new EnumData("BrowserStop");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Eject] = new EnumData("Eject");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.LaunchApp1] = new EnumData("LaunchApp1");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.LaunchApp2] = new EnumData("LaunchApp2");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.LaunchMail] = new EnumData("LaunchMail");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.MediaPlayPause] = new EnumData("MediaPlayPause");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.MediaSelect] = new EnumData("MediaSelect");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.MediaStop] = new EnumData("MediaStop");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.MediaTrackNext] = new EnumData("MediaTrackNext");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.MediaTrackPrevious] = new EnumData("MediaTrackPrevious");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Power] = new EnumData("Power");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.Sleep] = new EnumData("Sleep");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.AudioVolumeDown] = new EnumData("AudioVolumeDown");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.AudioVolumeMute] = new EnumData("AudioVolumeMute");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.AudioVolumeUp] = new EnumData("AudioVolumeUp");
			TABLE[16][(int)CssUI.DOM.Events.EKeyboardCode.WakeUp] = new EnumData("WakeUp");
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
				

			/* CssUI.DOM.EAttributeName */
			#region CssUI.DOM.EAttributeName
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[1].Add(new AtomicString("id"), CssUI.DOM.EAttributeName.ID);
			KEYWORD[1].Add(new AtomicString("name"), CssUI.DOM.EAttributeName.Name);
			KEYWORD[1].Add(new AtomicString("class"), CssUI.DOM.EAttributeName.Class);
			KEYWORD[1].Add(new AtomicString("width"), CssUI.DOM.EAttributeName.Width);
			KEYWORD[1].Add(new AtomicString("height"), CssUI.DOM.EAttributeName.Height);
			KEYWORD[1].Add(new AtomicString("is"), CssUI.DOM.EAttributeName.IS);
			KEYWORD[1].Add(new AtomicString("title"), CssUI.DOM.EAttributeName.Title);
			KEYWORD[1].Add(new AtomicString("lang"), CssUI.DOM.EAttributeName.Lang);
			KEYWORD[1].Add(new AtomicString("dir"), CssUI.DOM.EAttributeName.Dir);
			KEYWORD[1].Add(new AtomicString("translate"), CssUI.DOM.EAttributeName.Translate);
			KEYWORD[1].Add(new AtomicString("nonce"), CssUI.DOM.EAttributeName.Nonce);
			KEYWORD[1].Add(new AtomicString("text"), CssUI.DOM.EAttributeName.Text);
			KEYWORD[1].Add(new AtomicString("label"), CssUI.DOM.EAttributeName.Label);
			KEYWORD[1].Add(new AtomicString("accesskey"), CssUI.DOM.EAttributeName.AccessKey);
			KEYWORD[1].Add(new AtomicString("spellcheck"), CssUI.DOM.EAttributeName.Spellcheck);
			KEYWORD[1].Add(new AtomicString("autocapitalize"), CssUI.DOM.EAttributeName.Autocapitalize);
			KEYWORD[1].Add(new AtomicString("contenteditable"), CssUI.DOM.EAttributeName.ContentEditable);
			KEYWORD[1].Add(new AtomicString("draggable"), CssUI.DOM.EAttributeName.Draggable);
			KEYWORD[1].Add(new AtomicString("tabindex"), CssUI.DOM.EAttributeName.TabIndex);
			KEYWORD[1].Add(new AtomicString("disabled"), CssUI.DOM.EAttributeName.Disabled);
			KEYWORD[1].Add(new AtomicString("hidden"), CssUI.DOM.EAttributeName.Hidden);
			KEYWORD[1].Add(new AtomicString("href"), CssUI.DOM.EAttributeName.HREF);
			KEYWORD[1].Add(new AtomicString("type"), CssUI.DOM.EAttributeName.Type);
			KEYWORD[1].Add(new AtomicString("slot"), CssUI.DOM.EAttributeName.Slot);
			KEYWORD[1].Add(new AtomicString("media"), CssUI.DOM.EAttributeName.Media);
			KEYWORD[1].Add(new AtomicString("dropzone"), CssUI.DOM.EAttributeName.Dropzone);
			KEYWORD[1].Add(new AtomicString("alt"), CssUI.DOM.EAttributeName.Alt);
			KEYWORD[1].Add(new AtomicString("src"), CssUI.DOM.EAttributeName.Src);
			KEYWORD[1].Add(new AtomicString("srcset"), CssUI.DOM.EAttributeName.SrcSet);
			KEYWORD[1].Add(new AtomicString("href"), CssUI.DOM.EAttributeName.Href);
			KEYWORD[1].Add(new AtomicString("sizes"), CssUI.DOM.EAttributeName.Sizes);
			KEYWORD[1].Add(new AtomicString("usemap"), CssUI.DOM.EAttributeName.UseMap);
			KEYWORD[1].Add(new AtomicString("ismap"), CssUI.DOM.EAttributeName.IsMap);
			KEYWORD[1].Add(new AtomicString("crossorigin"), CssUI.DOM.EAttributeName.CrossOrigin);
			KEYWORD[1].Add(new AtomicString("referrerpolicy"), CssUI.DOM.EAttributeName.ReferrerPolicy);
			KEYWORD[1].Add(new AtomicString("decoding"), CssUI.DOM.EAttributeName.Decoding);
			KEYWORD[1].Add(new AtomicString("for"), CssUI.DOM.EAttributeName.For);
			KEYWORD[1].Add(new AtomicString("span"), CssUI.DOM.EAttributeName.Span);
			KEYWORD[1].Add(new AtomicString("colspan"), CssUI.DOM.EAttributeName.ColSpan);
			KEYWORD[1].Add(new AtomicString("rowspan"), CssUI.DOM.EAttributeName.RowSpan);
			KEYWORD[1].Add(new AtomicString("headers"), CssUI.DOM.EAttributeName.Headers);
			KEYWORD[1].Add(new AtomicString("scope"), CssUI.DOM.EAttributeName.Scope);
			KEYWORD[1].Add(new AtomicString("abbr"), CssUI.DOM.EAttributeName.Abbr);
			KEYWORD[1].Add(new AtomicString("checked"), CssUI.DOM.EAttributeName.Checked);
			KEYWORD[1].Add(new AtomicString("inputmode"), CssUI.DOM.EAttributeName.InputMode);
			KEYWORD[1].Add(new AtomicString("enterkeyhint"), CssUI.DOM.EAttributeName.EnterKeyHint);
			KEYWORD[1].Add(new AtomicString("autocomplete"), CssUI.DOM.EAttributeName.Autocomplete);
			KEYWORD[1].Add(new AtomicString("form"), CssUI.DOM.EAttributeName.Form);
			KEYWORD[1].Add(new AtomicString("value"), CssUI.DOM.EAttributeName.Value);
			KEYWORD[1].Add(new AtomicString("selected"), CssUI.DOM.EAttributeName.Selected);
			KEYWORD[1].Add(new AtomicString("required"), CssUI.DOM.EAttributeName.Required);
			KEYWORD[1].Add(new AtomicString("dirname"), CssUI.DOM.EAttributeName.Dirname);
			#endregion
				

			/* CssUI.DOM.EAutoCapitalizationHint */
			#region CssUI.DOM.EAutoCapitalizationHint
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[2].Add(new AtomicString(""), CssUI.DOM.EAutoCapitalizationHint.Default);
			KEYWORD[2].Add(new AtomicString("off"), CssUI.DOM.EAutoCapitalizationHint.Off);
			KEYWORD[2].Add(new AtomicString("none"), CssUI.DOM.EAutoCapitalizationHint.None);
			KEYWORD[2].Add(new AtomicString("on"), CssUI.DOM.EAutoCapitalizationHint.On);
			KEYWORD[2].Add(new AtomicString("sentences"), CssUI.DOM.EAutoCapitalizationHint.Sentences);
			KEYWORD[2].Add(new AtomicString("words"), CssUI.DOM.EAutoCapitalizationHint.Words);
			KEYWORD[2].Add(new AtomicString("characters"), CssUI.DOM.EAutoCapitalizationHint.Characters);
			#endregion
				

			/* CssUI.DOM.ECellScope */
			#region CssUI.DOM.ECellScope
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[3].Add(new AtomicString("auto"), CssUI.DOM.ECellScope.Auto);
			KEYWORD[3].Add(new AtomicString("row"), CssUI.DOM.ECellScope.Row);
			KEYWORD[3].Add(new AtomicString("col"), CssUI.DOM.ECellScope.Column);
			KEYWORD[3].Add(new AtomicString("rowgroup"), CssUI.DOM.ECellScope.RowGroup);
			KEYWORD[3].Add(new AtomicString("colgroup"), CssUI.DOM.ECellScope.ColumnGroup);
			#endregion
				

			/* CssUI.DOM.EContentEditable */
			#region CssUI.DOM.EContentEditable
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[4].Add(new AtomicString("true"), CssUI.DOM.EContentEditable.True);
			KEYWORD[4].Add(new AtomicString("false"), CssUI.DOM.EContentEditable.False);
			KEYWORD[4].Add(new AtomicString("inherit"), CssUI.DOM.EContentEditable.Inherit);
			#endregion
				

			/* CssUI.DOM.EDir */
			#region CssUI.DOM.EDir
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[5].Add(new AtomicString("ltr"), CssUI.DOM.EDir.Ltr);
			KEYWORD[5].Add(new AtomicString("rtl"), CssUI.DOM.EDir.Rtl);
			KEYWORD[5].Add(new AtomicString("auto"), CssUI.DOM.EDir.Auto);
			#endregion
				

			/* CssUI.DOM.EDraggable */
			#region CssUI.DOM.EDraggable
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[6].Add(new AtomicString("true"), CssUI.DOM.EDraggable.True);
			KEYWORD[6].Add(new AtomicString("false"), CssUI.DOM.EDraggable.False);
			#endregion
				

			/* CssUI.DOM.EEnterKeyHint */
			#region CssUI.DOM.EEnterKeyHint
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[7].Add(new AtomicString("enter"), CssUI.DOM.EEnterKeyHint.Enter);
			KEYWORD[7].Add(new AtomicString("done"), CssUI.DOM.EEnterKeyHint.Done);
			KEYWORD[7].Add(new AtomicString("go"), CssUI.DOM.EEnterKeyHint.Go);
			KEYWORD[7].Add(new AtomicString("next"), CssUI.DOM.EEnterKeyHint.Next);
			KEYWORD[7].Add(new AtomicString("previous"), CssUI.DOM.EEnterKeyHint.Previous);
			KEYWORD[7].Add(new AtomicString("search"), CssUI.DOM.EEnterKeyHint.Search);
			KEYWORD[7].Add(new AtomicString("send"), CssUI.DOM.EEnterKeyHint.Send);
			#endregion
				

			/* CssUI.DOM.EInputMode */
			#region CssUI.DOM.EInputMode
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[8].Add(new AtomicString("none"), CssUI.DOM.EInputMode.None);
			KEYWORD[8].Add(new AtomicString("text"), CssUI.DOM.EInputMode.Text);
			KEYWORD[8].Add(new AtomicString("tel"), CssUI.DOM.EInputMode.Tel);
			KEYWORD[8].Add(new AtomicString("url"), CssUI.DOM.EInputMode.Url);
			KEYWORD[8].Add(new AtomicString("email"), CssUI.DOM.EInputMode.Email);
			KEYWORD[8].Add(new AtomicString("numeric"), CssUI.DOM.EInputMode.Numeric);
			KEYWORD[8].Add(new AtomicString("decimal"), CssUI.DOM.EInputMode.Decimal);
			KEYWORD[8].Add(new AtomicString("search"), CssUI.DOM.EInputMode.Search);
			#endregion
				

			/* CssUI.DOM.EInputType */
			#region CssUI.DOM.EInputType
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[9].Add(new AtomicString("hidden"), CssUI.DOM.EInputType.Hidden);
			KEYWORD[9].Add(new AtomicString("text"), CssUI.DOM.EInputType.Text);
			KEYWORD[9].Add(new AtomicString("search"), CssUI.DOM.EInputType.Search);
			KEYWORD[9].Add(new AtomicString("tel"), CssUI.DOM.EInputType.Telephone);
			KEYWORD[9].Add(new AtomicString("url"), CssUI.DOM.EInputType.Url);
			KEYWORD[9].Add(new AtomicString("email"), CssUI.DOM.EInputType.Email);
			KEYWORD[9].Add(new AtomicString("password"), CssUI.DOM.EInputType.Password);
			KEYWORD[9].Add(new AtomicString("date"), CssUI.DOM.EInputType.Date);
			KEYWORD[9].Add(new AtomicString("month"), CssUI.DOM.EInputType.Month);
			KEYWORD[9].Add(new AtomicString("week"), CssUI.DOM.EInputType.Week);
			KEYWORD[9].Add(new AtomicString("time"), CssUI.DOM.EInputType.Time);
			KEYWORD[9].Add(new AtomicString("datetime-local"), CssUI.DOM.EInputType.Local);
			KEYWORD[9].Add(new AtomicString("number"), CssUI.DOM.EInputType.Number);
			KEYWORD[9].Add(new AtomicString("range"), CssUI.DOM.EInputType.Range);
			KEYWORD[9].Add(new AtomicString("color"), CssUI.DOM.EInputType.Color);
			KEYWORD[9].Add(new AtomicString("checkbox"), CssUI.DOM.EInputType.Checkbox);
			KEYWORD[9].Add(new AtomicString("radio"), CssUI.DOM.EInputType.Radio);
			KEYWORD[9].Add(new AtomicString("file"), CssUI.DOM.EInputType.File);
			KEYWORD[9].Add(new AtomicString("submit"), CssUI.DOM.EInputType.Submit);
			KEYWORD[9].Add(new AtomicString("image"), CssUI.DOM.EInputType.Image);
			KEYWORD[9].Add(new AtomicString("reset"), CssUI.DOM.EInputType.reset);
			KEYWORD[9].Add(new AtomicString("button"), CssUI.DOM.EInputType.button);
			#endregion
				

			/* CssUI.DOM.ESpellcheck */
			#region CssUI.DOM.ESpellcheck
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[10].Add(new AtomicString(""), CssUI.DOM.ESpellcheck.Default);
			KEYWORD[10].Add(new AtomicString("true"), CssUI.DOM.ESpellcheck.True);
			KEYWORD[10].Add(new AtomicString("false"), CssUI.DOM.ESpellcheck.False);
			#endregion
				

			/* CssUI.DOM.Enums.EElementTag */
			#region CssUI.DOM.Enums.EElementTag
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[11].Add(new AtomicString("div"), CssUI.DOM.Enums.EElementTag.Div);
			KEYWORD[11].Add(new AtomicString("html"), CssUI.DOM.Enums.EElementTag.Html);
			KEYWORD[11].Add(new AtomicString("body"), CssUI.DOM.Enums.EElementTag.Body);
			KEYWORD[11].Add(new AtomicString("head"), CssUI.DOM.Enums.EElementTag.Head);
			KEYWORD[11].Add(new AtomicString("template"), CssUI.DOM.Enums.EElementTag.Template);
			KEYWORD[11].Add(new AtomicString("slot"), CssUI.DOM.Enums.EElementTag.Slot);
			#endregion
				

			/* CssUI.DOM.Enums.EQuirksMode */
			#region CssUI.DOM.Enums.EQuirksMode
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[12].Add(new AtomicString("quirks"), CssUI.DOM.Enums.EQuirksMode.Quirks);
			KEYWORD[12].Add(new AtomicString("no-quirks"), CssUI.DOM.Enums.EQuirksMode.NoQuirks);
			KEYWORD[12].Add(new AtomicString("limited-quirks"), CssUI.DOM.Enums.EQuirksMode.LimitedQuirks);
			#endregion
				

			/* CssUI.DOM.Enums.EScrollLogicalPosition */
			#region CssUI.DOM.Enums.EScrollLogicalPosition
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[13].Add(new AtomicString("start"), CssUI.DOM.Enums.EScrollLogicalPosition.Start);
			KEYWORD[13].Add(new AtomicString("center"), CssUI.DOM.Enums.EScrollLogicalPosition.Center);
			KEYWORD[13].Add(new AtomicString("end"), CssUI.DOM.Enums.EScrollLogicalPosition.End);
			KEYWORD[13].Add(new AtomicString("nearest"), CssUI.DOM.Enums.EScrollLogicalPosition.Nearest);
			#endregion
				

			/* CssUI.DOM.Enums.EShadowRootMode */
			#region CssUI.DOM.Enums.EShadowRootMode
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[14].Add(new AtomicString("open"), CssUI.DOM.Enums.EShadowRootMode.Open);
			KEYWORD[14].Add(new AtomicString("closed"), CssUI.DOM.Enums.EShadowRootMode.Closed);
			#endregion
				

			/* CssUI.DOM.Events.EEventName */
			#region CssUI.DOM.Events.EEventName
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[15].Add(new AtomicString(""), CssUI.DOM.Events.EEventName.None);
			KEYWORD[15].Add(new AtomicString("abort"), CssUI.DOM.Events.EEventName.Abort);
			KEYWORD[15].Add(new AtomicString("auxclick"), CssUI.DOM.Events.EEventName.AuxClick);
			KEYWORD[15].Add(new AtomicString("blur"), CssUI.DOM.Events.EEventName.Blur);
			KEYWORD[15].Add(new AtomicString("cancel"), CssUI.DOM.Events.EEventName.Cancel);
			KEYWORD[15].Add(new AtomicString("canplay"), CssUI.DOM.Events.EEventName.CanPlay);
			KEYWORD[15].Add(new AtomicString("canplaythrough"), CssUI.DOM.Events.EEventName.CanPlayThrough);
			KEYWORD[15].Add(new AtomicString("change"), CssUI.DOM.Events.EEventName.Change);
			KEYWORD[15].Add(new AtomicString("click"), CssUI.DOM.Events.EEventName.Click);
			KEYWORD[15].Add(new AtomicString("close"), CssUI.DOM.Events.EEventName.Close);
			KEYWORD[15].Add(new AtomicString("contextmenu"), CssUI.DOM.Events.EEventName.ContextMenu);
			KEYWORD[15].Add(new AtomicString("cuechange"), CssUI.DOM.Events.EEventName.CueChange);
			KEYWORD[15].Add(new AtomicString("dblclick"), CssUI.DOM.Events.EEventName.DoubleClick);
			KEYWORD[15].Add(new AtomicString("drag"), CssUI.DOM.Events.EEventName.Drag);
			KEYWORD[15].Add(new AtomicString("dragend"), CssUI.DOM.Events.EEventName.DragEnd);
			KEYWORD[15].Add(new AtomicString("dragenter"), CssUI.DOM.Events.EEventName.DragEnter);
			KEYWORD[15].Add(new AtomicString("dragexit"), CssUI.DOM.Events.EEventName.DragExit);
			KEYWORD[15].Add(new AtomicString("dragleave"), CssUI.DOM.Events.EEventName.DragLeave);
			KEYWORD[15].Add(new AtomicString("dragover"), CssUI.DOM.Events.EEventName.DragOver);
			KEYWORD[15].Add(new AtomicString("dragstart"), CssUI.DOM.Events.EEventName.DragStart);
			KEYWORD[15].Add(new AtomicString("drop"), CssUI.DOM.Events.EEventName.Drop);
			KEYWORD[15].Add(new AtomicString("durationchange"), CssUI.DOM.Events.EEventName.DurationChange);
			KEYWORD[15].Add(new AtomicString("emptied"), CssUI.DOM.Events.EEventName.Emptied);
			KEYWORD[15].Add(new AtomicString("ended"), CssUI.DOM.Events.EEventName.Ended);
			KEYWORD[15].Add(new AtomicString("error"), CssUI.DOM.Events.EEventName.Error);
			KEYWORD[15].Add(new AtomicString("focus"), CssUI.DOM.Events.EEventName.Focus);
			KEYWORD[15].Add(new AtomicString("formdata"), CssUI.DOM.Events.EEventName.FormData);
			KEYWORD[15].Add(new AtomicString("input"), CssUI.DOM.Events.EEventName.Input);
			KEYWORD[15].Add(new AtomicString("invalid"), CssUI.DOM.Events.EEventName.Invalid);
			KEYWORD[15].Add(new AtomicString("keydown"), CssUI.DOM.Events.EEventName.KeyDown);
			KEYWORD[15].Add(new AtomicString("keypress"), CssUI.DOM.Events.EEventName.KeyPress);
			KEYWORD[15].Add(new AtomicString("keyup"), CssUI.DOM.Events.EEventName.KeyUp);
			KEYWORD[15].Add(new AtomicString("load"), CssUI.DOM.Events.EEventName.Load);
			KEYWORD[15].Add(new AtomicString("loadeddata"), CssUI.DOM.Events.EEventName.LoadedData);
			KEYWORD[15].Add(new AtomicString("loadedmetadata"), CssUI.DOM.Events.EEventName.LoadedMetadata);
			KEYWORD[15].Add(new AtomicString("loadend"), CssUI.DOM.Events.EEventName.LoadEnd);
			KEYWORD[15].Add(new AtomicString("loadstart"), CssUI.DOM.Events.EEventName.LoadStart);
			KEYWORD[15].Add(new AtomicString("mousedown"), CssUI.DOM.Events.EEventName.MouseDown);
			KEYWORD[15].Add(new AtomicString("mouseenter"), CssUI.DOM.Events.EEventName.MouseEnter);
			KEYWORD[15].Add(new AtomicString("mouseleave"), CssUI.DOM.Events.EEventName.MouseLeave);
			KEYWORD[15].Add(new AtomicString("mousemove"), CssUI.DOM.Events.EEventName.MouseMove);
			KEYWORD[15].Add(new AtomicString("mouseout"), CssUI.DOM.Events.EEventName.MouseOut);
			KEYWORD[15].Add(new AtomicString("mouseover"), CssUI.DOM.Events.EEventName.MouseOver);
			KEYWORD[15].Add(new AtomicString("mouseup"), CssUI.DOM.Events.EEventName.MouseUp);
			KEYWORD[15].Add(new AtomicString("wheel"), CssUI.DOM.Events.EEventName.Wheel);
			KEYWORD[15].Add(new AtomicString("pause"), CssUI.DOM.Events.EEventName.Pause);
			KEYWORD[15].Add(new AtomicString("play"), CssUI.DOM.Events.EEventName.Play);
			KEYWORD[15].Add(new AtomicString("playing"), CssUI.DOM.Events.EEventName.Playing);
			KEYWORD[15].Add(new AtomicString("progress"), CssUI.DOM.Events.EEventName.Progress);
			KEYWORD[15].Add(new AtomicString("ratechange"), CssUI.DOM.Events.EEventName.RateChange);
			KEYWORD[15].Add(new AtomicString("reset"), CssUI.DOM.Events.EEventName.Reset);
			KEYWORD[15].Add(new AtomicString("resize"), CssUI.DOM.Events.EEventName.Resize);
			KEYWORD[15].Add(new AtomicString("scroll"), CssUI.DOM.Events.EEventName.Scroll);
			KEYWORD[15].Add(new AtomicString("securitypolicyviolation"), CssUI.DOM.Events.EEventName.SecurityPolicyViolation);
			KEYWORD[15].Add(new AtomicString("seeked"), CssUI.DOM.Events.EEventName.Seeked);
			KEYWORD[15].Add(new AtomicString("seeking"), CssUI.DOM.Events.EEventName.Seeking);
			KEYWORD[15].Add(new AtomicString("select"), CssUI.DOM.Events.EEventName.Select);
			KEYWORD[15].Add(new AtomicString("stalled"), CssUI.DOM.Events.EEventName.Stalled);
			KEYWORD[15].Add(new AtomicString("submit"), CssUI.DOM.Events.EEventName.Submit);
			KEYWORD[15].Add(new AtomicString("suspend"), CssUI.DOM.Events.EEventName.Suspend);
			KEYWORD[15].Add(new AtomicString("timeupdate"), CssUI.DOM.Events.EEventName.TimeUpdate);
			KEYWORD[15].Add(new AtomicString("toggle"), CssUI.DOM.Events.EEventName.Toggle);
			KEYWORD[15].Add(new AtomicString("volumechange"), CssUI.DOM.Events.EEventName.VolumeChange);
			KEYWORD[15].Add(new AtomicString("waiting"), CssUI.DOM.Events.EEventName.Waiting);
			KEYWORD[15].Add(new AtomicString("selectstart"), CssUI.DOM.Events.EEventName.SelectStart);
			KEYWORD[15].Add(new AtomicString("selectionchange"), CssUI.DOM.Events.EEventName.SelectionChange);
			KEYWORD[15].Add(new AtomicString("copy"), CssUI.DOM.Events.EEventName.Copy);
			KEYWORD[15].Add(new AtomicString("cut"), CssUI.DOM.Events.EEventName.Cut);
			KEYWORD[15].Add(new AtomicString("paste"), CssUI.DOM.Events.EEventName.Paste);
			KEYWORD[15].Add(new AtomicString("afterprint"), CssUI.DOM.Events.EEventName.AfterPrint);
			KEYWORD[15].Add(new AtomicString("beforeprint"), CssUI.DOM.Events.EEventName.BeforePrint);
			KEYWORD[15].Add(new AtomicString("beforeunload"), CssUI.DOM.Events.EEventName.BeforeUnload);
			KEYWORD[15].Add(new AtomicString("hashchange"), CssUI.DOM.Events.EEventName.HashChange);
			KEYWORD[15].Add(new AtomicString("languagechange"), CssUI.DOM.Events.EEventName.LanguageChange);
			KEYWORD[15].Add(new AtomicString("message"), CssUI.DOM.Events.EEventName.Message);
			KEYWORD[15].Add(new AtomicString("messageerror"), CssUI.DOM.Events.EEventName.MessageError);
			KEYWORD[15].Add(new AtomicString("offline"), CssUI.DOM.Events.EEventName.Offline);
			KEYWORD[15].Add(new AtomicString("online"), CssUI.DOM.Events.EEventName.Online);
			KEYWORD[15].Add(new AtomicString("pagehide"), CssUI.DOM.Events.EEventName.PageHide);
			KEYWORD[15].Add(new AtomicString("pageshow"), CssUI.DOM.Events.EEventName.PageShow);
			KEYWORD[15].Add(new AtomicString("popstate"), CssUI.DOM.Events.EEventName.PopState);
			KEYWORD[15].Add(new AtomicString("rejectionhandled"), CssUI.DOM.Events.EEventName.RejectionHandled);
			KEYWORD[15].Add(new AtomicString("storage"), CssUI.DOM.Events.EEventName.Storage);
			KEYWORD[15].Add(new AtomicString("unhandledrejection"), CssUI.DOM.Events.EEventName.UnhandledRejection);
			KEYWORD[15].Add(new AtomicString("unload"), CssUI.DOM.Events.EEventName.Unload);
			KEYWORD[15].Add(new AtomicString("slotchange"), CssUI.DOM.Events.EEventName.SlotChange);
			#endregion
				

			/* CssUI.DOM.Events.EKeyboardCode */
			#region CssUI.DOM.Events.EKeyboardCode
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[16].Add(new AtomicString(""), CssUI.DOM.Events.EKeyboardCode.Invalid);
			KEYWORD[16].Add(new AtomicString("Backquote"), CssUI.DOM.Events.EKeyboardCode.Backquote);
			KEYWORD[16].Add(new AtomicString("Backslash"), CssUI.DOM.Events.EKeyboardCode.Backslash);
			KEYWORD[16].Add(new AtomicString("Backspace"), CssUI.DOM.Events.EKeyboardCode.Backspace);
			KEYWORD[16].Add(new AtomicString("BracketLeft"), CssUI.DOM.Events.EKeyboardCode.BracketLeft);
			KEYWORD[16].Add(new AtomicString("BracketRight"), CssUI.DOM.Events.EKeyboardCode.BracketRight);
			KEYWORD[16].Add(new AtomicString("Comma"), CssUI.DOM.Events.EKeyboardCode.Comma);
			KEYWORD[16].Add(new AtomicString("Digit0"), CssUI.DOM.Events.EKeyboardCode.Digit0);
			KEYWORD[16].Add(new AtomicString("Digit1"), CssUI.DOM.Events.EKeyboardCode.Digit1);
			KEYWORD[16].Add(new AtomicString("Digit2"), CssUI.DOM.Events.EKeyboardCode.Digit2);
			KEYWORD[16].Add(new AtomicString("Digit3"), CssUI.DOM.Events.EKeyboardCode.Digit3);
			KEYWORD[16].Add(new AtomicString("Digit4"), CssUI.DOM.Events.EKeyboardCode.Digit4);
			KEYWORD[16].Add(new AtomicString("Digit5"), CssUI.DOM.Events.EKeyboardCode.Digit5);
			KEYWORD[16].Add(new AtomicString("Digit6"), CssUI.DOM.Events.EKeyboardCode.Digit6);
			KEYWORD[16].Add(new AtomicString("Digit7"), CssUI.DOM.Events.EKeyboardCode.Digit7);
			KEYWORD[16].Add(new AtomicString("Digit8"), CssUI.DOM.Events.EKeyboardCode.Digit8);
			KEYWORD[16].Add(new AtomicString("Digit9"), CssUI.DOM.Events.EKeyboardCode.Digit9);
			KEYWORD[16].Add(new AtomicString("Equal"), CssUI.DOM.Events.EKeyboardCode.Equal);
			KEYWORD[16].Add(new AtomicString("IntlBackslash"), CssUI.DOM.Events.EKeyboardCode.IntlBackslash);
			KEYWORD[16].Add(new AtomicString("IntlRo"), CssUI.DOM.Events.EKeyboardCode.IntlRo);
			KEYWORD[16].Add(new AtomicString("IntlYen"), CssUI.DOM.Events.EKeyboardCode.IntlYen);
			KEYWORD[16].Add(new AtomicString("KeyA"), CssUI.DOM.Events.EKeyboardCode.KeyA);
			KEYWORD[16].Add(new AtomicString("KeyB"), CssUI.DOM.Events.EKeyboardCode.KeyB);
			KEYWORD[16].Add(new AtomicString("KeyC"), CssUI.DOM.Events.EKeyboardCode.KeyC);
			KEYWORD[16].Add(new AtomicString("KeyD"), CssUI.DOM.Events.EKeyboardCode.KeyD);
			KEYWORD[16].Add(new AtomicString("KeyE"), CssUI.DOM.Events.EKeyboardCode.KeyE);
			KEYWORD[16].Add(new AtomicString("KeyF"), CssUI.DOM.Events.EKeyboardCode.KeyF);
			KEYWORD[16].Add(new AtomicString("KeyG"), CssUI.DOM.Events.EKeyboardCode.KeyG);
			KEYWORD[16].Add(new AtomicString("KeyH"), CssUI.DOM.Events.EKeyboardCode.KeyH);
			KEYWORD[16].Add(new AtomicString("KeyI"), CssUI.DOM.Events.EKeyboardCode.KeyI);
			KEYWORD[16].Add(new AtomicString("KeyJ"), CssUI.DOM.Events.EKeyboardCode.KeyJ);
			KEYWORD[16].Add(new AtomicString("KeyK"), CssUI.DOM.Events.EKeyboardCode.KeyK);
			KEYWORD[16].Add(new AtomicString("KeyL"), CssUI.DOM.Events.EKeyboardCode.KeyL);
			KEYWORD[16].Add(new AtomicString("KeyM"), CssUI.DOM.Events.EKeyboardCode.KeyM);
			KEYWORD[16].Add(new AtomicString("KeyN"), CssUI.DOM.Events.EKeyboardCode.KeyN);
			KEYWORD[16].Add(new AtomicString("KeyO"), CssUI.DOM.Events.EKeyboardCode.KeyO);
			KEYWORD[16].Add(new AtomicString("KeyP"), CssUI.DOM.Events.EKeyboardCode.KeyP);
			KEYWORD[16].Add(new AtomicString("KeyQ"), CssUI.DOM.Events.EKeyboardCode.KeyQ);
			KEYWORD[16].Add(new AtomicString("KeyR"), CssUI.DOM.Events.EKeyboardCode.KeyR);
			KEYWORD[16].Add(new AtomicString("KeyS"), CssUI.DOM.Events.EKeyboardCode.KeyS);
			KEYWORD[16].Add(new AtomicString("KeyT"), CssUI.DOM.Events.EKeyboardCode.KeyT);
			KEYWORD[16].Add(new AtomicString("KeyU"), CssUI.DOM.Events.EKeyboardCode.KeyU);
			KEYWORD[16].Add(new AtomicString("KeyV"), CssUI.DOM.Events.EKeyboardCode.KeyV);
			KEYWORD[16].Add(new AtomicString("KeyW"), CssUI.DOM.Events.EKeyboardCode.KeyW);
			KEYWORD[16].Add(new AtomicString("KeyX"), CssUI.DOM.Events.EKeyboardCode.KeyX);
			KEYWORD[16].Add(new AtomicString("KeyY"), CssUI.DOM.Events.EKeyboardCode.KeyY);
			KEYWORD[16].Add(new AtomicString("KeyZ"), CssUI.DOM.Events.EKeyboardCode.KeyZ);
			KEYWORD[16].Add(new AtomicString("Minus"), CssUI.DOM.Events.EKeyboardCode.Minus);
			KEYWORD[16].Add(new AtomicString("Period"), CssUI.DOM.Events.EKeyboardCode.Period);
			KEYWORD[16].Add(new AtomicString("Quote"), CssUI.DOM.Events.EKeyboardCode.Quote);
			KEYWORD[16].Add(new AtomicString("Semicolon"), CssUI.DOM.Events.EKeyboardCode.Semicolon);
			KEYWORD[16].Add(new AtomicString("Slash"), CssUI.DOM.Events.EKeyboardCode.Slash);
			KEYWORD[16].Add(new AtomicString("AltLeft"), CssUI.DOM.Events.EKeyboardCode.AltLeft);
			KEYWORD[16].Add(new AtomicString("AltRight"), CssUI.DOM.Events.EKeyboardCode.AltRight);
			KEYWORD[16].Add(new AtomicString("CapsLock"), CssUI.DOM.Events.EKeyboardCode.CapsLock);
			KEYWORD[16].Add(new AtomicString("ContextMenu"), CssUI.DOM.Events.EKeyboardCode.ContextMenu);
			KEYWORD[16].Add(new AtomicString("ControlLeft"), CssUI.DOM.Events.EKeyboardCode.ControlLeft);
			KEYWORD[16].Add(new AtomicString("ControlRight"), CssUI.DOM.Events.EKeyboardCode.ControlRight);
			KEYWORD[16].Add(new AtomicString("Enter"), CssUI.DOM.Events.EKeyboardCode.Enter);
			KEYWORD[16].Add(new AtomicString("MetaLeft"), CssUI.DOM.Events.EKeyboardCode.MetaLeft);
			KEYWORD[16].Add(new AtomicString("MetaRight"), CssUI.DOM.Events.EKeyboardCode.MetaRight);
			KEYWORD[16].Add(new AtomicString("ShiftLeft"), CssUI.DOM.Events.EKeyboardCode.ShiftLeft);
			KEYWORD[16].Add(new AtomicString("ShiftRight"), CssUI.DOM.Events.EKeyboardCode.ShiftRight);
			KEYWORD[16].Add(new AtomicString("Space"), CssUI.DOM.Events.EKeyboardCode.Space);
			KEYWORD[16].Add(new AtomicString("Tab"), CssUI.DOM.Events.EKeyboardCode.Tab);
			KEYWORD[16].Add(new AtomicString("Convert"), CssUI.DOM.Events.EKeyboardCode.Convert);
			KEYWORD[16].Add(new AtomicString("KanaMode"), CssUI.DOM.Events.EKeyboardCode.KanaMode);
			KEYWORD[16].Add(new AtomicString("Lang1"), CssUI.DOM.Events.EKeyboardCode.Lang1);
			KEYWORD[16].Add(new AtomicString("Lang2"), CssUI.DOM.Events.EKeyboardCode.Lang2);
			KEYWORD[16].Add(new AtomicString("Lang3"), CssUI.DOM.Events.EKeyboardCode.Lang3);
			KEYWORD[16].Add(new AtomicString("Lang4"), CssUI.DOM.Events.EKeyboardCode.Lang4);
			KEYWORD[16].Add(new AtomicString("Lang5"), CssUI.DOM.Events.EKeyboardCode.Lang5);
			KEYWORD[16].Add(new AtomicString("NonConvert"), CssUI.DOM.Events.EKeyboardCode.NonConvert);
			KEYWORD[16].Add(new AtomicString("Delete"), CssUI.DOM.Events.EKeyboardCode.Delete);
			KEYWORD[16].Add(new AtomicString("End"), CssUI.DOM.Events.EKeyboardCode.End);
			KEYWORD[16].Add(new AtomicString("Help"), CssUI.DOM.Events.EKeyboardCode.Help);
			KEYWORD[16].Add(new AtomicString("Home"), CssUI.DOM.Events.EKeyboardCode.Home);
			KEYWORD[16].Add(new AtomicString("Insert"), CssUI.DOM.Events.EKeyboardCode.Insert);
			KEYWORD[16].Add(new AtomicString("PageDown"), CssUI.DOM.Events.EKeyboardCode.PageDown);
			KEYWORD[16].Add(new AtomicString("PageUp"), CssUI.DOM.Events.EKeyboardCode.PageUp);
			KEYWORD[16].Add(new AtomicString("ArrowDown"), CssUI.DOM.Events.EKeyboardCode.ArrowDown);
			KEYWORD[16].Add(new AtomicString("ArrowLeft"), CssUI.DOM.Events.EKeyboardCode.ArrowLeft);
			KEYWORD[16].Add(new AtomicString("ArrowRight"), CssUI.DOM.Events.EKeyboardCode.ArrowRight);
			KEYWORD[16].Add(new AtomicString("ArrowUp"), CssUI.DOM.Events.EKeyboardCode.ArrowUp);
			KEYWORD[16].Add(new AtomicString("NumLock"), CssUI.DOM.Events.EKeyboardCode.NumLock);
			KEYWORD[16].Add(new AtomicString("Numpad0"), CssUI.DOM.Events.EKeyboardCode.Numpad0);
			KEYWORD[16].Add(new AtomicString("Numpad1"), CssUI.DOM.Events.EKeyboardCode.Numpad1);
			KEYWORD[16].Add(new AtomicString("Numpad2"), CssUI.DOM.Events.EKeyboardCode.Numpad2);
			KEYWORD[16].Add(new AtomicString("Numpad3"), CssUI.DOM.Events.EKeyboardCode.Numpad3);
			KEYWORD[16].Add(new AtomicString("Numpad4"), CssUI.DOM.Events.EKeyboardCode.Numpad4);
			KEYWORD[16].Add(new AtomicString("Numpad5"), CssUI.DOM.Events.EKeyboardCode.Numpad5);
			KEYWORD[16].Add(new AtomicString("Numpad6"), CssUI.DOM.Events.EKeyboardCode.Numpad6);
			KEYWORD[16].Add(new AtomicString("Numpad7"), CssUI.DOM.Events.EKeyboardCode.Numpad7);
			KEYWORD[16].Add(new AtomicString("Numpad8"), CssUI.DOM.Events.EKeyboardCode.Numpad8);
			KEYWORD[16].Add(new AtomicString("Numpad9"), CssUI.DOM.Events.EKeyboardCode.Numpad9);
			KEYWORD[16].Add(new AtomicString("NumpadAdd"), CssUI.DOM.Events.EKeyboardCode.NumpadAdd);
			KEYWORD[16].Add(new AtomicString("NumpadBackspace"), CssUI.DOM.Events.EKeyboardCode.NumpadBackspace);
			KEYWORD[16].Add(new AtomicString("NumpadClear"), CssUI.DOM.Events.EKeyboardCode.NumpadClear);
			KEYWORD[16].Add(new AtomicString("NumpadClearEntry"), CssUI.DOM.Events.EKeyboardCode.NumpadClearEntry);
			KEYWORD[16].Add(new AtomicString("NumpadComma"), CssUI.DOM.Events.EKeyboardCode.NumpadComma);
			KEYWORD[16].Add(new AtomicString("NumpadDecimal"), CssUI.DOM.Events.EKeyboardCode.NumpadDecimal);
			KEYWORD[16].Add(new AtomicString("NumpadDivide"), CssUI.DOM.Events.EKeyboardCode.NumpadDivide);
			KEYWORD[16].Add(new AtomicString("NumpadEnter"), CssUI.DOM.Events.EKeyboardCode.);
			KEYWORD[16].Add(new AtomicString("NumpadEqual"), CssUI.DOM.Events.EKeyboardCode.NumpadEqual);
			KEYWORD[16].Add(new AtomicString("NumpadHash"), CssUI.DOM.Events.EKeyboardCode.NumpadHash);
			KEYWORD[16].Add(new AtomicString("NumpadMemoryAdd"), CssUI.DOM.Events.EKeyboardCode.NumpadMemoryAdd);
			KEYWORD[16].Add(new AtomicString("NumpadMemoryClear"), CssUI.DOM.Events.EKeyboardCode.NumpadMemoryClear);
			KEYWORD[16].Add(new AtomicString("NumpadMemoryRecall"), CssUI.DOM.Events.EKeyboardCode.NumpadMemoryRecall);
			KEYWORD[16].Add(new AtomicString("NumpadMemoryStore"), CssUI.DOM.Events.EKeyboardCode.NumpadMemoryStore);
			KEYWORD[16].Add(new AtomicString("NumpadMemorySubtract"), CssUI.DOM.Events.EKeyboardCode.NumpadMemorySubtract);
			KEYWORD[16].Add(new AtomicString("NumpadMultiply"), CssUI.DOM.Events.EKeyboardCode.NumpadMultiply);
			KEYWORD[16].Add(new AtomicString("NumpadParenLeft"), CssUI.DOM.Events.EKeyboardCode.NumpadParenLeft);
			KEYWORD[16].Add(new AtomicString("NumpadParenRight"), CssUI.DOM.Events.EKeyboardCode.NumpadParenRight);
			KEYWORD[16].Add(new AtomicString("NumpadStar"), CssUI.DOM.Events.EKeyboardCode.NumpadStar);
			KEYWORD[16].Add(new AtomicString("NumpadSubtract"), CssUI.DOM.Events.EKeyboardCode.NumpadSubtract);
			KEYWORD[16].Add(new AtomicString("Escape"), CssUI.DOM.Events.EKeyboardCode.Escape);
			KEYWORD[16].Add(new AtomicString("F1"), CssUI.DOM.Events.EKeyboardCode.F1);
			KEYWORD[16].Add(new AtomicString("F2"), CssUI.DOM.Events.EKeyboardCode.F2);
			KEYWORD[16].Add(new AtomicString("F3"), CssUI.DOM.Events.EKeyboardCode.F3);
			KEYWORD[16].Add(new AtomicString("F4"), CssUI.DOM.Events.EKeyboardCode.F4);
			KEYWORD[16].Add(new AtomicString("F5"), CssUI.DOM.Events.EKeyboardCode.F5);
			KEYWORD[16].Add(new AtomicString("F6"), CssUI.DOM.Events.EKeyboardCode.F6);
			KEYWORD[16].Add(new AtomicString("F7"), CssUI.DOM.Events.EKeyboardCode.F7);
			KEYWORD[16].Add(new AtomicString("F8"), CssUI.DOM.Events.EKeyboardCode.F8);
			KEYWORD[16].Add(new AtomicString("F9"), CssUI.DOM.Events.EKeyboardCode.F9);
			KEYWORD[16].Add(new AtomicString("F10"), CssUI.DOM.Events.EKeyboardCode.F10);
			KEYWORD[16].Add(new AtomicString("F11"), CssUI.DOM.Events.EKeyboardCode.F11);
			KEYWORD[16].Add(new AtomicString("F12"), CssUI.DOM.Events.EKeyboardCode.F12);
			KEYWORD[16].Add(new AtomicString("Fn"), CssUI.DOM.Events.EKeyboardCode.Fn);
			KEYWORD[16].Add(new AtomicString("FnLock"), CssUI.DOM.Events.EKeyboardCode.FnLock);
			KEYWORD[16].Add(new AtomicString("PrintScreen"), CssUI.DOM.Events.EKeyboardCode.PrintScreen);
			KEYWORD[16].Add(new AtomicString("ScrollLock"), CssUI.DOM.Events.EKeyboardCode.ScrollLock);
			KEYWORD[16].Add(new AtomicString("Pause"), CssUI.DOM.Events.EKeyboardCode.Pause);
			KEYWORD[16].Add(new AtomicString("BrowserBack"), CssUI.DOM.Events.EKeyboardCode.BrowserBack);
			KEYWORD[16].Add(new AtomicString("BrowserFavorites"), CssUI.DOM.Events.EKeyboardCode.BrowserFavorites);
			KEYWORD[16].Add(new AtomicString("BrowserForward"), CssUI.DOM.Events.EKeyboardCode.BrowserForward);
			KEYWORD[16].Add(new AtomicString("BrowserHome"), CssUI.DOM.Events.EKeyboardCode.BrowserHome);
			KEYWORD[16].Add(new AtomicString("BrowserRefresh"), CssUI.DOM.Events.EKeyboardCode.BrowserRefresh);
			KEYWORD[16].Add(new AtomicString("BrowserSearch"), CssUI.DOM.Events.EKeyboardCode.BrowserSearch);
			KEYWORD[16].Add(new AtomicString("BrowserStop"), CssUI.DOM.Events.EKeyboardCode.BrowserStop);
			KEYWORD[16].Add(new AtomicString("Eject"), CssUI.DOM.Events.EKeyboardCode.Eject);
			KEYWORD[16].Add(new AtomicString("LaunchApp1"), CssUI.DOM.Events.EKeyboardCode.LaunchApp1);
			KEYWORD[16].Add(new AtomicString("LaunchApp2"), CssUI.DOM.Events.EKeyboardCode.LaunchApp2);
			KEYWORD[16].Add(new AtomicString("LaunchMail"), CssUI.DOM.Events.EKeyboardCode.LaunchMail);
			KEYWORD[16].Add(new AtomicString("MediaPlayPause"), CssUI.DOM.Events.EKeyboardCode.MediaPlayPause);
			KEYWORD[16].Add(new AtomicString("MediaSelect"), CssUI.DOM.Events.EKeyboardCode.MediaSelect);
			KEYWORD[16].Add(new AtomicString("MediaStop"), CssUI.DOM.Events.EKeyboardCode.MediaStop);
			KEYWORD[16].Add(new AtomicString("MediaTrackNext"), CssUI.DOM.Events.EKeyboardCode.MediaTrackNext);
			KEYWORD[16].Add(new AtomicString("MediaTrackPrevious"), CssUI.DOM.Events.EKeyboardCode.MediaTrackPrevious);
			KEYWORD[16].Add(new AtomicString("Power"), CssUI.DOM.Events.EKeyboardCode.Power);
			KEYWORD[16].Add(new AtomicString("Sleep"), CssUI.DOM.Events.EKeyboardCode.Sleep);
			KEYWORD[16].Add(new AtomicString("AudioVolumeDown"), CssUI.DOM.Events.EKeyboardCode.AudioVolumeDown);
			KEYWORD[16].Add(new AtomicString("AudioVolumeMute"), CssUI.DOM.Events.EKeyboardCode.AudioVolumeMute);
			KEYWORD[16].Add(new AtomicString("AudioVolumeUp"), CssUI.DOM.Events.EKeyboardCode.AudioVolumeUp);
			KEYWORD[16].Add(new AtomicString("WakeUp"), CssUI.DOM.Events.EKeyboardCode.WakeUp);
			#endregion
				

		#endregion
		}
		#endregion
	}
}

