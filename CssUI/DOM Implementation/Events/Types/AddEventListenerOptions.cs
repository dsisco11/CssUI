
namespace CssUI.DOM
{
    public class AddEventListenerOptions : EventListenerOptions
    {
        public readonly bool passive = false;
        public readonly bool once = false;

        public AddEventListenerOptions(bool capture = false, bool passive = false, bool once = false) : base(capture)
        {
            this.passive = passive;
            this.once = once;
        }
    }
}
