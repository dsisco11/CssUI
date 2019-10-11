using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace CssUI.CORE
{
    /// <summary>
    /// A genericized object which compiles a lookup-table for any code object with a given attribute present in the executing assembly
    /// </summary>
    /// <typeparam name="TAttribute">Attribute type to search for</typeparam>
    public class MetaTable<TAttribute> where TAttribute : Attribute
    {
        #region Properties
        /// <summary>
        /// A list of all metadata items
        /// </summary>
        private readonly (TypeInfo, TAttribute)[] Items;
        /// <summary>
        /// A speedy map of type handles to their index within the table
        /// </summary>
        private readonly Dictionary<RuntimeTypeHandle, int> TypeHandleMap;
        #endregion

        #region Accessors
        public int Length => Items.Length;

        public (TypeInfo, TAttribute) this[int index] {
            get => Items[index];
            set => Items[index] = value;
        }
        #endregion

        #region Constructors
        public MetaTable(int Capacity)
        {
            Items = Find_All_Meta(Capacity);
            TypeHandleMap = CreateTypeMap();
        }
        #endregion

        #region Map Compilation
        private (TypeInfo, TAttribute)[] Find_All_Meta(int Capacity)
        {
            List<(TypeInfo, TAttribute)> RetVal = new List<(TypeInfo, TAttribute)>(Capacity);
            Type MetaAttributeType = typeof(TAttribute);
            var assembly = Assembly.GetExecutingAssembly();

            foreach (TypeInfo ty in assembly.DefinedTypes)
            {
                if (ty.IsEnum)
                {
                    var metaAttr = (TAttribute)ty.GetCustomAttribute(MetaAttributeType, false);
                    if (metaAttr != null)
                    {
                        RetVal.Add((ty, metaAttr));
                    }
                }
            }

            return RetVal.ToArray();
        }
        private Dictionary<RuntimeTypeHandle, int> CreateTypeMap()
        {
            var RetVal = new Dictionary<RuntimeTypeHandle, int>();

            for (int typeIndex = 0; typeIndex < Items.Length; typeIndex++)
            {
                RetVal.Add(Items[typeIndex].Item1.TypeHandle, typeIndex);
            }

            return RetVal;
        }
        #endregion

        #region Lookups
        /// <summary>
        /// Lookup the index of an item within the LUT by its runtime type handle (fastest method)
        /// </summary>
        /// <param name="Handle">Runtime type handle of the item to lookup</param>
        /// <param name="outIndex">Index of the item within the LUT</param>
        /// <returns>Success</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Lookup(RuntimeTypeHandle Handle, out int outIndex)
        {
            if (TypeHandleMap.TryGetValue(Handle, out int Index))
            {
                outIndex = Index;
                return true;
            }

            outIndex = -1;
            return false;
        }

        /// <summary>
        /// Lookup the index of an item within the LUT by its runtime type handle (fastest method)
        /// </summary>
        /// <param name="Handle">Runtime type handle of the item to lookup</param>
        /// <param name="outIndex">Index of the item within the LUT</param>
        /// <returns>Success</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Lookup(RuntimeTypeHandle Handle)
        {
            if (TypeHandleMap.TryGetValue(Handle, out int Index))
            {
                return Index;
            }

            return -1;
        }

        /// <summary>
        /// Lookup the index of an item within the LUT by its <see cref="Type"/>
        /// </summary>
        /// <param name="outIndex">Index of the item within the LUT</param>
        /// <returns>Success</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Lookup<T>(out int outIndex)
        {
            if (TypeHandleMap.TryGetValue(typeof(T).TypeHandle, out int outMapValue))
            {
                outIndex = outMapValue;
                return true;
            }

            outIndex = -1;
            return false;
        }

        /// <summary>
        /// Lookup the index of an item within the LUT by its <see cref="Type"/>
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <returns>Index of the item within the LUT</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Lookup<T>()
        {
            if (TypeHandleMap.TryGetValue(typeof(T).TypeHandle, out int outIndex))
            {
                return outIndex;
            }

            return -1;
        }
        #endregion
    }
}
