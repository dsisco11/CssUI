


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
namespace CssUI
{
	internal static class CSSUIEnumTables
	{
		static Dictionary<RuntimeTypeHandle, int> TypeHandleMap = CreateTypeMap();
		
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		static Dictionary<RuntimeTypeHandle, int> CreateTypeMap()
		{	
			var typeMap = new Dictionary<RuntimeTypeHandle, int>();
			typeMap[typeof(CssUI.CSS.EColor).TypeHandle] = 0;
			typeMap[typeof(CssUI.DOM.EAttributeName).TypeHandle] = 1;
			typeMap[typeof(CssUI.DOM.EAutoComplete).TypeHandle] = 2;
			typeMap[typeof(CssUI.DOM.EAutofill).TypeHandle] = 3;
			typeMap[typeof(CssUI.DOM.EAutofillContact).TypeHandle] = 4;
			typeMap[typeof(CssUI.DOM.EAutofillHint).TypeHandle] = 5;
			typeMap[typeof(CssUI.DOM.EAutofillMode).TypeHandle] = 6;
			typeMap[typeof(CssUI.DOM.EBrowsingTarget).TypeHandle] = 7;
			typeMap[typeof(CssUI.DOM.EContentEditable).TypeHandle] = 8;
			typeMap[typeof(CssUI.DOM.EDesignMode).TypeHandle] = 9;
			typeMap[typeof(CssUI.DOM.EDir).TypeHandle] = 10;
			typeMap[typeof(CssUI.DOM.EDirName).TypeHandle] = 11;
			typeMap[typeof(CssUI.DOM.EEncType).TypeHandle] = 12;
			typeMap[typeof(CssUI.DOM.EFormMethod).TypeHandle] = 13;
			typeMap[typeof(CssUI.DOM.EInputType).TypeHandle] = 14;
			typeMap[typeof(CssUI.DOM.Events.EEventName).TypeHandle] = 15;
			typeMap[typeof(CssUI.DOM.Events.EKeyboardCode).TypeHandle] = 16;
			
			return typeMap;
		}


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Get_Enum_Index<Ty>()
        {
			if (TypeHandleMap.TryGetValue(typeof(Ty).TypeHandle, out int outIndex))
			{
				return outIndex;
			}

			return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Get_Enum_Index(RuntimeTypeHandle Handle)
        {
			if (TypeHandleMap.TryGetValue(Handle, out int outIndex))
			{
				return outIndex;
			}

			return -1;
        }
		
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Lookup_Index<Ty>(out int outIndex) where Ty : struct
		{
			if (TypeHandleMap.TryGetValue(typeof(Ty).TypeHandle, out int outMapValue))
			{
				outIndex = outMapValue;
				return true;
			}

			outIndex = -1;
			return false;
		}

		
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Lookup_Enum_Index(string Name)
		{		
            switch(Name)
            {
				case "EColor": return 0;
				case "EAttributeName": return 1;
				case "EAutoComplete": return 2;
				case "EAutofill": return 3;
				case "EAutofillContact": return 4;
				case "EAutofillHint": return 5;
				case "EAutofillMode": return 6;
				case "EBrowsingTarget": return 7;
				case "EContentEditable": return 8;
				case "EDesignMode": return 9;
				case "EDir": return 10;
				case "EDirName": return 11;
				case "EEncType": return 12;
				case "EFormMethod": return 13;
				case "EInputType": return 14;
				case "EEventName": return 15;
				case "EKeyboardCode": return 16;
				default: return -1;//throw new NotImplementedException($"No lookup-index has defined for enum type '{typeof(Ty).Name}'");
            }
		}



		#region Static Enum Tables
		internal static readonly EnumData[][] TABLE;
		internal static readonly List<Dictionary<AtomicString, dynamic>> KEYWORD;

		static CSSUIEnumTables()
		{
			TABLE = new EnumData[18][];
			KEYWORD = new List<Dictionary<AtomicString, dynamic>>(18);
			int maxIndex = 0;

			#region Table Data 
			/* CssUI.CSS.EColor */
			#region CssUI.CSS.EColor
			maxIndex = (int)Enum.GetValues(typeof(CssUI.CSS.EColor)).Cast<CssUI.CSS.EColor>().Max();
			TABLE[0] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EAttributeName */
			#region CssUI.DOM.EAttributeName
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EAttributeName)).Cast<CssUI.DOM.EAttributeName>().Max();
			TABLE[1] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EAutoComplete */
			#region CssUI.DOM.EAutoComplete
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EAutoComplete)).Cast<CssUI.DOM.EAutoComplete>().Max();
			TABLE[2] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EAutofill */
			#region CssUI.DOM.EAutofill
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EAutofill)).Cast<CssUI.DOM.EAutofill>().Max();
			TABLE[3] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EAutofillContact */
			#region CssUI.DOM.EAutofillContact
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EAutofillContact)).Cast<CssUI.DOM.EAutofillContact>().Max();
			TABLE[4] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EAutofillHint */
			#region CssUI.DOM.EAutofillHint
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EAutofillHint)).Cast<CssUI.DOM.EAutofillHint>().Max();
			TABLE[5] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EAutofillMode */
			#region CssUI.DOM.EAutofillMode
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EAutofillMode)).Cast<CssUI.DOM.EAutofillMode>().Max();
			TABLE[6] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EBrowsingTarget */
			#region CssUI.DOM.EBrowsingTarget
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EBrowsingTarget)).Cast<CssUI.DOM.EBrowsingTarget>().Max();
			TABLE[7] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EContentEditable */
			#region CssUI.DOM.EContentEditable
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EContentEditable)).Cast<CssUI.DOM.EContentEditable>().Max();
			TABLE[8] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EDesignMode */
			#region CssUI.DOM.EDesignMode
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EDesignMode)).Cast<CssUI.DOM.EDesignMode>().Max();
			TABLE[9] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EDir */
			#region CssUI.DOM.EDir
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EDir)).Cast<CssUI.DOM.EDir>().Max();
			TABLE[10] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EDirName */
			#region CssUI.DOM.EDirName
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EDirName)).Cast<CssUI.DOM.EDirName>().Max();
			TABLE[11] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EEncType */
			#region CssUI.DOM.EEncType
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EEncType)).Cast<CssUI.DOM.EEncType>().Max();
			TABLE[12] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EFormMethod */
			#region CssUI.DOM.EFormMethod
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EFormMethod)).Cast<CssUI.DOM.EFormMethod>().Max();
			TABLE[13] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EInputType */
			#region CssUI.DOM.EInputType
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EInputType)).Cast<CssUI.DOM.EInputType>().Max();
			TABLE[14] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.Events.EEventName */
			#region CssUI.DOM.Events.EEventName
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.Events.EEventName)).Cast<CssUI.DOM.Events.EEventName>().Max();
			TABLE[15] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.Events.EKeyboardCode */
			#region CssUI.DOM.Events.EKeyboardCode
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.Events.EKeyboardCode)).Cast<CssUI.DOM.Events.EKeyboardCode>().Max();
			TABLE[16] = new EnumData[maxIndex+1];
			#endregion
			

		#endregion
		/* Generate Reverse lookup maps */
		#region Reverse Lookup
			/* CssUI.CSS.EColor */
			#region CssUI.CSS.EColor
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			#endregion
				

			/* CssUI.DOM.EAttributeName */
			#region CssUI.DOM.EAttributeName
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			#endregion
				

			/* CssUI.DOM.EAutoComplete */
			#region CssUI.DOM.EAutoComplete
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			#endregion
				

			/* CssUI.DOM.EAutofill */
			#region CssUI.DOM.EAutofill
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			#endregion
				

			/* CssUI.DOM.EAutofillContact */
			#region CssUI.DOM.EAutofillContact
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			#endregion
				

			/* CssUI.DOM.EAutofillHint */
			#region CssUI.DOM.EAutofillHint
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			#endregion
				

			/* CssUI.DOM.EAutofillMode */
			#region CssUI.DOM.EAutofillMode
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			#endregion
				

			/* CssUI.DOM.EBrowsingTarget */
			#region CssUI.DOM.EBrowsingTarget
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			#endregion
				

			/* CssUI.DOM.EContentEditable */
			#region CssUI.DOM.EContentEditable
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			#endregion
				

			/* CssUI.DOM.EDesignMode */
			#region CssUI.DOM.EDesignMode
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			#endregion
				

			/* CssUI.DOM.EDir */
			#region CssUI.DOM.EDir
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			#endregion
				

			/* CssUI.DOM.EDirName */
			#region CssUI.DOM.EDirName
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			#endregion
				

			/* CssUI.DOM.EEncType */
			#region CssUI.DOM.EEncType
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			#endregion
				

			/* CssUI.DOM.EFormMethod */
			#region CssUI.DOM.EFormMethod
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			#endregion
				

			/* CssUI.DOM.EInputType */
			#region CssUI.DOM.EInputType
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			#endregion
				

			/* CssUI.DOM.Events.EEventName */
			#region CssUI.DOM.Events.EEventName
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			#endregion
				

			/* CssUI.DOM.Events.EKeyboardCode */
			#region CssUI.DOM.Events.EKeyboardCode
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			#endregion
				

		#endregion
		}
		#endregion
	}
}

