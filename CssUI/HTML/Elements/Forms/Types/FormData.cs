using CssUI.DOM;
using CssUI.DOM.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CssUI.HTML
{
    public class FormData : IEnumerable<Tuple<string, FormDataEntryValue>>
    {/* Docs: https://xhr.spec.whatwg.org/#formdata */
        #region Properties
        public readonly HTMLFormElement form;
        private readonly List<Tuple<string, FormDataEntryValue>> Entries = new List<Tuple<string, FormDataEntryValue>>();
        #endregion

        #region Constructor
        public FormData(HTMLFormElement form, List<Tuple<string, FormDataEntryValue>> Entries)
        {
            this.form = form;
            this.Entries = Entries ?? throw new InvalidStateError();
        }

        public FormData(HTMLFormElement form)
        {/* Docs: https://xhr.spec.whatwg.org/#dom-formdata */
            this.form = form;
            var list = FormCommon.Construct_Form_Entry_List(form);
            Entries = list ?? throw new InvalidStateError();
        }

        public FormData(FormData other)
        {/* Docs: https://xhr.spec.whatwg.org/#dom-formdata */
            form = other.form;
            List<Tuple<string, FormDataEntryValue>> list = null;
            if (other.Entries != null)
            {
                list = new List<Tuple<string, FormDataEntryValue>>(other.Entries);
            }

            Entries = list ?? throw new InvalidStateError();
        }
        #endregion

        public void append(string name, string value)
        {/* Docs: https://xhr.spec.whatwg.org/#dom-formdata-append */
            var entry = new FormDataEntryValue(EFormDataValueType.String, name, value);
            Entries.Add(new Tuple<string, FormDataEntryValue>(name, entry));
        }

        public void append(string name, ReadOnlyMemory<byte> blobValue, string filename = null)
        {/* Docs: https://xhr.spec.whatwg.org/#dom-formdata-append */
            FormDataEntryValue entry = null;
            if (string.IsNullOrEmpty(filename))
            {
                /* 3) If value is a Blob object and not a File object, then set value to a new File object, representing the same bytes, whose name attribute value is "blob". */
                entry = new FormDataEntryValue(EFormDataValueType.Blob, name, new FileBlob(blobValue, "blob"));
            }
            else
            {
                entry = new FormDataEntryValue(EFormDataValueType.File, name, new FileBlob(blobValue, filename));
            }

            Entries.Add(new Tuple<string, FormDataEntryValue>(name, entry));
        }

        public void delete(string name)
        {/* Docs: https://xhr.spec.whatwg.org/#dom-formdata-delete */
            Entries.RemoveAll(e => StringCommon.StrEq(name, e.Item1));
        }

        public FormDataEntryValue get(string name)
        {/* Docs: https://xhr.spec.whatwg.org/#dom-formdata-get */
            return Entries.First(e => StringCommon.StrEq(name, e.Item1)).Item2;
        }

        public IReadOnlyCollection<FormDataEntryValue> getAll(string name)
        {/* Docs: https://xhr.spec.whatwg.org/#dom-formdata-getall */
            return Entries.Where(e => StringCommon.StrEq(name, e.Item1)).Select(e => e.Item2).ToArray();
        }

        public bool has(string name)
        {/* Docs: https://xhr.spec.whatwg.org/#dom-formdata-has */
            return Entries.Any(e => StringCommon.StrEq(name, e.Item1));
        }

        public void set(string name, string value)
        {/* Docs: https://xhr.spec.whatwg.org/#dom-formdata-set */
            var entry = new FormDataEntryValue(EFormDataValueType.String, name, value);
            int index = Entries.FindIndex(e => StringCommon.StrEq(name, e.Item1));
            Entries.RemoveAll(e => StringCommon.StrEq(name, e.Item1));
            if (index < 0)
            {
                Entries.Add(new Tuple<string, FormDataEntryValue>(name, entry));
            }
            else
            {
                Entries.Insert(index, new Tuple<string, FormDataEntryValue>(name, entry));
            }
        }

        public void set(string name, ReadOnlyMemory<byte> blobValue, string filename = null)
        {/* Docs: https://xhr.spec.whatwg.org/#dom-formdata-set */
            FormDataEntryValue entry = null;
            if (string.IsNullOrEmpty(filename))
            {
                /* 3) If value is a Blob object and not a File object, then set value to a new File object, representing the same bytes, whose name attribute value is "blob". */
                entry = new FormDataEntryValue(EFormDataValueType.Blob, name, new FileBlob(blobValue, "blob"));
            }
            else
            {
                entry = new FormDataEntryValue(EFormDataValueType.File, name, new FileBlob(blobValue, filename));
            }

            int index = Entries.FindIndex(e => StringCommon.StrEq(name, e.Item1));
            Entries.RemoveAll(e => StringCommon.StrEq(name, e.Item1));
            if (index < 0)
            {
                Entries.Add(new Tuple<string, FormDataEntryValue>(name, entry));
            }
            else
            {
                Entries.Insert(index, new Tuple<string, FormDataEntryValue>(name, entry));
            }
        }

        public IEnumerator<Tuple<string, FormDataEntryValue>> GetEnumerator()
        {
            return ((IEnumerable<Tuple<string, FormDataEntryValue>>)Entries).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Tuple<string, FormDataEntryValue>>)Entries).GetEnumerator();
        }
    }
}
