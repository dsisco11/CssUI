using CssUI.DOM.Events;

namespace CssUI.DOM
{
    public interface IGlobalEventCallbacks
    {
        event EventCallback onAbort;
        event EventCallback onAuxClick;
        event EventCallback onBlur;
        event EventCallback onCancel;
        event EventCallback onCanPlay;
        event EventCallback onCanPlayThrough;
        event EventCallback onChange;
        event EventCallback onClick;
        event EventCallback onClose;
        event EventCallback onContextMenu;
        event EventCallback onCueChange;
        event EventCallback onDblClick;
        event EventCallback onDrag;
        event EventCallback onDragEnd;
        event EventCallback onDragEnter;
        event EventCallback onDragExit;
        event EventCallback onDragLeave;
        event EventCallback onDragOver;
        event EventCallback onDragStart;
        event EventCallback onDrop;
        event EventCallback onDurationChange;
        event EventCallback onEmptied;
        event EventCallback onEnded;
        /* Users should just catch any thrown DOMExceptions in their own code, we aren't ACTUALLY a web browser */
        // OnErrorEventCallback onError { get; } 
        event EventCallback onFocus;
        event EventCallback onFormData;
        event EventCallback onInput;
        event EventCallback onInvalid;
        event EventCallback onKeyDown;
        event EventCallback onKeyPress;
        event EventCallback onKeyUp;
        event EventCallback onLoad;
        event EventCallback onLoadedData;
        event EventCallback onLoadedMetadata;
        event EventCallback onLoadEnd;
        event EventCallback onLoadStart;
        event EventCallback onMouseDown;
        event EventCallback onMouseEnter;
        event EventCallback onMouseLeave;
        event EventCallback onMouseMove;
        event EventCallback onMouseOut;
        event EventCallback onMouseOver;
        event EventCallback onMouseUp;
        event EventCallback onWheel;
        event EventCallback onPause;
        event EventCallback onPlay;
        event EventCallback onPlaying;
        event EventCallback onProgress;
        event EventCallback onRateChange;
        event EventCallback onReset;
        event EventCallback onResize;
        event EventCallback onScroll;
        event EventCallback onSecurityPolicyViolation;
        event EventCallback onSeeked;
        event EventCallback onSeeking;
        event EventCallback onSelect;
        event EventCallback onStalled;
        event EventCallback onSubmit;
        event EventCallback onSuspend;
        event EventCallback onTimeUpdate;
        event EventCallback onToggle;
        event EventCallback onVolumeChange;
        event EventCallback onWaiting;

        /* Docs: https://w3c.github.io/selection-api/#extensions-to-globalEventCallbacks-interface */
        event EventCallback onSelectStart;
        event EventCallback onSelectionChange;
    }
}
