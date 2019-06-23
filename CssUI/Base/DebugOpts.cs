using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    [Flags]
    public enum EDebugOpt : uint
    {
        ALL = 0,
        BOUND_DRAW = (1 << 0),
        CHILD_BOUND_DRAW = (1 << 1),
        LOG_BLOCK_CHANGES = (1 << 2),
        LOG_PROPERTY_CHANGES = (1 << 3),
        LOG_LAYOUT_CHANGES = (1 << 4),
    }

    public class DebugOpts
    {
        /// <summary>
        /// The element will outline all of it's block boxes
        /// </summary>
        public bool Draw_Bounds = false;
        /// <summary>
        /// The element will outline all child elements block boxes
        /// </summary>
        public bool Draw_Child_Bounds = false;
        /// <summary>
        /// The element will overlay text at it's top displaying it's current size.
        /// </summary>
        public bool Draw_Size = false;
        public bool Log_Block_Changes = false;
        public bool Log_Layout_Changes = false;
        public bool Log_Property_Changes = false;
        public bool Log_Block_Resolved = false;

        public void Enable(EDebugOpt opt = EDebugOpt.ALL)
        {
            if (opt.HasFlag(EDebugOpt.BOUND_DRAW) || opt == EDebugOpt.ALL) Draw_Bounds = true;
            if (opt.HasFlag(EDebugOpt.CHILD_BOUND_DRAW) || opt == EDebugOpt.ALL) Draw_Child_Bounds = true;
            if (opt.HasFlag(EDebugOpt.LOG_BLOCK_CHANGES) || opt == EDebugOpt.ALL) Log_Block_Changes = true;
            if (opt.HasFlag(EDebugOpt.LOG_LAYOUT_CHANGES) || opt == EDebugOpt.ALL) Log_Layout_Changes = true;
            if (opt.HasFlag(EDebugOpt.LOG_PROPERTY_CHANGES) || opt == EDebugOpt.ALL) Log_Property_Changes = true;
        }

        public void Disable(EDebugOpt opt = EDebugOpt.ALL)
        {
            if (opt.HasFlag(EDebugOpt.BOUND_DRAW) || opt == EDebugOpt.ALL) Draw_Bounds = false;
            if (opt.HasFlag(EDebugOpt.CHILD_BOUND_DRAW) || opt == EDebugOpt.ALL) Draw_Child_Bounds = false;
            if (opt.HasFlag(EDebugOpt.LOG_BLOCK_CHANGES) || opt == EDebugOpt.ALL) Log_Block_Changes = false;
            if (opt.HasFlag(EDebugOpt.LOG_LAYOUT_CHANGES) || opt == EDebugOpt.ALL) Log_Layout_Changes = false;
            if (opt.HasFlag(EDebugOpt.LOG_PROPERTY_CHANGES) || opt == EDebugOpt.ALL) Log_Property_Changes = false;
        }
    }
}
