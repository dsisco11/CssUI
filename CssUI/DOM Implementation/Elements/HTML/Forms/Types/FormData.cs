using System.Collections.Generic;

namespace CssUI.DOM
{
    public class FormData
    {/* Docs: https://xhr.spec.whatwg.org/#formdata */
        #region Properties
        public readonly HTMLFormElement form;
        public readonly List<FormDataValue> entries = new List<FormDataValue>();
        #endregion

        #region Constructor
        public FormData(HTMLFormElement form)
        {
            this.form = form;
        }
        #endregion

        void append(USVString name, USVString value);
        void append(USVString name, Blob blobValue, optional USVString filename);
        void delete(USVString name);
        FormDataEntryValue? get(USVString name);
        sequence<FormDataEntryValue> getAll(USVString name);
        boolean has(USVString name);
        void set(USVString name, USVString value);
        void set(USVString name, Blob blobValue, optional USVString filename);
        iterable<USVString, FormDataEntryValue>;
    }
}
