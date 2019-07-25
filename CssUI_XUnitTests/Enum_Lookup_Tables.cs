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
        public List<Type> domEnumList = new List<Type>();
        public List<Type> cssEnumList = new List<Type>();

        public Enum_Lookup_Tables()
        {
            domEnumList = new List<Type>();
            cssEnumList = new List<Type>();

            var allTypes = System.Reflection.Assembly.GetExecutingAssembly().DefinedTypes;
            Attribute attr = null;

            foreach (Type type in allTypes)
            {
                attr = type.GetCustomAttribute(typeof(CssUI.DOM.Internal.DomEnumAttribute));
                if (attr != null)
                {
                    domEnumList.Add(type);
                }

                attr = type.GetCustomAttribute(typeof(CssUI.CSS.Internal.CssEnumAttribute));
                if (attr != null)
                {
                    cssEnumList.Add(type);
                }
            }
        }

        /// <summary>
        /// Ensures that all enum types flagged as a DomEnum have been compiled into a LUT
        /// </summary>
        [Fact]
        public void Test_DOM_Enums()
        {
            foreach(var enumType in domEnumList)
            {
                /* Compile a list of all values from this enum */
                var allValues = Enum.GetValues(enumType);
                foreach (var value in allValues)
                {
                    string keyword = Enum.GetName(enumType, value);
                    Assert.True( DomLookup.Is_Declared(enumType, keyword) );
                }
            }
        }

        /// <summary>
        /// Ensures that all enum types flagged as a DomEnum have been compiled into a LUT
        /// </summary>
        [Fact]
        public void Test_CSS_Enums()
        {
            foreach(var enumType in cssEnumList)
            {
                /* Compile a list of all values from this enum */
                var allValues = Enum.GetValues(enumType);
                foreach (var value in allValues)
                {
                    string keyword = Enum.GetName(enumType, value);
                    Assert.True( CssLookup.Is_Declared(enumType, keyword) );
                }
            }
        }
    }
}
