


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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Get_Enum_Index<Ty>()
        {
            if (Lookup_Index<Ty>(out int outIndex))
			{
				return outIndex;
			}

			return -1;
        }

		static Dictionary<RuntimeTypeHandle, int> TypeHandleMap = CreateTypeMap();
		
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		static Dictionary<RuntimeTypeHandle, int> CreateTypeMap()
		{	
			var typeMap = new Dictionary<RuntimeTypeHandle, int>();
			typeMap[typeof(CssUI.DOM.EAutofill).TypeHandle] = 0;
			typeMap[typeof(CssUI.DOM.EContentEditable).TypeHandle] = 1;
			
			return typeMap;
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
				case "EAutofill": return 0;
				case "EContentEditable": return 1;
				default: return -1;//throw new NotImplementedException($"No lookup-index has defined for enum type '{typeof(Ty).Name}'");
            }
		}



		#region Static Enum Tables
		internal static readonly EnumData[][] TABLE;
		internal static readonly List<Dictionary<AtomicString, dynamic>> KEYWORD;

		static CSSUIEnumTables()
		{
			TABLE = new EnumData[3][];
			KEYWORD = new List<Dictionary<AtomicString, dynamic>>(3);
			int maxIndex = 0;

			#region Table Data 
			/* CssUI.DOM.EAutofill */
			#region CssUI.DOM.EAutofill
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EAutofill)).Cast<CssUI.DOM.EAutofill>().Max();
			TABLE[0] = new EnumData[maxIndex+1];
			TABLE[0][(int)CssUI.DOM.EAutofill.Off] = new EnumData("off", 1, EAutofillCategory.Off);
			TABLE[0][(int)CssUI.DOM.EAutofill.On] = new EnumData("on", 1, EAutofillCategory.Automatic);
			TABLE[0][(int)CssUI.DOM.EAutofill.Name] = new EnumData("name", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Honorific_Prefix] = new EnumData("honorific-prefix", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Given_Name] = new EnumData("given-name", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Additional_Name] = new EnumData("additional-name", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Family_Name] = new EnumData("family-name", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Honorific_Suffix] = new EnumData("honorific-suffix", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Nickname] = new EnumData("nickname", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Organization_Title] = new EnumData("organization-title", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Username] = new EnumData("username", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.New_Password] = new EnumData("new-password", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Current_Password] = new EnumData("current-password", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.One_Time_Code] = new EnumData("one-time-code", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Organization] = new EnumData("organization", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Street_Address] = new EnumData("street-address", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Address_Line1] = new EnumData("address-line1", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Address_Line2] = new EnumData("address-line2", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Address_Line3] = new EnumData("address-line3", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Address_Level4] = new EnumData("address-level4", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Address_Level3] = new EnumData("address-level3", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Address_Level2] = new EnumData("address-level2", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Address_Level1] = new EnumData("address-level1", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Country] = new EnumData("country", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Country_Name] = new EnumData("country-name", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Postal_Code] = new EnumData("postal-code", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.CC_Name] = new EnumData("cc-name", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.CC_Given_Name] = new EnumData("cc-given-name", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.CC_Additional_Name] = new EnumData("cc-additional-name", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.CC_Family_Name] = new EnumData("cc-family-name", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.CC_Number] = new EnumData("cc-number", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.CC_Exp] = new EnumData("cc-exp", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.CC_Exp_Month] = new EnumData("cc-exp-month", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.CC_Exp_Year] = new EnumData("cc-exp-year", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.CC_Csc] = new EnumData("cc-csc", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.CC_Type] = new EnumData("cc-type", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Transaction_Currency] = new EnumData("transaction-currency", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Transaction_Amount] = new EnumData("transaction-amount", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Language] = new EnumData("language", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Bday] = new EnumData("bday", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Bday_Day] = new EnumData("bday-day", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Bday_Month] = new EnumData("bday-month", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Bday_Year] = new EnumData("bday-year", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Sex] = new EnumData("sex", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Url] = new EnumData("url", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Photo] = new EnumData("photo", 3, EAutofillCategory.Normal);
			TABLE[0][(int)CssUI.DOM.EAutofill.Tel] = new EnumData("tel", 4, EAutofillCategory.Contact);
			TABLE[0][(int)CssUI.DOM.EAutofill.Tel_Country_Code] = new EnumData("tel-country-code", 4, EAutofillCategory.Contact);
			TABLE[0][(int)CssUI.DOM.EAutofill.Tel_National] = new EnumData("tel-national", 4, EAutofillCategory.Contact);
			TABLE[0][(int)CssUI.DOM.EAutofill.Tel_Area_Code] = new EnumData("tel-area-code", 4, EAutofillCategory.Contact);
			TABLE[0][(int)CssUI.DOM.EAutofill.Tel_Local] = new EnumData("tel-local", 4, EAutofillCategory.Contact);
			TABLE[0][(int)CssUI.DOM.EAutofill.Tel_Local_Prefix] = new EnumData("tel-local-prefix", 4, EAutofillCategory.Contact);
			TABLE[0][(int)CssUI.DOM.EAutofill.Tel_Local_Suffix] = new EnumData("tel-local-suffix", 4, EAutofillCategory.Contact);
			TABLE[0][(int)CssUI.DOM.EAutofill.Tel_Extension] = new EnumData("tel-extension", 4, EAutofillCategory.Contact);
			TABLE[0][(int)CssUI.DOM.EAutofill.Email] = new EnumData("email", 4, EAutofillCategory.Contact);
			TABLE[0][(int)CssUI.DOM.EAutofill.Impp] = new EnumData("impp", 4, EAutofillCategory.Contact);
			#endregion
			

			/* CssUI.DOM.EContentEditable */
			#region CssUI.DOM.EContentEditable
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.EContentEditable)).Cast<CssUI.DOM.EContentEditable>().Max();
			TABLE[1] = new EnumData[maxIndex+1];
			TABLE[1][(int)CssUI.DOM.EContentEditable.True] = new EnumData("true");
			TABLE[1][(int)CssUI.DOM.EContentEditable.False] = new EnumData("false");
			TABLE[1][(int)CssUI.DOM.EContentEditable.Inherit] = new EnumData("inherit");
			#endregion
			

		#endregion
		/* Generate Reverse lookup maps */
		#region Reverse Lookup
			/* CssUI.DOM.EAutofill */
			#region CssUI.DOM.EAutofill
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[0].Add(new AtomicString("off"), CssUI.DOM.EAutofill.Off);
			KEYWORD[0].Add(new AtomicString("on"), CssUI.DOM.EAutofill.On);
			KEYWORD[0].Add(new AtomicString("name"), CssUI.DOM.EAutofill.Name);
			KEYWORD[0].Add(new AtomicString("honorific-prefix"), CssUI.DOM.EAutofill.Honorific_Prefix);
			KEYWORD[0].Add(new AtomicString("given-name"), CssUI.DOM.EAutofill.Given_Name);
			KEYWORD[0].Add(new AtomicString("additional-name"), CssUI.DOM.EAutofill.Additional_Name);
			KEYWORD[0].Add(new AtomicString("family-name"), CssUI.DOM.EAutofill.Family_Name);
			KEYWORD[0].Add(new AtomicString("honorific-suffix"), CssUI.DOM.EAutofill.Honorific_Suffix);
			KEYWORD[0].Add(new AtomicString("nickname"), CssUI.DOM.EAutofill.Nickname);
			KEYWORD[0].Add(new AtomicString("organization-title"), CssUI.DOM.EAutofill.Organization_Title);
			KEYWORD[0].Add(new AtomicString("username"), CssUI.DOM.EAutofill.Username);
			KEYWORD[0].Add(new AtomicString("new-password"), CssUI.DOM.EAutofill.New_Password);
			KEYWORD[0].Add(new AtomicString("current-password"), CssUI.DOM.EAutofill.Current_Password);
			KEYWORD[0].Add(new AtomicString("one-time-code"), CssUI.DOM.EAutofill.One_Time_Code);
			KEYWORD[0].Add(new AtomicString("organization"), CssUI.DOM.EAutofill.Organization);
			KEYWORD[0].Add(new AtomicString("street-address"), CssUI.DOM.EAutofill.Street_Address);
			KEYWORD[0].Add(new AtomicString("address-line1"), CssUI.DOM.EAutofill.Address_Line1);
			KEYWORD[0].Add(new AtomicString("address-line2"), CssUI.DOM.EAutofill.Address_Line2);
			KEYWORD[0].Add(new AtomicString("address-line3"), CssUI.DOM.EAutofill.Address_Line3);
			KEYWORD[0].Add(new AtomicString("address-level4"), CssUI.DOM.EAutofill.Address_Level4);
			KEYWORD[0].Add(new AtomicString("address-level3"), CssUI.DOM.EAutofill.Address_Level3);
			KEYWORD[0].Add(new AtomicString("address-level2"), CssUI.DOM.EAutofill.Address_Level2);
			KEYWORD[0].Add(new AtomicString("address-level1"), CssUI.DOM.EAutofill.Address_Level1);
			KEYWORD[0].Add(new AtomicString("country"), CssUI.DOM.EAutofill.Country);
			KEYWORD[0].Add(new AtomicString("country-name"), CssUI.DOM.EAutofill.Country_Name);
			KEYWORD[0].Add(new AtomicString("postal-code"), CssUI.DOM.EAutofill.Postal_Code);
			KEYWORD[0].Add(new AtomicString("cc-name"), CssUI.DOM.EAutofill.CC_Name);
			KEYWORD[0].Add(new AtomicString("cc-given-name"), CssUI.DOM.EAutofill.CC_Given_Name);
			KEYWORD[0].Add(new AtomicString("cc-additional-name"), CssUI.DOM.EAutofill.CC_Additional_Name);
			KEYWORD[0].Add(new AtomicString("cc-family-name"), CssUI.DOM.EAutofill.CC_Family_Name);
			KEYWORD[0].Add(new AtomicString("cc-number"), CssUI.DOM.EAutofill.CC_Number);
			KEYWORD[0].Add(new AtomicString("cc-exp"), CssUI.DOM.EAutofill.CC_Exp);
			KEYWORD[0].Add(new AtomicString("cc-exp-month"), CssUI.DOM.EAutofill.CC_Exp_Month);
			KEYWORD[0].Add(new AtomicString("cc-exp-year"), CssUI.DOM.EAutofill.CC_Exp_Year);
			KEYWORD[0].Add(new AtomicString("cc-csc"), CssUI.DOM.EAutofill.CC_Csc);
			KEYWORD[0].Add(new AtomicString("cc-type"), CssUI.DOM.EAutofill.CC_Type);
			KEYWORD[0].Add(new AtomicString("transaction-currency"), CssUI.DOM.EAutofill.Transaction_Currency);
			KEYWORD[0].Add(new AtomicString("transaction-amount"), CssUI.DOM.EAutofill.Transaction_Amount);
			KEYWORD[0].Add(new AtomicString("language"), CssUI.DOM.EAutofill.Language);
			KEYWORD[0].Add(new AtomicString("bday"), CssUI.DOM.EAutofill.Bday);
			KEYWORD[0].Add(new AtomicString("bday-day"), CssUI.DOM.EAutofill.Bday_Day);
			KEYWORD[0].Add(new AtomicString("bday-month"), CssUI.DOM.EAutofill.Bday_Month);
			KEYWORD[0].Add(new AtomicString("bday-year"), CssUI.DOM.EAutofill.Bday_Year);
			KEYWORD[0].Add(new AtomicString("sex"), CssUI.DOM.EAutofill.Sex);
			KEYWORD[0].Add(new AtomicString("url"), CssUI.DOM.EAutofill.Url);
			KEYWORD[0].Add(new AtomicString("photo"), CssUI.DOM.EAutofill.Photo);
			KEYWORD[0].Add(new AtomicString("tel"), CssUI.DOM.EAutofill.Tel);
			KEYWORD[0].Add(new AtomicString("tel-country-code"), CssUI.DOM.EAutofill.Tel_Country_Code);
			KEYWORD[0].Add(new AtomicString("tel-national"), CssUI.DOM.EAutofill.Tel_National);
			KEYWORD[0].Add(new AtomicString("tel-area-code"), CssUI.DOM.EAutofill.Tel_Area_Code);
			KEYWORD[0].Add(new AtomicString("tel-local"), CssUI.DOM.EAutofill.Tel_Local);
			KEYWORD[0].Add(new AtomicString("tel-local-prefix"), CssUI.DOM.EAutofill.Tel_Local_Prefix);
			KEYWORD[0].Add(new AtomicString("tel-local-suffix"), CssUI.DOM.EAutofill.Tel_Local_Suffix);
			KEYWORD[0].Add(new AtomicString("tel-extension"), CssUI.DOM.EAutofill.Tel_Extension);
			KEYWORD[0].Add(new AtomicString("email"), CssUI.DOM.EAutofill.Email);
			KEYWORD[0].Add(new AtomicString("impp"), CssUI.DOM.EAutofill.Impp);
			#endregion
				

			/* CssUI.DOM.EContentEditable */
			#region CssUI.DOM.EContentEditable
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[1].Add(new AtomicString("true"), CssUI.DOM.EContentEditable.True);
			KEYWORD[1].Add(new AtomicString("false"), CssUI.DOM.EContentEditable.False);
			KEYWORD[1].Add(new AtomicString("inherit"), CssUI.DOM.EContentEditable.Inherit);
			#endregion
				

		#endregion
		}
		#endregion
	}
}

