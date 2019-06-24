#if WINDOWS
using System;
using System.Runtime.InteropServices;

namespace CssUI.Platform.Win32
{
    public static class Kernel32
    {
        [DllImport("kernel32.dll")]
        public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
    }

    [FlagsAttribute]
    public enum EXECUTION_STATE : uint
    {
        ES_SYSTEM_REQUIRED = 0x00000001,
        ES_DISPLAY_REQUIRED = 0x00000002,
        // Legacy flag, should not be used.
        // ES_USER_PRESENT   = 0x00000004,
        ES_AWAYMODE_REQUIRED = 0x00000040,
        ES_CONTINUOUS = 0x80000000,
    }
}
#endif