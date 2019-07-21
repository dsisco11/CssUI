
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
                new AttributeDefinition(EAttributeName.Width, EAttributeType.Integer, null, null, 0x0),
                new AttributeDefinition(EAttributeName.Height, EAttributeType.Integer, null, null, 0x0),

                new AttributeDefinition(EAttributeName.ID, EAttributeType.String, null, null, 0x0),
                new AttributeDefinition(EAttributeName.Class, EAttributeType.String, null, null, 0x0),
                new AttributeDefinition(EAttributeName.Slot, EAttributeType.String, null, null, 0x0),
                

                new AttributeDefinition(EAttributeName.Media, EAttributeType.String, null, null, 0x0),

                /* HTML Element Attributes */
                new AttributeDefinition(EAttributeName.Title, EAttributeType.String, null, null, 0x0),
                new AttributeDefinition(EAttributeName.Lang, EAttributeType.String, null, null, 0x0),
                new AttributeDefinition(EAttributeName.Translate, EAttributeType.Boolean, null, null, 0x0),
                new AttributeDefinition(EAttributeName.Dir, EAttributeType.String, null, null, 0x0),
                new AttributeDefinition(EAttributeName.Nonce, EAttributeType.String, null, null, 0x0),

                new AttributeDefinition(EAttributeName.TabIndex, EAttributeType.Integer, null, null, 0x0),
                new AttributeDefinition(EAttributeName.Disabled, EAttributeType.Boolean, null, null, 0x0),
                new AttributeDefinition(EAttributeName.Hidden, EAttributeType.Boolean, null, null, 0x0),

                new AttributeDefinition(EAttributeName.AccessKey, EAttributeType.KeyCombo, null, null, 0x0),
                new AttributeDefinition(EAttributeName.Draggable, EAttributeType.Boolean, null, null, 0x0),
                new AttributeDefinition(EAttributeName.SpellCheck, EAttributeType.Boolean, null, null, 0x0),
                new AttributeDefinition(EAttributeName.AutoCapitalize, EAttributeType.String, null, null, 0x0),

                new AttributeDefinition(EAttributeName.ContentEditable, EAttributeType.Enumerated, null, null, 0x0, DomLookup.Get_Keywords<EContentEditable>(), typeof(EContentEditable)),

                /* Image Attributes */
                new AttributeDefinition(EAttributeName.Src, EAttributeType.String, null, null, 0x0),
                new AttributeDefinition(EAttributeName.SrcSet, EAttributeType.String, null, null, 0x0),
                new AttributeDefinition(EAttributeName.Alt, EAttributeType.String, null, null, 0x0),
                new AttributeDefinition(EAttributeName.Sizes, EAttributeType.String, null, null, 0x0),
                new AttributeDefinition(EAttributeName.CrossOrigin, EAttributeType.String, null, null, 0x0),
                new AttributeDefinition(EAttributeName.UseMap, EAttributeType.String, null, null, 0x0),
                new AttributeDefinition(EAttributeName.IsMap, EAttributeType.Boolean, null, null, 0x0),
                new AttributeDefinition(EAttributeName.ReferrerPolicy, EAttributeType.String, null, null, 0x0),
                new AttributeDefinition(EAttributeName.Decoding, EAttributeType.String, null, null, 0x0),

                /* Table Attributes */
                new AttributeDefinition(EAttributeName.Scope, EAttributeType.Enumerated, AttributeValue.From_Enum(ECellScope.Auto), AttributeValue.From_Enum(ECellScope.Auto), EAttributeFlags.None, DomLookup.Get_Keywords<ECellScope>()),
                new AttributeDefinition(EAttributeName.Abbr, EAttributeType.String, null, null, 0x0),
                new AttributeDefinition(EAttributeName.ColSpan, EAttributeType.NonNegative_Integer, null, null, 0x0),
                new AttributeDefinition(EAttributeName.RowSpan, EAttributeType.NonNegative_Integer, null, null, 0x0),

            };
        }
    }
}
