﻿using CssUI.DOM;
using CssUI.DOM.Enums;
using CssUI.DOM.Events;
using CssUI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.HTML
{
    /// <summary>
    /// Provides utility functions for form and form-related elements
    /// </summary>
    public static class FormCommon
    {

        static string strNewline = new string(new char[] { UnicodeCommon.CHAR_CARRIAGE_RETURN, UnicodeCommon.CHAR_LINE_FEED });
        public static void Append_Entry_To_List(ref List<Tuple<string, FormDataEntryValue>> entryList, string name, dynamic value, bool prevent_line_break_normalization_flag = false)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#append-an-entry */
            /* 1) For name, replace every occurrence of U+000D (CR) not followed by U+000A (LF), and every occurrence of U+000A (LF) not preceded by U+000D (CR), by a string consisting of a U+000D (CR) and U+000A (LF). */
            var normalizedName = StringCommon.Replace(name.AsMemory(), FilterCRLF.Instance, strNewline.AsSpan());

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
                    string normalizedValue = StringCommon.Replace(((string)value).AsMemory(), FilterCRLF.Instance, strNewline.AsSpan());
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
            FormAssociatedElement formElement = (FormAssociatedElement)element;
            if (formElement.submission_value.Type == ESubmissionValue.EntryList)
            {
                FormData data = formElement.submission_value.Value;
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
            if (nameAttr.AsString().Length <= 0)
            {
                return;
            }

            /* 3) If the element's submission value is not null, append an entry to entry list with the name attribute value and the submission value. */
            if (!(formElement.submission_value is null))
            {
                Append_Entry_To_List(ref entryList, nameAttr.AsString(), formElement.submission_value);
            }
        }

        #region Forms
        public static void Reset_Form_Owner(FormAssociatedElement element)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#reset-the-form-owner */

            /* 1) Unset element's parser inserted flag. */
            element.bParserInserted = false;

            /* 2) If all of the following conditions are true 
                *      element's form owner is not null
                *      element is not listed or its form content attribute is not present
                *      element's form owner is its nearest form element ancestor after the change to the ancestor chain
                * then do nothing, and return. */
            if (element.form != null)
            {
                if (!DOMCommon.Is_Listed_Element(element) || !element.hasAttribute(EAttributeName.Form))
                {
                    var nearestForm = DOMCommon.Get_Nth_Ancestor<HTMLFormElement>(element, 1, null, ENodeFilterMask.SHOW_ELEMENT);
                    if (ReferenceEquals(element.form, nearestForm))
                    {
                        /* Do nothing, and return */
                        return;
                    }
                }
            }

            element.form = null;

            /* 4) If element is listed, has a form content attribute, and is connected, then: */
            if (DOMCommon.Is_Listed_Element(element) && element.hasAttribute(EAttributeName.Form) && element.isConnected)
            {
                /* 1) If the first element in element's tree, in tree order, to have an ID that is case-sensitively equal to element's form content attribute's value, is a form element, then associate the element with that form element. */
                var idValue = element.getAttribute(EAttributeName.Form).AsString();
                Element searchResult = (Element)DOMCommon.Get_Nth_Ancestor(element, 1, new FilterAttribute(EAttributeName.ID, AttributeValue.From(idValue)), ENodeFilterMask.SHOW_ELEMENT);
                if (searchResult != null && searchResult is HTMLFormElement form)
                {
                    element.form = form;
                }
            }
            /* 5) Otherwise, if element has an ancestor form element, then associate element with the nearest such ancestor form element. */
            else
            {
                var nearestForm = DOMCommon.Get_Nth_Ancestor<HTMLFormElement>(element, 1, null, ENodeFilterMask.SHOW_ELEMENT);
                if (nearestForm != null)
                {
                    element.form = nearestForm;
                }
            }
        }


        public static List<Tuple<string, FormDataEntryValue>> Construct_Form_Entry_List(HTMLFormElement form, Element submitter = null, string encoding = null)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#constructing-the-form-data-set */
            if (form.bConstructingEntryList)
                return null;

            form.bConstructingEntryList = true;

            var controls = (IEnumerable<FormAssociatedElement>)DOMCommon.Get_Descendents(form, FilterIsSubmittable.Instance, ENodeFilterMask.SHOW_ELEMENT).Where(child => child is FormAssociatedElement childElement && ReferenceEquals(form, childElement.form));
            List<Tuple<string, FormDataEntryValue>> entryList = new List<Tuple<string, FormDataEntryValue>>();

            /* 5) For each element field in controls, in tree order: */
            foreach (FormAssociatedElement field in controls)
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
#if ENABLE_OBJECT_PLUGINS
                else if (field is HTMLObjectElement objectElement && objectElement.Plugin is null)
                {
                    continue;
                }
#endif
                else if (null != DOMCommon.Get_Nth_Ancestor<HTMLDataListElement>(field, 1, null, ENodeFilterMask.SHOW_ELEMENT))
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
                if (nameValue == null || nameValue.AsString().Length <= 0)
                {
                    continue;
                }
                string name = nameValue.AsString();

                /* 6) If the field element is a select element, then for each option element in the select element's list of options whose selectedness is true and that is not disabled, append an entry to entry list with name and the value of the option element. */
                if (field is HTMLSelectElement selectElement)
                {
                    foreach (var option in selectElement.options)
                    {
                        if (option.selected && !option.disabled)
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
                        string value = inputElement.getAttribute(EAttributeName.Value)?.AsString() ?? "on";
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
                    var charset = inputElement.formEnctype.HasValue ? Lookup.Keyword(inputElement.formEnctype.Value) : "UTF-8";
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
                    var formElement = field;
                    Append_Entry_To_List(ref entryList, name, formElement.value);
                }

                /* 13) If the element has a dirname attribute, and that attribute's value is not the empty string, then: */
                if (field.hasAttribute(EAttributeName.Dirname, out Attr outDirname) && !string.IsNullOrEmpty(outDirname.Value.AsString()))
                {
                    string dirname = outDirname.Value.AsString();
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


        public static bool Statically_Validate_Form_Constraints(HTMLFormElement form, out IReadOnlyCollection<HTMLElement> outInvalidControls)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#statically-validate-the-constraints */

            var controls = (IEnumerable<FormAssociatedElement>)DOMCommon.Get_Descendents(form, FilterIsSubmittable.Instance, ENodeFilterMask.SHOW_ELEMENT).Where(child => child is ISubmittableElement childElement && ReferenceEquals(form, childElement.form));
            LinkedList<HTMLElement> invalidElements = new LinkedList<HTMLElement>();

            foreach (var field in controls)
            {
                if (Is_Barred_From_Validation(field)) continue;
                if (field.check_satisfies_constraints()) continue;
                invalidElements.AddLast(field);
            }

            if (!invalidElements.Any())
            {
                outInvalidControls = Array.Empty<HTMLElement>();
                return true;
            }

            LinkedList<HTMLElement> unhandledInvalidElements = new LinkedList<HTMLElement>();

            foreach (var field in invalidElements)
            {
                var notCanceled = field.dispatchEvent(new Event(EEventName.Invalid, new EventInit() { cancelable = true }));
                if (notCanceled)
                {
                    unhandledInvalidElements.AddLast(field);
                }
            }

            outInvalidControls = unhandledInvalidElements;
            return false;
        }

        public static bool Interactively_Validate_Form_Constraints(HTMLFormElement form)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#interactively-validate-the-constraints */
            if (Statically_Validate_Form_Constraints(form, out IReadOnlyCollection<HTMLElement> unhandledInvalidControls))
            {
                return true;
            }

            /* XXX: Finish invalid input constraint reporting */
            /* 3) Report the problems with the constraints of at least one of the elements given in unhandled invalid controls to the user. */

            /* User agents may focus one of those elements in the process, by running the focusing steps for that element, and may change the scrolling position of the document, 
             * or perform some other action that brings the element to the user's attention. 
             * For elements that are form-associated custom elements, user agents should use their validation anchor instead, for the purposes of these actions. */

            /* User agents may report more than one constraint violation. */

            /* User agents may coalesce related constraint violation reports if appropriate (e.g. if multiple radio buttons in a group are marked as required, only one error need be reported). */

            /* If one of the controls is not being rendered (e.g. it has the hidden attribute set) then user agents may report a script error. */

            return false;
        }



        public static bool Is_Barred_From_Validation(HTMLElement element)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#barred-from-constraint-validation */

            /* Constraint validation: If an element is disabled, it is barred from constraint validation. */
            if (element.disabled)
            {
                return true;
            }

            /* Constraint validation: If the readonly attribute is specified on a form-associated custom element, the element is barred from constraint validation. */
            if (DOMCommon.Is_Form_Associated_Custom_Element(element))
            {
                if (element.hasAttribute(EAttributeName.ReadOnly))
                {
                    return true;
                }
            }

            /* Constraint validation: object elements are always barred from constraint validation. */
            if (element is HTMLObjectElement)
            {
                return true;
            }
            /* Constraint validation: If the readonly attribute is specified on a textarea element, the element is barred from constraint validation. */
            else if (element is HTMLTextAreaElement textArea)
            {
                if (textArea.readOnly)
                    return true;
            }
            else if (element is HTMLInputElement inputElement)
            {
                if (inputElement.readOnly)
                    return true;

                switch (inputElement.type)
                {
                    case EInputType.Hidden:
                    case EInputType.Reset:
                    case EInputType.Button:
                        return true;
                }
            }
            else if (element is HTMLButtonElement buttonElement)
            {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#the-button-element:barred-from-constraint-validation */
                /* XXX: finish this */
                switch (buttonElement.type)
                {
                    case EButtonType.Button:
                    case EButtonType.Reset:
                        return true;
                }
            }

            /* Constraint validation: If an element has a datalist element ancestor, it is barred from constraint validation. */
            var datalistAncestor = DOMCommon.Get_Nth_Ancestor<HTMLDataListElement>(element, 1, null, ENodeFilterMask.SHOW_ELEMENT);
            if (datalistAncestor != null)
            {
                return true;
            }

            return false;
        }

        public static void Reset_Form(HTMLFormElement form)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#concept-form-reset */
            var reset = form.dispatchEvent(new Event(EEventName.Reset, new EventInit() { bubbles = true, cancelable = true }));
            if (reset)
            {
                var resettableList = DOMCommon.Get_Descendents<IResettableElement>(form, new FilterFormOwner(form), ENodeFilterMask.SHOW_ELEMENT);
                foreach (IResettableElement resettable in resettableList)
                {
                    resettable.Reset();
                }
            }
        }

        public static void Submit_Form(HTMLFormElement form, Element submitter, bool? submitted_from_submit = null)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#concept-form-submit */
            /* XXX: implement form submission, we should support form submitting to actual web addresses */
            throw new NotImplementedException();

            if (form.bConstructingEntryList) return;

            var formDocument = form.nodeDocument;
            var formBrowsingContext = formDocument.BrowsingContext;
            if (!submitted_from_submit.HasValue || !submitted_from_submit.Value)
            {

            }
        }
        #endregion
    }
}
