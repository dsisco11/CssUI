using CssUI.DOM.Events;

namespace CssUI.DOM
{
    public interface IGlobalEventHandlers
    {
        event EventHandler onabort;
        event EventHandler onauxclick;
        event EventHandler onblur;
        event EventHandler oncancel;
        event EventHandler oncanplay;
        event EventHandler oncanplaythrough;
        event EventHandler onchange;
        event EventHandler onclick;
        event EventHandler onclose;
        event EventHandler oncontextmenu;
        event EventHandler oncuechange;
        event EventHandler ondblclick;
        event EventHandler ondrag;
        event EventHandler ondragend;
        event EventHandler ondragenter;
        event EventHandler ondragexit;
        event EventHandler ondragleave;
        event EventHandler ondragover;
        event EventHandler ondragstart;
        event EventHandler ondrop;
        event EventHandler ondurationchange;
        event EventHandler onemptied;
        event EventHandler onended;
        /* Users should just catch any thrown DOMExceptions in their own code, we aren't ACTUALLY a web browser */
        // OnErrorEventHandler onerror { get; } 
        event EventHandler onfocus;
        event EventHandler onformdata;
        event EventHandler oninput;
        event EventHandler oninvalid;
        event EventHandler onkeydown;
        event EventHandler onkeypress;
        event EventHandler onkeyup;
        event EventHandler onload;
        event EventHandler onloadeddata;
        event EventHandler onloadedmetadata;
        event EventHandler onloadend;
        event EventHandler onloadstart;
        event EventHandler onmousedown;
        event EventHandler onmouseenter;
        event EventHandler onmouseleave;
        event EventHandler onmousemove;
        event EventHandler onmouseout;
        event EventHandler onmouseover;
        event EventHandler onmouseup;
        event EventHandler onwheel;
        event EventHandler onpause;
        event EventHandler onplay;
        event EventHandler onplaying;
        event EventHandler onprogress;
        event EventHandler onratechange;
        event EventHandler onreset;
        event EventHandler onresize;
        event EventHandler onscroll;
        event EventHandler onsecuritypolicyviolation;
        event EventHandler onseeked;
        event EventHandler onseeking;
        event EventHandler onselect;
        event EventHandler onstalled;
        event EventHandler onsubmit;
        event EventHandler onsuspend;
        event EventHandler ontimeupdate;
        event EventHandler ontoggle;
        event EventHandler onvolumechange;
        event EventHandler onwaiting;
    }
}
