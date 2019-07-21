
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
                new AttributeDefinition(EAttributeName.Width, false, 0x0, EAttributeType.Integer, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.Height, false, 0x0, EAttributeType.Integer, MissingValueDefault: null),

                new AttributeDefinition(EAttributeName.ID, false, 0x0, EAttributeType.String, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.Class, false, 0x0, EAttributeType.String, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.Slot, false, 0x0, EAttributeType.String, MissingValueDefault: null),
                

                new AttributeDefinition(EAttributeName.Media, false, 0x0, EAttributeType.String, MissingValueDefault: null),

                /* HTML Element Attributes */
                new AttributeDefinition(EAttributeName.Title, false, 0x0, EAttributeType.String, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.Lang, false, 0x0, EAttributeType.String, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.Translate, false, 0x0, EAttributeType.Boolean, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.Dir, false, 0x0, EAttributeType.String, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.Nonce, false, 0x0, EAttributeType.String, MissingValueDefault: null),

                new AttributeDefinition(EAttributeName.TabIndex, false, 0x0, EAttributeType.Integer, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.Disabled, false, 0x0, EAttributeType.Boolean, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.Hidden, false, 0x0, EAttributeType.Boolean, MissingValueDefault: null),

                new AttributeDefinition(EAttributeName.AccessKey, false, 0x0, EAttributeType.KeyCombo, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.Draggable, false, 0x0, EAttributeType.Boolean, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.SpellCheck, false, 0x0, EAttributeType.Boolean, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.AutoCapitalize, false, 0x0, EAttributeType.String, MissingValueDefault: null),

                new AttributeDefinition(EAttributeName.ContentEditable, false, 0x0, EAttributeType.Enumerated, DomLookup.Get_Keywords<EContentEditable>(), null),

                /* Image Attributes */
                new AttributeDefinition(EAttributeName.Src, false, 0x0, EAttributeType.String, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.SrcSet, false, 0x0, EAttributeType.String, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.Alt, false, 0x0, EAttributeType.String, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.Sizes, false, 0x0, EAttributeType.String, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.CrossOrigin, false, 0x0, EAttributeType.String, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.UseMap, false, 0x0, EAttributeType.String, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.IsMap, false, 0x0, EAttributeType.Boolean, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.ReferrerPolicy, false, 0x0, EAttributeType.String, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.Decoding, false, 0x0, EAttributeType.String, MissingValueDefault: null),

                /* Table Attributes */
                new AttributeDefinition(EAttributeName.Scope, false, 0x0, EAttributeType.String, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.Abbr, false, 0x0, EAttributeType.String, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.ColSpan, false, 0x0, EAttributeType.Integer, MissingValueDefault: null),
                new AttributeDefinition(EAttributeName.RowSpan, false, 0x0, EAttributeType.Integer, MissingValueDefault: null),

            };
        }
    }
}
