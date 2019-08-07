using CssUI.DOM;

namespace CssUI.HTML
{
    /// <summary>
    /// Represents one of the data types that can be handled by a form.
    /// The valid data types are: a string, a File(<see cref="FileBlob"/>), or a list of entries(<see cref="FormData"/>).
    /// </summary>
    public class FormSubmissionValue
    {
        #region Properties
        public readonly dynamic Value;
        public readonly ESubmissionValue Type;
        #endregion

        #region Constructors
        public FormSubmissionValue(string Value)
        {
            Type = ESubmissionValue.String;
            this.Value = Value;
        }

        public FormSubmissionValue(FileBlob Value)
        {
            Type = ESubmissionValue.File;
            this.Value = Value;
        }

        public FormSubmissionValue(FormData Value)
        {
            Type = ESubmissionValue.EntryList;
            this.Value = Value;
        }
        #endregion

        #region Implicit
        public static implicit operator FormSubmissionValue(string value) => ReferenceEquals(null, value) ? null : new FormSubmissionValue(value);
        public static implicit operator FormSubmissionValue(FileBlob value) => ReferenceEquals(null, value) ? null : new FormSubmissionValue(value);
        public static implicit operator FormSubmissionValue(FormData value) => ReferenceEquals(null, value) ? null : new FormSubmissionValue(value);
        #endregion
    }


    public enum ESubmissionValue
    {
        String,
        File,
        EntryList,
    }

}
