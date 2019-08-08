using CssUI.DOM.Enums;
using CssUI.HTML;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CssUI.DOM
{
    public static class DomDefinitions
    {
        internal static readonly ReadOnlyDictionary<AtomicName<EAttributeName>, List<AttributeDefinition>> AttributeDefinitions;

        static DomDefinitions()
        {
            var AttrDefs = Create_Attribute_Definitions();
            // Add all of our definitions to a backing dictionary
            var AttrDict = new Dictionary<AtomicName<EAttributeName>, List<AttributeDefinition>>();
            foreach (var def in AttrDefs)
            {
                if (!AttrDict.ContainsKey(def.Name))
                {
                    AttrDict.Add(def.Name, new List<AttributeDefinition>(3));
                }

                AttrDict[def.Name].Add(def);
            }
            // Finally all of our definitions go into this global readonly map of attribute definitions!
            AttributeDefinitions = new ReadOnlyDictionary<AtomicName<EAttributeName>, List<AttributeDefinition>>(AttrDict);
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

                new AttributeDefinition(EAttributeName.EncType, EAttributeType.Enumerated, AttributeValue.From_Enum(EEncType.FormData), AttributeValue.From_Enum(EEncType.FormData), EAttributeFlags.None, Lookup.Get_Keywords<EEncType>(), typeof(EEncType)),
                new AttributeDefinition(EAttributeName.Method, EAttributeType.Enumerated, AttributeValue.From_Enum(EFormMethod.Get), AttributeValue.From_Enum(EFormMethod.Get), EAttributeFlags.None, Lookup.Get_Keywords<EFormMethod>(), typeof(EFormMethod)),
                new AttributeDefinition(EAttributeName.NoValidate, EAttributeType.Boolean, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Target, EAttributeType.String, null, null, EAttributeFlags.None),


                /* Link Attributes */
                new AttributeDefinition(EAttributeName.Rel, EAttributeType.String, null, null, EAttributeFlags.None),


                /* Dialog Attributes */
                new AttributeDefinition(EAttributeName.Open, EAttributeType.Boolean, null, null, EAttributeFlags.None),

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
                
                /* QUOTE ATTRIBUTES */
                new AttributeDefinition(EAttributeName.Cite, EAttributeType.String, null, null, EAttributeFlags.None),


                /* Table Attributes */
                new AttributeDefinition(EAttributeName.Scope, EAttributeType.Enumerated, AttributeValue.From_Enum(ECellScope.Auto), AttributeValue.From_Enum(ECellScope.Auto), EAttributeFlags.None, Lookup.Get_Keywords<ECellScope>()),
                new AttributeDefinition(EAttributeName.Abbr, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Span, EAttributeType.NonNegative_Integer, AttributeValue.One, AttributeValue.One, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.ColSpan, EAttributeType.NonNegative_Integer, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.RowSpan, EAttributeType.NonNegative_Integer, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Cols, EAttributeType.NonNegative_Integer, AttributeValue.From_Integer(20U), null, EAttributeFlags.None, null, null, lowerRange: 1u),
                new AttributeDefinition(EAttributeName.Rows, EAttributeType.NonNegative_Integer, AttributeValue.From_Integer(2U), null, EAttributeFlags.None, null, null, lowerRange: 1u),


                /* Input Attributes */
                new AttributeDefinition(typeof(HTMLInputElement), EAttributeName.Type, EAttributeType.Enumerated, AttributeValue.From_Enum(EInputType.Text), AttributeValue.From_Enum(EInputType.Text), EAttributeFlags.None, Lookup.Get_Keywords<EInputType>()),
                new AttributeDefinition(EAttributeName.InputMode, EAttributeType.Enumerated, null, null, EAttributeFlags.None, Lookup.Get_Keywords<EInputMode>()),
                new AttributeDefinition(EAttributeName.EnterKeyHint, EAttributeType.Enumerated, null, null, EAttributeFlags.None, Lookup.Get_Keywords<EEnterKeyHint>()),
                new AttributeDefinition(EAttributeName.Autocomplete, EAttributeType.Enumerated, AttributeValue.From_Enum(EAutoComplete.On), AttributeValue.From_Enum(EAutoComplete.On), EAttributeFlags.None, Lookup.Get_Keywords<EAutoComplete>()),
                new AttributeDefinition(EAttributeName.Autofocus, EAttributeType.Boolean, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Multiple, EAttributeType.Boolean, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Required, EAttributeType.Boolean, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Size, EAttributeType.NonNegative_Integer, AttributeValue.From_Integer(0u), null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Placeholder, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.MinLength, EAttributeType.NonNegative_Integer, null, null, EAttributeFlags.None, null, null),
                new AttributeDefinition(EAttributeName.MaxLength, EAttributeType.NonNegative_Integer, null, null, EAttributeFlags.None, null, null),

                new AttributeDefinition(EAttributeName.Accept, EAttributeType.String, null, null, EAttributeFlags.None),

                new AttributeDefinition(EAttributeName.Action, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.FormAction, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.FormEncType, EAttributeType.Enumerated, null, AttributeValue.From_Enum(EEncType.FormData), EAttributeFlags.None, Lookup.Get_Keywords<EEncType>(), typeof(EEncType)),
                new AttributeDefinition(EAttributeName.FormMethod, EAttributeType.Enumerated, null, null, EAttributeFlags.None, Lookup.Get_Keywords<EFormMethod>(), typeof(EFormMethod)),
                new AttributeDefinition(EAttributeName.FormNoValidate, EAttributeType.Boolean, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.FormTarget, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.List, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Min, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Max, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Pattern, EAttributeType.String, null, null, EAttributeFlags.None),


                /* TextArea Attributes */
                new AttributeDefinition(EAttributeName.ReadOnly, EAttributeType.Boolean, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Wrap, EAttributeType.String, null, null, EAttributeFlags.None),


                /* FORM Attributes */
                new AttributeDefinition(EAttributeName.Form, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Value, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Selected, EAttributeType.Boolean, null, null, EAttributeFlags.None),
                new AttributeDefinition(EAttributeName.Dirname, EAttributeType.Enumerated, null, null, EAttributeFlags.None, Lookup.Get_Keywords<EDirName>()),
                new AttributeDefinition(EAttributeName.AcceptCharset, EAttributeType.String, null, null, EAttributeFlags.None),


                /* Object Attributes */
                new AttributeDefinition(EAttributeName.Data, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(typeof(HTMLObjectElement), EAttributeName.Type, EAttributeType.String, null, null, EAttributeFlags.None),


                /* Button Attributes */
                new AttributeDefinition(typeof(HTMLButtonElement), EAttributeName.Type, EAttributeType.Enumerated, AttributeValue.From_Enum(EButtonType.Submit), AttributeValue.From_Enum(EButtonType.Submit), EAttributeFlags.None, Lookup.Get_Keywords<EButtonType>()),


                /* IFrame Attributes */
                new AttributeDefinition(typeof(HTMLIFrameElement), EAttributeName.Sandbox, EAttributeType.Enumerated, null, null, EAttributeFlags.None, null, null, null, null, Lookup.Get_Keywords<ESandbox>()),
                new AttributeDefinition(typeof(HTMLIFrameElement), EAttributeName.Srcdoc, EAttributeType.String, null, null, EAttributeFlags.None),
                new AttributeDefinition(typeof(HTMLIFrameElement), EAttributeName.Allow, EAttributeType.String, null, null, EAttributeFlags.None),
            };
        }
    }
}
