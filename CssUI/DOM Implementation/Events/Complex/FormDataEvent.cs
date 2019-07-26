namespace CssUI.DOM.Events
{
    public class FormDataEvent : Event
    {
        public readonly FormData formData;

        public FormDataEvent(FormDataEventInit eventInitDict) : base(EEventName.FormData, eventInitDict)
        {
            formData = eventInitDict?.formData;
        }
    }
}
