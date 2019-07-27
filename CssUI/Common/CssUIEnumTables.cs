


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
			typeMap[typeof(CssUI.DOM.EAttributeName).TypeHandle] = 0;
			typeMap[typeof(CssUI.DOM.EAutofill).TypeHandle] = 1;
			typeMap[typeof(CssUI.DOM.EAutofillContact).TypeHandle] = 2;
			typeMap[typeof(CssUI.DOM.EAutofillHint).TypeHandle] = 3;
			typeMap[typeof(CssUI.DOM.EAutofillMode).TypeHandle] = 4;
			typeMap[typeof(CssUI.DOM.EContentEditable).TypeHandle] = 5;
			typeMap[typeof(CssUI.DOM.EDir).TypeHandle] = 6;
			typeMap[typeof(CssUI.DOM.EDirName).TypeHandle] = 7;
			
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
				case "EAttributeName": return 0;
				case "EAutofill": return 1;
				case "EAutofillContact": return 2;
				case "EAutofillHint": return 3;
				case "EAutofillMode": return 4;
				case "EContentEditable": return 5;
				case "EDir": return 6;
				case "EDirName": return 7;
				default: return -1;//throw new NotImplementedException($"No lookup-index has defined for enum type '{typeof(Ty).Name}'");
            }
		}



		#region Static Enum Tables
		internal static readonly EnumData[][] TABLE;
		internal static readonly List<Dictionary<AtomicString, dynamic>> KEYWORD;

		static CSSUIEnumTables()
		{
			TABLE = new EnumData[9][];
			KEYWORD = new List<Dictionary<AtomicString, dynamic>>(9);
			int maxIndex = 0;

			#region Table Data 
			/* CssUI.DOM.EAttributeName */
			#region CssUI.DOM.EAttributeName
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EAttributeName)).Cast<CssUI.DOM.EAttributeName>().Max();
			TABLE[0] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EAutofill */
			#region CssUI.DOM.EAutofill
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EAutofill)).Cast<CssUI.DOM.EAutofill>().Max();
			TABLE[1] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EAutofillContact */
			#region CssUI.DOM.EAutofillContact
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EAutofillContact)).Cast<CssUI.DOM.EAutofillContact>().Max();
			TABLE[2] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EAutofillHint */
			#region CssUI.DOM.EAutofillHint
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EAutofillHint)).Cast<CssUI.DOM.EAutofillHint>().Max();
			TABLE[3] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EAutofillMode */
			#region CssUI.DOM.EAutofillMode
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EAutofillMode)).Cast<CssUI.DOM.EAutofillMode>().Max();
			TABLE[4] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EContentEditable */
			#region CssUI.DOM.EContentEditable
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EContentEditable)).Cast<CssUI.DOM.EContentEditable>().Max();
			TABLE[5] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EDir */
			#region CssUI.DOM.EDir
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EDir)).Cast<CssUI.DOM.EDir>().Max();
			TABLE[6] = new EnumData[maxIndex+1];
			#endregion
			

			/* CssUI.DOM.EDirName */
			#region CssUI.DOM.EDirName
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EDirName)).Cast<CssUI.DOM.EDirName>().Max();
			TABLE[7] = new EnumData[maxIndex+1];
			#endregion
			

		#endregion
		/* Generate Reverse lookup maps */
		#region Reverse Lookup
			/* CssUI.DOM.EAttributeName */
			#region CssUI.DOM.EAttributeName
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
				

			/* CssUI.DOM.EContentEditable */
			#region CssUI.DOM.EContentEditable
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
				

		#endregion
		}
		#endregion
	}
}

