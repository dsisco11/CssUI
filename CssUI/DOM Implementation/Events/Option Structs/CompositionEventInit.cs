
namespace CssUI.DOM.Events
{
    public class CompositionEventInit : UIEventInit
    {
        public string data;

        public CompositionEventInit(string data = "")
        {
            this.data = data;
        }
    }
}
