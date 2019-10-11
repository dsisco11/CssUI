using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using CssUI.CORE;

namespace CssUI.HTML
{
    /// <summary>
    /// Compiles (at runtime) a table of metadata for all values of any class marked with the <see cref="MetaElementAttribute"/>.
    /// </summary>
    internal static class HTMLElementTable
    {
        #region Lookup Tables
        public static readonly MetaTable<MetaElementAttribute> Meta = new MetaTable<MetaElementAttribute>(64);
        public static readonly ElementMetadata[] TABLE;
        /// <summary>
        /// Maps meta-keyword strings to their actual metadata
        /// </summary>
        public static readonly Dictionary<AtomicString, ElementMetadata> KEYWORD;
        #endregion

        /// <summary>
        /// Populates the metadata tables
        /// </summary>
        static HTMLElementTable()
        {
            // Allocate space for the first level of our tables
            TABLE = new ElementMetadata[Meta.Length];
            KEYWORD = new Dictionary<AtomicString, ElementMetadata>(Meta.Length);

            // Loop through all meta-enums and populate both of their tables
            for (int index = 0; index < Meta.Length; index++)
            {
                // Find all the meta-values for this item
                (Type, MetaElementAttribute) metaInfo = Meta[index];
                // Initialize the tables for this item
                ElementMetadata data = new ElementMetadata(metaInfo.Item2.Name, metaInfo.Item1);
                TABLE[index] = data;
                KEYWORD.Add(new AtomicString(data.LocalName), data);
            }

        }

        /// <summary>
        /// Dumps all compiled metadata into the given file as a Json structure
        /// </summary>
        /// <param name="filename"></param>
        public static async Task Dump_Metadata(string filename)
        {
            using System.IO.Stream Stream = new System.IO.FileStream(filename, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, System.IO.FileShare.None);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            await JsonSerializer.SerializeAsync(Stream, TABLE).ConfigureAwait(continueOnCapturedContext: false);
        }

    }
}
