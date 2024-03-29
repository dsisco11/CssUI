﻿using System;
using CssUI.DOM.Events;

namespace CssUI.DOM
{
    public abstract partial class Window : IGlobalEventCallbacks, IWindowEventCallbacks
    {

        #region Global Events
        public event EventCallback onAbort
        {
            add
            {
                ((IGlobalEventCallbacks)document).onAbort += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onAbort -= value;
            }
        }
        public event EventCallback onAuxClick
        {
            add
            {
                ((IGlobalEventCallbacks)document).onAuxClick += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onAuxClick -= value;
            }
        }
        public event EventCallback onBlur
        {
            add
            {
                ((IGlobalEventCallbacks)document).onBlur += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onBlur -= value;
            }
        }
        public event EventCallback onCancel
        {
            add
            {
                ((IGlobalEventCallbacks)document).onCancel += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onCancel -= value;
            }
        }
        public event EventCallback onCanPlay
        {
            add
            {
                ((IGlobalEventCallbacks)document).onCanPlay += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onCanPlay -= value;
            }
        }
        public event EventCallback onCanPlayThrough
        {
            add
            {
                ((IGlobalEventCallbacks)document).onCanPlayThrough += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onCanPlayThrough -= value;
            }
        }
        public event EventCallback onChange
        {
            add
            {
                ((IGlobalEventCallbacks)document).onChange += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onChange -= value;
            }
        }
        public event EventCallback onClick
        {
            add
            {
                ((IGlobalEventCallbacks)document).onClick += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onClick -= value;
            }
        }
        public event EventCallback onClose
        {
            add
            {
                ((IGlobalEventCallbacks)document).onClose += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onClose -= value;
            }
        }
        public event EventCallback onContextMenu
        {
            add
            {
                ((IGlobalEventCallbacks)document).onContextMenu += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onContextMenu -= value;
            }
        }
        public event EventCallback onCueChange
        {
            add
            {
                ((IGlobalEventCallbacks)document).onCueChange += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onCueChange -= value;
            }
        }
        public event EventCallback onDblClick
        {
            add
            {
                ((IGlobalEventCallbacks)document).onDblClick += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onDblClick -= value;
            }
        }
        public event EventCallback onDrag
        {
            add
            {
                ((IGlobalEventCallbacks)document).onDrag += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onDrag -= value;
            }
        }
        public event EventCallback onDragEnd
        {
            add
            {
                ((IGlobalEventCallbacks)document).onDragEnd += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onDragEnd -= value;
            }
        }
        public event EventCallback onDragEnter
        {
            add
            {
                ((IGlobalEventCallbacks)document).onDragEnter += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onDragEnter -= value;
            }
        }
        public event EventCallback onDragExit
        {
            add
            {
                ((IGlobalEventCallbacks)document).onDragExit += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onDragExit -= value;
            }
        }
        public event EventCallback onDragLeave
        {
            add
            {
                ((IGlobalEventCallbacks)document).onDragLeave += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onDragLeave -= value;
            }
        }
        public event EventCallback onDragOver
        {
            add
            {
                ((IGlobalEventCallbacks)document).onDragOver += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onDragOver -= value;
            }
        }
        public event EventCallback onDragStart
        {
            add
            {
                ((IGlobalEventCallbacks)document).onDragStart += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onDragStart -= value;
            }
        }
        public event EventCallback onDrop
        {
            add
            {
                ((IGlobalEventCallbacks)document).onDrop += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onDrop -= value;
            }
        }
        public event EventCallback onDurationChange
        {
            add
            {
                ((IGlobalEventCallbacks)document).onDurationChange += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onDurationChange -= value;
            }
        }
        public event EventCallback onEmptied
        {
            add
            {
                ((IGlobalEventCallbacks)document).onEmptied += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onEmptied -= value;
            }
        }
        public event EventCallback onEnded
        {
            add
            {
                ((IGlobalEventCallbacks)document).onEnded += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onEnded -= value;
            }
        }
        public event EventCallback onFocus
        {
            add
            {
                ((IGlobalEventCallbacks)document).onFocus += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onFocus -= value;
            }
        }
        public event EventCallback onFormData
        {
            add
            {
                ((IGlobalEventCallbacks)document).onFormData += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onFormData -= value;
            }
        }
        public event EventCallback onInput
        {
            add
            {
                ((IGlobalEventCallbacks)document).onInput += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onInput -= value;
            }
        }
        public event EventCallback onInvalid
        {
            add
            {
                ((IGlobalEventCallbacks)document).onInvalid += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onInvalid -= value;
            }
        }
        public event EventCallback onKeyDown
        {
            add
            {
                ((IGlobalEventCallbacks)document).onKeyDown += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onKeyDown -= value;
            }
        }
        public event EventCallback onKeyPress
        {
            add
            {
                ((IGlobalEventCallbacks)document).onKeyPress += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onKeyPress -= value;
            }
        }
        public event EventCallback onKeyUp
        {
            add
            {
                ((IGlobalEventCallbacks)document).onKeyUp += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onKeyUp -= value;
            }
        }
        public event EventCallback onLoad
        {
            add
            {
                ((IGlobalEventCallbacks)document).onLoad += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onLoad -= value;
            }
        }
        public event EventCallback onLoadedData
        {
            add
            {
                ((IGlobalEventCallbacks)document).onLoadedData += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onLoadedData -= value;
            }
        }
        public event EventCallback onLoadedMetadata
        {
            add
            {
                ((IGlobalEventCallbacks)document).onLoadedMetadata += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onLoadedMetadata -= value;
            }
        }
        public event EventCallback onLoadEnd
        {
            add
            {
                ((IGlobalEventCallbacks)document).onLoadEnd += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onLoadEnd -= value;
            }
        }
        public event EventCallback onLoadStart
        {
            add
            {
                ((IGlobalEventCallbacks)document).onLoadStart += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onLoadStart -= value;
            }
        }
        public event EventCallback onMouseDown
        {
            add
            {
                ((IGlobalEventCallbacks)document).onMouseDown += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onMouseDown -= value;
            }
        }
        public event EventCallback onMouseEnter
        {
            add
            {
                ((IGlobalEventCallbacks)document).onMouseEnter += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onMouseEnter -= value;
            }
        }
        public event EventCallback onMouseLeave
        {
            add
            {
                ((IGlobalEventCallbacks)document).onMouseLeave += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onMouseLeave -= value;
            }
        }
        public event EventCallback onMouseMove
        {
            add
            {
                ((IGlobalEventCallbacks)document).onMouseMove += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onMouseMove -= value;
            }
        }
        public event EventCallback onMouseOut
        {
            add
            {
                ((IGlobalEventCallbacks)document).onMouseOut += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onMouseOut -= value;
            }
        }
        public event EventCallback onMouseOver
        {
            add
            {
                ((IGlobalEventCallbacks)document).onMouseOver += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onMouseOver -= value;
            }
        }
        public event EventCallback onMouseUp
        {
            add
            {
                ((IGlobalEventCallbacks)document).onMouseUp += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onMouseUp -= value;
            }
        }
        public event EventCallback onWheel
        {
            add
            {
                ((IGlobalEventCallbacks)document).onWheel += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onWheel -= value;
            }
        }
        public event EventCallback onPause
        {
            add
            {
                ((IGlobalEventCallbacks)document).onPause += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onPause -= value;
            }
        }
        public event EventCallback onPlay
        {
            add
            {
                ((IGlobalEventCallbacks)document).onPlay += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onPlay -= value;
            }
        }
        public event EventCallback onPlaying
        {
            add
            {
                ((IGlobalEventCallbacks)document).onPlaying += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onPlaying -= value;
            }
        }
        public event EventCallback onProgress
        {
            add
            {
                ((IGlobalEventCallbacks)document).onProgress += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onProgress -= value;
            }
        }
        public event EventCallback onRateChange
        {
            add
            {
                ((IGlobalEventCallbacks)document).onRateChange += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onRateChange -= value;
            }
        }
        public event EventCallback onReset
        {
            add
            {
                ((IGlobalEventCallbacks)document).onReset += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onReset -= value;
            }
        }
        public event EventCallback onResize
        {
            add
            {
                ((IGlobalEventCallbacks)document).onResize += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onResize -= value;
            }
        }
        public event EventCallback onScroll
        {
            add
            {
                ((IGlobalEventCallbacks)document).onScroll += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onScroll -= value;
            }
        }
        public event EventCallback onSecurityPolicyViolation
        {
            add
            {
                ((IGlobalEventCallbacks)document).onSecurityPolicyViolation += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onSecurityPolicyViolation -= value;
            }
        }
        public event EventCallback onSeeked
        {
            add
            {
                ((IGlobalEventCallbacks)document).onSeeked += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onSeeked -= value;
            }
        }
        public event EventCallback onSeeking
        {
            add
            {
                ((IGlobalEventCallbacks)document).onSeeking += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onSeeking -= value;
            }
        }
        public event EventCallback onSelect
        {
            add
            {
                ((IGlobalEventCallbacks)document).onSelect += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onSelect -= value;
            }
        }
        public event EventCallback onStalled
        {
            add
            {
                ((IGlobalEventCallbacks)document).onStalled += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onStalled -= value;
            }
        }
        public event EventCallback onSubmit
        {
            add
            {
                ((IGlobalEventCallbacks)document).onSubmit += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onSubmit -= value;
            }
        }
        public event EventCallback onSuspend
        {
            add
            {
                ((IGlobalEventCallbacks)document).onSuspend += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onSuspend -= value;
            }
        }
        public event EventCallback onTimeUpdate
        {
            add
            {
                ((IGlobalEventCallbacks)document).onTimeUpdate += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onTimeUpdate -= value;
            }
        }
        public event EventCallback onToggle
        {
            add
            {
                ((IGlobalEventCallbacks)document).onToggle += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onToggle -= value;
            }
        }
        public event EventCallback onVolumeChange
        {
            add
            {
                ((IGlobalEventCallbacks)document).onVolumeChange += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onVolumeChange -= value;
            }
        }
        public event EventCallback onWaiting
        {
            add
            {
                ((IGlobalEventCallbacks)document).onWaiting += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onWaiting -= value;
            }
        }
        public event EventCallback onSelectStart
        {
            add
            {
                ((IGlobalEventCallbacks)document).onSelectStart += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onSelectStart -= value;
            }
        }
        public event EventCallback onSelectionChange
        {
            add
            {
                ((IGlobalEventCallbacks)document).onSelectionChange += value;
            }

            remove
            {
                ((IGlobalEventCallbacks)document).onSelectionChange -= value;
            }
        }
        #endregion

        #region Window Events
        public event EventCallback onHashChange;
        public event EventCallback onLanguageChange;
        public event EventCallback onMessage;
        public event EventCallback onMessagEerror;
        public event EventCallback onOffline;
        public event EventCallback onOnline;
        public event EventCallback onPageHide;
        public event EventCallback onPageShow;
        public event EventCallback onPopState;
        public event EventCallback onRejectionHandled;
        public event EventCallback onStorage;
        public event EventCallback onUnhandledRejection;
        #endregion
    }
}
