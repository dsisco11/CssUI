using CssUI.CSS;
using CssUI.Fonts;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CssUI.Internal
{
    #region Delegates
    /// <summary>
    /// Provides a function that can resolve used values
    /// </summary>
    /// <param name="Property"></param>
    /// <param name="ComputedValue"></param>
    /// <returns></returns>
    public delegate dynamic PropertyResolutionDelegate(ICssProperty Property);
    #endregion


    /// <summary>
    /// Provides access to all of the functions needed for resolving the various stages of <see cref="CssProperty"/> values
    /// </summary>
    public static partial class CssPropertyResolver
    {

        #region Properties
        /// <summary>
        /// Maps CSS property names to the function used to resolve their used value
        /// </summary>
        public static ConcurrentDoubleKeyDictionary<AtomicString, ECssPropertyStage, PropertyResolutionDelegate> Map = new ConcurrentDoubleKeyDictionary<AtomicString, ECssPropertyStage, PropertyResolutionDelegate>();
        //public static Dictionary<AtomicString, Dictionary<ECssPropertyStage, PropertyResolutionDelegate>> Map = new Dictionary<AtomicString, Dictionary<ECssPropertyStage, PropertyResolutionDelegate>>(32);
        #endregion

        #region Getters

        public static PropertyResolutionDelegate Get(AtomicString Property, ECssPropertyStage Stage)
        {
            return Map[Property, Stage];
        }
        #endregion

        #region Constructor
        static CssPropertyResolver()
        {

            Map.TryAdd("font-family", ECssPropertyStage.Used, Font_Family_Used);

            Map.TryAdd("font-size", ECssPropertyStage.Computed, Font_Size_Actual);

            Map.TryAdd("font-weight", ECssPropertyStage.Computed, Font_Weight_Computed);

        }
        #endregion

    }


}
