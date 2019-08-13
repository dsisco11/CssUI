using CssUI;
using CssUI.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace CssUI_XUnitTests
{
    /// <summary>
    /// These tests will ensure all enumerations marked as such have been put in a LUT
    /// </summary>
    public class Enum_Lookup_Tables
    {
        public List<Type> metaEnumList = new List<Type>();

        public Enum_Lookup_Tables()
        {
            metaEnumList = new List<Type>();

            var allTypes = System.Reflection.Assembly.GetExecutingAssembly().DefinedTypes;
            Attribute attr = null;

            foreach (Type type in allTypes)
            {
                attr = type.GetCustomAttribute(typeof(CssUI.Internal.MetaEnumAttribute));
                if (attr != null)
                {
                    metaEnumList.Add(type);
                }
            }
        }

        /// <summary>
        /// Ensures that all enum types flagged as a DomEnum have been compiled into a LUT
        /// </summary>
        [Fact]
        public void Test_Meta_Enums()
        {
            foreach(var enumType in metaEnumList)
            {
                /* Compile a list of all values from this enum */
                var allValues = Enum.GetValues(enumType);
                foreach (var value in allValues)
                {
                    string keyword = Enum.GetName(enumType, value);
                    Assert.True( Lookup.Is_Declared(enumType, keyword) );
                }
            }
        }

    }
}
