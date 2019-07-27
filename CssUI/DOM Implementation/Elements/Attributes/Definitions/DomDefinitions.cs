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
            // Finally all of our definitions go into this global readonly map of attribute definitions!
            AttributeDefinitions = new ReadOnlyDictionary<AtomicName<EAttributeName>, AttributeDefinition>(AttrDict);
        }



        static IEnumerable<AttributeDefinition> Create_Attribute_Definitions()
        {
            return new AttributeDefinition[]
            {
                new AttributeDefinition(EAttributeName.Width, EAttributeType.Integer, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Height, EAttributeType.Integer, null, null, EAttributeFlags.None),

                new AttributeDefinition(EAttributeName.ID, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Class, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Slot, EAttributeType.String, null, null, EAttributeFlags.None),

                new AttributeDefinition(EAttributeName.IS, EAttributeType.String, null, null, EAttributeFlags.None),
                

                new AttributeDefinition(EAttributeName.Media, EAttributeType.String, null, null, EAttributeFlags.None),

                /* HTML Element Attributes */
                new AttributeDefinition(EAttributeName.Title, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Lang, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Translate, EAttributeType.Boolean, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Dir, EAttributeType.Enumerated, null, null, EAttributeFlags.None, Lookup.Get_Keywords<EDir>()),
                new AttributeDefinition(EAttributeName.Nonce, EAttributeType.String, null, null, EAttributeFlags.None),

                /* The tabIndex IDL attribute must reflect the value of the tabindex content attribute. The default value is 0 if the element is an a, area, button, iframe, input, select, or textarea element, or is a summary element that is a summary for its parent details. The default value is −1 otherwise. */
                new AttributeDefinition(EAttributeName.TabIndex, EAttributeType.Integer, AttributeValue.NegativeOne, AttributeValue.NegativeOne, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Disabled, EAttributeType.Boolean, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Hidden, EAttributeType.Boolean, null, null, EAttributeFlags.None),

                new AttributeDefinition(EAttributeName.AccessKey, EAttributeType.KeyCombo, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Draggable, EAttributeType.Enumerated, AttributeValue.From_Enum(EDraggable.Auto), AttributeValue.From_Enum(EDraggable.Auto), EAttributeFlags.None, Lookup.Get_Keywords<EDraggable>(), typeof(EDraggable)),

                new AttributeDefinition(EAttributeName.Spellcheck, EAttributeType.Enumerated, AttributeValue.From_Enum(ESpellcheck.Default), AttributeValue.From_Enum(ESpellcheck.Default), EAttributeFlags.None, Lookup.Get_Keywords<ESpellcheck>()),
                new AttributeDefinition(EAttributeName.Autocapitalize, EAttributeType.Enumerated, AttributeValue.From_Enum(EAutoCapitalizationHint.Default), AttributeValue.From_Enum(EAutoCapitalizationHint.Sentences), EAttributeFlags.None, Lookup.Get_Keywords<EAutoCapitalizationHint>()),

                new AttributeDefinition(EAttributeName.ContentEditable, EAttributeType.Enumerated, null, null, EAttributeFlags.None, Lookup.Get_Keywords<EContentEditable>(), typeof(EContentEditable)),

                /* Image Attributes */
                new AttributeDefinition(EAttributeName.Src, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.SrcSet, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Alt, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Sizes, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.CrossOrigin, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.UseMap, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.IsMap, EAttributeType.Boolean, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.ReferrerPolicy, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Decoding, EAttributeType.String, null, null, EAttributeFlags.None),

                
                /* LABEL ATTRIBUTES */
                new AttributeDefinition(EAttributeName.For, EAttributeType.String, null, null, EAttributeFlags.None),


                /* Table Attributes */
                new AttributeDefinition(EAttributeName.Scope, EAttributeType.Enumerated, AttributeValue.From_Enum(ECellScope.Auto), AttributeValue.From_Enum(ECellScope.Auto), EAttributeFlags.None, Lookup.Get_Keywords<ECellScope>()),
                new AttributeDefinition(EAttributeName.Abbr, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Span, EAttributeType.NonNegative_Integer, AttributeValue.One, AttributeValue.One, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.ColSpan, EAttributeType.NonNegative_Integer, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.RowSpan, EAttributeType.NonNegative_Integer, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Cols, EAttributeType.NonNegative_Integer, AttributeValue.From_Integer(20U), null, EAttributeFlags.None, null, null, lowerRange: 1u),
                new AttributeDefinition(EAttributeName.Rows, EAttributeType.NonNegative_Integer, AttributeValue.From_Integer(2U), null, EAttributeFlags.None, null, null, lowerRange: 1u),


                /* Input Attributes */
                new AttributeDefinition(EAttributeName.Type, EAttributeType.Enumerated, AttributeValue.From_Enum(EInputType.Text), AttributeValue.From_Enum(EInputType.Text), EAttributeFlags.None, Lookup.Get_Keywords<EInputType>()),
                new AttributeDefinition(EAttributeName.InputMode, EAttributeType.Enumerated, null, null, EAttributeFlags.None, Lookup.Get_Keywords<EInputMode>()),
                new AttributeDefinition(EAttributeName.EnterKeyHint, EAttributeType.Enumerated, null, null, EAttributeFlags.None, Lookup.Get_Keywords<EEnterKeyHint>()),
                new AttributeDefinition(EAttributeName.Autocomplete, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Autofocus, EAttributeType.Boolean, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Multiple, EAttributeType.Boolean, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Required, EAttributeType.Boolean, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Size, EAttributeType.NonNegative_Integer, AttributeValue.From_Integer(0u), null, EAttributeFlags.None),


                /* FORM Attributes */
                new AttributeDefinition(EAttributeName.Form, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Value, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Selected, EAttributeType.Boolean, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Dirname, EAttributeType.Enumerated, null, null, EAttributeFlags.None, Lookup.Get_Keywords<EDirName>()),

            };
        }
    }
}
