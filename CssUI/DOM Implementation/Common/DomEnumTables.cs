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
namespace CssUI.DOM.Internal
{
	internal static class DomEnumTables
	{
        public static int Get_Enum_Index<Ty>()
        {
            switch(typeof(Ty).Name)
            {
				case "EContentEditable": return 0;
				default: throw new NotImplementedException($"No lookup-index has defined for enum type '{typeof(Ty).Name}'");
            }
        }


		#region Static Enum Tables
		internal static readonly string[][] TABLE;
		internal static readonly List<Dictionary<AtomicString, dynamic>> KEYWORD;

		static DomEnumTables()
		{
			TABLE = new string[2][];
			KEYWORD = new List<Dictionary<AtomicString, dynamic>>(2);
			int maxIndex = 0;
			/* CssUI.DOM.Enums.EContentEditable */
			maxIndex = (int)Enum.GetValues(typeof(CssUI.DOM.Enums.EContentEditable)).Cast<CssUI.DOM.Enums.EContentEditable>().Max();
			TABLE[0] = new string[maxIndex+1];
			TABLE[0][(int)CssUI.DOM.Enums.EContentEditable.True] = "true";
			TABLE[0][(int)CssUI.DOM.Enums.EContentEditable.False] = "false";
			TABLE[0][(int)CssUI.DOM.Enums.EContentEditable.Inherit] = "inherit";



		/* Generate Reverse lookup maps */
			/* CssUI.DOM.Enums.EContentEditable */
			KEYWORD.Add(new Dictionary<AtomicString, dynamic>());
			KEYWORD[0].Add(new AtomicString("true"), CssUI.DOM.Enums.EContentEditable.True);
			KEYWORD[0].Add(new AtomicString("false"), CssUI.DOM.Enums.EContentEditable.False);
			KEYWORD[0].Add(new AtomicString("inherit"), CssUI.DOM.Enums.EContentEditable.Inherit);


		}
		#endregion
	}
}

