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
				case "EContentEditable": return 0;
				case "EElementTag": return 1;
				case "EShadowRootMode": return 2;
				case "EEventName": return 3;
				default: return -1;//throw new NotImplementedException($"No lookup-index has defined for enum type '{typeof(Ty).Name}'");
            }
		}



		#region Static Enum Tables
		internal static readonly string[][] TABLE;
		internal static readonly List<Dictionary<AtomicString, dynamic>> KEYWORD;

		static DomEnumTables()
		{
			TABLE = new string[5][];
			KEYWORD = new List<Dictionary<AtomicString, dynamic>>(5);
			int maxIndex = 0;
			/* CssUI.DOM.Enums.EContentEditable */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.Enums.EContentEditable)).Cast<CssUI.DOM.Enums.EContentEditable>().Max();
			TABLE[0] = new string[maxIndex+1];
			TABLE[0][(int)CssUI.DOM.Enums.EContentEditable.True] = "true";
			TABLE[0][(int)CssUI.DOM.Enums.EContentEditable.False] = "false";
			TABLE[0][(int)CssUI.DOM.Enums.EContentEditable.Inherit] = "inherit";


			/* CssUI.DOM.Enums.EElementTag */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.Enums.EElementTag)).Cast<CssUI.DOM.Enums.EElementTag>().Max();
			TABLE[1] = new string[maxIndex+1];
			TABLE[1][(int)CssUI.DOM.Enums.EElementTag.Div] = "div";
			TABLE[1][(int)CssUI.DOM.Enums.EElementTag.Html] = "html";
			TABLE[1][(int)CssUI.DOM.Enums.EElementTag.Body] = "body";
			TABLE[1][(int)CssUI.DOM.Enums.EElementTag.Head] = "head";
			TABLE[1][(int)CssUI.DOM.Enums.EElementTag.Template] = "template";
			TABLE[1][(int)CssUI.DOM.Enums.EElementTag.Slot] = "slot";


			/* CssUI.DOM.Enums.EShadowRootMode */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.Enums.EShadowRootMode)).Cast<CssUI.DOM.Enums.EShadowRootMode>().Max();
			TABLE[2] = new string[maxIndex+1];
			TABLE[2][(int)CssUI.DOM.Enums.EShadowRootMode.Open] = "open";
			TABLE[2][(int)CssUI.DOM.Enums.EShadowRootMode.Closed] = "closed";


			/* CssUI.DOM.Events.EEventName */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.Events.EEventName)).Cast<CssUI.DOM.Events.EEventName>().Max();
			TABLE[3] = new string[maxIndex+1];
			TABLE[3][(int)CssUI.DOM.Events.EEventName.None] = "";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Abort] = "abort";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.AuxClick] = "auxclick";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Blur] = "blur";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Cancel] = "cancel";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.CanPlay] = "canplay";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.CanPlayThrough] = "canplaythrough";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Change] = "change";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Click] = "click";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Close] = "close";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.ContextMenu] = "contextmenu";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.CueChange] = "cuechange";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.DoubleClick] = "dblclick";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Drag] = "drag";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.DragEnd] = "dragend";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.DragEnter] = "dragenter";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.DragExit] = "dragexit";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.DragLeave] = "dragleave";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.DragOver] = "dragover";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.DragStart] = "dragstart";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Drop] = "drop";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.DurationChange] = "durationchange";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Emptied] = "emptied";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Ended] = "ended";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Error] = "error";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Focus] = "focus";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.FormData] = "formdata";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Input] = "input";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Invalid] = "invalid";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.KeyDown] = "keydown";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.KeyPress] = "keypress";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.KeyUp] = "keyup";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Load] = "load";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.LoadedData] = "loadeddata";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.LoadedMetadata] = "loadedmetadata";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.LoadEnd] = "loadend";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.LoadStart] = "loadstart";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.MouseDown] = "mousedown";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.MouseEnter] = "mouseenter";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.MouseLeave] = "mouseleave";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.MouseMove] = "mousemove";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.MouseOut] = "mouseout";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.MouseOver] = "mouseover";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.MouseUp] = "mouseup";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Wheel] = "wheel";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Pause] = "pause";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Play] = "play";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Playing] = "playing";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Progress] = "progress";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.RateChange] = "ratechange";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Reset] = "reset";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Resize] = "resize";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Scroll] = "scroll";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.SecurityPolicyViolation] = "securitypolicyviolation";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Seeked] = "seeked";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Seeking] = "seeking";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Select] = "select";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Stalled] = "stalled";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Submit] = "submit";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Suspend] = "suspend";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Timeupdate] = "timeupdate";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Toggle] = "toggle";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Volumechange] = "volumechange";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Waiting] = "waiting";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.SelectStart] = "selectstart";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.SelectionChange] = "selectionchange";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Copy] = "copy";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Cut] = "cut";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Paste] = "paste";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.AfterPrint] = "afterprint";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.BeforePrint] = "beforeprint";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.BeforeUnload] = "beforeunload";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.HashChange] = "hashchange";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.LanguageChange] = "languagechange";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Message] = "message";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.MessageError] = "messageerror";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Offline] = "offline";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Online] = "online";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.PageHide] = "pagehide";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.PageShow] = "pageshow";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.PopState] = "popstate";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.RejectionHandled] = "rejectionhandled";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Storage] = "storage";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.UnhandledRejection] = "unhandledrejection";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.Unload] = "unload";
			TABLE[3][(int)CssUI.DOM.Events.EEventName.SlotChange] = "slotchange";



		/* Generate Reverse lookup maps */
			/* CssUI.DOM.Enums.EContentEditable */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[0].Add(new AtomicString("true"), CssUI.DOM.Enums.EContentEditable.True);
			KEYWORD[0].Add(new AtomicString("false"), CssUI.DOM.Enums.EContentEditable.False);
			KEYWORD[0].Add(new AtomicString("inherit"), CssUI.DOM.Enums.EContentEditable.Inherit);


			/* CssUI.DOM.Enums.EElementTag */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[1].Add(new AtomicString("div"), CssUI.DOM.Enums.EElementTag.Div);
			KEYWORD[1].Add(new AtomicString("html"), CssUI.DOM.Enums.EElementTag.Html);
			KEYWORD[1].Add(new AtomicString("body"), CssUI.DOM.Enums.EElementTag.Body);
			KEYWORD[1].Add(new AtomicString("head"), CssUI.DOM.Enums.EElementTag.Head);
			KEYWORD[1].Add(new AtomicString("template"), CssUI.DOM.Enums.EElementTag.Template);
			KEYWORD[1].Add(new AtomicString("slot"), CssUI.DOM.Enums.EElementTag.Slot);


			/* CssUI.DOM.Enums.EShadowRootMode */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[2].Add(new AtomicString("open"), CssUI.DOM.Enums.EShadowRootMode.Open);
			KEYWORD[2].Add(new AtomicString("closed"), CssUI.DOM.Enums.EShadowRootMode.Closed);


			/* CssUI.DOM.Events.EEventName */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[3].Add(new AtomicString(""), CssUI.DOM.Events.EEventName.None);
			KEYWORD[3].Add(new AtomicString("abort"), CssUI.DOM.Events.EEventName.Abort);
			KEYWORD[3].Add(new AtomicString("auxclick"), CssUI.DOM.Events.EEventName.AuxClick);
			KEYWORD[3].Add(new AtomicString("blur"), CssUI.DOM.Events.EEventName.Blur);
			KEYWORD[3].Add(new AtomicString("cancel"), CssUI.DOM.Events.EEventName.Cancel);
			KEYWORD[3].Add(new AtomicString("canplay"), CssUI.DOM.Events.EEventName.CanPlay);
			KEYWORD[3].Add(new AtomicString("canplaythrough"), CssUI.DOM.Events.EEventName.CanPlayThrough);
			KEYWORD[3].Add(new AtomicString("change"), CssUI.DOM.Events.EEventName.Change);
			KEYWORD[3].Add(new AtomicString("click"), CssUI.DOM.Events.EEventName.Click);
			KEYWORD[3].Add(new AtomicString("close"), CssUI.DOM.Events.EEventName.Close);
			KEYWORD[3].Add(new AtomicString("contextmenu"), CssUI.DOM.Events.EEventName.ContextMenu);
			KEYWORD[3].Add(new AtomicString("cuechange"), CssUI.DOM.Events.EEventName.CueChange);
			KEYWORD[3].Add(new AtomicString("dblclick"), CssUI.DOM.Events.EEventName.DoubleClick);
			KEYWORD[3].Add(new AtomicString("drag"), CssUI.DOM.Events.EEventName.Drag);
			KEYWORD[3].Add(new AtomicString("dragend"), CssUI.DOM.Events.EEventName.DragEnd);
			KEYWORD[3].Add(new AtomicString("dragenter"), CssUI.DOM.Events.EEventName.DragEnter);
			KEYWORD[3].Add(new AtomicString("dragexit"), CssUI.DOM.Events.EEventName.DragExit);
			KEYWORD[3].Add(new AtomicString("dragleave"), CssUI.DOM.Events.EEventName.DragLeave);
			KEYWORD[3].Add(new AtomicString("dragover"), CssUI.DOM.Events.EEventName.DragOver);
			KEYWORD[3].Add(new AtomicString("dragstart"), CssUI.DOM.Events.EEventName.DragStart);
			KEYWORD[3].Add(new AtomicString("drop"), CssUI.DOM.Events.EEventName.Drop);
			KEYWORD[3].Add(new AtomicString("durationchange"), CssUI.DOM.Events.EEventName.DurationChange);
			KEYWORD[3].Add(new AtomicString("emptied"), CssUI.DOM.Events.EEventName.Emptied);
			KEYWORD[3].Add(new AtomicString("ended"), CssUI.DOM.Events.EEventName.Ended);
			KEYWORD[3].Add(new AtomicString("error"), CssUI.DOM.Events.EEventName.Error);
			KEYWORD[3].Add(new AtomicString("focus"), CssUI.DOM.Events.EEventName.Focus);
			KEYWORD[3].Add(new AtomicString("formdata"), CssUI.DOM.Events.EEventName.FormData);
			KEYWORD[3].Add(new AtomicString("input"), CssUI.DOM.Events.EEventName.Input);
			KEYWORD[3].Add(new AtomicString("invalid"), CssUI.DOM.Events.EEventName.Invalid);
			KEYWORD[3].Add(new AtomicString("keydown"), CssUI.DOM.Events.EEventName.KeyDown);
			KEYWORD[3].Add(new AtomicString("keypress"), CssUI.DOM.Events.EEventName.KeyPress);
			KEYWORD[3].Add(new AtomicString("keyup"), CssUI.DOM.Events.EEventName.KeyUp);
			KEYWORD[3].Add(new AtomicString("load"), CssUI.DOM.Events.EEventName.Load);
			KEYWORD[3].Add(new AtomicString("loadeddata"), CssUI.DOM.Events.EEventName.LoadedData);
			KEYWORD[3].Add(new AtomicString("loadedmetadata"), CssUI.DOM.Events.EEventName.LoadedMetadata);
			KEYWORD[3].Add(new AtomicString("loadend"), CssUI.DOM.Events.EEventName.LoadEnd);
			KEYWORD[3].Add(new AtomicString("loadstart"), CssUI.DOM.Events.EEventName.LoadStart);
			KEYWORD[3].Add(new AtomicString("mousedown"), CssUI.DOM.Events.EEventName.MouseDown);
			KEYWORD[3].Add(new AtomicString("mouseenter"), CssUI.DOM.Events.EEventName.MouseEnter);
			KEYWORD[3].Add(new AtomicString("mouseleave"), CssUI.DOM.Events.EEventName.MouseLeave);
			KEYWORD[3].Add(new AtomicString("mousemove"), CssUI.DOM.Events.EEventName.MouseMove);
			KEYWORD[3].Add(new AtomicString("mouseout"), CssUI.DOM.Events.EEventName.MouseOut);
			KEYWORD[3].Add(new AtomicString("mouseover"), CssUI.DOM.Events.EEventName.MouseOver);
			KEYWORD[3].Add(new AtomicString("mouseup"), CssUI.DOM.Events.EEventName.MouseUp);
			KEYWORD[3].Add(new AtomicString("wheel"), CssUI.DOM.Events.EEventName.Wheel);
			KEYWORD[3].Add(new AtomicString("pause"), CssUI.DOM.Events.EEventName.Pause);
			KEYWORD[3].Add(new AtomicString("play"), CssUI.DOM.Events.EEventName.Play);
			KEYWORD[3].Add(new AtomicString("playing"), CssUI.DOM.Events.EEventName.Playing);
			KEYWORD[3].Add(new AtomicString("progress"), CssUI.DOM.Events.EEventName.Progress);
			KEYWORD[3].Add(new AtomicString("ratechange"), CssUI.DOM.Events.EEventName.RateChange);
			KEYWORD[3].Add(new AtomicString("reset"), CssUI.DOM.Events.EEventName.Reset);
			KEYWORD[3].Add(new AtomicString("resize"), CssUI.DOM.Events.EEventName.Resize);
			KEYWORD[3].Add(new AtomicString("scroll"), CssUI.DOM.Events.EEventName.Scroll);
			KEYWORD[3].Add(new AtomicString("securitypolicyviolation"), CssUI.DOM.Events.EEventName.SecurityPolicyViolation);
			KEYWORD[3].Add(new AtomicString("seeked"), CssUI.DOM.Events.EEventName.Seeked);
			KEYWORD[3].Add(new AtomicString("seeking"), CssUI.DOM.Events.EEventName.Seeking);
			KEYWORD[3].Add(new AtomicString("select"), CssUI.DOM.Events.EEventName.Select);
			KEYWORD[3].Add(new AtomicString("stalled"), CssUI.DOM.Events.EEventName.Stalled);
			KEYWORD[3].Add(new AtomicString("submit"), CssUI.DOM.Events.EEventName.Submit);
			KEYWORD[3].Add(new AtomicString("suspend"), CssUI.DOM.Events.EEventName.Suspend);
			KEYWORD[3].Add(new AtomicString("timeupdate"), CssUI.DOM.Events.EEventName.Timeupdate);
			KEYWORD[3].Add(new AtomicString("toggle"), CssUI.DOM.Events.EEventName.Toggle);
			KEYWORD[3].Add(new AtomicString("volumechange"), CssUI.DOM.Events.EEventName.Volumechange);
			KEYWORD[3].Add(new AtomicString("waiting"), CssUI.DOM.Events.EEventName.Waiting);
			KEYWORD[3].Add(new AtomicString("selectstart"), CssUI.DOM.Events.EEventName.SelectStart);
			KEYWORD[3].Add(new AtomicString("selectionchange"), CssUI.DOM.Events.EEventName.SelectionChange);
			KEYWORD[3].Add(new AtomicString("copy"), CssUI.DOM.Events.EEventName.Copy);
			KEYWORD[3].Add(new AtomicString("cut"), CssUI.DOM.Events.EEventName.Cut);
			KEYWORD[3].Add(new AtomicString("paste"), CssUI.DOM.Events.EEventName.Paste);
			KEYWORD[3].Add(new AtomicString("afterprint"), CssUI.DOM.Events.EEventName.AfterPrint);
			KEYWORD[3].Add(new AtomicString("beforeprint"), CssUI.DOM.Events.EEventName.BeforePrint);
			KEYWORD[3].Add(new AtomicString("beforeunload"), CssUI.DOM.Events.EEventName.BeforeUnload);
			KEYWORD[3].Add(new AtomicString("hashchange"), CssUI.DOM.Events.EEventName.HashChange);
			KEYWORD[3].Add(new AtomicString("languagechange"), CssUI.DOM.Events.EEventName.LanguageChange);
			KEYWORD[3].Add(new AtomicString("message"), CssUI.DOM.Events.EEventName.Message);
			KEYWORD[3].Add(new AtomicString("messageerror"), CssUI.DOM.Events.EEventName.MessageError);
			KEYWORD[3].Add(new AtomicString("offline"), CssUI.DOM.Events.EEventName.Offline);
			KEYWORD[3].Add(new AtomicString("online"), CssUI.DOM.Events.EEventName.Online);
			KEYWORD[3].Add(new AtomicString("pagehide"), CssUI.DOM.Events.EEventName.PageHide);
			KEYWORD[3].Add(new AtomicString("pageshow"), CssUI.DOM.Events.EEventName.PageShow);
			KEYWORD[3].Add(new AtomicString("popstate"), CssUI.DOM.Events.EEventName.PopState);
			KEYWORD[3].Add(new AtomicString("rejectionhandled"), CssUI.DOM.Events.EEventName.RejectionHandled);
			KEYWORD[3].Add(new AtomicString("storage"), CssUI.DOM.Events.EEventName.Storage);
			KEYWORD[3].Add(new AtomicString("unhandledrejection"), CssUI.DOM.Events.EEventName.UnhandledRejection);
			KEYWORD[3].Add(new AtomicString("unload"), CssUI.DOM.Events.EEventName.Unload);
			KEYWORD[3].Add(new AtomicString("slotchange"), CssUI.DOM.Events.EEventName.SlotChange);


		}
		#endregion
	}
}

