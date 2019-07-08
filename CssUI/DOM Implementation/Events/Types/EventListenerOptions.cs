
namespace CssUI.DOM
{
    public class EventListenerOptions
    {
        public readonly bool capture = false;

        public EventListenerOptions(bool capture=false)
        {
            this.capture = capture;
        }
    }
}
