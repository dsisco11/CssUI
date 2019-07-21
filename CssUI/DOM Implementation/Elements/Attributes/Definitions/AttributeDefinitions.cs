
using CssUI.DOM.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CssUI.DOM
{
    public static class DomDefinitions
    {
        internal static readonly ReadOnlyDictionary<AtomicName<EAttributeName>, AttributeDefinition> AttributeDefinitions;

        static DomDefinitions()
        {
            var AttrDefs = Create_Attribute_Definitions();
            // Add all of our definitions to a backing dictionary
            var AttrDict = new Dictionary<AtomicName<EAttributeName>, AttributeDefinition>();
            foreach (var def in AttrDefs) { AttrDict.Add(def.Name, def); }
            // Finally all of our definitions go into this global readonly map of Css property definitions!
            AttributeDefinitions = new ReadOnlyDictionary<AtomicName<EAttributeName>, AttributeDefinition>(AttrDict);
        }



        static IEnumerable<AttributeDefinition> Create_Attribute_Definitions()
        {
            return new AttributeDefinition[]
            {
                new AttributeDefinition(EAttributeName.Width, false, 0x0, null, EAttributeType.Numeric),
                new AttributeDefinition(EAttributeName.Height, false, 0x0, null, EAttributeType.Numeric),

                new AttributeDefinition(EAttributeName.ID, false, 0x0, null, EAttributeType.String),
                new AttributeDefinition(EAttributeName.Class, false, 0x0, null, EAttributeType.String),
                new AttributeDefinition(EAttributeName.Slot, false, 0x0, null, EAttributeType.String),
                

                new AttributeDefinition(EAttributeName.Media, false, 0x0, null, EAttributeType.String),

                /* HTML Element Attributes */
                new AttributeDefinition(EAttributeName.Title, false, 0x0, null, EAttributeType.String),
                new AttributeDefinition(EAttributeName.Lang, false, 0x0, null, EAttributeType.String),
                new AttributeDefinition(EAttributeName.Translate, false, 0x0, null, EAttributeType.Boolean),
                new AttributeDefinition(EAttributeName.Dir, false, 0x0, null, EAttributeType.String),
                new AttributeDefinition(EAttributeName.Nonce, false, 0x0, null, EAttributeType.String),

                new AttributeDefinition(EAttributeName.TabIndex, false, 0x0, null, EAttributeType.Numeric),
                new AttributeDefinition(EAttributeName.Disabled, false, 0x0, null, EAttributeType.Boolean),
                new AttributeDefinition(EAttributeName.Hidden, false, 0x0, null, EAttributeType.Boolean),

                new AttributeDefinition(EAttributeName.AccessKey, false, 0x0, null, EAttributeType.KeyCombo),
                new AttributeDefinition(EAttributeName.Draggable, false, 0x0, null, EAttributeType.Boolean),
                new AttributeDefinition(EAttributeName.SpellCheck, false, 0x0, null, EAttributeType.Boolean),
                new AttributeDefinition(EAttributeName.AutoCapitalize, false, 0x0, null, EAttributeType.String),

                new AttributeDefinition(EAttributeName.ContentEditable, false, 0x0, null, EAttributeType.Enumerated, DomLookup.Get_Keywords<EContentEditable>()),

                /* Image Attributes */
                new AttributeDefinition(EAttributeName.Src, false, 0x0, null, EAttributeType.String),
                new AttributeDefinition(EAttributeName.SrcSet, false, 0x0, null, EAttributeType.String),
                new AttributeDefinition(EAttributeName.Alt, false, 0x0, null, EAttributeType.String),
                new AttributeDefinition(EAttributeName.Sizes, false, 0x0, null, EAttributeType.String),
                new AttributeDefinition(EAttributeName.CrossOrigin, false, 0x0, null, EAttributeType.String),
                new AttributeDefinition(EAttributeName.UseMap, false, 0x0, null, EAttributeType.String),
                new AttributeDefinition(EAttributeName.IsMap, false, 0x0, null, EAttributeType.Boolean),
                new AttributeDefinition(EAttributeName.ReferrerPolicy, false, 0x0, null, EAttributeType.String),
                new AttributeDefinition(EAttributeName.Decoding, false, 0x0, null, EAttributeType.String),

                /* Table Attributes */
                new AttributeDefinition(EAttributeName.Scope, false, 0x0, null, EAttributeType.String),
                new AttributeDefinition(EAttributeName.Abbr, false, 0x0, null, EAttributeType.String),
                new AttributeDefinition(EAttributeName.ColSpan, false, 0x0, null, EAttributeType.Numeric),
                new AttributeDefinition(EAttributeName.RowSpan, false, 0x0, null, EAttributeType.Numeric),

            };
        }
    }
}
