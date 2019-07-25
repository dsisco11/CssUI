using CssUI.DOM.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CssUI.DOM
{
    public class FormData : IEnumerable<Tuple<string, FormDataEntryValue>>
    {/* Docs: https://xhr.spec.whatwg.org/#formdata */
        #region Properties
        public readonly HTMLFormElement form;
        private readonly List<Tuple<string, FormDataEntryValue>> Entries = new List<Tuple<string, FormDataEntryValue>>();
        #endregion

        #region Constructor
        public FormData(HTMLFormElement form)
        {/* Docs: https://xhr.spec.whatwg.org/#dom-formdata */
            this.form = form;
            var list = DOMCommon.Construct_Form_Entry_List(form);
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
            if (filename is null || filename.Length == 0)
            {
                entry = new FormDataEntryValue(EFormDataValueType.Blob, name, new FileBlob("blob", blobValue));
            }
            else
            {
                entry = new FormDataEntryValue(EFormDataValueType.File, name, new FileBlob(filename, blobValue));
            }

            Entries.Add(new Tuple<string, FormDataEntryValue>(name, entry));
        }

        public void delete(string name)
        {/* Docs: https://xhr.spec.whatwg.org/#dom-formdata-delete */
            Entries.RemoveAll(e => StringExt.streq(name.AsSpan(), e.Item1.AsSpan()));
        }

        public FormDataEntryValue get(string name)
        {/* Docs: https://xhr.spec.whatwg.org/#dom-formdata-get */
            return Entries.First(e => StringExt.streq(name.AsSpan(), e.Item1.AsSpan())).Item2;
        }

        public IReadOnlyCollection<FormDataEntryValue> getAll(string name)
        {/* Docs: https://xhr.spec.whatwg.org/#dom-formdata-getall */
            return Entries.Where(e => StringExt.streq(name.AsSpan(), e.Item1.AsSpan())).Select(e => e.Item2).ToArray();
        }

        public bool has(string name)
        {/* Docs: https://xhr.spec.whatwg.org/#dom-formdata-has */
            return Entries.Any(e => StringExt.streq(name.AsSpan(), e.Item1.AsSpan()));
        }

        public void set(string name, string value)
        {/* Docs: https://xhr.spec.whatwg.org/#dom-formdata-set */
            var entry = new FormDataEntryValue(EFormDataValueType.String, name, value);
            int index = Entries.FindIndex(e => StringExt.streq(name.AsSpan(), e.Item1.AsSpan()));
            Entries.RemoveAll(e => StringExt.streq(name.AsSpan(), e.Item1.AsSpan()));
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
            if (filename is null || filename.Length == 0)
            {
                entry = new FormDataEntryValue(EFormDataValueType.Blob, name, new FileBlob("blob", blobValue));
            }
            else
            {
                entry = new FormDataEntryValue(EFormDataValueType.File, name, new FileBlob(filename, blobValue));
            }

            int index = Entries.FindIndex(e => StringExt.streq(name.AsSpan(), e.Item1.AsSpan()));
            Entries.RemoveAll(e => StringExt.streq(name.AsSpan(), e.Item1.AsSpan()));
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
