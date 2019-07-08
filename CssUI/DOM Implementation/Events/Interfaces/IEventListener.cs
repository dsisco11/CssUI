
namespace CssUI.DOM
{
    public interface IEventListener
    {
        string type { get; }
        IEventListener callback { get; }
        bool capture { get; }
        bool passive { get; }
        bool once { get; }
        bool removed { get; set; }


        void handleEvent(Event Event);
    }
}
