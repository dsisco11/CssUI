﻿using CssUI.DOM.Enums;
using CssUI.DOM.Events;
using CssUI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.DOM
{
    /// <summary>
    /// Provides utility functions for form and form-related elements
    /// </summary>
    public static class FormCommon
    {

        public static void Append_Entry_To_List(ref List<Tuple<string, FormDataEntryValue>> entryList, string name, dynamic value, bool prevent_line_break_normalization_flag = false)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#append-an-entry */
            /* 1) For name, replace every occurrence of U+000D (CR) not followed by U+000A (LF), and every occurrence of U+000A (LF) not preceded by U+000D (CR), by a string consisting of a U+000D (CR) and U+000A (LF). */
            string replace = new string(new char[] { UnicodeCommon.CHAR_CARRIAGE_RETURN, UnicodeCommon.CHAR_LINE_FEED });
            var normalizedName = StringCommon.Replace(name.AsMemory(), FilterCRLF.Instance, replace.AsSpan());

            /* 2) Replace name with the result of converting to a sequence of Unicode scalar values. */
            name = UnicodeCommon.Convert_To_Scalar_Values(normalizedName.AsMemory());

            /* 3) If value is not a File object, then: */
            if (!(value is FileBlob valueFile))
            {
                if (!(value is string))
                {
                    throw new ArgumentException($"Expected value to be string, not: {value.GetType().Name}");
                }

                if (!prevent_line_break_normalization_flag)
                {
                    string normalizedValue = StringCommon.Replace(((string)value).AsMemory(), FilterCRLF.Instance, replace.AsSpan());
                    value = UnicodeCommon.Convert_To_Scalar_Values(normalizedValue.AsMemory());
                }

                var entryValue = new FormDataEntryValue(EFormDataValueType.String, name, value);
                entryList.Add(new Tuple<string, FormDataEntryValue>(name, entryValue));
            }
            else
            {
                var entryValue = new FormDataEntryValue(valueFile.name.Equals("blob") ? EFormDataValueType.Blob : EFormDataValueType.File, name, value);
                entryList.Add(new Tuple<string, FormDataEntryValue>(name, entryValue));
            }
        }

        public static void Run_Entry_Construction_Algorithm(ref List<Tuple<string, FormDataEntryValue>> entryList, Element element)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#face-entry-construction */
            if (!DOMCommon.Is_Form_Associated_Custom_Element(element))
            {
                return;
            }

            /* 1) If element's submission value is a list of entries, then append each item of element's submission value to entry list, and return. */
            IFormAssociatedElement formElement = (IFormAssociatedElement)element;
            if (formElement.submission_value is FormData data)
            {
                foreach (var entry in data)
                {
                    entryList.Add(new Tuple<string, FormDataEntryValue>(entry.Item1, entry.Item2));
                }
            }

            /* 2) If the element does not have a name attribute specified, or its name attribute's value is the empty string, then return. */
            if (!element.hasAttribute(EAttributeName.Name))
            {
                return;
            }

            var nameAttr = element.getAttribute(EAttributeName.Name);
            if (nameAttr.Get_String().Length <= 0)
            {
                return;
            }

            /* 3) If the element's submission value is not null, append an entry to entry list with the name attribute value and the submission value. */
            if (!(formElement.submission_value is null))
            {
                Append_Entry_To_List(ref entryList, nameAttr.Get_String(), formElement.submission_value);
            }
        }

        #region Forms
        public static void Reset_Form_Owner(Element element)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#reset-the-form-owner */
            if (!(element is IFormAssociatedElement formElement))
                return;


            /* 1) Unset element's parser inserted flag. */
            formElement.bParserInserted = false;

            /* 2) If all of the following conditions are true 
                *      element's form owner is not null
                *      element is not listed or its form content attribute is not present
                *      element's form owner is its nearest form element ancestor after the change to the ancestor chain
                * then do nothing, and return. */
            if (formElement.form != null)
            {
                if (!DOMCommon.Is_Listed_Element(element) || !element.hasAttribute(EAttributeName.Form))
                {
                    var nearestForm = DOMCommon.Get_Nth_Ancestor(element, 1, FilterForms.Instance, ENodeFilterMask.SHOW_ELEMENT);
                    if (ReferenceEquals(formElement.form, nearestForm))
                    {
                        /* Do nothing, and return */
                        return;
                    }
                }
            }

            formElement.form = null;

            /* 4) If element is listed, has a form content attribute, and is connected, then: */
            if (DOMCommon.Is_Listed_Element(element) && element.hasAttribute(EAttributeName.Form) && element.isConnected)
            {
                /* 1) If the first element in element's tree, in tree order, to have an ID that is case-sensitively equal to element's form content attribute's value, is a form element, then associate the element with that form element. */
                var idValue = element.getAttribute(EAttributeName.Form).Get_String();
                Element searchResult = (Element)DOMCommon.Get_Nth_Ancestor(element, 1, new FilterAttribute(EAttributeName.ID, AttributeValue.From_String(idValue)), ENodeFilterMask.SHOW_ELEMENT);
                if (searchResult != null && searchResult is HTMLFormElement form)
                {
                    formElement.form = form;
                }
            }
            /* 5) Otherwise, if element has an ancestor form element, then associate element with the nearest such ancestor form element. */
            else
            {
                var nearestForm = DOMCommon.Get_Nth_Ancestor(element, 1, FilterForms.Instance, ENodeFilterMask.SHOW_ELEMENT);
                if (nearestForm != null)
                {
                    formElement.form = (HTMLFormElement)nearestForm;
                }
            }
        }


        public static List<Tuple<string, FormDataEntryValue>> Construct_Form_Entry_List(HTMLFormElement form, Element submitter = null, string encoding = null)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#constructing-the-form-data-set */
            if (form.bConstructingEntryList)
                return null;

            form.bConstructingEntryList = true;

            var controls = (IEnumerable<Element>)DOMCommon.Get_Descendents(form, FilterIsSubmittable.Instance, ENodeFilterMask.SHOW_ELEMENT).Where(child => child is IFormAssociatedElement childElement && ReferenceEquals(form, childElement.form));
            List<Tuple<string, FormDataEntryValue>> entryList = new List<Tuple<string, FormDataEntryValue>>();

            /* 5) For each element field in controls, in tree order: */
            foreach (Element field in controls)
            {
                /* 
                 * If any of the following is true:
                 *      The field element is disabled.
                 *      The field element is a button but it is not submitter.
                 *      The field element is an input element whose type attribute is in the Checkbox state and whose checkedness is false.
                 *      The field element is an input element whose type attribute is in the Radio Button state and whose checkedness is false.
                 *      The field element is an object element that is not using a plugin.
                 *      The field element has a datalist element ancestor.
                 *  Then continue.
                 */
                if ((field as HTMLElement).disabled)
                {
                    continue;
                }
                else if (field is HTMLButtonElement && !ReferenceEquals(field, submitter))
                {
                    continue;
                }
                else if (field is HTMLInputElement)
                {
                    var inputElement = (HTMLInputElement)field;
                    if (inputElement.type == EInputType.Checkbox && !inputElement.Checked)
                    {
                        continue;
                    }
                    else if (inputElement.type == EInputType.Radio && !inputElement.Checked)
                    {
                        continue;
                    }
                }
                else if (field is HTMLObjectElement objectElement && objectElement.plugin == null)
                {
                    continue;
                }
                else if (null != DOMCommon.Get_Nth_Ancestor(field, 1, new FilterElementType(typeof(HTMLDataListElement)), ENodeFilterMask.SHOW_ELEMENT))
                {
                    continue;
                }



                /* 2) If the field element is an input element whose type attribute is in the Image Button state, then: */
                if (field is HTMLInputElement)
                {
                    var inputElement = (HTMLInputElement)field;
                    if (inputElement.type == EInputType.Image)
                    {
                        string inputName = inputElement.name.Length > 0 ? inputElement.name : string.Empty;

                        var namex = string.Concat(inputName, UnicodeCommon.CHAR_FULL_STOP, UnicodeCommon.CHAR_X_LOWER);
                        var namey = string.Concat(inputName, UnicodeCommon.CHAR_FULL_STOP, UnicodeCommon.CHAR_Y_LOWER);

                        /* 4) The field element is submitter, and before this algorithm was invoked the user indicated a coordinate. 
                         * Let x be the x-component of the coordinate selected by the user, and let y be the y-component of the coordinate selected by the user. */
                        if (ReferenceEquals(field, submitter))
                        {
                            /* 5) Append an entry to entry list with namex and x. */
                            Append_Entry_To_List(ref entryList, namex, inputElement.selected_coordinate_x);
                            /* 6) Append an entry to entry list with namey and y. */
                            Append_Entry_To_List(ref entryList, namey, inputElement.selected_coordinate_y);
                            /* 7) Continue */
                            continue;
                        }
                    }
                }

                /* 3) If the field is a form-associated custom element, then perform the entry construction algorithm given field and entry list, then continue. */
                if (DOMCommon.Is_Form_Associated_Custom_Element(field))
                {
                    Run_Entry_Construction_Algorithm(ref entryList, field);
                    continue;
                }

                /* 4) If either the field element does not have a name attribute specified, or its name attribute's value is the empty string, then continue. */
                var nameValue = field.getAttribute(EAttributeName.Name);
                if (nameValue == null || nameValue.Get_String().Length <= 0)
                {
                    continue;
                }
                string name = nameValue.Get_String();

                /* 6) If the field element is a select element, then for each option element in the select element's list of options whose selectedness is true and that is not disabled, append an entry to entry list with name and the value of the option element. */
                if (field is HTMLSelectElement selectElement)
                {
                    foreach (var option in selectElement.options)
                    {
                        if (option.selectedness && !option.disabled)
                        {
                            Append_Entry_To_List(ref entryList, name, option.value);
                        }
                    }
                }
                /* 7) Otherwise, if the field element is an input element whose type attribute is in the Checkbox state or the Radio Button state, then: */
                else if (field is HTMLInputElement && ((field as HTMLInputElement).type == EInputType.Checkbox || (field as HTMLInputElement).type == EInputType.Radio))
                {
                    var inputElement = (HTMLInputElement)field;
                    if (inputElement.type == EInputType.Checkbox || inputElement.type == EInputType.Radio)
                    {
                        string value = inputElement.getAttribute(EAttributeName.Value)?.Get_String() ?? "on";
                        Append_Entry_To_List(ref entryList, name, value);
                    }
                }
                /* 8) Otherwise, if the field element is an input element whose type attribute is in the File Upload state, then: */
                else if (field is HTMLInputElement && (field as HTMLInputElement).type == EInputType.File)
                {
                    var inputElement = (HTMLInputElement)field;
                    /* 1) If there are no selected files, then append an entry to entry list with name and a new File object with an empty name, application/octet-stream as type, and an empty body. */
                    if (!inputElement.files.Any())
                    {
                        FileBlob file = new FileBlob(null, string.Empty, new FilePropertyBag() { type = "application/octet-stream" });
                        Append_Entry_To_List(ref entryList, name, file);
                    }
                    else
                    {
                        foreach (FileBlob file in inputElement.files)
                        {
                            Append_Entry_To_List(ref entryList, name, file);
                        }
                    }
                }
                /* 9) Otherwise, if the field element is an object element: try to obtain a form submission value from the plugin, and if that is successful, append an entry to entry list with name and the returned form submission value. */
                else if (field is HTMLObjectElement)
                {
                }
                /* 10) Otherwise, if the field element is an input element whose type attribute is in the Hidden state and name is "_charset_": */
                else if (field is HTMLInputElement && (field as HTMLInputElement).type == EInputType.Hidden && (field as HTMLInputElement).name.Equals("_charset_"))
                {
                    var inputElement = (HTMLInputElement)field;
                    var charset = string.IsNullOrEmpty(inputElement.formEnctype) ? "UTF-8" : inputElement.formEnctype;
                    Append_Entry_To_List(ref entryList, name, charset);
                }
                /* 11) Otherwise, if the field element is a textarea element, append an entry to entry list with name and the value of the field element, and the prevent line break normalization flag set. */
                else if (field is HTMLTextAreaElement)
                {
                    var textElement = (HTMLTextAreaElement)field;
                    Append_Entry_To_List(ref entryList, name, textElement.value);
                }
                /* 12) Otherwise, append an entry to entry list with name and the value of the field element. */
                else
                {
                    var formElement = (IFormAssociatedElement)field;
                    Append_Entry_To_List(ref entryList, name, formElement.get_value());
                }

                /* 13) If the element has a dirname attribute, and that attribute's value is not the empty string, then: */
                if (field.hasAttribute(EAttributeName.Dirname, out Attr outDirname) && !string.IsNullOrEmpty(outDirname.Value.Get_String()))
                {
                    string dirname = outDirname.Value.Get_String();
                    string dir = (field as HTMLElement).directionality == CSS.EDirection.LTR ? "ltr" : "rtl";
                    Append_Entry_To_List(ref entryList, dirname, dir);
                }
            }

            /* 6) Let form data be a new FormData object associated with entry list. */
            var formdata = new FormData(form, entryList);

            /* 7) Fire an event named formdata at form using FormDataEvent, with the formData attribute initialized to form data and the bubbles attribute initialized to true. */
            var fdEvent = new FormDataEvent(new FormDataEventInit() { formData = formdata, bubbles = true });
            form.dispatchEvent(fdEvent);

            form.bConstructingEntryList = false;
            return entryList;
        }


        public static bool Check_Form_Validity(Element element)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#statically-validate-the-constraints */

        }


        public static bool Check_Form_Associated_Element_Barred_From_Validation(Element element)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#barred-from-constraint-validation */


            return false;
        }

        /// <summary>
        /// Returns whether a given element satisfies it's constraints
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool Check_Form_Associated_Element_Satisfies_Constraints(Element element)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#suffering-from-being-missing */

            if (DOMCommon.Is_Form_Associated_Custom_Element(element))
            {
                var formElement = ((IFormAssociatedElement)element);
                EValidityState flags = formElement.validity_flags;
                if (0 != (flags & (EValidityState.valueMissing | EValidityState.typeMismatch | EValidityState.patternMismatch | EValidityState.tooLong | EValidityState.tooShort | EValidityState.rangeUnderflow | EValidityState.rangeOverflow | EValidityState.stepMismatch | EValidityState.badInput)))
                {
                    return false;
                }

                if (!ReferenceEquals(null, formElement.custom_validity_error_message) && formElement.custom_validity_error_message.Length > 0)
                {
                    return false;
                }
            }

            /* XXX: Implement the validation for select elements and radio button groups */
            /* When a control has no value but has a required attribute (input required, textarea required); or, more complicated rules for select elements and controls in radio button groups, as specified in their sections. */
            if (element.hasAttribute(EAttributeName.Required))
            {
                if (element is HTMLInputElement inputElement && (ReferenceEquals(null, inputElement.value) || inputElement.value.Length <= 0))
                {
                    return false;
                }
            }

        }
        #endregion
    }
}
